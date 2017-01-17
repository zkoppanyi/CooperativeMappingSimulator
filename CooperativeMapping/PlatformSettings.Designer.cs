namespace CooperativeMapping
{
    partial class PlatformSettings
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
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPageSettings = new System.Windows.Forms.TabPage();
            this.groupBoxCommunication = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.comboBoxCommunicationModel = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBoxController = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.comboBoxController = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tabPagePlatform = new System.Windows.Forms.TabPage();
            this.propertyGrid = new System.Windows.Forms.PropertyGrid();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.tabPageController = new System.Windows.Forms.TabPage();
            this.propertyGridStrategy = new System.Windows.Forms.PropertyGrid();
            this.tabPageCommunication = new System.Windows.Forms.TabPage();
            this.propertyGridCommunication = new System.Windows.Forms.PropertyGrid();
            this.tabControl.SuspendLayout();
            this.tabPageSettings.SuspendLayout();
            this.groupBoxCommunication.SuspendLayout();
            this.groupBoxController.SuspendLayout();
            this.tabPagePlatform.SuspendLayout();
            this.tabPageController.SuspendLayout();
            this.tabPageCommunication.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl.Controls.Add(this.tabPageSettings);
            this.tabControl.Controls.Add(this.tabPagePlatform);
            this.tabControl.Controls.Add(this.tabPageController);
            this.tabControl.Controls.Add(this.tabPageCommunication);
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(466, 319);
            this.tabControl.TabIndex = 0;
            this.tabControl.SelectedIndexChanged += new System.EventHandler(this.tabControl_SelectedIndexChanged);
            // 
            // tabPageSettings
            // 
            this.tabPageSettings.Controls.Add(this.groupBoxCommunication);
            this.tabPageSettings.Controls.Add(this.groupBoxController);
            this.tabPageSettings.Location = new System.Drawing.Point(4, 22);
            this.tabPageSettings.Name = "tabPageSettings";
            this.tabPageSettings.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageSettings.Size = new System.Drawing.Size(458, 293);
            this.tabPageSettings.TabIndex = 0;
            this.tabPageSettings.Text = "Settings";
            this.tabPageSettings.UseVisualStyleBackColor = true;
            // 
            // groupBoxCommunication
            // 
            this.groupBoxCommunication.Controls.Add(this.label3);
            this.groupBoxCommunication.Controls.Add(this.comboBoxCommunicationModel);
            this.groupBoxCommunication.Controls.Add(this.label4);
            this.groupBoxCommunication.Location = new System.Drawing.Point(3, 120);
            this.groupBoxCommunication.Name = "groupBoxCommunication";
            this.groupBoxCommunication.Size = new System.Drawing.Size(447, 88);
            this.groupBoxCommunication.TabIndex = 3;
            this.groupBoxCommunication.TabStop = false;
            this.groupBoxCommunication.Text = "Communication";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 25);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(151, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Choose communication model:";
            // 
            // comboBoxCommunicationModel
            // 
            this.comboBoxCommunicationModel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxCommunicationModel.FormattingEnabled = true;
            this.comboBoxCommunicationModel.Location = new System.Drawing.Point(6, 48);
            this.comboBoxCommunicationModel.Name = "comboBoxCommunicationModel";
            this.comboBoxCommunicationModel.Size = new System.Drawing.Size(432, 21);
            this.comboBoxCommunicationModel.TabIndex = 1;
            this.comboBoxCommunicationModel.SelectedIndexChanged += new System.EventHandler(this.comboBoxMap_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 25);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(0, 13);
            this.label4.TabIndex = 0;
            // 
            // groupBoxController
            // 
            this.groupBoxController.Controls.Add(this.label2);
            this.groupBoxController.Controls.Add(this.comboBoxController);
            this.groupBoxController.Controls.Add(this.label1);
            this.groupBoxController.Location = new System.Drawing.Point(3, 6);
            this.groupBoxController.Name = "groupBoxController";
            this.groupBoxController.Size = new System.Drawing.Size(447, 108);
            this.groupBoxController.TabIndex = 0;
            this.groupBoxController.TabStop = false;
            this.groupBoxController.Text = "Controller";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 25);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(92, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Choose controller:";
            // 
            // comboBoxController
            // 
            this.comboBoxController.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxController.FormattingEnabled = true;
            this.comboBoxController.Location = new System.Drawing.Point(6, 48);
            this.comboBoxController.Name = "comboBoxController";
            this.comboBoxController.Size = new System.Drawing.Size(432, 21);
            this.comboBoxController.TabIndex = 1;
            this.comboBoxController.SelectedIndexChanged += new System.EventHandler(this.comboBoxController_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(0, 13);
            this.label1.TabIndex = 0;
            // 
            // tabPagePlatform
            // 
            this.tabPagePlatform.Controls.Add(this.propertyGrid);
            this.tabPagePlatform.Location = new System.Drawing.Point(4, 22);
            this.tabPagePlatform.Name = "tabPagePlatform";
            this.tabPagePlatform.Padding = new System.Windows.Forms.Padding(3);
            this.tabPagePlatform.Size = new System.Drawing.Size(458, 293);
            this.tabPagePlatform.TabIndex = 1;
            this.tabPagePlatform.Text = "Platform";
            this.tabPagePlatform.UseVisualStyleBackColor = true;
            // 
            // propertyGrid
            // 
            this.propertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid.Location = new System.Drawing.Point(3, 3);
            this.propertyGrid.Name = "propertyGrid";
            this.propertyGrid.Size = new System.Drawing.Size(452, 287);
            this.propertyGrid.TabIndex = 0;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(351, 321);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(108, 41);
            this.button1.TabIndex = 1;
            this.button1.Text = "OK";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(237, 321);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(108, 41);
            this.button2.TabIndex = 2;
            this.button2.Text = "Delete";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // tabPageController
            // 
            this.tabPageController.Controls.Add(this.propertyGridStrategy);
            this.tabPageController.Location = new System.Drawing.Point(4, 22);
            this.tabPageController.Name = "tabPageController";
            this.tabPageController.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageController.Size = new System.Drawing.Size(458, 293);
            this.tabPageController.TabIndex = 2;
            this.tabPageController.Text = "Controller";
            this.tabPageController.UseVisualStyleBackColor = true;
            this.tabPageController.Click += new System.EventHandler(this.tabPageStrategy_Click);
            // 
            // propertyGridStrategy
            // 
            this.propertyGridStrategy.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGridStrategy.Location = new System.Drawing.Point(3, 3);
            this.propertyGridStrategy.Name = "propertyGridStrategy";
            this.propertyGridStrategy.Size = new System.Drawing.Size(452, 287);
            this.propertyGridStrategy.TabIndex = 1;
            // 
            // tabPageCommunication
            // 
            this.tabPageCommunication.Controls.Add(this.propertyGridCommunication);
            this.tabPageCommunication.Location = new System.Drawing.Point(4, 22);
            this.tabPageCommunication.Name = "tabPageCommunication";
            this.tabPageCommunication.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageCommunication.Size = new System.Drawing.Size(458, 293);
            this.tabPageCommunication.TabIndex = 3;
            this.tabPageCommunication.Text = "Communication";
            this.tabPageCommunication.UseVisualStyleBackColor = true;
            // 
            // propertyGridCommunication
            // 
            this.propertyGridCommunication.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGridCommunication.Location = new System.Drawing.Point(3, 3);
            this.propertyGridCommunication.Name = "propertyGridCommunication";
            this.propertyGridCommunication.Size = new System.Drawing.Size(452, 287);
            this.propertyGridCommunication.TabIndex = 2;
            // 
            // PlatformSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(466, 364);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.tabControl);
            this.Name = "PlatformSettings";
            this.Text = "Platform Settings...";
            this.Load += new System.EventHandler(this.PlatformSettings_Load);
            this.tabControl.ResumeLayout(false);
            this.tabPageSettings.ResumeLayout(false);
            this.groupBoxCommunication.ResumeLayout(false);
            this.groupBoxCommunication.PerformLayout();
            this.groupBoxController.ResumeLayout(false);
            this.groupBoxController.PerformLayout();
            this.tabPagePlatform.ResumeLayout(false);
            this.tabPageController.ResumeLayout(false);
            this.tabPageCommunication.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabPageSettings;
        private System.Windows.Forms.TabPage tabPagePlatform;
        private System.Windows.Forms.PropertyGrid propertyGrid;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.GroupBox groupBoxController;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comboBoxController;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBoxCommunication;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox comboBoxCommunicationModel;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TabPage tabPageController;
        private System.Windows.Forms.PropertyGrid propertyGridStrategy;
        private System.Windows.Forms.TabPage tabPageCommunication;
        private System.Windows.Forms.PropertyGrid propertyGridCommunication;
    }
}