using UnityEngine;
using System.Collections;

namespace GCloudSDK
{
    public abstract class IUploaderListener : AndroidJavaProxy
    {
        public IUploaderListener() : base("com.giant.sdk.gcloud.listener.IUploaderListener") { }
  
        protected void onSuccess(string file, string url)
        {
            TwoAction action = new TwoAction((object t, object t1) => { onUploadSuccess((string)t, (string)t1); }, file, url);
            Loom.DispatchToMainThread(action);
        }
        protected void onFailed(string file, int errorcode, string msg)
        {
            ThreeAction action = new ThreeAction((object t, object t1, object t2) => { onUploadFailed((string)t, (int)t1, (string)t2); }, file, errorcode, msg);
            Loom.DispatchToMainThread(action);
        }
        protected void onProgress(string file, float percentindec)
        {
            TwoAction action = new TwoAction((object t, object t1) => { onUploadProgress((string)t, (float)t1); }, file, percentindec);
            Loom.DispatchToMainThread(action);
        }
        protected void onStatus(string file, int statuscode)
        {
            TwoAction action = new TwoAction((object t, object t1) => { onUploadStatus((string)t, (int)t1); }, file, statuscode);
            Loom.DispatchToMainThread(action);
        }
        protected void onCancel(string file)
        {
            OneAction action = new OneAction((object t) => { onUploadCancel((string)t); }, file);
            Loom.DispatchToMainThread(action);
        }

        public abstract void onUploadSuccess(string file, string url);
        public abstract void onUploadFailed(string file, int errorcode, string msg);
        public abstract void onUploadProgress(string file, float percentindec);
        public abstract void onUploadStatus(string file, int statuscode);
        public abstract void onUploadCancel(string file);
    }
}
