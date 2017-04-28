using UnityEngine;

namespace GCloudSDK
{
    public class GCloudBlank
    {
		#if UNITY_ANDROID
		protected AndroidJavaObject gCloudJavaObject = null;

		protected AndroidJavaClass _activityClass = null;
		protected AndroidJavaObject _activityContext = null;
		#endif
        public virtual void Destroy()
        {
        }

        public virtual bool Initialize(int GID, int ZID, long UID, string clientName, string clientVer)
        {
            return false;
        }

        public void SetLogDebug(bool flag)
        {

        }

        public void SwitchServer(int GID, int ZID, long UID, string clientName, string clientVer)
        {

        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public bool IsGCloudFileExist(string file)
        {
            return false;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public bool DeleteGCloudFile(string file)
        {
            return false;
        }

        // <summary>
        ///
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public bool IsFileExist(string file)
        {
            return false;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public bool DeleteFile(string file)
        {
            return false;
        }

        /// <summary>
        ///
        /// </summary>
        public void DeleteAllGCloudFile()
        {
            
        }

        public void SetUploaderListener(IUploaderListener listener)
        {

        }

        public void UploadFile(string filename)
        {
            
        }

        public void SetDownloaderListener(IDownloaderListener listener)
        {

        }

        public void DownloadFile(string url)
        {
            
        }

		/** 设置服务器*/
		public void SetHttpServer(string httpsrv)
		{
		
		}

		/** 暂停 可以恢复*/
		public void PauseDownload(string url)
		{

		}
		
		/** 暂停后继续下载 */
		public void ResumeDownload(string url)
		{

		}
		
		/** 取消 不可以恢复*/
		public void CancelDownload(string url)
		{

		}

		/** 发送二进制数据文件到服务器 */
		public string UploadBinary(byte[] buffer, string extension)
		{
			return "";
		}
		
		/** 发送文件到服务器 永久存储*/
		public void UploadPersistFile(string fileDir)
		{

		}
		
		/** 发送二进制数据文件到服务器 永久存储*/
		public string UploadPersistBinary(byte[] buffer, string extension)
		{
			return "";
		}
		
		/** 取消上传 */
		public void CancelFileUpload(string file)
		{
		
		}


    }
}
