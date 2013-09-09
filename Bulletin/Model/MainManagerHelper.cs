using BulletinLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Xml.Serialization;

namespace Bulletin.Model
{
    public class MainManager
    {
        public static void DownloadConfig(string source, string target)
        {
            List<DownloadTask> list = new List<DownloadTask>();

            list.Add(new DownloadTask(source, target, true));

            DownloadManager.Instance.AddTasks(list);
        }

        public static void LoadItems(string filename)
        {
            try
            {
                XmlSerializer xsi = new XmlSerializer(typeof(List<ItemBase>));
                using (FileStream fs = new FileStream(filename, FileMode.Open))
                {
                    BroadManager.Instance.Items = xsi.Deserialize(fs) as List<ItemBase>;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("加载信息配置文件失败:{0}", ex.ToString()));
                Application.Current.Shutdown();
            }
        }

        public static int[] GetRandomArray(int Number, int minNum, int maxNum)
        {
            int j;
            int[] b = new int[Number];
            Random r = new Random();
            for (j = 0; j < Number; j++)
            {
                int i = r.Next(minNum, maxNum + 1);
                int num = 0;
                for (int k = 0; k < j; k++)
                {
                    if (b[k] == i)
                    {
                        num = num + 1;
                    }
                }
                if (num == 0)
                {
                    b[j] = i;
                }
                else
                {
                    j = j - 1;
                }
            }
            return b;
        }
    }
}
