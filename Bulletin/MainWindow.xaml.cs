using Bulletin.Model;
using Bulletin.View;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
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
using BulletinLibrary;
using System.Windows.Threading;
using System.Configuration;
//X 371 Y 174 961 * 497
namespace Bulletin {
	public partial class MainWindow : Window {
		public DateTime SysDateTime {
			get { return (DateTime)GetValue(SysDateTimeProperty); }
			set { SetValue(SysDateTimeProperty, value); }
		}

		public MainWindow() {
			InitializeComponent();
			this.Top = 0;
			this.Left = 0;
			this.Title = "BuulletinApp";
			this.Height = 768;
			this.Width = 1366;
			this.WindowStyle = System.Windows.WindowStyle.None;
			this.AllowsTransparency = true;
			this.Background = Brushes.Transparent;
			this.Topmost = true;
			this.Loaded += OnLoaded;

			BroadManager.Instance.Register();

			_tmSysDateTimer = new DispatcherTimer();
			_tmSysDateTimer.Tick += new EventHandler(tmSysDateTimer_Tick);
			_tmSysDateTimer.Interval = TimeSpan.FromSeconds(1);
			_tmSysDateTimer.Start();

			standbytimer = new System.Timers.Timer();
			standbytimer.Interval = Convert.ToDouble(ConfigurationManager.AppSettings["Interval"]);
			standbytimer.Elapsed += standbytimer_Elapsed;

			marqueetimer = new System.Timers.Timer();
			marqueetimer.Interval = Convert.ToDouble(ConfigurationManager.AppSettings["Marquee"]);
			marqueetimer.Elapsed += marqueetimer_Elapsed;

			ieWin = new WebView();
			galleryWin = new GalleryView();
			exeWin = new ExeView();
			exeWin.OnOpenExeEvent += exeWin_OnOpenExeEvent;

			if (!string.IsNullOrEmpty(BroadManager.Instance.Items[0].PlayPath))
				standby.Source = new Uri(BroadManager.Instance.Items[0].PlayPath, UriKind.Relative);
		}

		void standbytimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e) {
			this.Dispatcher.BeginInvoke(new Action(() => {
				exeWin.Hide();
				galleryWin.Hide();
				ieWin.Hide();
				buttons.SelectedIndex = -1;
				if (!BroadManager.Instance.IsBack) {
					standby.Visibility = System.Windows.Visibility.Visible;
					welcome.Visibility = System.Windows.Visibility.Hidden;
					if (standby.Source != null) {
						if (!isPlay) {
							standby.Play();
							isPlay = true;
						}
					}
				}else{
					welcome.Visibility = System.Windows.Visibility.Visible;
					standby.Visibility = System.Windows.Visibility.Hidden;
				}
			}));
		}

		void marqueetimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e) {
			this.Dispatcher.BeginInvoke(new Action(() => {
				BroadManager.Instance.LoadMarquee();
				marqueecontrol.MarqueeContent = BroadManager.Instance.Lamp;
				marqueecontrol.StartMarqueeing();
			}));
		}

		void exeWin_OnOpenExeEvent(object sender, EventArgs e) {
			IntPtr handle = ProcessEx.FindWindow(null, "BuulletinApp");
			ProcessEx.SetWindowPos(handle, 0, 0, 0, 0);
			//MessageBox.Show(handle.ToString());
			//this.WindowState = System.Windows.WindowState.Minimized;
		}

		void OnLoaded(object sender, RoutedEventArgs e) {
			if (ieWin != null) SetWindow(ieWin);
			if (galleryWin != null) SetWindow(galleryWin);
			if (exeWin != null) SetWindow(exeWin);
			if (BroadManager.Instance.IsBack) {
				welcome.Visibility = System.Windows.Visibility.Visible;
				standby.Visibility = System.Windows.Visibility.Hidden;
			}else{
				welcome.Visibility = System.Windows.Visibility.Hidden;
				standby.Visibility = System.Windows.Visibility.Visible;
			}
			if (!standbytimer.Enabled) standbytimer.Start();
			//BtnNavigate(BroadManager.Instance.Items[1]);
		}

		private void SetWindow(Window arg) {
			arg.Owner = this;
			arg.Left = 306;
			arg.Top = 126;
			arg.Width = 1041;
			arg.Height = 562;
			arg.Hide();
		}

		void tmSysDateTimer_Tick(object sender, EventArgs e) {
			SysDateTime = DateTime.Now;
		}

		private void Grid_MouseUp(object sender, MouseButtonEventArgs e) {
			if (welcome.Visibility == System.Windows.Visibility.Visible) {
				welcome.Visibility = System.Windows.Visibility.Hidden;
				standby.Visibility = System.Windows.Visibility.Hidden;
				standby.Stop();
				isPlay = false;
			}
			if (standbytimer.Enabled) standbytimer.Stop();
			BtnNavigate((sender as FrameworkElement).Tag);
			Thread.Sleep(300);
			standbytimer.Start();
		}

		private void media_MediaEnded(object sender, RoutedEventArgs e) {
			standby.Position = new TimeSpan(0, 0, 0, 0, 0);
			standby.Play();
			isPlay = true;
		}

		private void BtnNavigate(object arg) {
			if (arg != null) {
				var item = arg as ItemBase;

				if (!string.IsNullOrEmpty(BroadManager.Instance.CurrentExe)) {
					BroadManager.Instance.KillExe(BroadManager.Instance.CurrentExe);
				}

				if (!string.IsNullOrEmpty(item.Url)) {
					ieWin.Navigate(string.Format(@"http://{0}", item.Url));
					exeWin.Hide();
					galleryWin.Hide();
					ieWin.Show();
				}

				if (!string.IsNullOrEmpty(item.PhotoPath)) {
					if (BroadManager.Instance.Albums.Count > 0) {
						galleryWin.DataContext = BroadManager.Instance.Albums.FirstOrDefault(p => p.Name == item.Name);
						ieWin.Hide();
						exeWin.Hide();
						galleryWin.Show();
					}
				}

				if (!string.IsNullOrEmpty(item.ExePath)) {
					if (BroadManager.Instance.ExeFiles.Count > 0) {
						exeWin.DataContext = BroadManager.Instance.ExeFiles.FirstOrDefault(p => p.Name == item.ExePath);
						ieWin.Hide();
						galleryWin.Hide();
						exeWin.Show();
					}
				}
			}
		}

		public static readonly DependencyProperty SysDateTimeProperty =
			DependencyProperty.Register("SysDateTime", typeof(DateTime), typeof(MainWindow));

		private WebView ieWin;
		private ExeView exeWin;
		private GalleryView galleryWin;
		private DispatcherTimer _tmSysDateTimer = null;
		private System.Timers.Timer standbytimer;
		private System.Timers.Timer marqueetimer;
		private bool isPlay = false;
	}
}
