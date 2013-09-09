using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using System.Xml.Serialization;
using BulletinLibrary;
using Bulletin.Model;
using System.Reflection;
using System.Windows.Threading;

namespace Bulletin
{
    public partial class App : Application
    {
        public bool IsBack { get; set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            if (File.Exists(valider))
            {
                var dll = Assembly.LoadFile(valider);
                Type type = dll.GetType("Config.Valider");
                var d = type.InvokeMember("Do", BindingFlags.DeclaredOnly |
                            BindingFlags.Public | BindingFlags.Static | BindingFlags.InvokeMethod, null, null, null);
                if (!Convert.ToBoolean(d))
                {
                    MessageBox.Show("程序已过期，请联系开发公司...");
                    App.Current.Shutdown();
                }
            }

            if (!Funtions.CheckfirstInstance())
            {
                MessageBox.Show("已经启动了一个程序。");
                return;
            }

            SplashWindow appSplash = new SplashWindow();
            appSplash.Show();
            appSplash.Completed += () =>
            {
                Thread.Sleep(100);
                if (main == null)
                {
                    main = new MainWindow();
                    appSplash.Close();
                    main.Show();
                }
                else
                {
                    if (!main.reloadtimer.Enabled) main.reloadtimer.Start();
                    if (!main.marqueetimer.Enabled) main.marqueetimer.Start();
                }             

            };

            base.OnStartup(e);
        }

        public static MainWindow main;
        private static string valider = System.IO.Path.Combine(Environment.CurrentDirectory, @"config.dll");
        public static string CITYCODE = "2137082";
        public static string CONFIGPATH = ConfigurationManager.AppSettings["root"];
    }
}
