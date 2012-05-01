using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Threading;
using System.Runtime.InteropServices;
using System.Net.Sockets;
using System.Drawing.Drawing2D;
using System.Reflection;

namespace PhotoAlbum
{
    public partial class PhotoAlbum : Form
    {
        /* issues
         * clean up code /repitition
         * asyncronous
         * 
         * todo
         * FileSystemWatcher
         * resolve abnormal termination with single process[done]
         */
        private static string appName = Application.ProductName;
        private static string appExecName = Application.ExecutablePath;
        private static string appIconName = appExecName;
        private static string appPath = Application.StartupPath;
        private static string appCompany = Application.CompanyName;
        private bool fullScreen;

        private Image image;
        private bool dragHorizontal;
        private bool dragVeritcal;
        private bool dragging;
        private int mousePreviousX;
        private int mousePreviousY;
        private float clipY = 0;
        private float clipX = 0;
        private string[] fileList;
        private int imageIndex;

        private bool loop = true;
        private bool allowUrls = true;
        private bool scanDirectory = true;
        private bool allowMultipleWindows = true;
        private bool resetZoom = true;
        private bool fitimage = true;
        private Color bgColour = Color.Black;

        private Cursor hand, closeHand;
        private Hashtable icons = new Hashtable();
        private string clipboardImageText = "Clipboard Image";
        private int frames = 0;
        private string keyName = @"HKEY_CURRENT_USER\Software\Classes\" + appCompany.ToLower() + "\\" + appName.ToLower();

        private bool canRecieveKey = true;

        private enum Zoom {FitImage, ActualSize, Zoom}
        private Zoom scale = Zoom.ActualSize;
        private float[] zoomFactor = { 0.0625f, 0.125f, 0.25f, 0.5f, 0.75f, 1.0f, 1.25f, 1.50f, 1.75f, 2.0f, 4.0f, 8.0f, 16.0f, 32.0f, 64.0f, 128.0f };
        private float scaleFactor = 1.0f;
        private RectangleF bltSrcRect, bltDestRect;

        private bool hasAlpha;

        private string upgradeUrl = "http://www.outerdev.heliohost.org/Musa/applications.php";
        private string upgradeQueryString = "application=" + appName + "&request=";
        private string userAgent = "Application://" + appCompany + "." + appName;

        private bool running = true;
        private Socket server;
        private string greeting = "salam";
        public bool exitApplication = false;

        private string logFile = appPath + "\\log.txt";
        
        private bool cancelRemoteLoad;
        private bool loadingRemoteImage;
        private string remoteImage;
        private bool staticImage;
        private int scrollMultiplier;
        private bool exitEsc;

