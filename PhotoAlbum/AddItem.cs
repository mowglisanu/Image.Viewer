using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace PhotoAlbum
{
    public partial class AddItem : Form
    {
        private PhotoAlbum parent;
        private string result;
        public static string addItem(PhotoAlbum parent)
        {
            AddItem item = new AddItem(parent);
            if (item.ShowDialog(parent) == DialogResult.OK)
            {
                return item.result;
            }
            return null;
        }

        private AddItem(PhotoAlbum parent)
        {
            InitializeComponent();
            this.parent = parent;
        }

        private void buttonOkay_Click(object sender, EventArgs e)
        {
            if (!File.Exists(textBoxItem.Text) && !Uri.IsWellFormedUriString(textBoxItem.Text, UriKind.Absolute))
            {
                MessageBox.Show("This file name is invalid!", "Error");

            }
            else
            {
                result = textBoxItem.Text;
                DialogResult = DialogResult.OK;
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            if (parent.openFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                textBoxItem.Text = parent.openFileDialog.FileName;
            }
            buttonOkay.Select();
        }

        private void AddItem_Validating(object sender, CancelEventArgs e)
        {
            MessageBox.Show(e.ToString());
        }
    }
}
