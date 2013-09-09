using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BulletinLibrary;
using ConfigApp.Model;
using Shell32;
using System.Configuration;
using System.Diagnostics;
using IWshRuntimeLibrary;

namespace ConfigApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += OnLoaded;
        }


        public void KillExe(string path)
        {
            System.Diagnostics.Process[] ps = System.Diagnostics.Process.GetProcessesByName(path);
            foreach (System.Diagnostics.Process p in ps)
            {
                p.Kill();
            }
        }

        void OnLoaded(object sender, RoutedEventArgs e)
        {
            rsssource.Text = string.Format("{0}\n{1}\n{2}",
                @"http://rss.sina.com.cn/news/marquee/ddt.xml",
                @"http://www.infzm.com/rss/home/rss2.0.xml",
                @"http://news.sohu.com/rss/scroll.xml");

            // LoadGridPng();
        }

        //private void LoadGridPng()
        //{
        //    this.top.Source = new BitmapImage(new Uri(toppath));
        //    this.left.Source = new BitmapImage(new Uri(leftpath));
        //    this.right.Source = new BitmapImage(new Uri(rightpath));
        //    this.bottom.Source = new BitmapImage(new Uri(bottompath));
        //}

        private void OnSave(object sender, RoutedEventArgs e)
        {
            foreach (var item in ConfigManager.Instance.Items)
            {
                if (rdback.IsChecked == true)
                    item.IsBack = "true";
                else
                    item.IsBack = "false";
            }
            ConfigManager.Instance.Save();

            string filename = "Item.xml";
            string source = System.IO.Path.Combine(ConfigManager.CONFIGPATH, "Resource/Item.xml"); ;
            string url = string.Format("{0}/{1}", ConfigurationManager.AppSettings["ftp"].ToString(), filename);

            try
            {
                FTPHelper.UploadNews(source, url);
                System.Windows.MessageBox.Show("配置文件上传成功");
                App.Current.Shutdown();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("配置文件上传失败");
            }
        }

        private void OnOpenPhotoDictionary(object sender, RoutedEventArgs e)
        {
            var index = (int)(sender as System.Windows.Controls.Button).Tag;
            var dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var item = ConfigManager.Instance.Items.FirstOrDefault(t => t.Index == index);
                item.PhotoPath = dialog.SelectedPath;
                item.ExePath = string.Empty;
                item.Url = string.Empty;
            }
        }

        private void OnOpenExeDictionary(object sender, RoutedEventArgs e)
        {
            var index = (int)(sender as System.Windows.Controls.Button).Tag;
            var dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var item = ConfigManager.Instance.Items.FirstOrDefault(t => t.Index == index);
                item.ExePath = dialog.SelectedPath;
                item.PhotoPath = string.Empty;
                item.Url = string.Empty;
            }
        }

        private void fileupload_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = @"文本文件(*.txt)|*.txt";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string path = dialog.FileName;
                foreach (var item in ConfigManager.Instance.Items)
                {
                    item.Marquee = System.IO.Path.GetFileName(path);
                }

                string filename = System.IO.Path.GetFileName(path);
                string source = path;
                string url = string.Format("{0}/{1}", ConfigurationManager.AppSettings["ftp"].ToString(), filename);

                try
                {
                    FTPHelper.UploadNews(source, url);
                    System.Windows.MessageBox.Show("通知上传成功");
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show("通知上传失败");
                }
            }
        }

        private void rssupload_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ConfigManager.Instance.SaveRSS(rsssource.Text.Split('\n'));
        }

        private void rssstart_MouseUp(object sender, MouseButtonEventArgs e)
        {
            string rssservice = ConfigurationManager.AppSettings["rssservice"].ToString();
            try
            {
                KillExe(System.IO.Path.GetFileNameWithoutExtension(rssservice));
                Process.Start(new ProcessStartInfo(rssservice));
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("启动RSS抓取服务失败，请检查配置文件是否正确，网络是否联通");
            }
        }

        private void Label_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var dialog = new OpenFileDialog();
            var str = (sender as System.Windows.Controls.Label).Tag.ToString();
            int width = 0;
            int height = 0;
            if (str.ToLower() == "top")
            {
                maxwidth = 1366;
                maxheight = 126;
            }
            else if (str.ToLower() == "bottom")
            {
                maxwidth = 1366;
                maxheight = 80;
            }
            else if (str.ToLower() == "left")
            {
                maxwidth = 306;
                maxheight = 562;
            }
            else if (str.ToLower() == "right")
            {
                maxwidth = 31;
                maxheight = 562;
            }
            else if (str.ToLower().Contains("button"))
            {
                maxwidth = 201;
                maxheight = 49;
            }
            dialog.Filter = @"PNG 文件(*.png)|*.png";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string path = dialog.FileName;
                ShellClass sh = new ShellClass();
                Shell32.Folder dir = sh.NameSpace(System.IO.Path.GetDirectoryName(path));
                FolderItem item = dir.ParseName(System.IO.Path.GetFileName(path));
                string det = dir.GetDetailsOf(item, 31);

                Regex r = new Regex(@"(\d+)[^\d]+(\d+)");
                if (r.IsMatch(det))
                {
                    var m = r.Match(det);
                    width = Convert.ToInt32(m.Groups[1].Value);
                    height = Convert.ToInt32(m.Groups[2].Value);
                }

                if (width == maxwidth && height == maxheight)
                {
                    this.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        Upload(path, str.ToLower());

                    }));
                }
                else
                {
                    System.Windows.MessageBox.Show("上传图片的像素不正确，请重新上传");
                }
            }
        }

        private void videoupload_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = @"avi文件(*.avi)|*.avi|rmvb文件(*.rmvb)|*.rmvb|mpg文件(*.mpg)|*.mpg|wmv文件(*.wmv)|*.wmv";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                foreach (var item in ConfigManager.Instance.Items)
                {
                    item.PlayPath = dialog.FileName;
                }
            }
        }

        private void backupload_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = @"JPG文件(*.jpg)|*.jpg";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string path = dialog.FileName;

                UploadJPG(path, @"background");
            }
        }

        private void UploadJPG(string path, string source)
        {
            try
            {
                string desfile = System.IO.Path.Combine(ConfigManager.CONFIGPATH, @"Resource\Images\" + source + ".jpg");
                //System.Windows.MessageBox.Show(sourcefile);
                System.IO.File.Copy(path, desfile, true);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("该文件已存在，请选择不同的文件上传");
            }
        }

        private void Upload(string path, string source)
        {
            try
            {
                string desfile = System.IO.Path.Combine(ConfigManager.CONFIGPATH, @"Resource\Images\" + source + ".png");
                //System.Windows.MessageBox.Show(sourcefile);
                System.IO.File.Copy(path, desfile, true);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("该文件已存在，请选择不同的文件上传");
            }
        }

        private int maxwidth;
        private int maxheight;
        private string toppath = System.IO.Path.Combine(System.Environment.CurrentDirectory, @"Resource\Images\top.png");
        private string leftpath = System.IO.Path.Combine(System.Environment.CurrentDirectory, @"Resource\Images\left.png");
        private string rightpath = System.IO.Path.Combine(System.Environment.CurrentDirectory, @"Resource\Images\right.png");
        private string bottompath = System.IO.Path.Combine(System.Environment.CurrentDirectory, @"Resource\Images\bottom.png");
    }
}
