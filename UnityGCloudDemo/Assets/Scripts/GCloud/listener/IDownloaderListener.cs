using UnityEngine;
using System.Collections;

namespace GCloudSDK
{
    public abstract class IDownloaderListener : AndroidJavaProxy
    {
        public IDownloaderListener() : base("com.giant.sdk.gcloud.listener.IDownloaderListener") { }

        protected void onSuccess(string url, string file)
        {
            TwoAction action = new TwoAction((object t, object t1) => { onDownloadSuccess((string)t, (string)t1); }, url, file);
            Loom.DispatchToMainThread(action);
        }
        protected void onFailed(string url, int errorcode, string msg)
        {
            ThreeAction action = new ThreeAction((object t, object t1, object t2) => { onDownloadFailed((string)t, (int)t1, (string)t2); }, url, errorcode, msg);
            Loom.DispatchToMainThread(action);
        }
        protected void onProgress(string url, float percentindec)
        {
            TwoAction action = new TwoAction((object t, object t1) => { onDownloadProgress((string)t, (float)t1); }, url, percentindec);
            Loom.DispatchToMainThread(action);
        }
        protected void onStatus(string url, int statuscode)
        {
            TwoAction action = new TwoAction((object t, object t1) => { onDownloadStatus((string)t, (int)t1); }, url, statuscode);
            Loom.DispatchToMainThread(action);
        }
        protected void onCancel(string url)
        {
            OneAction action = new OneAction((object t) => { onDownloadCancel((string)t); }, url);
            Loom.DispatchToMainThread(action);
        }

        public abstract void onDownloadFailed(string url, int errorcode, string msg);
        public abstract void onDownloadSuccess(string url, string file);
        public abstract void onDownloadProgress(string url, float percentindec);
        public abstract void onDownloadStatus(string url, int statuscode);
        public abstract void onDownloadCancel(string url);
    }
}
