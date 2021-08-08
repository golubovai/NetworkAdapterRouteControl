using System.Net;

namespace NetworkAdapterRouteControl
{
    partial class SettingForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingForm));
            this.routeDestinationListBox = new System.Windows.Forms.ListBox();
            this.routeDestinationGroupBox = new System.Windows.Forms.GroupBox();
            this.routeDestinationTextBox = new System.Windows.Forms.TextBox();
            this.addRouteDestinationButton = new System.Windows.Forms.Button();
            this.adapterGroupBox = new System.Windows.Forms.GroupBox();
            this.adapterComboBox = new System.Windows.Forms.ComboBox();
            this.interfaceMetricNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.syncPeriodGroupBox = new System.Windows.Forms.GroupBox();
            this.msLabel = new System.Windows.Forms.Label();
            this.syncPeriodNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.metricGroupBox = new System.Windows.Forms.GroupBox();
            this.adapterMetricLabel = new System.Windows.Forms.Label();
            this.routeMetricLabel = new System.Windows.Forms.Label();
            this.routeMetricNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.exitButton = new System.Windows.Forms.Button();
            this.saveButton = new System.Windows.Forms.Button();
            this.routeDestinationToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.routeDestinationGroupBox.SuspendLayout();
            this.adapterGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.interfaceMetricNumericUpDown)).BeginInit();
            this.syncPeriodGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.syncPeriodNumericUpDown)).BeginInit();
            this.metricGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.routeMetricNumericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // routeDestinationListBox
            // 
            this.routeDestinationListBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.routeDestinationListBox.FormattingEnabled = true;
            this.routeDestinationListBox.Location = new System.Drawing.Point(8, 19);
            this.routeDestinationListBox.Name = "routeDestinationListBox";
            this.routeDestinationListBox.Size = new System.Drawing.Size(157, 108);
            this.routeDestinationListBox.TabIndex = 1;
            this.routeDestinationListBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.routeDestinationListBox_KeyDown);
            // 
            // routeDestinationGroupBox
            // 
            this.routeDestinationGroupBox.Controls.Add(this.routeDestinationTextBox);
            this.routeDestinationGroupBox.Controls.Add(this.addRouteDestinationButton);
            this.routeDestinationGroupBox.Controls.Add(this.routeDestinationListBox);
            this.routeDestinationGroupBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.routeDestinationGroupBox.Location = new System.Drawing.Point(7, 7);
            this.routeDestinationGroupBox.Name = "routeDestinationGroupBox";
            this.routeDestinationGroupBox.Size = new System.Drawing.Size(172, 166);
            this.routeDestinationGroupBox.TabIndex = 3;
            this.routeDestinationGroupBox.TabStop = false;
            this.routeDestinationGroupBox.Text = "Список адресов назначения";
            // 
            // routeDestinationTextBox
            // 
            this.routeDestinationTextBox.Location = new System.Drawing.Point(8, 135);
            this.routeDestinationTextBox.MaxLength = 15;
            this.routeDestinationTextBox.Name = "routeDestinationTextBox";
            this.routeDestinationTextBox.Size = new System.Drawing.Size(130, 20);
            this.routeDestinationTextBox.TabIndex = 2;
            this.routeDestinationTextBox.TextChanged += new System.EventHandler(this.routeDestinationTextBox_TextChanged);
            this.routeDestinationTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.routeDestinationTextBox_KeyDown);
            // 
            // addRouteDestinationButton
            // 
            this.addRouteDestinationButton.BackColor = System.Drawing.Color.Transparent;
            this.addRouteDestinationButton.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("addRouteDestinationButton.BackgroundImage")));
            this.addRouteDestinationButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.addRouteDestinationButton.FlatAppearance.BorderSize = 0;
            this.addRouteDestinationButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.addRouteDestinationButton.Location = new System.Drawing.Point(141, 133);
            this.addRouteDestinationButton.Margin = new System.Windows.Forms.Padding(0);
            this.addRouteDestinationButton.Name = "addRouteDestinationButton";
            this.addRouteDestinationButton.Size = new System.Drawing.Size(24, 24);
            this.addRouteDestinationButton.TabIndex = 3;
            this.addRouteDestinationButton.UseVisualStyleBackColor = false;
            this.addRouteDestinationButton.Click += new System.EventHandler(this.addRouteDestinationButton_Click);
            // 
            // adapterGroupBox
            // 
            this.adapterGroupBox.Controls.Add(this.adapterComboBox);
            this.adapterGroupBox.Location = new System.Drawing.Point(7, 179);
            this.adapterGroupBox.Name = "adapterGroupBox";
            this.adapterGroupBox.Padding = new System.Windows.Forms.Padding(8);
            this.adapterGroupBox.Size = new System.Drawing.Size(334, 48);
            this.adapterGroupBox.TabIndex = 4;
            this.adapterGroupBox.TabStop = false;
            this.adapterGroupBox.Text = "Сетевой адаптер";
            // 
            // adapterComboBox
            // 
            this.adapterComboBox.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.adapterComboBox.FormattingEnabled = true;
            this.adapterComboBox.Location = new System.Drawing.Point(8, 19);
            this.adapterComboBox.Name = "adapterComboBox";
            this.adapterComboBox.Size = new System.Drawing.Size(318, 21);
            this.adapterComboBox.TabIndex = 7;
            // 
            // interfaceMetricNumericUpDown
            // 
            this.interfaceMetricNumericUpDown.Location = new System.Drawing.Point(83, 22);
            this.interfaceMetricNumericUpDown.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.interfaceMetricNumericUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.interfaceMetricNumericUpDown.Name = "interfaceMetricNumericUpDown";
            this.interfaceMetricNumericUpDown.Size = new System.Drawing.Size(56, 20);
            this.interfaceMetricNumericUpDown.TabIndex = 5;
            this.interfaceMetricNumericUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // syncPeriodGroupBox
            // 
            this.syncPeriodGroupBox.Controls.Add(this.msLabel);
            this.syncPeriodGroupBox.Controls.Add(this.syncPeriodNumericUpDown);
            this.syncPeriodGroupBox.Location = new System.Drawing.Point(185, 10);
            this.syncPeriodGroupBox.Name = "syncPeriodGroupBox";
            this.syncPeriodGroupBox.Padding = new System.Windows.Forms.Padding(8);
            this.syncPeriodGroupBox.Size = new System.Drawing.Size(156, 77);
            this.syncPeriodGroupBox.TabIndex = 6;
            this.syncPeriodGroupBox.TabStop = false;
            this.syncPeriodGroupBox.Text = "Период синхронизации";
            // 
            // msLabel
            // 
            this.msLabel.AutoSize = true;
            this.msLabel.Location = new System.Drawing.Point(71, 22);
            this.msLabel.Name = "msLabel";
            this.msLabel.Size = new System.Drawing.Size(21, 13);
            this.msLabel.TabIndex = 1;
            this.msLabel.Text = "мс";
            // 
            // syncPeriodNumericUpDown
            // 
            this.syncPeriodNumericUpDown.Location = new System.Drawing.Point(9, 20);
            this.syncPeriodNumericUpDown.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.syncPeriodNumericUpDown.Minimum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.syncPeriodNumericUpDown.Name = "syncPeriodNumericUpDown";
            this.syncPeriodNumericUpDown.Size = new System.Drawing.Size(56, 20);
            this.syncPeriodNumericUpDown.TabIndex = 4;
            this.syncPeriodNumericUpDown.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            // 
            // metricGroupBox
            // 
            this.metricGroupBox.Controls.Add(this.adapterMetricLabel);
            this.metricGroupBox.Controls.Add(this.routeMetricLabel);
            this.metricGroupBox.Controls.Add(this.interfaceMetricNumericUpDown);
            this.metricGroupBox.Controls.Add(this.routeMetricNumericUpDown);
            this.metricGroupBox.Location = new System.Drawing.Point(185, 93);
            this.metricGroupBox.Name = "metricGroupBox";
            this.metricGroupBox.Padding = new System.Windows.Forms.Padding(8);
            this.metricGroupBox.Size = new System.Drawing.Size(156, 80);
            this.metricGroupBox.TabIndex = 10;
            this.metricGroupBox.TabStop = false;
            this.metricGroupBox.Text = "Метрика";
            // 
            // adapterMetricLabel
            // 
            this.adapterMetricLabel.AutoSize = true;
            this.adapterMetricLabel.Location = new System.Drawing.Point(20, 24);
            this.adapterMetricLabel.Name = "adapterMetricLabel";
            this.adapterMetricLabel.Size = new System.Drawing.Size(55, 13);
            this.adapterMetricLabel.TabIndex = 2;
            this.adapterMetricLabel.Text = "Адаптера";
            // 
            // routeMetricLabel
            // 
            this.routeMetricLabel.AutoSize = true;
            this.routeMetricLabel.Location = new System.Drawing.Point(17, 51);
            this.routeMetricLabel.Name = "routeMetricLabel";
            this.routeMetricLabel.Size = new System.Drawing.Size(58, 13);
            this.routeMetricLabel.TabIndex = 11;
            this.routeMetricLabel.Text = "Маршрута";
            // 
            // routeMetricNumericUpDown
            // 
            this.routeMetricNumericUpDown.Location = new System.Drawing.Point(82, 49);
            this.routeMetricNumericUpDown.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.routeMetricNumericUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.routeMetricNumericUpDown.Name = "routeMetricNumericUpDown";
            this.routeMetricNumericUpDown.Size = new System.Drawing.Size(56, 20);
            this.routeMetricNumericUpDown.TabIndex = 6;
            this.routeMetricNumericUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // exitButton
            // 
            this.exitButton.Location = new System.Drawing.Point(267, 244);
            this.exitButton.Margin = new System.Windows.Forms.Padding(5);
            this.exitButton.Name = "exitButton";
            this.exitButton.Size = new System.Drawing.Size(75, 25);
            this.exitButton.TabIndex = 9;
            this.exitButton.Text = "Отмена";
            this.exitButton.UseVisualStyleBackColor = true;
            this.exitButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // saveButton
            // 
            this.saveButton.Location = new System.Drawing.Point(185, 244);
            this.saveButton.Margin = new System.Windows.Forms.Padding(5, 5, 15, 5);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(75, 25);
            this.saveButton.TabIndex = 8;
            this.saveButton.Text = "Сохранить";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // SettingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(354, 281);
            this.Controls.Add(this.exitButton);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.syncPeriodGroupBox);
            this.Controls.Add(this.adapterGroupBox);
            this.Controls.Add(this.metricGroupBox);
            this.Controls.Add(this.routeDestinationGroupBox);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(370, 320);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(370, 320);
            this.Name = "SettingForm";
            this.Padding = new System.Windows.Forms.Padding(7);
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Параметры контроля маршрутизации";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.SettingForm_Load);
            this.routeDestinationGroupBox.ResumeLayout(false);
            this.routeDestinationGroupBox.PerformLayout();
            this.adapterGroupBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.interfaceMetricNumericUpDown)).EndInit();
            this.syncPeriodGroupBox.ResumeLayout(false);
            this.syncPeriodGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.syncPeriodNumericUpDown)).EndInit();
            this.metricGroupBox.ResumeLayout(false);
            this.metricGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.routeMetricNumericUpDown)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox routeDestinationListBox;
        private System.Windows.Forms.GroupBox routeDestinationGroupBox;
        private System.Windows.Forms.GroupBox adapterGroupBox;
        private System.Windows.Forms.NumericUpDown interfaceMetricNumericUpDown;
        private System.Windows.Forms.Button addRouteDestinationButton;
        private System.Windows.Forms.ComboBox adapterComboBox;
        private System.Windows.Forms.GroupBox syncPeriodGroupBox;
        private System.Windows.Forms.NumericUpDown syncPeriodNumericUpDown;
        private System.Windows.Forms.GroupBox metricGroupBox;
        private System.Windows.Forms.Label adapterMetricLabel;
        private System.Windows.Forms.Label routeMetricLabel;
        private System.Windows.Forms.NumericUpDown routeMetricNumericUpDown;
        private System.Windows.Forms.Button exitButton;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.Label msLabel;
        private System.Windows.Forms.TextBox routeDestinationTextBox;
        private System.Windows.Forms.ToolTip routeDestinationToolTip;
    }
}