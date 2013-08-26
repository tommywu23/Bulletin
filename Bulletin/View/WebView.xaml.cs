using Bulletin.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using mshtml;

namespace Bulletin.View {
	public partial class WebView : Window {
		public event Action<string> OnNavigated;
		public event Action<string> OnComplete;

		public WebView() {
			InitializeComponent();		
		}

		private void Browser_Navigated(object sender, NavigationEventArgs e) {
			URL = e.Uri.ToString();
			if (OnNavigated != null) OnNavigated(URL);
		}

		private void Browser_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e) {
			try {
				foreach (IHTMLElement archor in ((IHTMLDocument2)browser.Document).links) {
					archor.setAttribute("target", "_self");
				}
				foreach (IHTMLElement form in ((IHTMLDocument2)browser.Document).forms) {
					form.setAttribute("target", "_self");
				}
				if ((browser.Document as HTMLDocument).readyState == "complete" && OnComplete != null) OnComplete(URL);
			} catch { }
		}

		private void Browser_NewWindow(object sender, CancelEventArgs e) {
			e.Cancel = true;
		}

		public void Navigate(string url) {
			string address = url.Trim();
			if (string.IsNullOrEmpty(address)) return;
			URL = address;
			try {
				if (address.StartsWith("http://"))
					browser.Navigate(new Uri(address));
				else {
					if (address.ToLower().StartsWith("about:")) {
						browser.Navigate(new Uri("about:blank"));
					} else
						browser.Navigate(new Uri("http://" + address));
				}
			} catch { }
		}

		private WebBrowserHelper webBrowserHelper;
		private string URL;

		private void Window_Loaded(object sender, RoutedEventArgs e) {
			webBrowserHelper = new WebBrowserHelper(browser);
			webBrowserHelper.NewWindow += Browser_NewWindow;

			browser.LoadCompleted += new LoadCompletedEventHandler(Browser_LoadCompleted);
			browser.Navigated += new NavigatedEventHandler(Browser_Navigated);
			browser.SuppressScriptErrors(true);
		}

		private void Window_Closing(object sender, CancelEventArgs e) {
			if (webBrowserHelper != null) webBrowserHelper.Disconnect();
		}
	}
}
