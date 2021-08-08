using System;
using System.Collections;
using System.Windows.Forms;
using System.Configuration;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using NetworkAdapterRouteControl.WinApi;

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

        private void SettingForm_Load(object sender, EventArgs e)
        {
            this.routeDestinationListBox.Items.AddRange(ReadSetting("RouteDestinationList").Split(',') as object[]);
            if (this.routeDestinationListBox.Items.Count > 0) {
                this.routeDestinationListBox.SelectedIndex = 0;
            }
            var adaptersInfo = IpHelper.GetAdaptersInfo();

            this.adapterComboBox.DataSource = adaptersInfo;
            this.adapterComboBox.SelectedIndex =
                adaptersInfo.FindIndex(i => i.Description == ReadSetting("AdapterDescription"));
            this.routeMetricNumericUpDown.Value = decimal.Parse(ReadSetting("RouteMetric"), CultureInfo.InvariantCulture.NumberFormat);
            this.interfaceMetricNumericUpDown.Value = decimal.Parse(ReadSetting("InterfaceMetric"), CultureInfo.InvariantCulture.NumberFormat);
            this.syncPeriodNumericUpDown.Value = decimal.Parse(ReadSetting("SyncPeriod"), CultureInfo.InvariantCulture.NumberFormat);
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            config.AppSettings.Settings["RouteDestinationList"].Value = String.Join(",", 
                ((IEnumerable)this.routeDestinationListBox.Items).Cast<object>().Select(x => x.ToString()).ToArray());
            config.AppSettings.Settings["AdapterDescription"].Value = (adapterComboBox.SelectedValue as AdapterInfo)?.Description;
            config.AppSettings.Settings["RouteMetric"].Value = this.routeMetricNumericUpDown.Value.ToString(CultureInfo.InvariantCulture.NumberFormat);
            config.AppSettings.Settings["InterfaceMetric"].Value = this.interfaceMetricNumericUpDown.Value.ToString(CultureInfo.InvariantCulture.NumberFormat);
            config.AppSettings.Settings["SyncPeriod"].Value = this.syncPeriodNumericUpDown.Value.ToString(CultureInfo.InvariantCulture.NumberFormat);
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
            (this.Owner as MainForm)?.SyncSettings();
            this.Close();
        }

        private void addRouteDestinationButton_Click(object sender, EventArgs e)
        {
            if (!routDestinationRegex.IsMatch(routeDestinationTextBox.Text) || !IPAddress.TryParse(this.routeDestinationTextBox.Text, out var routeDestination))
            {
                routeDestinationToolTip.Show("Неверный формат адреса", this.routeDestinationTextBox, new Point(10, -25), 1000);
                return;
            }
            this.routeDestinationListBox.Items.Add(routeDestination.ToString());
            this.routeDestinationTextBox.Text = "";
            
        }

        private void routeDestinationListBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                if (this.routeDestinationListBox.SelectedIndex >= 0)
                {
                    var index = this.routeDestinationListBox.SelectedIndex;
                    this.routeDestinationListBox.Items.RemoveAt(index);
                    if (this.routeDestinationListBox.Items.Count > index)
                    {
                        this.routeDestinationListBox.SelectedIndex = index;
                    }
                    else if (this.routeDestinationListBox.Items.Count > 0)
                    {
                        this.routeDestinationListBox.SelectedIndex = this.routeDestinationListBox.Items.Count - 1;
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

        private void routeDestinationTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (!IsNumericKey(e) && !IsEditKey(e) && !IsDotKey(e) && !IsForwardKey(e)) e.SuppressKeyPress = true;
            if (IsDotKey(e))
            {
                this.routeDestinationTextBox.AppendText(".");
                e.SuppressKeyPress = true;
            }
            preRouteDestinationValue = this.routeDestinationTextBox.Text;
        }

        private void routeDestinationTextBox_TextChanged(object sender, EventArgs e)
        {
            if (ignoreRouteDestinationChange)
            {
                ignoreRouteDestinationChange = false;
                return;
            }
            var value = this.routeDestinationTextBox.Text;
            if (value.Length == 0) return;
            if (value.Length < 15)
            {
                var dotCount = value.Length - value.Replace(".", "").Length;
                if (value.EndsWith(".") && dotCount < 4) value += "0";
                for (var i = dotCount; i < 3; i++) value += ".0";
            }
            if (routDestinationRegex.IsMatch(value)) return;
            var selectionStart = this.routeDestinationTextBox.SelectionStart;
            ignoreRouteDestinationChange = true;
            this.routeDestinationTextBox.Text = preRouteDestinationValue;
            this.routeDestinationTextBox.SelectionStart = Math.Min(selectionStart, this.routeDestinationTextBox.Text.Length);
            this.routeDestinationTextBox.SelectionLength = 0;
            routeDestinationToolTip.Show("Ошибка ввода", this.routeDestinationTextBox, new Point(10, -25), 1000);
        }
    }
}
