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

namespace ConfigApp {
	public partial class MainWindow : Window {
		public MainWindow() {
			InitializeComponent();
		}

		private void OnSave(object sender, RoutedEventArgs e) {
			foreach (var item in ConfigManager.Instance.Items) {
				if (rdback.IsChecked == true)
					item.IsBack = "true";
				else
					item.IsBack = "false";
			}
			ConfigManager.Instance.Save();
		}

		private void OnOpenPhotoDictionary(object sender, RoutedEventArgs e) {
			var index = (int)(sender as System.Windows.Controls.Button).Tag;
			var dialog = new FolderBrowserDialog();
			if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
				var item = ConfigManager.Instance.Items.FirstOrDefault(t => t.Index == index);
				item.PhotoPath = dialog.SelectedPath;
				item.ExePath = string.Empty;
				item.Url = string.Empty;
			}
		}

		private void OnOpenExeDictionary(object sender, RoutedEventArgs e) {
			var index = (int)(sender as System.Windows.Controls.Button).Tag;
			var dialog = new FolderBrowserDialog();
			if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
				var item = ConfigManager.Instance.Items.FirstOrDefault(t => t.Index == index);
				item.ExePath = dialog.SelectedPath;
				item.PhotoPath = string.Empty;
				item.Url = string.Empty;
			}
		}

		private void fileupload_MouseUp(object sender, MouseButtonEventArgs e) {
			var dialog = new OpenFileDialog();
			dialog.Filter = @"文本文件(*.txt)|*.txt";
			if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
				string path = dialog.FileName;
				foreach (var item in ConfigManager.Instance.Items) {
					item.Marquee = path;
				}
			}
		}

		private void Label_MouseUp(object sender, MouseButtonEventArgs e) {
			var dialog = new OpenFileDialog();
			var str = (sender as System.Windows.Controls.Label).Tag.ToString();
			int width = 0;
			int height = 0;
			if (str.ToLower() == "top") {
				maxwidth = 1366;
				maxheight = 126;
			} else if (str.ToLower() == "bottom") {
				maxwidth = 1366;
				maxheight = 80;
			} else if (str.ToLower() == "left") {
				maxwidth = 306;
				maxheight = 562;
			} else if (str.ToLower() == "right") {
				maxwidth = 31;
				maxheight = 562;
			}
			dialog.Filter = @"PNG 文件(*.png)|*.png";
			if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
				string path = dialog.FileName;
				ShellClass sh = new ShellClass();
				Folder dir = sh.NameSpace(System.IO.Path.GetDirectoryName(path));
				FolderItem item = dir.ParseName(System.IO.Path.GetFileName(path));
				string det = dir.GetDetailsOf(item, 31);

				Regex r = new Regex(@"(\d+)[^\d]+(\d+)");
				if (r.IsMatch(det)) {
					var m = r.Match(det);
					width = Convert.ToInt32(m.Groups[1].Value);
					height = Convert.ToInt32(m.Groups[2].Value);
				}

				if (width == maxwidth && height == maxheight) {
					Upload(path, str.ToLower());
				} else {
					System.Windows.MessageBox.Show("上传图片的像素不正确，请重新上传");
				}
			}
		}

		private void videoupload_MouseUp(object sender, MouseButtonEventArgs e) {
			var dialog = new OpenFileDialog();
			dialog.Filter = @"avi文件(*.avi)|*.avi|rmvb文件(*.rmvb)|*.rmvb|mpg文件(*.mpg)|*.mpg|wmv文件(*.wmv)|*.wmv";
			if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
				foreach (var item in ConfigManager.Instance.Items) {
					item.PlayPath = dialog.FileName;
				}
			}
		}

		private void backupload_MouseUp(object sender, MouseButtonEventArgs e) {
			var dialog = new OpenFileDialog();
			dialog.Filter = @"JPG文件(*.jpg)|*.jpg";
			if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
				string path = dialog.FileName;

				UploadJPG(path, @"background");
			}
		}

		private void UploadJPG(string path, string source) {
			string desfile = System.IO.Path.Combine(ConfigManager.CONFIGPATH, @"Resource\Images\" + source + ".jpg");
			//System.Windows.MessageBox.Show(sourcefile);
			File.Copy(path, desfile, true);
		}

		private void Upload(string path, string source) {
			string desfile = System.IO.Path.Combine(ConfigManager.CONFIGPATH, @"Resource\Images\" + source + ".png");
			//System.Windows.MessageBox.Show(sourcefile);
			File.Copy(path, desfile, true);
		}

		private int maxwidth;
		private int maxheight;
	}
}
