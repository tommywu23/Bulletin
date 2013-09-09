using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Threading;

namespace BulletinLibrary {
    public delegate void DownloadHandler(DownloadTask task);

    public class Downloader {
        public event DownloadHandler Completed;
        public event DownloadHandler Failed;
        public event DownloadHandler ProgressChanged;

        public string Id { get { return id; } }
        public bool IsBusy { get { return isBusy; } }

        public Downloader() {
            id = Guid.NewGuid().ToString();
            scheme2action[Uri.UriSchemeFtp] = new Action(FtpDownload);
        }

        public void Start(DownloadTask task) {
            this.task = task;
            isBusy = true;
            Start();
        }

        public void Stop() {
            exit = true;
        }

        public void BeginStart(DownloadTask task) {
            this.task = task;
            isBusy = true;
            new Thread(new ThreadStart(() => {
                Start();
            })).Start();
        }

        private void Start() {
            if (scheme2action.ContainsKey(task.Source.Scheme)) {
                task.Status = TaskStatus.Downloading;
                scheme2action[task.Source.Scheme]();
            } else {
                task.Status = TaskStatus.Abandon;
                FireCompleted(false);
            }
        }

        private void FtpDownload() {
            task.Temp = new Uri(string.Format("{0}.temp", task.Dest.AbsolutePath));
            if (File.Exists(task.Dest.AbsolutePath) && !task.Overwrite) {
                task.Status = TaskStatus.Completed;
                FireCompleted(false);
            } else if (File.Exists(task.Temp.AbsolutePath) && !task.Overwrite) {
                task.Status = TaskStatus.Completed;
                FireCompleted();
            } else {
                downPath = string.Format("{0}.down", task.Dest.AbsolutePath);
                totalSize = GetFileSize();
                FileInfo fileInfo = new FileInfo(downPath);
                if (!fileInfo.Directory.Exists) Directory.CreateDirectory(fileInfo.DirectoryName);
                commitBytesCount = fileInfo.Exists ? (int)fileInfo.Length : 0;
                if (FTPDownloadFile()) {
                    FinishDownload();
                    FireCompleted();
                } else {
                    FireFailed();
                }
            }
        }

        private long GetFileSize() {
            long result = 0;

            FtpWebRequest getSizeReq = FtpWebRequest.Create(task.Source.AbsoluteUri) as FtpWebRequest;
            getSizeReq.KeepAlive = false;
            getSizeReq.Method = WebRequestMethods.Ftp.GetFileSize;
            getSizeReq.UseBinary = true;
            getSizeReq.Timeout = 10000;
            getSizeReq.ReadWriteTimeout = 10000;
            try {
                using (FtpWebResponse getSizeRes = (FtpWebResponse)getSizeReq.GetResponse()) {
                    result = getSizeRes.ContentLength;
                }
            } catch (Exception ex) {
                Console.WriteLine(ex);
                result = 0;
            }

            return result;
        }

        private bool FTPDownloadFile() {
            bool result = false;

            FtpWebRequest req = FtpWebRequest.Create(task.Source.AbsoluteUri) as FtpWebRequest;
            req.KeepAlive = false;
            req.Method = WebRequestMethods.Ftp.DownloadFile;
            req.UseBinary = true;
            req.Timeout = 10000;
            req.ReadWriteTimeout = 10000;
            req.ContentOffset = commitBytesCount;

            try {
                using (FileStream fs = new FileStream(downPath, FileMode.Append)) {
                    using (Stream reqStream = req.GetResponse().GetResponseStream()) {
                        byte[] buffer = new byte[bufferSize];
                        int bytesCount = 0;

                        while (!exit) {
                            if ((bytesCount = reqStream.Read(buffer, 0, bufferSize)) == 0) {
                                result = true;
                                break;
                            } else {
                                fs.Write(buffer, 0, bytesCount);
                                fs.Flush();
                                commitBytesCount += bytesCount;

                                if (totalSize != 0) {
                                    task.Progress = (double)commitBytesCount / (double)totalSize;
                                } else {
                                    task.Progress = -1;
                                }
                                if (ProgressChanged != null) ProgressChanged(task);
                            }
                        }
                    }
                }
            } catch (Exception ex) {
                Console.WriteLine(ex);
                task.Status = TaskStatus.Waitting;
                FireFailed();
            }

            return result;
        }

        private void FinishDownload() {
            try {
                if (File.Exists(task.Dest.AbsolutePath)) File.Delete(task.Dest.AbsolutePath);
                if (File.Exists(task.Temp.AbsolutePath)) File.Delete(task.Temp.AbsolutePath);
                File.Move(downPath, task.Temp.AbsolutePath);
                task.Status = TaskStatus.Completed;
            } catch (IOException ex) {
                Console.WriteLine(ex);
                task.Status = TaskStatus.Waitting;
                FireFailed();
            }
        }

        private void FireCompleted() {
            FireCompleted(true);
        }

        private void FireCompleted(bool needUpdate) {
            isBusy = false;
            task.NeedUpdate = needUpdate;
            if (Completed != null) Completed(task);
        }

        private void FireFailed() {
            isBusy = false;
            task.NeedUpdate = false;
            if (Failed != null) Failed(task);
        }

        private Dictionary<string, Action> scheme2action = new Dictionary<string, Action>();
        private string id;
        private bool isBusy;
        private bool exit;
        private DownloadTask task;
        private const int bufferSize = 1 * 1024 * 1024;
        private string downPath;
        private long totalSize;
        private long commitBytesCount;
    }

    public class DownloadTask {
        public Uri Source { get; set; }
        public Uri Dest { get; set; }
        public Uri Temp { get; set; }
        public bool Overwrite { get; set; }
        public TaskStatus Status { get; set; }
        public double Progress { get; set; }
        public bool NeedUpdate { get; set; }

        public DownloadTask(string source, string dest, bool overwrite = false)
            : this(new Uri(source), new Uri(dest), overwrite) { }

        public DownloadTask(Uri source, Uri dest, bool overwrite = false) {
            this.Source = source;
            this.Dest = dest;
            this.Overwrite = overwrite;
            this.Progress = 0;
        }

        public bool Defer() {
            return ++deferCount <= 5;
        }

        private int deferCount;
    }

    public enum TaskStatus {
        Waitting,
        Downloading,
        Completed,
        Abandon
    }
}
