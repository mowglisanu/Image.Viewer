using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace PhotoAlbum
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            //try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                PhotoAlbum main = new PhotoAlbum(args);
                if (!main.exitApplication)
                {
                    Application.Run(main);
                }
            }
            //catch (Exception e)
            //{
                //MessageBox.Show(e.ToString(), "An unknown or unhandled error occured");
                //Application.Exit();
            //}
        }
    }
}
