﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;
using System.Linq;
using System.Configuration;
using System.Threading;
using System.Globalization;
using NetworkAdapterRouteControl.WinApi;
using log4net;


namespace NetworkAdapterRouteControl
{
    public partial class MainForm : Form
    {

        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly System.Threading.Timer _timer;

        private string _adapterDescription;
        private int _interfaceMetric;
        private int _routeMetric;
        private IPAddress[] _routeDestinationList;
        private int _syncPeriod;


        private static string ReadSetting(string key)
        {
            try
            {
                var appSettings = ConfigurationManager.AppSettings;
                return appSettings[key];
            }
            catch (ConfigurationErrorsException)
            {
                return string.Empty;
            }
        }

        private bool ReadSettings()
        {
            try
            {
                // AdapterDescription
                _adapterDescription = ReadSetting("AdapterDescription");
                if (string.IsNullOrEmpty(_adapterDescription))
                {
                    throw new Exception("Сетевой адаптер не задан");
                }

                // InterfaceMetric
                if (!int.TryParse(ReadSetting("InterfaceMetric"), NumberStyles.Integer, CultureInfo.InvariantCulture,
                    out _interfaceMetric))
                {
                    throw new Exception("Метрика адаптера не задана или не является целым числом");
                }

                if (_interfaceMetric <= 0)
                {
                    throw new Exception("Метрика адаптера должна быть больше нуля");
                }

                // RouteMetric
                if (!int.TryParse(ReadSetting("RouteMetric"), NumberStyles.Integer, CultureInfo.InvariantCulture,
                    out _routeMetric))
                {
                    throw new Exception("Метрика маршрута не задана или не является целым числом");
                }

                if (_routeMetric <= 0)
                {
                    throw new Exception("Метрика маршрута должна быть больше нуля");
                }

                // RouteDestinationList
                var routeDestinationValueList = ReadSetting("RouteDestinationList").Split(',');
                if (routeDestinationValueList.Length == 0)
                {
                    throw new Exception("Список адресов назначения не задан");
                }

                _routeDestinationList = new IPAddress[routeDestinationValueList.Length];
                for (int i = 0; i < routeDestinationValueList.Length; i++)
                {
                    if (!IPAddress.TryParse(routeDestinationValueList[i], out _routeDestinationList[i]))
                    {
                        throw new Exception(String.Format(
                            "Список адресов назначения содержит адрес в неверном формате {0}",
                            routeDestinationValueList[i]));
                    }
                }

                // SyncPeriod
                if (!int.TryParse(ReadSetting("SyncPeriod"), NumberStyles.Integer, CultureInfo.InvariantCulture,
                    out _syncPeriod))
                {
                    throw new Exception("Период синхронизации не задан или не является целым числом");
                }

                if (_syncPeriod <= 0)
                {
                    throw new Exception("Период синхронизации должен быть больше нуля");
                }

                return true;
            }
            catch (Exception e)
            {
                notifyIcon.Visible = true;
                notifyIcon.ShowBalloonTip(500, "Ошибка параметров", e.Message, ToolTipIcon.Error);
                return false;
            }
        }

