using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace BulletinLibrary
{
    public class FTPHelper
    {
        public static void UploadNews(string source, string url)
        {
            FileInfo fileInf = new FileInfo(source);
            FtpWebRequest reqFtp;

            reqFtp = (FtpWebRequest)FtpWebRequest.Create(new Uri(url));

            reqFtp.KeepAlive = false;

            reqFtp.Method = WebRequestMethods.Ftp.UploadFile;

            reqFtp.UseBinary = true;

            reqFtp.UsePassive = false;
            reqFtp.ContentLength = fileInf.Length;

            int buffLength = 4096;

            byte[] buff = new byte[buffLength];
            int contentLen;

            FileStream fs = fileInf.OpenRead();

            Stream strm = reqFtp.GetRequestStream();

            contentLen = fs.Read(buff, 0, buffLength);

            while (contentLen != 0)
            {
                strm.Write(buff, 0, contentLen);
                contentLen = fs.Read(buff, 0, buffLength);
            }

            strm.Close();
            fs.Close();


            //List<DownloadTask> list = new List<DownloadTask>();
            //string filename = @"News.xml";
            //string source = System.IO.Path.Combine(CONFIGPATH, filename);
            //string target = string.Format("{0}/{1}", ConfigurationManager.AppSettings["ftp"], filename);
            //list.Add(new DownloadTask(source, target, true));
            //logbox.Text = string.Format("{0}\n{1}", logbox.Text, "Upload News File");
            //DownloadManager.Instance.AddTasks(list);
        }
    }
}
