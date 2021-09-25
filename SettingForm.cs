using NetworkAdapterRouteControl.WinApi;
using System;
using System.Collections;
using System.Configuration;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Microsoft.Win32;

namespace NetworkAdapterRouteControl
{
    public partial class SettingForm : Form
    {

        private string preRouteDestinationValue = String.Empty;
        private bool ignoreRouteDestinationChange = false;
        private readonly Regex routDestinationRegex = new Regex(@"^((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$");

        public SettingForm()
        {
            InitializeComponent();
        }
        private static String ReadSetting(string key)
        {
            try
            {
                var appSettings = ConfigurationManager.AppSettings;
                return appSettings[key];
            }
            catch (ConfigurationErrorsException)
            {
                return String.Empty;
            }
        }

        private bool IsAutorun()
        {
            RegistryKey rk = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            if (rk.GetValue(Application.ProductName) == null)
                return false;
            else
                return true;
        }

        private void SettingForm_Load(object sender, EventArgs e)
        {
            routeDestinationListBox.Items.AddRange(ReadSetting("RouteDestinationList").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries) as object[]);
            if (routeDestinationListBox.Items.Count > 0) {
                routeDestinationListBox.SelectedIndex = 0;
            }
            var adaptersInfo = IpHelper.GetAdaptersInfo();
            adapterComboBox.DataSource = adaptersInfo;
            adapterComboBox.SelectedIndex = adaptersInfo.FindIndex(i => (i.Description == ReadSetting("AdapterDescription")));

            int value = int.MinValue;

            if (!int.TryParse(ReadSetting("RouteMetric"), NumberStyles.Integer, CultureInfo.InvariantCulture, out value)) { value = 1000; };
            routeMetricNumericUpDown.Value = Convert.ToDecimal(value);

            if (!int.TryParse(ReadSetting("InterfaceMetric"), NumberStyles.Integer, CultureInfo.InvariantCulture, out value)) { value = 1000; };
            interfaceMetricNumericUpDown.Value = Convert.ToDecimal(value);

            if (!int.TryParse(ReadSetting("SyncPeriod"), NumberStyles.Integer, CultureInfo.InvariantCulture, out value)) { value = 100; };
            syncPeriodNumericUpDown.Value = Convert.ToDecimal(value);

            autorunCheckBox.Checked = IsAutorun();
            
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            config.AppSettings.Settings["RouteDestinationList"].Value = 
                String.Join(",", ((IEnumerable)routeDestinationListBox.Items).Cast<object>().Select(x => x.ToString()).ToArray());
            config.AppSettings.Settings["AdapterDescription"].Value = (adapterComboBox.SelectedValue as AdapterInfo)?.Description;
            config.AppSettings.Settings["RouteMetric"].Value = routeMetricNumericUpDown.Value.ToString(CultureInfo.InvariantCulture.NumberFormat);
            config.AppSettings.Settings["InterfaceMetric"].Value = interfaceMetricNumericUpDown.Value.ToString(CultureInfo.InvariantCulture.NumberFormat);
            config.AppSettings.Settings["SyncPeriod"].Value = syncPeriodNumericUpDown.Value.ToString(CultureInfo.InvariantCulture.NumberFormat);
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
            (Owner as MainForm)?.SyncSettings();
            var isAutorun = IsAutorun();
            var autorunPath = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";
            if (autorunCheckBox.Checked && !isAutorun)
            {
                var regKey = Registry.CurrentUser.OpenSubKey(autorunPath, true);
                regKey.SetValue(Application.ProductName, "\"" + Application.ExecutablePath.ToString() + "\"", RegistryValueKind.String);
            } else if (!autorunCheckBox.Checked && isAutorun) {
                var regKey = Registry.CurrentUser.OpenSubKey(autorunPath, true);
                regKey.DeleteValue(Application.ProductName, false);
            }
            Close();
        }

