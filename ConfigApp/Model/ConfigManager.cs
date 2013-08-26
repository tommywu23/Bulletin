using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Xml.Serialization;
using BulletinLibrary;

namespace ConfigApp.Model {
	public class ConfigManager {
		public static ConfigManager Instance {
			get {
				return instance;
			}
		}

		public List<ItemBase> Items { get; set; }

		public void Save() {
			bool isUrl = true;
			foreach (var item in Items) {
				if (!string.IsNullOrEmpty(item.Url)) {
					if (!ItemBase.IsUrl(item.Url)) isUrl = false;
				}
			}

			if (isUrl) {
				foreach (var item in Items) {
					if (!string.IsNullOrEmpty(item.Url)) {
						item.ExePath = string.Empty;
						item.PhotoPath = string.Empty;
					}
				}

				try {
					XmlSerializer xs = new XmlSerializer(typeof(List<ItemBase>));
					using (FileStream fs = new FileStream(System.IO.Path.Combine(CONFIGPATH, "Resource/Item.xml"), FileMode.Create)) {
						xs.Serialize(fs, this.Items);
					}
				} catch (Exception ex) {
					throw ex;
				}
				Thread.Sleep(100);
				MessageBox.Show("配置文件保存成功,程序将关闭，如需继续配置请重启程序。");
				App.Current.Shutdown();
			}else{
				MessageBox.Show(string.Format("配置信息格式不正确，请检查"));
			}
		}

		public void Load() {
			System.Diagnostics.Process[] ps = System.Diagnostics.Process.GetProcessesByName("Bulletin");
			foreach (System.Diagnostics.Process p in ps) {
				p.Kill();
			}

			try {
				XmlSerializer xsi = new XmlSerializer(typeof(List<ItemBase>));
				using (FileStream fs = new FileStream(System.IO.Path.Combine(CONFIGPATH, "Resource/Item.xml"), FileMode.Open)) {
					this.Items = xsi.Deserialize(fs) as List<ItemBase>;
				}
			} catch (Exception ex) {
				MessageBox.Show(string.Format("加载信息配置文件失败:{0}", ex.ToString()));
				Application.Current.Shutdown();
			}
		}

		public static string CONFIGPATH = System.Environment.CurrentDirectory;
		private static ConfigManager instance = new ConfigManager();
	}
}