        private static void Sync(Object o)
        {
            var sender = (MainForm) o;
            try
            {
                var vpnAdapter = IpHelper.GetAdaptersInfo().Find(i => i.Description.Equals(sender._adapterDescription));
                if (vpnAdapter == null)
                {
                    throw new Exception(String.Format("Не найден сетевой адаптер {0}", sender._adapterDescription));
                }

                var routeTable = IpHelper.GetRouteTable(false);
                if (vpnAdapter.PrimaryGateway == null)
                {
                    vpnAdapter.PrimaryGateway = routeTable.Where(i => i.InterfaceIndex == vpnAdapter.Index)
                        ?.FirstOrDefault()?.GatewayIP;
                    if (vpnAdapter.PrimaryGateway == null && vpnAdapter.DhcpServer != null)
                    {
                        vpnAdapter.PrimaryGateway = vpnAdapter.DhcpServer;
                    }

                    if (vpnAdapter.PrimaryGateway == null) return;
                }

                var destinationHashSet = new HashSet<IPAddress>()
                {
                    IPAddress.Parse("0.0.0.0")
                };
                foreach (var routeDestination in sender._routeDestinationList)
                {
                    destinationHashSet.Add(routeDestination);
                }

                IpHelper.SetMetric(vpnAdapter.Index, sender._interfaceMetric);
                var currentDestinationHashSet = new HashSet<IPAddress>();
                foreach (var route in routeTable)
                {
                    if (route.InterfaceIndex != vpnAdapter.Index || route.DestinationIP.AddressFamily != AddressFamily.InterNetwork) continue;
                    if (destinationHashSet.Contains(route.DestinationIP))
                    {
                        if (route.Metric != sender._routeMetric + sender._interfaceMetric)
                        {
                            route.Metric = sender._routeMetric + sender._interfaceMetric;
                            IpHelper.SetRoute(route);
                        }
                        currentDestinationHashSet.Add(route.DestinationIP);
                    }
                    else
                    {
                        IpHelper.DeleteRoute(route);
                    }
                }

                foreach (var destinationIp in destinationHashSet)
                {
                    if (!currentDestinationHashSet.Contains(destinationIp))
                    {
                        IpHelper.CreateRoute(new RouteEntry()
                        {
                            DestinationIP = destinationIp,
                            SubnetMask = IPAddress.Parse("255.255.255.255"),
                            GatewayIP = vpnAdapter.PrimaryGateway,
                            InterfaceIndex = vpnAdapter.Index,
                            Metric = sender._routeMetric
                        });
                    }
                }
            }
            catch (Exception e)
            {
                log.Error(e.ToString());
                sender.StopSync(e.Message);
            }
        }

        public MainForm()
        {
            InitializeComponent();
            _timer = new System.Threading.Timer(Sync, this, Timeout.Infinite, Timeout.Infinite);
            StartSync();
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
            this.Visible = false;
        }

        private void StopSync(string message)
        {
            _timer.Change(Timeout.Infinite, Timeout.Infinite);
            if (InvokeRequired)
            {
                this.BeginInvoke((MethodInvoker)(() => this.notifyIcon.Visible = true));
                this.BeginInvoke((MethodInvoker)(() => this.notifyIcon.ShowBalloonTip(500, "Мониторинг параметров выключен", message, ToolTipIcon.Info)));
                this.BeginInvoke((MethodInvoker)(() => this.controlToolStripMenuItem.Checked = false));
            }
            else
            {
                notifyIcon.Visible = true;
                this.notifyIcon.ShowBalloonTip(500, "Остановлен", message, ToolTipIcon.Info);
                this.controlToolStripMenuItem.Checked = false;
            }
        }

        private void StartSync()
        {
            if (!ReadSettings()) return;
            _timer.Change(0, _syncPeriod);
            notifyIcon.Visible = true;
            notifyIcon.ShowBalloonTip(500, "Запущен", " ", ToolTipIcon.Info);
            this.controlToolStripMenuItem.Checked = true;
        }

        public void SyncSettings()
        {
            if (this.controlToolStripMenuItem.Checked) _timer.Change(Timeout.Infinite, Timeout.Infinite);
            if (!ReadSettings()) return;
            if (this.controlToolStripMenuItem.Checked) _timer.Change(0, _syncPeriod);
        }

        private void controlToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.controlToolStripMenuItem.Checked)
            {
                StopSync(" ");
            }
            else
            {
                StartSync();
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.notifyIcon.Visible = false;
            Application.Exit();
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var form = new SettingForm();
            form.ShowDialog(this);
        }

        private void notifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                mainContextMenuStrip.Show(MousePosition);
            } else if (e.Button == MouseButtons.Left)
            {
                var form = new SettingForm();
                form.ShowDialog(this);
            }
        }
    }
}