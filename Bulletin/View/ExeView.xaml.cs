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
using System.Windows.Shapes;
using BulletinLibrary;
using Bulletin.Model;

namespace Bulletin.View {
	public delegate void OpenExeEventHandler(object sender, EventArgs e);
	public partial class ExeView : Window {
		public event OpenExeEventHandler OnOpenExeEvent;
		public ExeView() {
			InitializeComponent();
			this.WindowStyle = System.Windows.WindowStyle.None;
			this.AllowsTransparency = true;
		}

		private void Grid_MouseUp(object sender, MouseButtonEventArgs e) {
			string exe = ((sender as Grid).Tag as ExeFile).Path;
			if (!string.IsNullOrEmpty(exe)) {
				BroadManager.Instance.ExeOpen(exe);
				BroadManager.Instance.CurrentExe = exe;
				if (OnOpenExeEvent != null) OnOpenExeEvent(this, EventArgs.Empty);
			}
		}
	}
}
