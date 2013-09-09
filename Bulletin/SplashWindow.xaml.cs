using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xml.Serialization;
using BulletinLibrary;
using Bulletin.Model;
using System.Configuration;

namespace Bulletin
{
    public delegate void LoadCompletedHandler();

    public partial class SplashWindow : Window
    {
        public event LoadCompletedHandler Completed;

        public SplashWindow()
        {
            InitializeComponent();
            this.Loaded += OnLoaded;
        }

        void OnLoaded(object sender, RoutedEventArgs e)
        {
            DownloadManager.Instance.Completed += (task) =>
            {
                File.Move(task.Temp.AbsolutePath, task.Dest.AbsolutePath);
                startlogo.Text = "配置文件下载完成";

                if (task.Dest.AbsolutePath.Contains("News.xml"))
                {
                    var items = System.IO.Path.Combine(App.CONFIGPATH, "Resource/Item.xml");
                    MainManager.LoadItems(items);
                    startlogo.Text = "加载配置文件完成";
                    if (BroadManager.Instance.Items.Count > 0)
                    {
                        marquee = true;
                        var filename = BroadManager.Instance.Items[0].Marquee;
                        var source = string.Format("{0}/{1}", ConfigurationManager.AppSettings["ftp"], filename);
                        var target = System.IO.Path.Combine(App.CONFIGPATH, "Resource/" + filename);
                        MainManager.DownloadConfig(source, target);
                        return;
                    }
                }

                LoadOther();
            };

            DownloadManager.Instance.Failed += (task) =>
            {
                startlogo.Text = "从FTP下载配置文件失败，请检查FTP是否连通！";
                LoadOther();
            };

            var source1 = string.Format("{0}/{1}", ConfigurationManager.AppSettings["ftp"], @"Item.xml");
            var target1 = System.IO.Path.Combine(App.CONFIGPATH, "Resource/Item.xml");
            MainManager.DownloadConfig(source1, target1);
            var source2 = string.Format("{0}/{1}", ConfigurationManager.AppSettings["ftp"], @"News.xml");
            var target2 = System.IO.Path.Combine(App.CONFIGPATH, "Resource/News.xml");
            MainManager.DownloadConfig(source2, target2);
            startlogo.Text = "准备下载配置文件";
        }

        private void LoadOther() {
            if (marquee)
            {
                marquee = false;

                try
                {
                    BroadManager.Instance.CurrentWeather = Weather.GetWeather();
                    startlogo.Text = "加载天气信息成功";
                }
                catch (Exception ex)
                {
                    startlogo.Text = "加载天气信息失败，请检查网络";
                }

                startlogo.Text = "加载媒体信息";

                Thread.Sleep(TimeSpan.FromSeconds(3));

                BroadManager.Instance.Load();

                if (Completed != null) Completed();
            }
        }

        public bool marquee = false;     
    }
}
