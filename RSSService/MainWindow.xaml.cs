using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using System.Xml.Serialization;
using BulletinLibrary;
using System.ServiceModel.Syndication;
using System.Configuration;
using System.Net;
using System.Threading;
using System.Windows.Threading;

namespace RSSService
{
    public partial class MainWindow : Window
    {
        public List<RSS> RssItems { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            rssservice = new DispatcherTimer();
            double min;
            if (!double.TryParse(ConfigurationManager.AppSettings["interval"], out min)) min = 50;

            rssservice.Interval = TimeSpan.FromMinutes(min);
            rssservice.Tick += rssservice_Tick;
            this.Loaded += OnLoaded;
        }

        void rssservice_Tick(object sender, EventArgs e)
        {
            DownloadConfig();
        }

        void OnLoaded(object sender, RoutedEventArgs e)
        {
            DownloadManager.Instance.Completed += (task) =>
            {
                if (isdownload)
                {
                    File.Move(task.Temp.AbsolutePath, task.Dest.AbsolutePath);
                    logbox.Text = string.Format("{0}\n{1}", logbox.Text, "Download Config File->Finish[" + DateTime.Now.ToLongTimeString() + "]");
                    LoadRss(task.Dest.AbsolutePath);
                    logbox.Text = string.Format("{0}\n{1}", logbox.Text, "Loading Config File->Finish[" + DateTime.Now.ToLongTimeString() + "]");
                    logbox.ScrollToEnd();
                    LoadFeed();
                }
                else
                {
                    logbox.Text = string.Format("{0}\n{1}", logbox.Text, "Upload News File->Finish[" + DateTime.Now.ToLongTimeString() + "]");
                    logbox.ScrollToEnd();
                }
            };

            DownloadManager.Instance.Failed += (task) =>
            {
                if (isdownload)
                {
                    logbox.Text = string.Format("{0}\n{1}", logbox.Text, "Download Config File->Failed[" + DateTime.Now.ToLongTimeString() + "]");
                }
                else
                {
                    logbox.Text = string.Format("{0}\n{1}", logbox.Text, "Upload News File->Failed[" + DateTime.Now.ToLongTimeString() + "]");
                }
                logbox.ScrollToEnd();
            };

            DownloadConfig();
            rssservice.Start();
        }

        private void LoadRss(string url)
        {
            if (File.Exists(url))
            {
                try
                {
                    XmlSerializer xsi = new XmlSerializer(typeof(List<RSS>));
                    using (FileStream fs = new FileStream(System.IO.Path.Combine(CONFIGPATH, "Resource/RSS.xml"), FileMode.Open))
                    {
                        this.RssItems = xsi.Deserialize(fs) as List<RSS>;
                    }
                }
                catch (Exception ex)
                {

                }
            }
        }

        private void SaveNews()
        {
            try
            {
                XmlSerializer xs = new XmlSerializer(typeof(List<string>));
                using (FileStream fs = new FileStream(System.IO.Path.Combine(CONFIGPATH, "News.xml"), FileMode.Create))
                {
                    xs.Serialize(fs, news);
                }

                logbox.Text = string.Format("{0}\n{1}-->{2}[{3}]", logbox.Text, "Save News", "Success", DateTime.Now.ToLongTimeString());
                logbox.ScrollToEnd();              
            }
            catch (Exception ex)
            {
                logbox.Text = string.Format("{0}\n{1}-->{2}[{3}]", logbox.Text, "Save News", "Failed", DateTime.Now.ToLongTimeString());
            }

            string filename = @"News.xml";
            string source = System.IO.Path.Combine(CONFIGPATH, filename);
            string url = string.Format("{0}/{1}", ConfigurationManager.AppSettings["ftp"].ToString(), filename);
            try
            {
                FTPHelper.UploadNews(source, url);
                logbox.Text = string.Format("{0}\n{1}-->{2}[{3}]", logbox.Text, "Upload News", "Complated", DateTime.Now.ToLongTimeString());
            }
            catch (Exception ex)
            {
                logbox.Text = string.Format("{0}\n{1}-->{2}[{3}]", logbox.Text, "Upload News", "Failed", DateTime.Now.ToLongTimeString());
            }

            logbox.ScrollToEnd();  
        }

        private void LoadFeed()
        {
            if (news.Count > 0) news.Clear();
            foreach (var each in this.RssItems)
            {
                try
                {
                    logbox.Text = string.Format("{0}\n{1}-->{2}[{3}]", logbox.Text, "Loading RSS Fead Source", each.Source.ToString(), DateTime.Now.ToLongTimeString());
                    //RSS 1.0 Load Methos
                    //using (var reader = XmlReader.Create(each.Source.ToString()))
                    //{
                    //    logbox.Text = string.Format("{0}\n{1}-->{2}[{3}]", logbox.Text,
                    //        "Loading RSS Fead Source", each.Source.ToString(), DateTime.Now.ToLongTimeString());

                    //    var data = SyndicationFeed.Load(reader);

                    //    foreach (SyndicationItem item in data.Items)
                    //    {
                    //        logbox.Text = string.Format("{0}\n{1}[{2}]", logbox.Text, DateTime.Now.ToLongTimeString(), item.Title.Text.ToString());
                    //    }
                    //}
                    var doc = new XmlDocument();
                    doc.Load(each.Source.ToString());
                    XmlNodeList list = doc.GetElementsByTagName("item");
                    foreach (XmlNode node in list)
                    {
                        XmlElement ele = (XmlElement)node;
                        var title = ele.GetElementsByTagName("title")[0].InnerText;
                        news.Add(title);

                        logbox.Text = string.Format("{0}\n{1}[{2}]", logbox.Text, DateTime.Now.ToLongTimeString(), title);

                    }
                }
                catch (Exception ex)
                {
                    logbox.Text = string.Format("{0}\n{1}-->{2}[{3}]", logbox.Text,
                           "Loading Failed", each.Source.ToString(), DateTime.Now.ToLongTimeString());
                }
                finally
                {
                    logbox.ScrollToEnd();
                }               
            }

            SaveNews();
        }

       

        private void DownloadConfig()
        {
            isdownload = true;
            logbox.Text = string.Format("{0}\n{1}", logbox.Text, "Service Start");
            List<DownloadTask> list = new List<DownloadTask>();
            string source = string.Format("{0}/{1}", ConfigurationManager.AppSettings["ftp"], @"Rss.xml");
            list.Add(new DownloadTask(source,
                System.IO.Path.Combine(CONFIGPATH, "Resource/Rss.xml"), true));
            logbox.Text = string.Format("{0}\n{1}", logbox.Text, "Downloading Config File");
            DownloadManager.Instance.AddTasks(list);
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            if (rssservice == null) return;
            rssservice.Start();
            DownloadConfig();
            stopservice.Visibility = System.Windows.Visibility.Visible;
            startservice.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            if (rssservice == null) return;
            rssservice.Stop();
            stopservice.Visibility = System.Windows.Visibility.Collapsed;
            startservice.Visibility = System.Windows.Visibility.Visible;
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            logbox.Text = "";
        }

        private DispatcherTimer rssservice;
        private bool isdownload = false;
        private List<string> news = new List<string>();
        public static string CONFIGPATH = ConfigurationManager.AppSettings["root"];
    }
}