        [DllImport("shell32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern void SHChangeNotify(int wEventId, int uFlags, IntPtr dwItem1, IntPtr dwItem2);

        public PhotoAlbum(string[] args)
        {
            InitializeComponent();
            loadConfigurations();
            startUp(args);
            if (!exitApplication)
            {
                processArgs(args);
            }
        }

        private void startUp(string[] args)
        {
            hand = new Cursor(Properties.Resources.hand.GetHicon());
            closeHand = new Cursor(Properties.Resources.closeHand.GetHicon());
            canvas.BackColor = bgColour;
            panelCanvas.BackColor = bgColour;
            exitFullScreenToolStripMenuItem.Visible = false;
            toolStripSeparatorFullScreen.Visible = false;
//            loadConfigurations();
            if (!AllowMultipleWindows)
            {
                tryToConnect(args);
            }
            helpProvider.SetShowHelp(this, true);
        }

        //client
        private void tryToConnect(string[] args)
        {
            try
            {
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint EndPoint = new IPEndPoint(IPAddress.Loopback, 43219);
                socket.Connect(EndPoint);
                byte[] buffer = new byte[2046];
                socket.Send(ASCIIEncoding.ASCII.GetBytes(greeting));
                int length = socket.Receive(buffer);
                if (length == greeting.Length)
                {
                    if (ASCIIEncoding.ASCII.GetString(buffer, 0, length) == greeting)
                    {
                        string s = String.Join("\n", args);
                        socket.Send(ASCIIEncoding.ASCII.GetBytes(s));
                        exitApplication = true;
                    }
                }
                socket.Close();
            }
            catch (SocketException e)
            {
                if (e.ErrorCode == 10061)
                {
                    Thread th = new Thread(new ThreadStart(serverThread));
                    th.Start();
                }
                else
                {
                    MessageBox.Show("An unexpected error occured", appName);
                    LogException(e.ToString());
                }
            }
            catch (Exception e)
            {
                LogException(e.ToString());
            }
        }
        //server
        private void serverThread()
        {
            try
            {
                server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint EndPoint = new IPEndPoint(IPAddress.Loopback, 43219);
                server.Bind(EndPoint);
                byte[] buffer = new byte[2046];
                int length;
                while (running)
                {
                    server.Listen(10);
                    Socket socket = server.Accept();
                    length = socket.Receive(buffer);
                    if (length == greeting.Length)
                    {
                        string s = ASCIIEncoding.ASCII.GetString(buffer, 0 , length);
                        if (s == greeting)
                        {
                            socket.Send(ASCIIEncoding.ASCII.GetBytes(greeting));
                            length = socket.Receive(buffer);
                            string arg = ASCIIEncoding.ASCII.GetString(buffer, 0, length);
                            string[] array = arg.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                            if (array.Length > 0)
                            {
                                Invoke((Delegate)close, null);
                                Invoke((processArgsDelegate)processArgs, new object[] { array });
                            }
                            Invoke((Delegate)Activate);
                        }
                    }
                    socket.Close();
                }
            }
            catch (SocketException e)
            {
                if (e.ErrorCode != 10004)
                {
                    MessageBox.Show("An unknoen error occured " + e.ErrorCode, appName);
                    LogException(e.ToString());
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("An unknoen error occured" + e.ToString(), appName);
                LogException(e.ToString());
            }
        }

        private void loadConfigurations()
        {
            if (Microsoft.Win32.Registry.GetValue(keyName, "exist", greeting) == null)
            {
                loadDefaultConfigurations();
                return;
            }
            Object x = 0;
            try
            {
                x = Microsoft.Win32.Registry.GetValue(keyName, "bgcolour", 0);
            }
            catch
            {
                x = 0;
            }
            finally
            {
                BackgroundColour = Color.FromArgb((int)x);
            }
            try
            {
                Left = (int)Microsoft.Win32.Registry.GetValue(keyName, "left", null);
                Top = (int)Microsoft.Win32.Registry.GetValue(keyName, "top", null);
                Width = (int)Microsoft.Win32.Registry.GetValue(keyName, "width", null);
                Height = (int)Microsoft.Win32.Registry.GetValue(keyName, "height", null);
            }
            catch
            {
                //Console.WriteLine("We don't need no stinky exception handling");
            }

            try
            {
                x = bool.Parse((string)Microsoft.Win32.Registry.GetValue(keyName, "loop", "true"));
            }
            catch
            {
                x = true;
            }
            finally
            {
                Loop = (bool)x;
            }
            try
            {
                x = bool.Parse((string)Microsoft.Win32.Registry.GetValue(keyName, "allowurls", "true"));
            }
            catch
            {
                x = true;
            }
            finally
            {
                allowUrls = (bool)x;
            }
            try
            {
                x = bool.Parse((string)Microsoft.Win32.Registry.GetValue(keyName, "scandir", "true"));
            }
            catch
            {
                x = true;
            }
            finally
            {
                scanDirectory = (bool)x;
            }
            try
            {
                x = bool.Parse((string)Microsoft.Win32.Registry.GetValue(keyName, "maximized", "false"));
            }
            catch
            {
                x = false;
            }
            finally
            {
                WindowState = (bool)x ? FormWindowState.Maximized : FormWindowState.Normal;
            }
            try 
            {
                x = bool.Parse((string)Microsoft.Win32.Registry.GetValue(keyName, "allowmultiplewindows", "false"));
            }
            catch
            {
                x = false;
            }
            finally
            {
                allowMultipleWindows = (bool)x;
            }
            try
            {
                x = bool.Parse((string)Microsoft.Win32.Registry.GetValue(keyName, "fitimage", "true"));
            }
            catch
            {
                x = true;
            }
            finally
            {
                fitimage = (bool)x;
            }
            try
            {
                x = bool.Parse((string)Microsoft.Win32.Registry.GetValue(keyName, "resetzoom", "true"));
            }
            catch
            {
                x = true;
            }
            finally
            {
                resetZoom = (bool)x;
            }
            try
            {
                x = bool.Parse((string)Microsoft.Win32.Registry.GetValue(keyName, "topmost", "false"));
            }
            catch
            {
                x = false;
            }
            finally
            {
                TopMost = (bool)x;
            }
            try
            {
                x = bool.Parse((string)Microsoft.Win32.Registry.GetValue(keyName, "exitesc", "false"));
            }
            catch
            {
                x = false;
            }
            finally
            {
                exitEsc = (bool)x;
            }
        }

        private void loadDefaultConfigurations()
        {
            TopMost = false;
            resetZoom = true;
            fitimage = true;
            allowMultipleWindows = false;
            WindowState = FormWindowState.Normal;
            scanDirectory = true;
            allowUrls = true;
            Loop = true;
            BackgroundColour = Color.Black;
            exitEsc = false;
        }

        private void processArgs(string[] args)
        {
            if (args.Length == 1)
            {
                fileList = args;
                imageIndex = buildList(fileList[0]);
                open(imageIndex);
            }
            else if (args.Length > 1)
            {
                fileList = args;
                buildIcons();
                imageIndex = 0;
                indexToolStripInit();
                open(imageIndex);
            }
            if (fitimage)
            {
                fitImage();
            }
        }

        private void indexToolStripInit()
        {
            totalToolStripLabel.Text = fileList.Length.ToString();
            for (int i = 1; i <= fileList.Length; i++)
            {
                ToolStripMenuItem item = new ToolStripMenuItem(i.ToString());
                item.Click += new EventHandler(indexToolStripMenuItem_Click);
                indexToolStripDropDownButton.DropDownItems.Add(item);
            }
            indexToolStripDropDownButton.Enabled = true;
        }

        private void toolStripButtonOpen_Click(object sender, EventArgs e)
        {
            open();
        }

        private void open()
        {
            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                int index = 0;
                fileList = openFileDialog.FileNames;
                if (fileList.Length == 1)
                {
                    index = buildList(fileList[0]);
                }
                open(index);
            }
        }

        private void open(int imageIndex)
        {
            toolStripStatusName.Text = fileList[imageIndex];
            toolStripStatusName.Image = (Bitmap)icons[Path.GetExtension(fileList[imageIndex]).ToLower()];
            toolStripStatusIndex.Text = (imageIndex + 1) + "/" + fileList.Length;
            this.imageIndex = imageIndex;
            indexToolStripDropDownButton.Text = (imageIndex + 1).ToString();
            open(fileList[imageIndex]);
            showFormat(true || false);
        }

        private bool showFormat(bool dummy)
        {
            if (image != null)
            {
                toolStripStatusFormat.Text = image.Width + "×" + image.Height + " " + image.PixelFormat.ToString();
                if (frames > 1)
                {
                    toolStripStatusFormat.Text += " " + frames + " Frames";
                }
            }
            else
            {
                toolStripStatusFormat.Text = "Format not recognized";
            }
            return true;
        }

        private void open(string fileName)
        {
            if (image != null) image.Dispose();
            cancelRemoteLoad = true;
            if (Uri.IsWellFormedUriString(fileName, UriKind.Absolute))
            {
                loadRemoteImage(fileName);
            }
            else
            {
                try
                {
                    image = new Bitmap(fileName);
                }
                catch (ArgumentException e)
                {
                    LogException(e.ToString());
                    image = canvas.ErrorImage;
                    MessageBox.Show(this, "Unable to open file: " + fileName + "\n" + e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            checkForTransperancy();
            frames = image.GetFrameCount(new FrameDimension(image.FrameDimensionsList[0]));
            if (resetZoom)
            {
                scaleFactor = 1.0f;
            }
            if (frames > 1)
            {
                staticImage = false;
            }
            else
            {
                staticImage = true;
            }
            indexToolStripDropDownButton.Enabled = true;
            layoutCanvas(true);

        }

        private void LogException(string p)
        {
            StringBuilder log = new StringBuilder();
            log.AppendLine("----------");
            log.AppendLine("Application: " + appName);
            log.AppendLine("Time: " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"));
            log.AppendLine("File: " + (fileList == null ? "" : fileList.Length > 0 ? fileList[imageIndex] : ""));
            log.AppendLine(p);
            File.AppendAllText(logFile, log.ToString());
        }

        private void loadRemoteImage(string fileName)
        {
            if (!loadingRemoteImage)
            {
                remoteImage = fileName;
                loadingRemoteImage = true;
                canvas.Image = image = Properties.Resources.loading;
                try
                {
                    cancelRemoteLoad = false;
                    Thread th = new Thread(new ThreadStart(lia));
                    th.Start();
                }
                catch (Exception e)
                {
                    LogException(e.ToString());
                    MessageBox.Show(this, "Unable to open file: " + fileName + "\n" + e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else if (fileName == remoteImage)
            {
                loadingRemoteImage = true;
                canvas.Image = image = Properties.Resources.loading;
                cancelRemoteLoad = false;
            }
            else
            {
                loadingRemoteImage = true;
                canvas.Image = image = Properties.Resources.remoteLoadBusy;
            }
        }
        private void lia()
        {
            try
            {
                remoteLoader.Load(fileList[imageIndex]);
                if (!cancelRemoteLoad)
                {
                    image = remoteLoader.Image;
                }
            }
            catch (Exception e)//WebException UnauthorizedAccessException
            {
                LogException(e.ToString());
                if (!cancelRemoteLoad)
                {
                    image = remoteLoader.ErrorImage;
                }
                MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            loadingRemoteImage = false;
            if (!cancelRemoteLoad)
            {
                checkForTransperancy();
                Invoke(new layoutCanvasDelegate(layoutCanvas), new object[] { true });
                Invoke(new layoutCanvasDelegate(showFormat), new object[] { true || false });
            }            
        }

        private void checkForTransperancy()
        {
            if (image == null)
            {
                return;
            }
            if (image.PixelFormat == PixelFormat.Alpha ||
                image.PixelFormat == PixelFormat.Canonical ||
                image.PixelFormat == PixelFormat.Format16bppArgb1555 ||
                image.PixelFormat == PixelFormat.Format32bppArgb ||
                image.PixelFormat == PixelFormat.Format32bppPArgb ||
                image.PixelFormat == PixelFormat.Format64bppArgb ||
                image.PixelFormat == PixelFormat.Format64bppPArgb ||
                image.PixelFormat == PixelFormat.PAlpha)
            {
                hasAlpha = true;
            }
            else if (image.PixelFormat == PixelFormat.Format1bppIndexed ||
                     image.PixelFormat == PixelFormat.Format4bppIndexed ||
                     image.PixelFormat == PixelFormat.Format8bppIndexed ||
                     image.PixelFormat == PixelFormat.Indexed)
            {
                foreach (Color c in image.Palette.Entries)
                {
                    if (c.A < 255)
                    {
                        hasAlpha = true;
                        return;
                    }
                }
            }
            else
            {
                hasAlpha = false;
            }
        }

        private void PhotoAlbum_DragEnter(object sender, DragEventArgs e)
        {
            if (Array.IndexOf(e.Data.GetFormats(), "FileDrop") >= 0)
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void canvas_DragDrop(object sender, DragEventArgs e)
        {
            close();
            int index = 0;
            fileList = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (fileList.Length == 1)
            {
                index = buildList(fileList[0]);
            }
            else
            {
                indexToolStripInit();
            }
            imageIndex = index;
            open(index);
            Activate();
        }

        private int buildList(string file)
        {
            if (Uri.IsWellFormedUriString(file, UriKind.Absolute))
            {
                return 0;
            }
            int index = 0;
            try
            {
                if (scanDirectory)
                {
                    string[] newList = GetFiles(Directory.GetParent(file).FullName, "*.jpg|*.jpeg|*.jpe|*.png|*.gif|*.bmp|*.tiff");
                    if (newList.Length > 1)
                    {
                        index = Array.IndexOf(newList, file);
                        if (index >= 0)
                        {
                            fileList = newList;
                        }
                        else
                        {
                            index = 0;
                        }
                    }
                }
                buildIcons();
            }
            catch (Exception e)
            {
                LogException(e.ToString());
                index = 0;
            }
            finally
            {
                indexToolStripInit();
            }
            return index;
        }

        private void buildIcons()
        {
            foreach (string s in fileList)
            {
                string ext = Path.GetExtension(s).ToLower();
                if (!icons.ContainsKey(ext))
                {
                    Icon icon;
                    try
                    {
                        icon = Icon.ExtractAssociatedIcon(s);
                    }
                    catch (ArgumentException e)
                    {
                        LogException(e.ToString());
                        icon = SystemIcons.Application;
                    }
                    catch (FileNotFoundException e)
                    {
                        LogException(e.ToString());
                        continue;
                    }
                    catch (Exception e)
                    {
                        LogException(e.ToString());
                        icon = SystemIcons.Error; MessageBox.Show(e.ToString());
                    }
                    /*catch (FileLoadException fe)
                    {
                        icon = SystemIcons.Error;
                    }*/
                    if (icon == null)
                    {
                        continue;
                    }

                    icons.Add(ext, icon.ToBitmap());
                }
            }
        }

        private string[] GetFiles(string sourceFolder, string filters)
        {
            string[] files = filters.Split('|').SelectMany(filter => System.IO.Directory.GetFiles(sourceFolder, filter, SearchOption.TopDirectoryOnly)).Distinct().ToArray();
            Array.Sort(files);
            return files;
        }

        private void canvas_MouseDown(object sender, MouseEventArgs e)
        {
            if ((e.Button == MouseButtons.Left) && (dragHorizontal || dragVeritcal))
            {
                dragging = true;
                mousePreviousX = e.X;
                mousePreviousY = e.Y;
            }
        }

        private void canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (dragging)
            {
                ((Control)sender).Cursor = closeHand;
                dragImage(mousePreviousX - e.X, mousePreviousY - e.Y);
                mousePreviousX = e.X;
                mousePreviousY = e.Y;

            }
        }


        void dragImage(int dx, int dy)
        {
            Control control;
            if (staticImage)
            {
                control = panelCanvas;
            }
            else
            {
                control = canvas;
            }
            int width = control.Width;
            int height = control.Height;
            if (dragHorizontal)
            {
                if (dx == 0 && !dragVeritcal)
                {
                    return;
                }
                clipX += dx / scaleFactor;
                if (clipX < 0)
                {
                    clipX = 0;
                }
                else if (clipX > (image.Width - width / scaleFactor))
                {
                    clipX = image.Width - width / scaleFactor;
                }

            }
            if (dragVeritcal)
            {
                if (dy == 0 && !dragHorizontal)
                {
                    return;
                }
                clipY += dy / scaleFactor;
                if (clipY < 0)
                {
                    clipY = 0;
                }
                else if (clipY > (image.Height - height / scaleFactor))
                {
                    clipY = image.Height - height / scaleFactor;
                }
            }
            if (staticImage)
            {
                bltSrcRect.X = clipX;
                bltSrcRect.Y = clipY;
                Invalidate();
            }
            else
            {
                Graphics g = Graphics.FromImage(canvas.Image);
                if (hasAlpha)
                {
                    g.Clear(bgColour);
                }
                g.DrawImage(image, new Rectangle(0, 0, (int)(image.Width), (int)(image.Height)), clipX, clipY, image.Width / scaleFactor, image.Height / scaleFactor, GraphicsUnit.Pixel);
                g.Dispose();
                canvas.Invalidate();
            }
        }

        private void canvas_MouseUp(object sender, MouseEventArgs e)
        {
            if ((e.Button == MouseButtons.Left) && dragging)
            {
                dragging = false;
                ((Control)sender).Cursor = hand;
            }
        }

        private void canvas_ClientSizeChanged(object sender, EventArgs e)
        {
            if (image == null || WindowState == FormWindowState.Minimized || !((Control)sender).Visible)
            {
                return;
            }
            layoutCanvas(false);
        }

        private bool layoutCanvas(bool resetImageLocation)
        {
            if (staticImage)
            {
                return layoutStaticCanvas(resetImageLocation);
            }
            if (image == null)
            {
                return false;
            }
            panelCanvas.Visible = false;
            canvas.Dock = DockStyle.Fill;
            if (loadingRemoteImage && !cancelRemoteLoad)
            {
                int xPad = (canvas.Width - image.Width) / 2;
                int yPad = (canvas.Height - image.Height) / 2;
                canvas.Padding = new Padding(xPad, yPad, xPad, yPad);
                return false;
            }
            int left = 0, right = 0, top = 0, bottom = 0;
            float scaledHeight = image.Height * scaleFactor;
            float scaledWidth = image.Width * scaleFactor;
            int width = (int)scaledWidth, height = (int)scaledHeight;
            if (canvas.Width <= 0 || canvas.Height <= 0)
            {
                return false;
            }
            bool reImagine = false;
            /**
             * Reset the image position
             * 
             */
            if (resetImageLocation)
            {
                clipX = clipY = 0;
            }
            if (canvas.Height >= scaledHeight)
            {
                top = bottom = (int)((canvas.Height - scaledHeight) / 2);
                if (dragVeritcal)
                {
                    reImagine = true;
                    dragVeritcal = false;
                    height = (int)scaledHeight;
                }
                clipY = 0;
            }
            else
            {
                dragVeritcal = true;
                height = canvas.Height;
                reImagine = true;
                canvas.Cursor = hand;
                float overFlow = canvas.Height - (scaledHeight - clipY*scaleFactor);
                if (overFlow > 0)
                {
                    clipY = (scaledHeight - canvas.Height)/scaleFactor;
                }
            }
            if (canvas.Width >= scaledWidth)
            {
                left = right = (int)((canvas.Width - scaledWidth) / 2);
                if (dragHorizontal)
                {
                    reImagine = true;
                    dragHorizontal = false;
                    width = (int)scaledWidth;
                }
                clipX = 0;
            }
            else
            {
                dragHorizontal = true;
                width = canvas.Width;
                reImagine = true;
                canvas.Cursor = hand;
                float overFlow = canvas.Width - (scaledWidth - clipX * scaleFactor);
                if (overFlow > 0)
                {
                    clipX = (scaledWidth - canvas.Width) / scaleFactor;
                }
            }
            if (!dragHorizontal && !dragVeritcal)
            {
                reImagine = false;
                canvas.Cursor = DefaultCursor;
            }
            if (scaleFactor != 1.0f)
            {
                reImagine = true;
            }
            Image canvasImage = image;
            if (reImagine)
            {
                if (scale == Zoom.FitImage)//remove scaledX
                {
                    bool noSizing = false;
                    if (image.Height > canvas.Height)
                    {
                        if (image.Height / canvas.Height >= image.Width / canvas.Width)
                        {
                            scaleFactor = (float)canvas.Height / image.Height;
                            width = (int)(image.Width * scaleFactor);
                            height = (int)(image.Height * scaleFactor);
                            top = bottom = 0;
                            left = right = (canvas.Width - width) / 2;
                        }
                        else
                        {
                            scaleFactor = (float)canvas.Width / image.Width;
                            width = (int)(image.Width * scaleFactor);
                            height = (int)(image.Height * scaleFactor);
                            left = right = 0;
                            top = bottom = (canvas.Height - height) / 2;
                        }
                    }
                    else if (image.Width > canvas.Width)
                    {
                        if (image.Height / canvas.Height >= image.Width / canvas.Width)
                        {
                            scaleFactor = (float)canvas.Height / image.Height;
                            width = (int)(image.Width * scaleFactor);
                            height = (int)(image.Height * scaleFactor);
                            top = bottom = 0;
                            left = right = (canvas.Width - width) / 2;
                        }
                        else
                        {
                            scaleFactor = (float)canvas.Width / image.Width;
                            width = (int)(image.Width * scaleFactor);
                            height = (int)(image.Height * scaleFactor);
                            left = right = 0;
                            top = bottom = (canvas.Height - height) / 2;
                        }
                    }
                    else
                    {
                        noSizing = true;
                    }
                    dragHorizontal = dragVeritcal = false;
                    if (width <= 0 || height <= 0)
                    {
                        return false;
                    }
                    if (noSizing)
                    {
                        canvasImage = image;
                        width = image.Width;//approximations of width and height aren't good enough
                        height = image.Height;
                        scaleFactor = 1.0f;
                    }
                    else
                    {
                        canvasImage = new Bitmap(image, width, height);
                    }
                    canvas.Cursor = DefaultCursor;
                }
                else
                {
                    Bitmap bm = new Bitmap(width, height);
                    if (bm.HorizontalResolution != image.HorizontalResolution || bm.VerticalResolution != image.HorizontalResolution)
                    {
                        bm.SetResolution(image.HorizontalResolution, image.VerticalResolution);
                    }
                    Graphics g = Graphics.FromImage(bm);
                    //g.FillRectangle(Brushes.Green, new Rectangle(0, 0, width, height));
                    g.DrawImage(image, new Rectangle(0, 0, (int)(width), (int)(height)), clipX, clipY, width/scaleFactor, height/scaleFactor, GraphicsUnit.Pixel);
                    //g.DrawImage(image, 0, 0, new RectangleF(clipX, clipY, width, height), GraphicsUnit.Pixel);
                    g.Dispose();
                    canvasImage = bm;
                }
            }
            validatePadding(width, height, left, top, ref right, ref bottom);
            canvas.Padding = new Padding(left, top, right, bottom);
            canvas.Image = canvasImage;
            canvas.Visible = true;
            canvas.Invalidate();
            zoomPcTtoolStripLabel.Text = (scaleFactor * 100) + "%";
            return true;
        }

        private bool layoutStaticCanvas(bool resetImageLocation)
        {
            if (image == null)
            {
                return false;
            }
            canvas.Visible = false;
            panelCanvas.Dock = DockStyle.Fill;
            panelCanvas.Visible = true;
            /*if (loadingRemoteImage && !cancelRemoteLoad)//to do!
            {
                int xPad = (panelCanvas.Width - image.Width) / 2;
                int yPad = (panelCanvas.Height - image.Height) / 2;
                canvas.Padding = new Padding(xPad, yPad, xPad, yPad);
                if (xPad < 0)
                {
                    //bltDestRect = new Rectangle(0, , ,);
                    //bltSrcRect = new Rectangle(-xPad, , ,);
                }
                bltDestRect = new Rectangle();
                bltSrcRect = new Rectangle();
                return true;
            }*/
            int left = 0, right = 0, top = 0, bottom = 0;
            if (panelCanvas.Width <= 0 || panelCanvas.Height <= 0)
            {
                return false;
            }
            /**
             * Reset for new Image
             * 
             */
            if (scale == Zoom.FitImage)
            {
                if (image.Height > panelCanvas.Height)
                {
                    if (image.Height / panelCanvas.Height >= image.Width / panelCanvas.Width)
                    {
                        scaleFactor = (float)panelCanvas.Height / image.Height;
                    }
                    else
                    {
                        scaleFactor = (float)panelCanvas.Width / image.Width;
                    }
                }
                else if (image.Width > panelCanvas.Width)
                {
                    if (image.Height / panelCanvas.Height >= image.Width / panelCanvas.Width)
                    {
                        scaleFactor = (float)panelCanvas.Height / image.Height;
                    }
                    else
                    {
                        scaleFactor = (float)panelCanvas.Width / image.Width;
                    }
                }
                else
                {
                    scaleFactor = 1.0f;
                }
            }
            if (resetImageLocation)
            {
                clipX = clipY = 0;
                Graphics g = panelCanvas.CreateGraphics();
                g.Clear(bgColour);
                g.Dispose();
            }
            float scaledHeight = image.Height * scaleFactor;
            float scaledWidth = image.Width * scaleFactor;
            int width = (int)scaledWidth, height = (int)scaledHeight;
            if (panelCanvas.Height >= scaledHeight)
            {
                top = bottom = (int)((panelCanvas.Height - scaledHeight) / 2);
                if (dragVeritcal)
                {
                    dragVeritcal = false;
                    height = (int)scaledHeight;
                }
                clipY = 0;
            }
            else
            {
                dragVeritcal = true;
                height = panelCanvas.Height;
                float overFlow = panelCanvas.Height - (scaledHeight - clipY * scaleFactor);
                if (overFlow > 0)
                {
                    clipY = (scaledHeight - panelCanvas.Height) / scaleFactor;
                }
            }
            if (panelCanvas.Width >= scaledWidth)
            {
                left = right = (int)((panelCanvas.Width - scaledWidth) / 2);
                if (dragHorizontal)
                {
                    dragHorizontal = false;
                    width = (int)scaledWidth;
                }
                clipX = 0;
            }
            else
            {
                dragHorizontal = true;
                width = panelCanvas.Width;
                float overFlow = panelCanvas.Width - (scaledWidth - clipX * scaleFactor);
                if (overFlow > 0)
                {
                    clipX = (scaledWidth - panelCanvas.Width) / scaleFactor;
                }
            }
            if (!dragHorizontal && !dragVeritcal)
            {
                panelCanvas.Cursor = DefaultCursor;
            }
            else
            {
                panelCanvas.Cursor = hand;
            }

            validateStaticPadding(width, height, left, top, ref right, ref bottom);
            bltSrcRect = new RectangleF(clipX, clipY, width / scaleFactor, height / scaleFactor);
            bltDestRect = new RectangleF(left<0?0:left, top<0?0:top, width, height);
            Invalidate();
            zoomPcTtoolStripLabel.Text = (scaleFactor * 100) + "%";
            return true;
        }


        private void validatePadding(int width, int height, int left, int top, ref int right, ref int bottom)
        {
            if (left + right + width != canvas.Width)
            {
                right += canvas.Width - (left + right + width);
            }
            if (top + bottom + height != canvas.Height)
            {
                bottom += canvas.Height - (top + bottom + height);
            }
        }

        private void validateStaticPadding(int width, int height, int left, int top, ref int right, ref int bottom)
        {
            if (left + right + width != panelCanvas.Width)
            {
                right += panelCanvas.Width - (left + right + width);
            }
            if (top + bottom + height != panelCanvas.Height)
            {
                bottom += panelCanvas.Height - (top + bottom + height);
            }
        }

        private void canvas_MouseCaptureChanged(object sender, EventArgs e)
        {
            dragging = false;
        }

        private void toolStripButtonNext_Click(object sender, EventArgs e)
        {
            next();
        }

        private void toolStripButtonPrevious_Click(object sender, EventArgs e)
        {
            previous();
        }

        private void toolStripButtonFirst_Click(object sender, EventArgs e)
        {
            first();
        }

        private void toolStripButtonLast_Click(object sender, EventArgs e)
        {
            last();
        }

        private void first()
        {
            if (fileList == null || fileList.Length == 1 || imageIndex == 0)
            {
                return;
            }
            imageIndex = 0;
            open(imageIndex);
        }

        private void next()
        {
            if (fileList == null || fileList.Length == 1)
            {
                return;
            }
            if (imageIndex + 1 >= fileList.Length)
            {
                if (!loop)
                {
                    return;
                }
            }
            imageIndex = (imageIndex + 1) % fileList.Length;
            open(imageIndex);
        }

        private void previous()
        {
            if (fileList == null || fileList.Length == 1)
            {
                return;
            }
            if (imageIndex - 1 < 0)
            {
                if (!loop)
                {
                    return;
                }
            }
            imageIndex--;
            if (imageIndex < 0)
            {
                imageIndex = fileList.Length - 1;
            }
            open(imageIndex);
        }

        private void last()
        {
            if (fileList == null || fileList.Length == 1 || imageIndex == fileList.Length - 1)
            {
                return;
            }
            imageIndex = fileList.Length - 1;
            open(imageIndex);
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            open();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            close();
        }

        private void nextImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            next();
        }

        private void previousImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            previous();
        }

        private void firstImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            first();
        }

        private void lastImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            last();
        }

        private void PhotoAlbum_KeyUp(object sender, KeyEventArgs e)
        {
            canRecieveKey = true;
            scrollMultiplier = 1;
        }

        private void PhotoAlbum_KeyDown(object sender, KeyEventArgs e)
        {
            if (dragVeritcal || dragHorizontal)
            {
                if (scrollMultiplier >= 10)
                {
                    scrollMultiplier = 1;
                }
                else
                {
                    scrollMultiplier++;
                }
            }
            switch (e.KeyCode)
            {
                case Keys.Up:
                    if (dragVeritcal)
                    {
                        if (e.Shift)
                        {
                            dragImage(0, -(int)(clipY + 1));//fix
                        }
                        else
                        {
                            dragImage(0, -4 * scrollMultiplier);
                        }
                    }
                    break;
                case Keys.Down:
                    if (dragVeritcal)
                    {
                        if (e.Shift)
                        {
                            dragImage(0, int.MaxValue);//fix
                        }
                        else
                        {
                            dragImage(0, 4 * scrollMultiplier);
                        }
                    }
                    break;
                case Keys.Left:
                    if (dragHorizontal)
                    {
                        if (e.Shift)
                        {
                            dragImage(-(int)(clipX + 1), 0);//fix
                        }
                        else
                        {
                            dragImage(-4 * scrollMultiplier, 0);
                        }
                    }
                    break;
                case Keys.Right:
                    if (dragHorizontal)
                    {
                        if (e.Shift)
                        {
                            dragImage(int.MaxValue, 0);
                        }
                        else
                        {
                            dragImage(4 * scrollMultiplier, 0);//fix
                        }
                    }
                    break;
            }
            if (canRecieveKey)
            {
                switch (e.KeyCode)
                {
                    case Keys.Left:
                        if (!dragHorizontal)
                        {
                            previous();
                        }
                        break;
                    case Keys.Right:
                        if (!dragHorizontal)
                        {
                            next();
                        }
                        break;
                    case Keys.Space:
                        next();
                        break;
                    case Keys.Home:
                        first();
                        break;
                    case Keys.End:
                        last();
                        break;
                    case Keys.PageDown:
                        next();
                        break;
                    case Keys.PageUp:
                        previous();
                        break;
                    case Keys.F:
                        fitImage();
                        break;
                    case Keys.A:
                        actualSize();
                        break;
                    case Keys.Add:
                        zoomIn();
                        break;
                    case Keys.Subtract:
                        zoomOut();
                        break;
                    case Keys.Escape:
                        if (FullScreen)
                        {
                            FullScreen = false;
                        }
                        else if (exitEsc)
                        {
                            Application.Exit();
                        }
                        break;
                    default:
                        return;
                }
                canRecieveKey = false;
            }
        }

        private void zoomOut()
        {
            if (image == null) { return; }
            foreach (float factor in zoomFactor.Reverse())
            {
                if (scaleFactor > factor)
                {
                    scale = Zoom.Zoom;
                    actualSizeToolStripMenuItem.Checked = false;
                    fitImageToolStripMenuItem.Checked = false;
                    fixupZoom(factor);
                    scaleFactor = factor;
                    layoutCanvas(false);
                    break;
                }
            }
        }

        private void zoomIn()
        {
            if (image == null) { return; }
            foreach (float factor in zoomFactor)
            {
                if (scaleFactor < factor)
                {
                    scale = Zoom.Zoom;
                    actualSizeToolStripMenuItem.Checked = false;
                    fitImageToolStripMenuItem.Checked = false;
                    fixupZoom(factor);
                    scaleFactor = factor;
                    layoutCanvas(false);
                    break;
                }
            }
        }
        void fixupZoom(float factor)
        {
            if (factor * image.Width > DisplayWidth)
            {
                clipX += (((DisplayWidth / scaleFactor) * factor) - DisplayWidth) / (2 * factor);
                if (clipX < 0)
                {
                    clipX = 0;
                }
            }
            if (factor * image.Height > DisplayHeight)
            {
                clipY += (((DisplayHeight / scaleFactor) * factor) - DisplayHeight) / (2 * factor);
                if (clipY < 0)
                {
                    clipY = 0;
                }
            }
        }

        private int DisplayWidth
        {
            get
            {
                if (staticImage)
                {
                    return panelCanvas.Width;
                }
                return canvas.Width;
            }
        }

        private int DisplayHeight
        {
            get
            {
                if (staticImage)
                {
                    return panelCanvas.Height;
                }
                return canvas.Height;
            }
        }

        private void PhotoAlbum_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta >= 120)
            {
                previous();
            }
            else if (e.Delta <= -120)
            {
                next();
            }
        }

        private void loopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Loop = !Loop;
        }

        private void copyImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            copy();
        }

        private void copy()
        {
            if (image != null)
            {
                Clipboard.SetImage(image);
            }
        }

        private void copyVisibleImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            copyVisibleImage();
        }

        private void copyVisibleImage()
        {
            if (staticImage)
            {
                if (image != null)
                {
                    Graphics g = Graphics.FromImage(image);
                    Image cb = new Bitmap((int)bltDestRect.Width, (int)bltDestRect.Height, g);
                    g.Dispose();
                    g = Graphics.FromImage(cb);
                    g.DrawImage(image, new RectangleF(0, 0, bltDestRect.Width, bltDestRect.Height), bltSrcRect, GraphicsUnit.Pixel);
                    g.Dispose();
                    Clipboard.SetImage(cb);
                    cb.Dispose();
                }
            }
            else
            {
                if (canvas.Image != null)
                {
                    Clipboard.SetImage(canvas.Image);
                }
            }
        }

        private void rotateLeft()
        {
            if (image != null)
            {
                image.RotateFlip(RotateFlipType.Rotate270FlipNone);
                layoutCanvas(true);
            }
        }

        private void rotateRight()
        {
            if (image != null)
            {
                image.RotateFlip(RotateFlipType.Rotate90FlipNone);
                layoutCanvas(true);
            }
        }

        private void mirrorVirtical()
        {
            if (image != null)
            {
                image.RotateFlip(RotateFlipType.RotateNoneFlipY);
                layoutCanvas(true);
            }
        }

        private void mirrorHorizontal()
        {
            if (image != null)
            {
                image.RotateFlip(RotateFlipType.RotateNoneFlipX);
                layoutCanvas(true);
            }
        }

        private void toolStripButtonRotateLeft_Click(object sender, EventArgs e)
        {
            rotateLeft();
        }

        private void toolStripButtonRotateRight_Click(object sender, EventArgs e)
        {
            rotateRight();
        }

        private void toolStripButtonMirrorVirtical_Click(object sender, EventArgs e)
        {
            mirrorVirtical();
        }

        private void toolStripButtonMirrorHorizontal_Click(object sender, EventArgs e)
        {
            mirrorHorizontal();
        }

        private void viewClipboardToolStripMenuItem_Click(object sender, EventArgs ea)
        {
            //fix be on by default
            if (Clipboard.ContainsImage())
            {
                close();
                try
                {
                    image = Clipboard.GetImage();
                }
                catch (Exception e)
                {
                    LogException(e.ToString());
                    MessageBox.Show(this, "Unable to get clipboard data", "Error");
                    return;
                }

                fileList = new string[] { clipboardImageText };
                checkForTransperancy();
                frames = image.GetFrameCount(new FrameDimension(image.FrameDimensionsList[0]));
                if (resetZoom)
                {
                    scaleFactor = 1.0f;
                }
                if (frames > 1)
                {
                    staticImage = false;
                }
                else
                {
                    staticImage = true;
                }
                imageIndex = 0;
                layoutCanvas(false);
                toolStripStatusName.Text = fileList[imageIndex];
                toolStripStatusName.Image = Properties.Resources.clipboard;
                toolStripStatusIndex.Text = "1/1";
                toolStripStatusFormat.Text = image.Width + "×" + image.Height + " " + image.PixelFormat.ToString();
            }
            else
            {
                MessageBox.Show(this, "No image data on Clipboard", "Info");
            }
        }

        private void close()
        {
            fileList = null;
            image.Dispose();
            image = null;
            canvas.Image = null;
            dragHorizontal = dragVeritcal = false;
            toolStripStatusName.Text = "No Image Loaded";
            toolStripStatusName.Image = null;
            toolStripStatusIndex.Text = "0/0";
            toolStripStatusFormat.Text = "0bpp";

            indexToolStripDropDownButton.Enabled = false;
            indexToolStripDropDownButton.Text = "0";
            indexToolStripDropDownButton.DropDownItems.Clear();
            indexToolStripDropDownButton.Enabled = false;

            totalToolStripLabel.Text = "0";
            if (staticImage)
            {
                panelCanvas.Hide();
            }
            else
            {
                canvas.Hide();
            }
        }

        private void optionsToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            if (Clipboard.ContainsImage())
            {
                viewClipboardToolStripMenuItem.Enabled = true;
            }
            else
            {
                viewClipboardToolStripMenuItem.Enabled = false;
            }
        }

