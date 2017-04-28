using UnityEngine;
using System.Collections;

#if UNITY_ANDROID
namespace GCloudSDK
{
    public class GCloudAndroid
    {
        protected AndroidJavaObject gCloudJavaObject = null;

        protected AndroidJavaClass _activityClass = null;
        protected AndroidJavaObject _activityContext = null;

		public GCloudAndroid(){
			gCloudJavaObject = new AndroidJavaObject("com.giant.sdk.gcloud.GCloud");
		}

        /// <summary>
        ///
        /// </summary>
        /// <param name="con"></param>
        /// <param name="GID"></param>
        /// <param name="ZID"></param>
        /// <param name="UID"></param>
        /// <param name="clientName"></param>
        /// <param name="clientVer"></param>
        /// <returns></returns>
        public virtual bool Initialize(/*Context con,*/ int GID, int ZID, long UID, string clientName, string clientVer)
        {
//            if (gCloudJavaObject == null)
//            {
//                gCloudJavaObject = new AndroidJavaObject("com.giant.sdk.gcloud.GCloud");
//            }
            _activityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            _activityContext = _activityClass.GetStatic<AndroidJavaObject>("currentActivity");

            return gCloudJavaObject.Call<bool>("initialize", _activityContext, GID, ZID, UID, clientName, clientVer);
        }

        /// <summary>
        ///
        /// </summary>
        public virtual void Destroy()
        {
            gCloudJavaObject.Call("destroy");

            if (gCloudJavaObject != null)
            {
                gCloudJavaObject.Dispose();
                gCloudJavaObject = null;
            }

            if (_activityClass != null)
            {
                _activityClass.Dispose();
                _activityClass = null;
            }

            if (_activityContext != null)
            {
                _activityContext.Dispose();
                _activityContext = null;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="flag"></param>
        public void SetLogDebug(bool flag)
        {
            gCloudJavaObject.Call("setLogDebug", flag);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="GID"></param>
        /// <param name="ZID"></param>
        /// <param name="UID"></param>
        /// <param name="appName"></param>
        /// <param name="appVer"></param>
        public void SwitchServer(int GID, int ZID, long UID, string appName, string appVer)
        {
            gCloudJavaObject.Call("switchServer", GID, ZID, UID, appName, appVer);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public bool IsGCloudFileExist(string file)
        {
            return gCloudJavaObject.Call<bool>("isGCloudFileExist", file);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public bool DeleteGCloudFile(string file)
        {
            return gCloudJavaObject.Call<bool>("deleteGCloudFile", file);
        }

        // <summary>
        ///
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public bool IsFileExist(string file)
        {
            return gCloudJavaObject.Call<bool>("isFileExist", file);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public bool DeleteFile(string file)
        {
            return gCloudJavaObject.Call<bool>("deleteFile", file);
        }

        /// <summary>
        ///
        /// </summary>
        public void DeleteAllGCloudFile()
        {
            gCloudJavaObject.Call("deleteAllGCloudFile");
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="listener"></param>
        public void SetUploaderListener(IUploaderListener listener)
        {
            gCloudJavaObject.Call("setUploaderListener", listener);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public void UploadFile(string filename)
        {
            gCloudJavaObject.Call("uploadFile", filename);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="listener"></param>
        public void SetDownloaderListener(IDownloaderListener listener)
        {
            gCloudJavaObject.Call("setDownloaderListener", listener);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public void DownloadFile(string url)
        {
            gCloudJavaObject.Call("downloadFile", url);
        }

        public void SetHttpServer(string httpsrv)
        {
            gCloudJavaObject.Call("setHttpServer", httpsrv);
        }

        /// <summary>
        /// 上传二进制数据
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="len"></param>
        /// <param name="extension">extension without .</param>
        /// <returns>临时文件路径</returns>
        public string UploadBinary(byte[] buffer, string extension)
        {
            return gCloudJavaObject.Call<string>("uploadBinary", buffer, extension);
        }

        /// <summary>
        /// 上传永久存储的二进制数据
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="len"></param>
        /// <param name="extension">extension without .</param>
        /// <returns>临时文件路径</returns>
        public string UploadPersistBinary(byte[] buffer, string extension)
        {
            return gCloudJavaObject.Call<string>("uploadPersistBinary", buffer, extension);
        }
 
        /// <summary>
        /// 上传永久存储的文件
        /// </summary>
        /// <param name="file"></param>
        public void UploadPersistFile(string file)
        {
            gCloudJavaObject.Call("uploadPersistFile", file);
        }

        /// <summary>
        /// 根据url删除永久存储的文件
        /// </summary>
        /// <param name="url"></param>
        public void RemovePersistFile(string url)
        {
            gCloudJavaObject.Call("removePersistFile", url);
        }

        public void CancelFileUpload(string file)
        {
            gCloudJavaObject.Call("cancelUpload", file);
        }

        public void CancelFileDownload(string url)
        {
            gCloudJavaObject.Call("cancelDownload", url);
        }
    }
}
#endif