namespace PhotoAlbum
{
    partial class Options
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
            this.tabGeneral = new System.Windows.Forms.TabPage();
            this.checkBoxExitWithEsc = new System.Windows.Forms.CheckBox();
            this.checkBoxResetZoom = new System.Windows.Forms.CheckBox();
            this.checkBoxTopmost = new System.Windows.Forms.CheckBox();
            this.checkBoxAllowMultipleWindows = new System.Windows.Forms.CheckBox();
            this.checkBoxAllowUrl = new System.Windows.Forms.CheckBox();
            this.checkBoxScanDir = new System.Windows.Forms.CheckBox();
            this.checkBoxLoop = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.buttonChangeBgColour = new System.Windows.Forms.Button();
            this.pictureBoxBackgroundColour = new System.Windows.Forms.PictureBox();
            this.tabFileAssociation = new System.Windows.Forms.TabPage();
            this.faLabel1 = new System.Windows.Forms.Label();
            this.checkBoxTiff = new System.Windows.Forms.CheckBox();
            this.checkBoxGif = new System.Windows.Forms.CheckBox();
            this.checkBoxBmp = new System.Windows.Forms.CheckBox();
            this.checkBoxPng = new System.Windows.Forms.CheckBox();
            this.checkBoxJpg = new System.Windows.Forms.CheckBox();
            this.checkBoxJpe = new System.Windows.Forms.CheckBox();
            this.checkBoxJpeg = new System.Windows.Forms.CheckBox();
            this.buttonOkay = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonApply = new System.Windows.Forms.Button();
            this.checkBoxFitImage = new System.Windows.Forms.CheckBox();
            this.tabControl.SuspendLayout();
            this.tabGeneral.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxBackgroundColour)).BeginInit();
            this.tabFileAssociation.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabGeneral);
            this.tabControl.Controls.Add(this.tabFileAssociation);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Top;
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(387, 216);
            this.tabControl.TabIndex = 0;
            // 
            // tabGeneral
            // 
            this.tabGeneral.Controls.Add(this.checkBoxFitImage);
            this.tabGeneral.Controls.Add(this.checkBoxExitWithEsc);
            this.tabGeneral.Controls.Add(this.checkBoxResetZoom);
            this.tabGeneral.Controls.Add(this.checkBoxTopmost);
            this.tabGeneral.Controls.Add(this.checkBoxAllowMultipleWindows);
            this.tabGeneral.Controls.Add(this.checkBoxAllowUrl);
            this.tabGeneral.Controls.Add(this.checkBoxScanDir);
            this.tabGeneral.Controls.Add(this.checkBoxLoop);
            this.tabGeneral.Controls.Add(this.groupBox1);
            this.tabGeneral.Location = new System.Drawing.Point(4, 22);
            this.tabGeneral.Name = "tabGeneral";
            this.tabGeneral.Padding = new System.Windows.Forms.Padding(3);
            this.tabGeneral.Size = new System.Drawing.Size(379, 190);
            this.tabGeneral.TabIndex = 2;
            this.tabGeneral.Text = "General";
            this.tabGeneral.UseVisualStyleBackColor = true;
            // 
            // checkBoxExitWithEsc
            // 
            this.checkBoxExitWithEsc.AutoSize = true;
            this.checkBoxExitWithEsc.Location = new System.Drawing.Point(8, 167);
            this.checkBoxExitWithEsc.Name = "checkBoxExitWithEsc";
            this.checkBoxExitWithEsc.Size = new System.Drawing.Size(89, 17);
            this.checkBoxExitWithEsc.TabIndex = 12;
            this.checkBoxExitWithEsc.Text = "Exit With Esc";
            this.checkBoxExitWithEsc.UseVisualStyleBackColor = true;
            // 
            // checkBoxResetZoom
            // 
            this.checkBoxResetZoom.AutoSize = true;
            this.checkBoxResetZoom.Location = new System.Drawing.Point(8, 144);
            this.checkBoxResetZoom.Name = "checkBoxResetZoom";
            this.checkBoxResetZoom.Size = new System.Drawing.Size(82, 17);
            this.checkBoxResetZoom.TabIndex = 11;
            this.checkBoxResetZoom.Text = "Reset zoom";
            this.checkBoxResetZoom.UseVisualStyleBackColor = true;
            this.checkBoxResetZoom.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // checkBoxTopmost
            // 
            this.checkBoxTopmost.AutoSize = true;
            this.checkBoxTopmost.Location = new System.Drawing.Point(8, 98);
            this.checkBoxTopmost.Name = "checkBoxTopmost";
            this.checkBoxTopmost.Size = new System.Drawing.Size(67, 17);
            this.checkBoxTopmost.TabIndex = 10;
            this.checkBoxTopmost.Text = "Topmost";
            this.checkBoxTopmost.UseVisualStyleBackColor = true;
            this.checkBoxTopmost.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // checkBoxAllowMultipleWindows
            // 
            this.checkBoxAllowMultipleWindows.AutoSize = true;
            this.checkBoxAllowMultipleWindows.Checked = true;
            this.checkBoxAllowMultipleWindows.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxAllowMultipleWindows.Location = new System.Drawing.Point(8, 75);
            this.checkBoxAllowMultipleWindows.Name = "checkBoxAllowMultipleWindows";
            this.checkBoxAllowMultipleWindows.Size = new System.Drawing.Size(133, 17);
            this.checkBoxAllowMultipleWindows.TabIndex = 9;
            this.checkBoxAllowMultipleWindows.Text = "Allow multiple windows";
            this.checkBoxAllowMultipleWindows.UseVisualStyleBackColor = true;
            this.checkBoxAllowMultipleWindows.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // checkBoxAllowUrl
            // 
            this.checkBoxAllowUrl.AutoSize = true;
            this.checkBoxAllowUrl.Checked = true;
            this.checkBoxAllowUrl.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxAllowUrl.Location = new System.Drawing.Point(8, 52);
            this.checkBoxAllowUrl.Name = "checkBoxAllowUrl";
            this.checkBoxAllowUrl.Size = new System.Drawing.Size(81, 17);
            this.checkBoxAllowUrl.TabIndex = 8;
            this.checkBoxAllowUrl.Text = "Accept Urls";
            this.checkBoxAllowUrl.UseVisualStyleBackColor = true;
            this.checkBoxAllowUrl.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // checkBoxScanDir
            // 
            this.checkBoxScanDir.AutoSize = true;
            this.checkBoxScanDir.Checked = true;
            this.checkBoxScanDir.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxScanDir.Location = new System.Drawing.Point(8, 29);
            this.checkBoxScanDir.Name = "checkBoxScanDir";
            this.checkBoxScanDir.Size = new System.Drawing.Size(149, 17);
            this.checkBoxScanDir.TabIndex = 7;
            this.checkBoxScanDir.Text = "Scan directory of open file";
            this.checkBoxScanDir.UseVisualStyleBackColor = true;
            this.checkBoxScanDir.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // checkBoxLoop
            // 
            this.checkBoxLoop.AutoSize = true;
            this.checkBoxLoop.Checked = true;
            this.checkBoxLoop.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxLoop.Location = new System.Drawing.Point(8, 6);
            this.checkBoxLoop.Name = "checkBoxLoop";
            this.checkBoxLoop.Size = new System.Drawing.Size(50, 17);
            this.checkBoxLoop.TabIndex = 6;
            this.checkBoxLoop.Text = "Loop";
            this.checkBoxLoop.UseVisualStyleBackColor = true;
            this.checkBoxLoop.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.buttonChangeBgColour);
            this.groupBox1.Controls.Add(this.pictureBoxBackgroundColour);
            this.groupBox1.Location = new System.Drawing.Point(257, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(124, 57);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Background Colour";
            // 
            // buttonChangeBgColour
            // 
            this.buttonChangeBgColour.Location = new System.Drawing.Point(37, 19);
            this.buttonChangeBgColour.Name = "buttonChangeBgColour";
            this.buttonChangeBgColour.Size = new System.Drawing.Size(64, 25);
            this.buttonChangeBgColour.TabIndex = 1;
            this.buttonChangeBgColour.Text = "Change";
            this.buttonChangeBgColour.UseVisualStyleBackColor = true;
            this.buttonChangeBgColour.Click += new System.EventHandler(this.buttonChangeBgColour_Click);
            // 
            // pictureBoxBackgroundColour
            // 
            this.pictureBoxBackgroundColour.BackColor = System.Drawing.Color.Black;
            this.pictureBoxBackgroundColour.Location = new System.Drawing.Point(6, 19);
            this.pictureBoxBackgroundColour.Name = "pictureBoxBackgroundColour";
            this.pictureBoxBackgroundColour.Size = new System.Drawing.Size(25, 25);
            this.pictureBoxBackgroundColour.TabIndex = 0;
            this.pictureBoxBackgroundColour.TabStop = false;
            this.pictureBoxBackgroundColour.DoubleClick += new System.EventHandler(this.buttonChangeBgColour_Click);
            // 
            // tabFileAssociation
            // 
            this.tabFileAssociation.Controls.Add(this.faLabel1);
            this.tabFileAssociation.Controls.Add(this.checkBoxTiff);
            this.tabFileAssociation.Controls.Add(this.checkBoxGif);
            this.tabFileAssociation.Controls.Add(this.checkBoxBmp);
            this.tabFileAssociation.Controls.Add(this.checkBoxPng);
            this.tabFileAssociation.Controls.Add(this.checkBoxJpg);
            this.tabFileAssociation.Controls.Add(this.checkBoxJpe);
            this.tabFileAssociation.Controls.Add(this.checkBoxJpeg);
            this.tabFileAssociation.Location = new System.Drawing.Point(4, 22);
            this.tabFileAssociation.Name = "tabFileAssociation";
            this.tabFileAssociation.Padding = new System.Windows.Forms.Padding(3);
            this.tabFileAssociation.Size = new System.Drawing.Size(379, 182);
            this.tabFileAssociation.TabIndex = 3;
            this.tabFileAssociation.Text = "File Associations";
            this.tabFileAssociation.UseVisualStyleBackColor = true;
            // 
            // faLabel1
            // 
            this.faLabel1.AutoSize = true;
            this.faLabel1.Location = new System.Drawing.Point(8, 3);
            this.faLabel1.Name = "faLabel1";
            this.faLabel1.Size = new System.Drawing.Size(287, 13);
            this.faLabel1.TabIndex = 15;
            this.faLabel1.Text = "Select the file types you want to associate with PhotoAlbum";
            // 
            // checkBoxTiff
            // 
            this.checkBoxTiff.AutoSize = true;
            this.checkBoxTiff.Location = new System.Drawing.Point(8, 132);
            this.checkBoxTiff.Name = "checkBoxTiff";
            this.checkBoxTiff.Size = new System.Drawing.Size(37, 17);
            this.checkBoxTiff.TabIndex = 14;
            this.checkBoxTiff.Text = "tiff";
            this.checkBoxTiff.UseVisualStyleBackColor = true;
            this.checkBoxTiff.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // checkBoxGif
            // 
            this.checkBoxGif.AutoSize = true;
            this.checkBoxGif.Location = new System.Drawing.Point(8, 109);
            this.checkBoxGif.Name = "checkBoxGif";
            this.checkBoxGif.Size = new System.Drawing.Size(37, 17);
            this.checkBoxGif.TabIndex = 13;
            this.checkBoxGif.Text = "gif";
            this.checkBoxGif.UseVisualStyleBackColor = true;
            this.checkBoxGif.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // checkBoxBmp
            // 
            this.checkBoxBmp.AutoSize = true;
            this.checkBoxBmp.Location = new System.Drawing.Point(8, 86);
            this.checkBoxBmp.Name = "checkBoxBmp";
            this.checkBoxBmp.Size = new System.Drawing.Size(46, 17);
            this.checkBoxBmp.TabIndex = 12;
            this.checkBoxBmp.Text = "bmp";
            this.checkBoxBmp.UseVisualStyleBackColor = true;
            this.checkBoxBmp.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // checkBoxPng
            // 
            this.checkBoxPng.AutoSize = true;
            this.checkBoxPng.Location = new System.Drawing.Point(8, 63);
            this.checkBoxPng.Name = "checkBoxPng";
            this.checkBoxPng.Size = new System.Drawing.Size(44, 17);
            this.checkBoxPng.TabIndex = 11;
            this.checkBoxPng.Text = "png";
            this.checkBoxPng.UseVisualStyleBackColor = true;
            this.checkBoxPng.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // checkBoxJpg
            // 
            this.checkBoxJpg.AutoSize = true;
            this.checkBoxJpg.Location = new System.Drawing.Point(106, 40);
            this.checkBoxJpg.Name = "checkBoxJpg";
            this.checkBoxJpg.Size = new System.Drawing.Size(40, 17);
            this.checkBoxJpg.TabIndex = 10;
            this.checkBoxJpg.Text = "jpg";
            this.checkBoxJpg.UseVisualStyleBackColor = true;
            this.checkBoxJpg.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // checkBoxJpe
            // 
            this.checkBoxJpe.AutoSize = true;
            this.checkBoxJpe.Location = new System.Drawing.Point(60, 40);
            this.checkBoxJpe.Name = "checkBoxJpe";
            this.checkBoxJpe.Size = new System.Drawing.Size(40, 17);
            this.checkBoxJpe.TabIndex = 9;
            this.checkBoxJpe.Text = "jpe";
            this.checkBoxJpe.UseVisualStyleBackColor = true;
            this.checkBoxJpe.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // checkBoxJpeg
            // 
            this.checkBoxJpeg.AutoSize = true;
            this.checkBoxJpeg.Location = new System.Drawing.Point(8, 40);
            this.checkBoxJpeg.Name = "checkBoxJpeg";
            this.checkBoxJpeg.Size = new System.Drawing.Size(46, 17);
            this.checkBoxJpeg.TabIndex = 8;
            this.checkBoxJpeg.Text = "jpeg";
            this.checkBoxJpeg.UseVisualStyleBackColor = true;
            this.checkBoxJpeg.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // buttonOkay
            // 
            this.buttonOkay.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOkay.Location = new System.Drawing.Point(146, 222);
            this.buttonOkay.Name = "buttonOkay";
            this.buttonOkay.Size = new System.Drawing.Size(75, 23);
            this.buttonOkay.TabIndex = 1;
            this.buttonOkay.Text = "Okay";
            this.buttonOkay.UseVisualStyleBackColor = true;
            this.buttonOkay.Click += new System.EventHandler(this.buttonOkay_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(308, 222);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 2;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // buttonApply
            // 
            this.buttonApply.Enabled = false;
            this.buttonApply.Location = new System.Drawing.Point(227, 222);
            this.buttonApply.Name = "buttonApply";
            this.buttonApply.Size = new System.Drawing.Size(75, 23);
            this.buttonApply.TabIndex = 3;
            this.buttonApply.Text = "Apply";
            this.buttonApply.UseVisualStyleBackColor = true;
            this.buttonApply.Click += new System.EventHandler(this.buttonApply_Click);
            // 
            // checkBoxFitImage
            // 
            this.checkBoxFitImage.AutoSize = true;
            this.checkBoxFitImage.Checked = true;
            this.checkBoxFitImage.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxFitImage.Location = new System.Drawing.Point(8, 121);
            this.checkBoxFitImage.Name = "checkBoxFitImage";
            this.checkBoxFitImage.Size = new System.Drawing.Size(68, 17);
            this.checkBoxFitImage.TabIndex = 13;
            this.checkBoxFitImage.Text = "Fit image";
            this.checkBoxFitImage.UseVisualStyleBackColor = true;
            this.checkBoxFitImage.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // Options
            // 
            this.AcceptButton = this.buttonOkay;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(387, 251);
            this.Controls.Add(this.buttonApply);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOkay);
            this.Controls.Add(this.tabControl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Options";
            this.Text = "Options";
            this.tabControl.ResumeLayout(false);
            this.tabGeneral.ResumeLayout(false);
            this.tabGeneral.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxBackgroundColour)).EndInit();
            this.tabFileAssociation.ResumeLayout(false);
            this.tabFileAssociation.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabGeneral;
        private System.Windows.Forms.Button buttonOkay;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonApply;
        private System.Windows.Forms.TabPage tabFileAssociation;
        private System.Windows.Forms.CheckBox checkBoxAllowUrl;
        private System.Windows.Forms.CheckBox checkBoxScanDir;
        private System.Windows.Forms.CheckBox checkBoxLoop;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button buttonChangeBgColour;
        private System.Windows.Forms.PictureBox pictureBoxBackgroundColour;
        private System.Windows.Forms.Label faLabel1;
        private System.Windows.Forms.CheckBox checkBoxTiff;
        private System.Windows.Forms.CheckBox checkBoxGif;
        private System.Windows.Forms.CheckBox checkBoxBmp;
        private System.Windows.Forms.CheckBox checkBoxPng;
        private System.Windows.Forms.CheckBox checkBoxJpg;
        private System.Windows.Forms.CheckBox checkBoxJpe;
        private System.Windows.Forms.CheckBox checkBoxJpeg;
        private System.Windows.Forms.CheckBox checkBoxAllowMultipleWindows;
        private System.Windows.Forms.CheckBox checkBoxTopmost;
        private System.Windows.Forms.CheckBox checkBoxResetZoom;
        private System.Windows.Forms.CheckBox checkBoxExitWithEsc;
        private System.Windows.Forms.CheckBox checkBoxFitImage;
    }
}