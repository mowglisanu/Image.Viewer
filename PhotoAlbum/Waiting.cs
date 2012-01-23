using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PhotoAlbum
{
    public partial class Waiting : Form
    {
        private object sender;
        private EventHandler cancel;
        public Waiting(EventHandler cancel)
        {
            InitializeComponent();
            this.cancel = cancel;
        }
        public void setMessage(string message)
        {
            this.messaage.Text = message;
        }
        public void show(IWin32Window parent, Object sender)
        {
            this.sender = sender;
            Show(parent);
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            cancel.Invoke(sender, null);
        }
    }
}
