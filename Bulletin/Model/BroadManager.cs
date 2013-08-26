using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using BulletinLibrary;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Configuration;
using IWshRuntimeLibrary;
using System.Windows.Threading;
using System.Windows.Media;
using System.Windows;

namespace Bulletin.Model {
	public class BroadManager {
		public static BroadManager Instance {
			get {
				return instance;
			}
		}

		public List<SolidColorBrush> Backgrounds { get; set; }

		public BroadManager() {
			if (ExeFiles == null) ExeFiles = new List<ExeFiles>();
			if (Albums == null) Albums = new List<Albums>();
			exelocation = new System.Timers.Timer();
			exelocation.Interval = 1000;
			exelocation.Elapsed += exelocation_Elapsed;
			LoadColor();
		}

		void exelocation_Elapsed(object sender, System.Timers.ElapsedEventArgs e) {
			var pro = Process.GetProcesses().Where(t => t.ProcessName == Path.GetFileNameWithoutExtension(currentpro)).FirstOrDefault();
			if (pro != null) {
				intptrlist.Clear();
				IntPtr handle = ProcessEx.GetHandle(pro.Id);
				intptrlist.Add(handle);
				//intptrlist.Add(ProcessEx.GetWindow(handle, 0));
				//intptrlist.Add(ProcessEx.GetWindow(handle, 1));
				//intptrlist.Add(ProcessEx.GetWindow(handle, 2));
				//intptrlist.Add(ProcessEx.GetWindow(handle, 3));
				//intptrlist.Add(ProcessEx.GetWindow(handle, 4));
				//intptrlist.Add(ProcessEx.GetWindow(handle, 5));
				foreach (var each in intptrlist) {
					ProcessEx.SetWindowPos(each);
				}
			}
		}

		public string CurrentExe { get; set; }

		public Weather CurrentWeather { get; set; }

		public List<ItemBase> Items { get; set; }

		public List<ExeFiles> ExeFiles { get; set; }

		public List<Albums> Albums { get; set; }

		public bool IsBack { get; set; }

		public string Lamp { get; set; }

		//private void Upload(string path) {
		//	string desfile = System.IO.Path.Combine(ConfigManager.CONFIGPATH, @"Resource\lamp.txt");
		//	if (!File.Exists(desfile)) {
		//		var myFile = File.Create(desfile);
		//		myFile.Close();
		//	}
		//	File.Copy(path, desfile, true);
		//}

		public void Register() {
			//DateTime dt1 = DateTime.Now;
			//DateTime dt2 = DateTime.Parse("2013-06-15");
			//if (DateTime.Compare(dt1, dt2) > 0) {
			//	App.Current.Shutdown();
			//}
		}

		public void LoadMarquee(){
			if (System.IO.File.Exists(lamppath)) {
				try {
					using (FileStream fs = new FileStream(lamppath, FileMode.Open)) {
						StreamReader rd = new StreamReader(fs, System.Text.Encoding.Default);
						rd.BaseStream.Seek(0, SeekOrigin.Begin);
						Lamp = rd.ReadToEnd();
						string str = string.Empty;
						int index = 0;
						foreach (string each in Lamp.Split(new char[] { '\r', '\n' })) {
							if (index < 10) str += each + "          ";
							index++;
						}
						Lamp = str;
					}
				} catch {
					//MessageBox.Show(ex.ToString());
				}
			}
		}

		public void Load() {
			if (this.Items != null) {
				if (this.Items.Count > 0) {
					//new Thread(new ThreadStart(() => {
					foreach (var item in this.Items) {
						if (!string.IsNullOrEmpty(item.PhotoPath)) LoadPhoto(item);
						if (!string.IsNullOrEmpty(item.ExePath)) LoadExe(item.ExePath);
					}
					//})).Start();

					lamppath = this.Items[0].Marquee;
				}
			}

			if (this.Items[0].IsBack.ToLower() == "true")
				this.IsBack = true;
			else
				this.IsBack = false;


			LoadMarquee();
		}

		private void LoadPhoto(ItemBase arg) {
			try {
				if (!Directory.Exists(arg.PhotoPath)) return;
				var d = new DirectoryInfo(arg.PhotoPath);
				var albums = new BulletinLibrary.Albums();
				albums.Index = arg.Index;
				albums.Name = arg.Name;
				albums.Path = arg.PhotoPath;

				foreach (var each in d.GetDirectories()) {
					var album = new Album();
					if (!Directory.Exists(each.FullName)) return;
					album.AlbumName = each.Name;
					var q = each.GetFiles().Where(p => !p.Attributes.HasFlag(FileAttributes.Hidden)
							 && allowsPhotoExtensions.Contains(Path.GetExtension(p.FullName).ToLower()))
							 .Select((p, i) => new Photo(p.FullName) { PhotoNumber = i, BelongsTo = album });
					if (q != null) {
						foreach (var item in q) {
							album.Photos.Add(item);
							if (album.Photos.Count > Convert.ToInt32(ConfigurationManager.AppSettings["MaxPhoto"])) break;
						}
						//album.ToList().AddRange(q);
						albums.Add(album);
					}

					if (albums.Count > Convert.ToInt32(ConfigurationManager.AppSettings["MaxAlbum"])) break;
				}

				this.Albums.Add(albums);
			} catch {

			}
		}

