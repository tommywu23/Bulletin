using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Interop;
using System.Windows.Controls;
using System.ComponentModel;
using OS;
using System.Threading;
using System.Windows.Input;
using ClientContract.Helper;
using System.Windows.Media.Animation;

namespace ClientContract {
	public class GTWindowBase : Window {
		public GTWindowBase() {
			this.WindowStyle = WindowStyle.None;
			this.AllowsTransparency = true;
			this.Background = Brushes.Transparent;
			this.Left = 0;
			this.Top = 0;
			this.Width = SystemParameters.PrimaryScreenWidth;
			this.Height = SystemParameters.PrimaryScreenHeight - this.Top;

#if DEBUG
#else
			this.Cursor = Cursors.None;
#endif

			this.SourceInitialized += (s1, e1) => {
				(PresentationSource.FromVisual(s1 as Visual) as HwndSource).AddHook(WindowProc);
			};

			this.Loaded += OnLoaded;
			this.Closed += OnClosed;
		}

		private void OnClosed(object sender, EventArgs e) {
			var parent = ParentProcessUtilities.GetParentProcess();
			Win32.SendMessage(parent.MainWindowHandle, GTAppMessage.AppClosed, 0, 0);
		}

		protected IntPtr WindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled) {
			if (msg == GTAppMessage.OpenApp) {
				if (showStory != null) {
					showStory.Begin();
				}
			} else if (msg == GTAppMessage.CloseApp) {
				CloseApp();
			}

			return IntPtr.Zero;
		}

		protected virtual void OpenApp() {

		}

		protected virtual void CloseApp() {
			var parent = ParentProcessUtilities.GetParentProcess();
			if (parent != null) {
				Win32.PostMessage(parent.MainWindowHandle, GTAppMessage.AppClosed, 0, 0);
			}

			if (hideStory != null) {
				hideStory.Begin();
			}
		}

		private void OnLoaded(object sender, RoutedEventArgs e) {
			var p = this.Content as Panel;
			p.RenderTransformOrigin = new Point(.5, .5);
			p.RenderTransform = new ScaleTransform();

			showStory = new Storyboard();
			DoubleAnimation a1 = new DoubleAnimation(1, TimeSpan.FromMilliseconds(300));
			DoubleAnimation a2 = new DoubleAnimation(1, TimeSpan.FromMilliseconds(300));
			Storyboard.SetTarget(a1, p);
			Storyboard.SetTarget(a2, p);
			Storyboard.SetTargetProperty(a1, new PropertyPath("RenderTransform.ScaleX"));
			Storyboard.SetTargetProperty(a2, new PropertyPath("RenderTransform.ScaleY"));
			showStory.Children.Add(a1);
			showStory.Children.Add(a2);
			showStory.Completed += OnShowStoryCompleted;

			hideStory = new Storyboard();
			DoubleAnimation a3 = new DoubleAnimation(0, TimeSpan.FromMilliseconds(300));
			DoubleAnimation a4 = new DoubleAnimation(0, TimeSpan.FromMilliseconds(300));
			Storyboard.SetTarget(a3, p);
			Storyboard.SetTarget(a4, p);
			Storyboard.SetTargetProperty(a3, new PropertyPath("RenderTransform.ScaleX"));
			Storyboard.SetTargetProperty(a4, new PropertyPath("RenderTransform.ScaleY"));
			hideStory.Children.Add(a3);
			hideStory.Children.Add(a4);

			Win32.SetForegroundWindow(new WindowInteropHelper(this).Handle);
		}

		void OnShowStoryCompleted(object sender, EventArgs e) {
			new Thread(() => {
				Thread.Sleep(100);
				Dispatcher.BeginInvoke((Action)(() => OpenApp()));
			}).Start();
		}

		private Storyboard showStory;
		private Storyboard hideStory;
	}
}

