using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Collections;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PhotoAlbum
{
    public partial class Options : Form
    {
        private PhotoAlbum parent;
        private bool modified;
        public Options(PhotoAlbum parent)
        {
            InitializeComponent();
            this.parent = parent;
            initializeOptions();
        }

        private void initializeOptions()
        {
            string result = parent.isProgIdRegistered("jpeg", "jpg");
            if (result == "yes")
            {
                checkBoxJpg.Checked = true;
            }
            result = parent.isProgIdRegistered("jpeg", "jpeg");
            if (result == "yes")
            {
                checkBoxJpeg.Checked = true;
            }
            result = parent.isProgIdRegistered("jpeg", "jpe");
            if (result == "yes")
            {
                checkBoxJpe.Checked = true;
            }
            result = parent.isProgIdRegistered("png", "png");
            if (result == "yes")
            {
                checkBoxPng.Checked = true;
            }
            result = parent.isProgIdRegistered("gif", "gif");
            if (result == "yes")
            {
                checkBoxGif.Checked = true;
            }
            result = parent.isProgIdRegistered("tiff", "tiff");
            if (result == "yes")
            {
                checkBoxTiff.Checked = true;
            }
            result = parent.isProgIdRegistered("bmp", "bmp");
            if (result == "yes")
            {
                checkBoxBmp.Checked = true;
            }

            pictureBoxBackgroundColour.BackColor = parent.BackgroundColour;
            checkBoxLoop.Checked = parent.Loop;
            checkBoxScanDir.Checked = parent.ScanDirectory;
            checkBoxAllowUrl.Checked = parent.AllowUrls;
            checkBoxTopmost.Checked = parent.TopMost;
            checkBoxExitWithEsc.Checked = parent.ExitEsc;
            checkBoxAllowMultipleWindows.Checked = parent.AllowMultipleWindows;
            checkBoxFitImage.Checked = parent.FitImage;
        }

        private void buttonChangeBgColour_Click(object sender, EventArgs e)
        {
            if (parent.colorDialog.ShowDialog(this) == DialogResult.OK)
            {
                if (pictureBoxBackgroundColour.BackColor != parent.colorDialog.Color)
                {
                    pictureBoxBackgroundColour.BackColor = parent.colorDialog.Color;
                    Modified = true;
                }
            }
        }

        private void checkBox_CheckedChanged(object sender, EventArgs e)
        {
            if (Visible)
                Modified = true;
        }

        private bool Modified { 
            set
            {
                buttonApply.Enabled = modified = value;
            }
            get
            {
                return modified;
            }
        }

        private void buttonApply_Click(object sender, EventArgs e)
        {
            saveChanges();
        }

        private void saveChanges()
        {
            Modified = false;
            parent.BackgroundColour = pictureBoxBackgroundColour.BackColor;
            parent.Loop = checkBoxLoop.Checked;
            parent.AllowUrls = checkBoxAllowUrl.Checked;
            parent.ScanDirectory = checkBoxScanDir.Checked;
            parent.TopMost = checkBoxTopmost.Checked;
            parent.ExitEsc = checkBoxExitWithEsc.Checked;
            parent.ResetZoom = checkBoxResetZoom.Checked;
            parent.AllowMultipleWindows = checkBoxAllowMultipleWindows.Checked;
            parent.FitImage = checkBoxFitImage.Checked;
            ArrayList ext = new ArrayList();
            if (checkBoxJpeg.Checked)
            {
                ext.Add("jpeg");
            }
            if (checkBoxJpe.Checked)
            {
                ext.Add("jpe");
            }
            if (checkBoxJpg.Checked)
            {
                ext.Add("jpg");
            }
            if (ext.Count > 0)
            {
                parent.registerProgId("jpeg", (string[])ext.ToArray(typeof(string)), "Jpeg Image", 303);
            }
            if (checkBoxPng.Checked)
            {
                parent.registerProgId("png", new string[] { "png" }, "Png Image", -4);
            }
            if (checkBoxGif.Checked)
            {
                parent.registerProgId("gif", new string[] { "gif" }, "Gif Image", 0);
            }
            if (checkBoxBmp.Checked)
            {
                parent.registerProgId("bmp", new string[] { "bmp" }, "Bitmapped Image", 0);
            }
            if (checkBoxTiff.Checked)
            {
                parent.registerProgId("tiff", new string[] { "tiff" }, "Tiff Image", 0);
            }
        }

        private void buttonOkay_Click(object sender, EventArgs e)
        {
            saveChanges();
        }

        internal void show()
        {
            if (ShowDialog(parent) == System.Windows.Forms.DialogResult.OK)
            {

            }
        }
    }
}
