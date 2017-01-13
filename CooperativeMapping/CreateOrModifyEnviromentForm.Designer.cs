namespace CooperativeMapping
{
    partial class CreateOrModifyEnviromentForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CreateOrModifyEnviromentForm));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.parametersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.limitsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mapImageBox = new System.Windows.Forms.PictureBox();
            this.propertyGridEnviroment = new System.Windows.Forms.PropertyGrid();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonObstacle = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonUndiscovered = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonDiscovered = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonPlatform = new System.Windows.Forms.ToolStripButton();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabelSelectedBinType = new System.Windows.Forms.ToolStripStatusLabel();
            this.textToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mapImageBox)).BeginInit();
            this.toolStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.parametersToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(804, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            this.menuStrip1.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.menuStrip1_ItemClicked);
            // 
            // parametersToolStripMenuItem
            // 
            this.parametersToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.limitsToolStripMenuItem,
            this.textToolStripMenuItem});
            this.parametersToolStripMenuItem.Name = "parametersToolStripMenuItem";
            this.parametersToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.parametersToolStripMenuItem.Text = "File";
            // 
            // limitsToolStripMenuItem
            // 
            this.limitsToolStripMenuItem.Name = "limitsToolStripMenuItem";
            this.limitsToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.limitsToolStripMenuItem.Text = "Save...";
            this.limitsToolStripMenuItem.Click += new System.EventHandler(this.limitsToolStripMenuItem_Click);
            // 
            // mapImageBox
            // 
            this.mapImageBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mapImageBox.BackColor = System.Drawing.Color.White;
            this.mapImageBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.mapImageBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mapImageBox.Location = new System.Drawing.Point(0, 52);
            this.mapImageBox.Name = "mapImageBox";
            this.mapImageBox.Size = new System.Drawing.Size(514, 464);
            this.mapImageBox.TabIndex = 3;
            this.mapImageBox.TabStop = false;
            this.mapImageBox.Click += new System.EventHandler(this.mapImageBox_Click);
            this.mapImageBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.mapImageBox_MouseUp);
            // 
            // propertyGridEnviroment
            // 
            this.propertyGridEnviroment.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.propertyGridEnviroment.Location = new System.Drawing.Point(520, 52);
            this.propertyGridEnviroment.Name = "propertyGridEnviroment";
            this.propertyGridEnviroment.Size = new System.Drawing.Size(272, 464);
            this.propertyGridEnviroment.TabIndex = 4;
            this.propertyGridEnviroment.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.propertyGridEnviroment_PropertyValueChanged);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton1,
            this.toolStripButtonObstacle,
            this.toolStripButtonUndiscovered,
            this.toolStripButtonDiscovered,
            this.toolStripButtonPlatform});
            this.toolStrip1.Location = new System.Drawing.Point(0, 24);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(804, 25);
            this.toolStrip1.TabIndex = 5;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton1.Text = "toolStripButton1";
            this.toolStripButton1.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // toolStripButtonObstacle
            // 
            this.toolStripButtonObstacle.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButtonObstacle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStripButtonObstacle.ForeColor = System.Drawing.Color.Red;
            this.toolStripButtonObstacle.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonObstacle.Name = "toolStripButtonObstacle";
            this.toolStripButtonObstacle.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonObstacle.Text = "O";
            this.toolStripButtonObstacle.TextImageRelation = System.Windows.Forms.TextImageRelation.Overlay;
            this.toolStripButtonObstacle.Click += new System.EventHandler(this.toolStripButtonObstacle_Click);
            // 
            // toolStripButtonUndiscovered
            // 
            this.toolStripButtonUndiscovered.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButtonUndiscovered.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStripButtonUndiscovered.ForeColor = System.Drawing.Color.Red;
            this.toolStripButtonUndiscovered.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonUndiscovered.Name = "toolStripButtonUndiscovered";
            this.toolStripButtonUndiscovered.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonUndiscovered.Text = "U";
            this.toolStripButtonUndiscovered.Click += new System.EventHandler(this.toolStripButtonUndiscovered_Click);
            // 
            // toolStripButtonDiscovered
            // 
            this.toolStripButtonDiscovered.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButtonDiscovered.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStripButtonDiscovered.ForeColor = System.Drawing.Color.Red;
            this.toolStripButtonDiscovered.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonDiscovered.Name = "toolStripButtonDiscovered";
            this.toolStripButtonDiscovered.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonDiscovered.Text = "D";
            this.toolStripButtonDiscovered.Click += new System.EventHandler(this.toolStripButtonDiscovered_Click);
            // 
            // toolStripButtonPlatform
            // 
            this.toolStripButtonPlatform.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButtonPlatform.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStripButtonPlatform.ForeColor = System.Drawing.Color.Red;
            this.toolStripButtonPlatform.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonPlatform.Name = "toolStripButtonPlatform";
            this.toolStripButtonPlatform.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonPlatform.Text = "P";
            this.toolStripButtonPlatform.Click += new System.EventHandler(this.toolStripButtonPlatform_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel,
            this.toolStripStatusLabelSelectedBinType});
            this.statusStrip1.Location = new System.Drawing.Point(0, 519);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(804, 22);
            this.statusStrip1.TabIndex = 6;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel
            // 
            this.toolStripStatusLabel.Name = "toolStripStatusLabel";
            this.toolStripStatusLabel.Size = new System.Drawing.Size(50, 17);
            this.toolStripStatusLabel.Text = "Status: -";
            // 
            // toolStripStatusLabelSelectedBinType
            // 
            this.toolStripStatusLabelSelectedBinType.Name = "toolStripStatusLabelSelectedBinType";
            this.toolStripStatusLabelSelectedBinType.Size = new System.Drawing.Size(53, 17);
            this.toolStripStatusLabelSelectedBinType.Text = "Bin type:";
            // 
            // textToolStripMenuItem
            // 
            this.textToolStripMenuItem.Name = "textToolStripMenuItem";
            this.textToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.textToolStripMenuItem.Text = "Text";
            this.textToolStripMenuItem.Click += new System.EventHandler(this.textToolStripMenuItem_Click);
            // 
            // CreateOrModifyEnviromentForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(804, 541);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.propertyGridEnviroment);
            this.Controls.Add(this.mapImageBox);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "CreateOrModifyEnviromentForm";
            this.Text = "Create or Modify Enviroment...";
            this.Load += new System.EventHandler(this.CreateOrModifyEnviroment_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mapImageBox)).EndInit();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem parametersToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem limitsToolStripMenuItem;
        private System.Windows.Forms.PictureBox mapImageBox;
        private System.Windows.Forms.PropertyGrid propertyGridEnviroment;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelSelectedBinType;
        private System.Windows.Forms.ToolStripButton toolStripButtonObstacle;
        private System.Windows.Forms.ToolStripButton toolStripButtonUndiscovered;
        private System.Windows.Forms.ToolStripButton toolStripButtonDiscovered;
        private System.Windows.Forms.ToolStripButton toolStripButtonPlatform;
        private System.Windows.Forms.ToolStripMenuItem textToolStripMenuItem;
    }
}