        private void AddRouteDestinationButton_Click(object sender, EventArgs e)
        {
            if (!routDestinationRegex.IsMatch(routeDestinationTextBox.Text) || !IPAddress.TryParse(routeDestinationTextBox.Text, out var routeDestination))
            {
                routeDestinationToolTip.Show("Неверный формат IP-адреса", routeDestinationTextBox, new Point(10, -25), 1000);
                return;
            }
            routeDestinationListBox.Items.Add(routeDestination.ToString());
            routeDestinationTextBox.Text = "";
            
        }

        private void RouteDestinationListBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                if (routeDestinationListBox.SelectedIndex >= 0)
                {
                    var index = routeDestinationListBox.SelectedIndex;
                    routeDestinationListBox.Items.RemoveAt(index);
                    if (routeDestinationListBox.Items.Count > index)
                    {
                        routeDestinationListBox.SelectedIndex = index;
                    }
                    else if (routeDestinationListBox.Items.Count > 0)
                    {
                        routeDestinationListBox.SelectedIndex = routeDestinationListBox.Items.Count - 1;
                    }
                }
            }
        }

        private bool IsEditKey(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Back || e.KeyCode == Keys.Delete)
            {
                return true;
            }
            else if (e.Modifiers == Keys.Control && (e.KeyCode == Keys.C ||
                                                     e.KeyCode == Keys.V ||
                                                     e.KeyCode == Keys.X))
            {
                return true;
            }
            else if (e.Modifiers == Keys.Shift && (e.KeyCode == Keys.Insert))
            {
                return true;
            }
            return false;
        }

        private bool IsEnterKey(KeyEventArgs e)
        {
            return e.KeyCode == Keys.Enter || e.KeyCode == Keys.Return;
        }

        private bool IsForwardKey(KeyEventArgs e)
        {
            return e.KeyCode == Keys.Right || e.KeyCode == Keys.Left;
        }

        private bool IsNumericKey(KeyEventArgs e)
        {
            if (e.KeyCode >= Keys.NumPad0 && e.KeyCode <= Keys.NumPad9) return true;
            return e.KeyCode >= Keys.D0 && e.KeyCode <= Keys.D9;
        }

        private bool IsDotKey(KeyEventArgs e)
        {
            return e.KeyCode == Keys.Space || e.KeyCode == Keys.OemPeriod || e.KeyCode == Keys.Decimal;
        }

        private bool IsReverseKey(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left ||
                 e.KeyCode == Keys.Up)
            {
                return true;
            }

            return false;
        }

        private void RouteDestinationTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (!IsNumericKey(e) && !IsEditKey(e) && !IsDotKey(e) && !IsForwardKey(e)) e.SuppressKeyPress = true;
            if (IsDotKey(e))
            {
                routeDestinationTextBox.AppendText(".");
                e.SuppressKeyPress = true;
            }
            preRouteDestinationValue = routeDestinationTextBox.Text;
        }

        private void RouteDestinationTextBox_TextChanged(object sender, EventArgs e)
        {
            if (ignoreRouteDestinationChange)
            {
                ignoreRouteDestinationChange = false;
                return;
            }
            var value = routeDestinationTextBox.Text;
            if (value.Length == 0) return;
            if (value.Length < 15)
            {
                var dotCount = value.Length - value.Replace(".", "").Length;
                if (value.EndsWith(".") && dotCount < 4) value += "0";
                for (var i = dotCount; i < 3; i++) value += ".0";
            }
            if (routDestinationRegex.IsMatch(value)) return;
            var selectionStart = routeDestinationTextBox.SelectionStart;
            ignoreRouteDestinationChange = true;
            routeDestinationTextBox.Text = preRouteDestinationValue;
            routeDestinationTextBox.SelectionStart = Math.Min(selectionStart, routeDestinationTextBox.Text.Length);
            routeDestinationTextBox.SelectionLength = 0;
            routeDestinationToolTip.Show("Неверный формат IP-адреса", routeDestinationTextBox, new Point(10, -25), 1000);
        }

    }
}