		private void LoadExe(string path) {
			if (!Directory.Exists(path)) return;
			var d = new DirectoryInfo(path);
			var files = new ExeFiles();
			files.Name = d.FullName;
			foreach (var each in d.GetFiles().Where(p => !p.Attributes.HasFlag(FileAttributes.Hidden)
						 && allowsExeExtensions.Contains(Path.GetExtension(p.FullName).ToLower()))) {
				var file = new ExeFile();
				file.FileName = Path.GetFileNameWithoutExtension(each.Name);
				file.Path = each.FullName;
				files.Add(file);
			}

			this.ExeFiles.Add(files);
		}

		private void LoadColor() {
			Backgrounds = new List<SolidColorBrush>();
			SolidColorBrush red = new SolidColorBrush();
			red.Color = Color.FromArgb(255, 188, 20, 26);
			SolidColorBrush black = new SolidColorBrush();
			black.Color = Color.FromArgb(255, 26, 26, 26);
			SolidColorBrush color3 = new SolidColorBrush();
			color3.Color = Color.FromArgb(255, 142, 43, 133);
			SolidColorBrush color4 = new SolidColorBrush();
			color4.Color = Color.FromArgb(255, 76, 61, 142);
			SolidColorBrush color5 = new SolidColorBrush();
			color5.Color = Color.FromArgb(255, 155, 200, 58);
			SolidColorBrush color6 = new SolidColorBrush();
			color6.Color = Color.FromArgb(255, 0, 139, 162);
			Backgrounds.Add(red);
			Backgrounds.Add(black);
			Backgrounds.Add(color3);
			Backgrounds.Add(color4);
			Backgrounds.Add(color5);
			Backgrounds.Add(color6);
		}

		private string GetFullName(string path) {
			string result;
			var shell = new WshShell();
			var shortcut = (IWshRuntimeLibrary.IWshShortcut)shell.CreateShortcut(path);
			result = shortcut.TargetPath;
			return result;
		}

		public void KillExe(string path) {
			System.Diagnostics.Process[] ps = System.Diagnostics.Process.GetProcessesByName(Path.GetFileNameWithoutExtension(GetFullName(path)));
			foreach (System.Diagnostics.Process p in ps) {
				p.Kill();
			}

			if (!string.IsNullOrEmpty(this.CurrentExe)) this.CurrentExe = string.Empty;
		}

		public void ExeOpen(string arg) {
			string path = GetFullName(arg);
			BroadManager.Instance.KillExe(arg);
			Process.Start(new ProcessStartInfo(path));
			Thread.Sleep(500);
			//if (exelocation.Enabled) exelocation.Stop();
			currentpro = path;
			//exelocation.Start();
		}

		private string currentpro;
		private System.Timers.Timer exelocation;
		private static List<IntPtr> intptrlist = new List<IntPtr>();
		//private string lamppath = System.IO.Path.Combine(App.CONFIGPATH, "Resource/lamp.txt");
		private string lamppath = string.Empty;
		private static BroadManager instance = new BroadManager();
		private readonly string[] allowsExeExtensions = new string[] { ".lnk" };
		private readonly string[] allowsPhotoExtensions = new string[] { ".png", ".jpg" };
	}

	public static class ProcessEx {
		private static class NativeMethods {
			internal const uint GW_OWNER = 4;

			internal delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

			[DllImport("User32.dll", CharSet = CharSet.Auto)]
			internal static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

			[DllImport("User32.dll", CharSet = CharSet.Auto)]
			internal static extern int GetWindowThreadProcessId(IntPtr hWnd, out IntPtr lpdwProcessId);

			[DllImport("User32.dll", CharSet = CharSet.Auto)]
			internal static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);

			[DllImport("User32.dll", CharSet = CharSet.Auto)]
			internal static extern bool IsWindowVisible(IntPtr hWnd);

			[DllImport("User32.dll", EntryPoint = "SetWindowPos")]
			internal static extern int SetWindowPos(IntPtr win_handle, int hWndInsertAfter, int x, int y, int width, int height, uint flags);

			[DllImport("user32.dll", SetLastError = true)]
			internal static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

		}

		public static IntPtr FindWindow(string lpClassName, string lpWindowName) {

			return NativeMethods.FindWindow(lpClassName, lpWindowName);
		}

		public static IntPtr GetWindow(IntPtr handle, uint cmd) {

			return NativeMethods.GetWindow(handle, cmd);
		}

		public static void SetWindowPos(IntPtr handle) {
			NativeMethods.SetWindowPos(handle, -1, 371, 174, 961, 497, 0x0040);
		}

		public static void SetWindowPos(IntPtr handle, int x, int y, int width, int height) {
			NativeMethods.SetWindowPos(handle, -1, x, y, width, height, 0x0040);
		}

		public static IntPtr GetHandle(int processId) {
			IntPtr MainWindowHandle = IntPtr.Zero;

			NativeMethods.EnumWindows(new NativeMethods.EnumWindowsProc((hWnd, lParam) => {
				IntPtr PID;
				NativeMethods.GetWindowThreadProcessId(hWnd, out PID);

				if (PID == lParam &&
					NativeMethods.IsWindowVisible(hWnd) &&
					NativeMethods.GetWindow(hWnd, NativeMethods.GW_OWNER) == IntPtr.Zero) {
					MainWindowHandle = hWnd;
					return false;
				}

				return true;

			}), new IntPtr(processId));

			return MainWindowHandle;
		}
	}
}