        private void saveFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            save(false);
        }

        private void save(bool visibleOnly)
        {
            if (image == null)
            {
                return;
            }
            Image saveImage;
            if (visibleOnly)
            {
                if (staticImage)
                {
                    Graphics dstGraphics, srcGraphics = Graphics.FromImage(image);
                    saveImage = (Image)new Bitmap((int)bltDestRect.Width, (int)bltDestRect.Height, srcGraphics);
                    srcGraphics.Dispose();
                    dstGraphics = Graphics.FromImage(saveImage);
                    dstGraphics.DrawImage(image, new RectangleF(0, 0, bltDestRect.Width, bltDestRect.Height), bltSrcRect, GraphicsUnit.Pixel);
                    dstGraphics.Dispose();
                }
                else
                {
                    saveImage = canvas.Image;
                }
            }
            else
            {
                saveImage = image;
            }
            if (saveImage == null)
            {
                MessageBox.Show("Nothing to save", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (toolStripStatusName.Text != clipboardImageText)
            {
                saveFileDialog.InitialDirectory = Path.GetDirectoryName(fileList[imageIndex]);
                saveFileDialog.FileName = Path.GetFileNameWithoutExtension(fileList[imageIndex]);
                
            }
            else
            {
                saveFileDialog.FileName = fileList[imageIndex];
            }
            saveFileDialog.Title = "Save \"" + fileList[imageIndex] + "\" As";
            if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                ImageFormat iFmt = ImageFormat.Bmp;
                switch (saveFileDialog.FilterIndex)
                {
                    case 1:
                        iFmt = ImageFormat.Jpeg;
                        break;
                    case 2:
                        iFmt = ImageFormat.Png;
                        break;
                    case 3:
                        iFmt = ImageFormat.Gif;
                        break;
                    case 4:
                        iFmt = ImageFormat.Bmp;
                        break;
                    case 5:
                        iFmt = ImageFormat.Tiff;
                        break;

                }
                try
                {
                    saveImage.Save(saveFileDialog.FileName, iFmt);
                }
                catch (Exception e)
                {
                    LogException(e.ToString());
                    MessageBox.Show(this, "Unable to save: " + saveFileDialog.FileName + "\n" + e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            else
            {
                return;
            }
            fileList = saveFileDialog.FileNames;
            int index = buildList(fileList[0]);
            imageIndex = index;
            open(imageIndex);
        }

        private void deleteFileToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            deleteFile();
        }

        private void deleteFile()
        {
            if (fileList != null && fileList.Length > 0 && File.Exists(fileList[imageIndex]) && MessageBox.Show(this, "Are you sure you want to delete " + fileList[imageIndex] + "?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                image.Dispose();
                image = null;
                canvas.Image = null;
                try
                {
                    File.Delete(fileList[imageIndex]);
                }
                catch (Exception e)
                {
                    LogException(e.ToString());
                    MessageBox.Show(this, "Unable to delete " + fileList[imageIndex] + "\n" + e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    open(fileList[imageIndex]);
                    return;
                }
                removeCurrentFile();
            }
        }

        private void removeCurrentFile()
        {
            ArrayList al = new ArrayList(fileList);
            al.RemoveAt(imageIndex);
            fileList = (string[])al.ToArray(typeof(string));
            if (fileList.Length == 0)
            {
                close();
            }
            else
            {
                if (imageIndex >= fileList.Length)
                {
                    imageIndex--;
                }
                open(imageIndex);
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            save(false);
        }

        private void copyFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            copyFile();
        }

        private void copyFile()
        {
            if (fileList == null || fileList.Length == 0 || !File.Exists(fileList[imageIndex]))
            {
                MessageBox.Show(this, "No file to copy", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            saveFileDialog.Title = "Copy File";
            saveFileDialog.FilterIndex = 6;
            saveFileDialog.FileName = Path.GetFileName(fileList[imageIndex]);
            if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                try
                {
                    File.Copy(fileList[imageIndex], saveFileDialog.FileName, true);
                }
                catch (Exception e)
                {
                    LogException(e.ToString());
                    MessageBox.Show(this, "Unable to copy file \"" + fileList[imageIndex] + "\" to \"" + saveFileDialog.FileName + "\n" + e.Message, "Error",  MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
        }

        private void copyFileToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            copyFile();
        }

        private void moveFile()
        {
            if (fileList == null || fileList.Length == 0 || !File.Exists(fileList[imageIndex]))
            {
                MessageBox.Show(this, "No file to move", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            saveFileDialog.Title = "Move File";
            saveFileDialog.FilterIndex = 6;
            saveFileDialog.FileName = Path.GetFileName(fileList[imageIndex]);
            if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                image.Dispose();
                image = null;
                if (!staticImage)
                {
                    canvas.Image.Dispose();
                    canvas.Image = null;
                }
                try
                {
                    File.Move(fileList[imageIndex], saveFileDialog.FileName);
                }
                catch (Exception e)
                {
                    LogException(e.ToString());
                    MessageBox.Show(this, "Unable to move file \"" + fileList[imageIndex] + "\" to \"" + saveFileDialog.FileName + "\n" + e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    open(fileList[imageIndex]);
                }
                removeCurrentFile();
            }
        }

        private void moveFileToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            moveFile();
        }

        private void saveVisibleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            save(true);
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new AboutBox().ShowDialog(this);
        }

        private void deleteFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            deleteFile();
        }

        private void moveFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            moveFile();
        }

        private void actualSizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            actualSize();
        }

        private void actualSize()
        {
            if (actualSizeToolStripMenuItem.Checked == false)
            {
                scale = Zoom.ActualSize;
                actualSizeToolStripMenuItem.Checked = true;
                fitImageToolStripMenuItem.Checked = false;
                scaleFactor = 1.0f;
                layoutCanvas(true);
            }
        }

        private void fitImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fitImage();
        }

        private void fitImage()
        {
            if (fitImageToolStripMenuItem.Checked == false)
            {
                scale = Zoom.FitImage;
                fitImageToolStripMenuItem.Checked = true;
                actualSizeToolStripMenuItem.Checked = false;
                scaleFactor = 1.0f;
                layoutCanvas(true);
            }
        }

        private void backgroundColourToolStripMenuItem_Click(object sender, EventArgs e)
        {
            colorDialog.Color = bgColour;
            if (colorDialog.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                setBgColour(colorDialog.Color);
            }
        }

        private void setBgColour(Color colour)
        {
            if (colour != null)
            {
                canvas.BackColor = bgColour = colour;
                layoutCanvas(false);
            }
        }

        public Color BackgroundColour
        {
            get
            {
                return bgColour;
            }
            set
            {
                setBgColour(value);
            }
        }
        public bool ExitEsc
        {
            get
            {
                return exitEsc;
            }
            set
            {
                exitEsc = value;
            }
        }

        public bool Loop
        {
            get
            {
                return loop;
            }
            set
            {
                loopToolStripMenuItem.Checked = loop = value;
            }
        }

        public bool AllowUrls
        {
            get
            {
                return allowUrls;
            }
            set
            {
                allowUrls = value;
            }
        }

        public bool AllowMultipleWindows
        {
            get
            {
                return allowMultipleWindows;
            }
            set
            {
                if (allowMultipleWindows == value)
                {
                    return;
                }
                if (value == true)
                {
                    Thread th = new Thread(new ThreadStart(serverThread));
                    th.Start();
                }
                else
                {
                    if (server != null)
                    {
                        server.Close();
                        server = null;
                    }
                }
                    allowMultipleWindows = value;
            }
        }

        public bool ScanDirectory
        {
            get
            {
                return scanDirectory;
            }
            set
            {
                scanDirectory = value;
            }
        }

        public string isProgIdRegistered(string _class, string extension)
        {
            try
            {
                string progID = @"HKEY_CURRENT_USER\Software\Classes\" + appCompany.ToLower() + "." + appName.ToLower() + "." + _class;
                if (Microsoft.Win32.Registry.GetValue(progID, "", null) == null)
                {
                    return "no";
                }
                progID = appCompany.ToLower() + "." + appName.ToLower() + "." + _class;
                if ((string)Microsoft.Win32.Registry.GetValue(@"HKEY_CURRENT_USER\Software\Classes\." + extension, "", null) != progID)
                {
                    return "no";
                }
                return "yes";
            }
            catch (UnauthorizedAccessException e)
            {
                LogException(e.ToString());
                return "accessdenied";
            }
            catch (System.Security.SecurityException e)
            {
                LogException(e.ToString());
                return "accessdenied";
            }
            catch (Exception e)
            {
                LogException(e.ToString());
                return "exception";
            }
        }

        public bool registerProgId(string _class, string[] extensions, string name, int iconIndex)
        {
            try
            {
                Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Software\Classes\" + appCompany.ToLower() + "." + appName.ToLower() + "." + _class, null, appName + " " + name);
                Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Software\Classes\" + appCompany.ToLower() + "." + appName.ToLower() + "." + _class + @"\shell\open\command", null, "\"" + appExecName + "\" \"%1\"");
                Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Software\Classes\" + appCompany.ToLower() + "." + appName.ToLower() + "." + _class + @"\DefaultIcon", null, appIconName + "," + iconIndex);

                string value = appCompany.ToLower() + "." + appName.ToLower() + "." + _class;
                foreach (string ext in extensions)
                {
                    Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Software\Classes\." + ext, null, value);
                }
            }
            catch (UnauthorizedAccessException e)
            {
                LogException(e.ToString());
                MessageBox.Show(e.Message, "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return false;
            }
            catch (System.Security.SecurityException e)
            {
                LogException(e.ToString());
                MessageBox.Show(e.Message, "Permission required", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return false;
            }
            catch (Exception e)
            {
                LogException(e.ToString());
                MessageBox.Show(e.Message, "Unabe to set file associations for "+name, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return false;
            }
            SHChangeNotify(0x08000000, 0x0000, (IntPtr)null, (IntPtr)null);//SHCNE_ASSOCCHANGED SHCNF_IDLIST
            return true;
        }

        private void waitingCancelButton_Click(object sender, EventArgs e)
        {

        }

        private void checkForUpdatesToolStripMenuItem_Click(object sender, EventArgs ea)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(upgradeUrl + "?" + upgradeQueryString + "newestversion" + "&now=" + new DateTime().ToString());
                request.AllowAutoRedirect = true;
                request.Method = "GET";
                request.UserAgent = userAgent;
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream());
                string version = reader.ReadToEnd();
                if (string.IsNullOrEmpty(version))
                {
                    LogException(new DateTime().ToString() + "\n" + version);
                }
                response.Close();
                Version current = new Version(Application.ProductVersion);
                if (current.CompareTo(new Version(version)) == 0)
                {
                    MessageBox.Show(this, "Version up to date.", "Update");
                }
                else if (MessageBox.Show(this, "Would you like to download the latest version?", "Newer Version Available", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                {
                    request = (HttpWebRequest)HttpWebRequest.Create(upgradeUrl + "?" + upgradeQueryString + "downloadredirect" + "&now=" + new DateTime().ToString()); 
                    request.AllowAutoRedirect = true;
                    request.Method = "GET";
                    request.UserAgent = userAgent;
                    response = (HttpWebResponse)request.GetResponse();
                    MessageBox.Show(response.Headers.ToString());
                    BinaryReader bReader = new BinaryReader(response.GetResponseStream());
                    SaveFileDialog updateFileDialog = new SaveFileDialog();
                    updateFileDialog.Title = "Save " + appName;
                    updateFileDialog.FileName = appName;
                    if (response.Headers[HttpResponseHeader.ContentType] == "application/x-msdownload")
                    {
                        updateFileDialog.DefaultExt = "exe";

                    }
                    else if (response.Headers[HttpResponseHeader.ContentType] == "application/zip")
                    {
                        updateFileDialog.DefaultExt = "zip";
                    }
                    if (updateFileDialog.ShowDialog(this) == DialogResult.OK)
                    {
                        FileStream f = File.Create(updateFileDialog.FileName);
                        byte[] data = new byte[1024];
                        int read;
                        while ((read = bReader.Read(data, 0, 1024)) > 0)
                        {
                            f.Write(data, 0, read);
                        }
                        f.Close();
                    }
                }
            }
            catch (NotSupportedException e)
            {
                LogException(e.ToString());
                MessageBox.Show("This operation may not be supported on your system\n" + e.Message, "Stop", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            catch (System.Security.SecurityException e)
            {
                LogException(e.ToString());
                MessageBox.Show("You may not have the required privilages to perform this operation.\n" + e.Message, "Stop", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            catch (WebException e)
            {
                LogException(e.ToString());
                string title;
                if (e.Status == WebExceptionStatus.ProtocolError)
                {
                    title = "Server response";
                }
                else
                {
                    title = "Error";
                }
                MessageBox.Show(e.Message, title, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            catch (Exception e)
            {
                LogException(e.ToString());
                MessageBox.Show(e.Message, "Stop", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void checkForUpdatesToolStripMenuItem_Click_post(object sender, EventArgs ea)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(upgradeUrl);
                request.AllowAutoRedirect = true;
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.UserAgent = userAgent;
                Stream stream = request.GetRequestStream();
                byte[] upgradePostData = Encoding.UTF8.GetBytes(upgradeQueryString);
                stream.Write(upgradePostData, 0, upgradePostData.Length);
                stream.Close();
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream());
                string version = reader.ReadToEnd();
                response.Close();
                Version current = new Version(Application.ProductVersion);
                if (current.CompareTo(new Version(version)) == 0)
                {
                    webBrowser.Url = new Uri("http://www.outerdev.heliohost.org");
                    canvas.Hide();
                    webBrowser.Show();
                }
            }
            catch (NotSupportedException e)
            {
                LogException(e.ToString());
                MessageBox.Show("This operation may not be supported on your system\n" + e.Message, "Stop", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            catch (System.Security.SecurityException e)
            {
                LogException(e.ToString());
                MessageBox.Show("You may not have the required privilages to perform this operation.\n" + e.Message, "Stop", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            catch (WebException e)
            {
                LogException(e.ToString());
                string title;
                if (e.Status == WebExceptionStatus.ProtocolError)
                {
                    title = "Server responce";
                }
                else
                {
                    title = "Error";
                }
                MessageBox.Show(e.Message, title, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            catch (Exception e)
            {
                LogException(e.ToString());
                MessageBox.Show(e.Message, "Stop", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void addToListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string result = AddItem.addItem(this);
            if (result != null && result.Length != 0)
            {
                fileList = fileList.Union(new string[] {result}).ToArray();
            }
        
        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new Options(this).show();
        }

        private void zoomInToolStripMenuItem_Click(object sender, EventArgs e)
        {
            zoomIn();
        }

        private void zoomOutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            zoomOut();
        }

        private void fullScreenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FullScreen = true;
        }

        public bool FitImage
        {
            get
            {
                return fitimage;
            }
            set
            {
                fitimage = value;
            }
        }

        public bool ResetZoom
        {
            get
            {
                return resetZoom;
            }
            set
            {
                resetZoom = value;
            }
        }

        public bool FullScreen {
            get
            {
                return fullScreen;
            }
            set
            {
                if (value == true)
                {
                    if (fullScreen == false)
                    {
                        FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
                        menu.Hide();
                        toolBar.Hide();
                        statusStrip.Hide();
                        TopMost = true;
                        WindowState = FormWindowState.Normal;//dirty hack
                        WindowState = FormWindowState.Maximized;
                        exitFullScreenToolStripMenuItem.Visible = true;
                        toolStripSeparatorFullScreen.Visible = true;
                        fullScreen = value;
                    }
                }
                else
                {
                    if (fullScreen == true)
                    {
                        menu.Show();
                        toolBar.Show();
                        statusStrip.Show();
                        WindowState = FormWindowState.Normal;
                        TopMost = false;
                        FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
                        exitFullScreenToolStripMenuItem.Visible = false;
                        toolStripSeparatorFullScreen.Visible = false;
                        fullScreen = value;
                    }
                }
            }
        }

        private void exitFullScreenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FullScreen = false;
        }

        private void PhotoAlbum_FormClosed(object sender, FormClosedEventArgs e)
        {
            running = false;
            if (server != null)
            {
                server.Close();
            }
            saveConfiguration();
            shellNotifyIcon.Visible = false;
        }

        private void saveConfiguration()
        {
            try
            {
                Microsoft.Win32.Registry.SetValue(keyName, "bgcolour", bgColour.ToArgb());
                Microsoft.Win32.Registry.SetValue(keyName, "left", Left);
                Microsoft.Win32.Registry.SetValue(keyName, "top", Top);
                Microsoft.Win32.Registry.SetValue(keyName, "width", Width);
                Microsoft.Win32.Registry.SetValue(keyName, "height", Height);
                Microsoft.Win32.Registry.SetValue(keyName, "allowurls", allowUrls.ToString());
                Microsoft.Win32.Registry.SetValue(keyName, "loop", loop.ToString());
                Microsoft.Win32.Registry.SetValue(keyName, "scandir", scanDirectory.ToString());
                Microsoft.Win32.Registry.SetValue(keyName, "maximized", (WindowState == FormWindowState.Maximized).ToString());
                Microsoft.Win32.Registry.SetValue(keyName, "topmost", TopMost.ToString());
                Microsoft.Win32.Registry.SetValue(keyName, "exitesc", exitEsc.ToString());
                Microsoft.Win32.Registry.SetValue(keyName, "resetzoom", resetZoom.ToString());
                Microsoft.Win32.Registry.SetValue(keyName, "fitimage", fitimage.ToString());
                Microsoft.Win32.Registry.SetValue(keyName, "fullscreen", FullScreen.ToString());
                Microsoft.Win32.Registry.SetValue(keyName, "allowmultiplewindows", allowMultipleWindows.ToString());
            }
            catch (Exception e)
            {
                LogException(e.ToString());
            }
        }

        private void printSetupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (printDialog.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                printDocument.DocumentName = fileList[imageIndex];
                printDocument.Print();
            }
        }

        private void printDocument_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            e.Graphics.DrawImage(image, new PointF(0, 0));
        }

        private void printPreviewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (printPreviewDialog.ShowDialog(this) == DialogResult.OK)
            {
                
            }
        }

        private void PhotoAlbum_SizeChanged(object sender, EventArgs e)
        {
        }

        private void PhotoAlbum_Paint(object sender, PaintEventArgs e)
        {
            if (image == null || DisplayWidth <= 0 || DisplayHeight <= 0)
            {
                return;
            }
            if (staticImage)
            {
                Image db = image;
                RectangleF dbSrc = bltSrcRect, dbDest = bltDestRect;
                Graphics g = panelCanvas.CreateGraphics();
                if (hasAlpha || !dragging)
                {
                    db = new Bitmap(panelCanvas.Width, panelCanvas.Height, g);
                    Graphics dbg = Graphics.FromImage(db);
                    dbg.Clear(bgColour);
                    dbg.DrawImage(image, bltDestRect, bltSrcRect, GraphicsUnit.Pixel);
                    dbDest = dbSrc = new RectangleF(0, 0, panelCanvas.Width, panelCanvas.Height);
                    dbg.Dispose();
                }
                g.DrawImage(db, dbDest, dbSrc, GraphicsUnit.Pixel);
                g.Dispose();
                if (!db.Equals(image))
                {
                    db.Dispose();
                }
            }
        }

        private void indexToolStripMenuItem_Click(object sender, EventArgs ev)
        {
            indexToolStripDropDownButton.Text = ((ToolStripMenuItem)sender).Text;
            goTo();
        }

        private void goTo()
        {
            if (fileList != null)
            {
                int newIndex;
                try
                {
                    newIndex = int.Parse(indexToolStripDropDownButton.Text) - 1;
                }
                catch/* (Exception e)*/
                {
                    indexToolStripDropDownButton.Text = (imageIndex + 1).ToString();
                    return;
                }
                if (newIndex < 0 || newIndex >= fileList.Length)
                {
                    indexToolStripDropDownButton.Text = (imageIndex + 1).ToString();
                    return;
                }
                open(newIndex);
            }
        }

        private void IndexToolStripTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar)) 
            {
                if (e.KeyChar.ToString() == "\r")
                {
                    goTo();
                }
                e.Handled = true;
            }
        }

        private void IndexToolStripTextBox_Click(object sender, EventArgs e)
        {

        }

        private void IndexToolStripTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void shellNotifyIcon_Click(object sender, EventArgs e)
        {
            MouseEventArgs me = (MouseEventArgs)e;
            if (me.Button != MouseButtons.Right)
            {
                if (Visible)
                {
                    Hide();
                }
                else
                {
                    Show();
                    Activate();
                }
            }
        }

        private void exitShellNotifyMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void zoomInToolStripButton_Click(object sender, EventArgs ev)
        {
            zoomIn();
        }

        private void zoomOutToolStripButton_Click(object sender, EventArgs e)
        {
            zoomOut();
        }
        /*        private void requestStreamHandler(IAsyncResult a)
                {
                    Console.WriteLine("6"); 
                    Async operation = (Async)a.AsyncState;
                    HttpWebRequest request = operation.request;
                    Console.WriteLine("7");
                    Stream stream = request.EndGetRequestStream(a);
                    Console.WriteLine("8");
                    byte[] upgradePostData = Encoding.UTF8.GetBytes(upgradePostString);
                    stream.Write(upgradePostData, 0, upgradePostData.Length);
                    stream.Close();
                    IAsyncResult result = request.BeginGetResponse(requestResponseCallback, operation);
                }
                private void requestResponseCallback(IAsyncResult a)
                {
                    Async operation = (Async)a.AsyncState;
                    HttpWebRequest request = operation.request;
                    HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(a);
                    operation.response = response;
                    Stream stream = response.GetResponseStream();
                    operation.read = stream;
                    operation.data = new byte[1024];
                    IAsyncResult result = stream.BeginRead(operation.data, operation.count, operation.data.Length, responseReadCallback, operation);
                }
                private void responseReadCallback(IAsyncResult a)
                {
                    Async operation = (Async)a.AsyncState;
                    Stream stream = operation.read;
                    int read = stream.EndRead(a);
                    if (read == 0)
                    {
                        stream.Close();
                        string version = Encoding.UTF8.GetString(operation.data);
                        operation.completed();
                        Version current = new Version(Application.ProductVersion);
                        if (current.CompareTo(new Version(version)) == 0)
                        {
                            webBrowser.Url = new Uri("http://www.outerdev.heliohost.org");
                            canvas.Hide();
                            webBrowser.Show();
                        }
                    }
                    else
                    {
                        operation.count += read;
                        IAsyncResult result = stream.BeginRead(operation.data, operation.count, operation.data.Length - operation.count, responseReadCallback, operation);
                    }
                }*/
        /*
                        private void wma()
                        {
                            msgQueue = System.Messaging.MessageQueue.Create(".\\Private$\\" + appCompany + appName, false);
                            System.Messaging.Message[] m;
                            msgQueue.Formatter = new System.Messaging.BinaryMessageFormatter();
                            try
                            {
                                while (running)
                                {
                                    try
                                    {
                                        m = msgQueue.GetAllMessages();
                                        if (m.Length > 0)
                                        {
                                            Invoke(new genericDelegate(processMessage), m.Last().Body);
                                            msgQueue.Purge();
                                        }
                                    }
                                    catch (Exception e)
                                    {
                                        MessageBox.Show(e.ToString());
                                        LogException(e.ToString());
                                        msgQueue.Purge();
                                    }
                                }
                            }
                            catch (ThreadInterruptedException e)
                            {
                                //
                            }
                        }
                        void processMessage(object data)
                        {
                            string[] args;
                            try
                            {
                                args = (string[])data;
                            }
                            catch (InvalidCastException e)
                            {
                                return;
                            }
                            processArgs(args);
                        }
                    if (msgThread != null)
                    {
                        msgQueue.Close();
                        System.Messaging.MessageQueue.Delete(".\\Private$\\" + appCompany + appName);
                    }
*/



    }
    public delegate bool layoutCanvasDelegate(bool reset);
    public delegate void Delegate();
    public delegate void genericDelegate(object data);
    public delegate void processArgsDelegate(string[] data);
    public enum AsyncOperation { UpdateCheck, OpenRemoteImage }
    /*    public class Async
        {
            public byte[] data;
            public int count = 0;
            public HttpWebRequest request;
            public HttpWebResponse response;
            public Stream read;
            private Stream write;
            private Waiting waitWin;
            private IWin32Window parent;
            private EventHandler cancel;
            public Async(AsyncOperation operation, EventHandler cancel, IWin32Window parent)
            {
                this.parent = parent;
                this.cancel = cancel;
                waitWin = new Waiting(cancelHandler);
            }
            public void completed()
            {
                if (waitWin.InvokeRequired)
                {

                }
                else
                {
                    waitWin.Close();
                }
            }
            public void showWindow(string message)
            {
                waitWin.setMessage(message);
                waitWin.Show(parent);
            }
            public void setMessage(string message)
            {
                waitWin.setMessage(message);
            }
            private void cancelHandler(object sender, EventArgs e)
            {
                cancel.Invoke(this, null);
            }

        }
            [DllImport("shell32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            public static extern void SHChangeNotify(uint wEventId, uint uFlags, IntPtr dwItem1, IntPtr dwItem2);
    */
    /*try
    {
        if (System.Messaging.MessageQueue.Exists(".\\Private$\\" + appCompany + appName))
        {
            System.Messaging.MessageQueue msgQueue = new System.Messaging.MessageQueue(".\\Private$\\" + appCompany + appName);
            System.Messaging.Message message = new System.Messaging.Message(args, new System.Messaging.BinaryMessageFormatter());
            msgQueue.Send(message);
            exitApplication = true;
        }
        else 
        {
            msgThread = new Thread(new ThreadStart(wma));
            msgThread.Start();
        }
    }
    catch (InvalidOperationException e)
    {
        MessageBox.Show(e.ToString());
    }*/
}
