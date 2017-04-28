using UnityEngine;
using System.Collections;
#if (UNITY_IOS || UNITY_IPHONE)
using AOT;
using System.Runtime.InteropServices;
using System;

namespace GCloudSDK
{
	public class GCloudIOS 
    {
		static protected IUploaderListener _uploadListener = null;
		static protected UploadCallBack _uploadCallback = new UploadCallBack();

		static protected IDownloaderListener _downloadListener = null;
		static protected DownloadCallBack _downloadCallback = new DownloadCallBack();

		[DllImport("__Internal")]
		protected static extern void GCloud_Constructer();
		
		[DllImport("__Internal")]
		protected static extern bool GCloud_Initialize(int GID, int ZID, long UID, string clientName, string clientVer);

		[DllImport("__Internal")]
		protected static extern bool GCloud_Destroy ();

		[DllImport("__Internal")]
		protected static extern void GCloud_SetLogDebug(bool flag);

		[DllImport("__Internal")]
		protected static extern void GCloud_SwitchServer(int GID, int ZID, long UID, string clientName, string clientVer);

		[DllImport("__Internal")]
		protected static extern void GCloud_UploadFile(string filename);

		[DllImport("__Internal")]
		protected static extern void GCloud_DownloadFile(string url);

		[DllImport("__Internal")]
		protected static extern void GCloud_SetUploadListener(UploadCallBack listener);

		[DllImport("__Internal")]
		protected static extern void GCloud_SetDownloadListener(DownloadCallBack  listener);

		[DllImport("__Internal")]
		protected static extern void GCloud_DeleteAllGCloudFile();

		[DllImport("__Internal")]
		protected static extern bool GCloud_IsGCloudFileExist(string file);

		[DllImport("__Internal")]
		protected static extern bool GCloud_DeleteGCloudFile(string file);

		[DllImport("__Internal")]
		protected static extern bool GCloud_IsFileExist(string filePath);

		[DllImport("__Internal")]
		protected static extern bool GCloud_DeleteFile(string filePath);

		/** 发送二进制数据文件到服务器 */
		[DllImport("__Internal")]
		protected static extern IntPtr GCloud_UploadBinary(IntPtr buffer,int length, string extension);
		
		/** 发送文件到服务器 永久存储*/
		[DllImport("__Internal")]
		protected static extern void GCloud_UploadPersistFile(string fileDir);
		
		/** 发送二进制数据文件到服务器 永久存储*/
		[DllImport("__Internal")]
		protected static extern IntPtr GCloud_UploadPersistBinary(IntPtr buffer,int length, string extension);
		
		/** 取消上传 */
		[DllImport("__Internal")]
		protected static extern void GCloud_CancelFileUpload(string file);

		/** 设置服务器*/
		[DllImport("__Internal")]
		protected static extern void GCloud_SetHttpServer(string httpsrv);
 
		/** 暂停 可以恢复*/
		[DllImport("__Internal")]
		protected static extern void GCloud_PauseDownload(string url);
		
		/** 暂停后继续下载 */
		[DllImport("__Internal")]
		protected static extern void GCloud_ResumeDownload(string url);
		
		/** 取消 不可以恢复*/
		[DllImport("__Internal")]
		protected static extern void GCloud_CancelDownload(string url);

		[StructLayout(LayoutKind.Sequential)]
		protected class UploadCallBack
		{
			public IntPtr onUploadSuccessCB;
			public IntPtr onUploadFailedCB;
            public IntPtr onUploadProgressCB;
            public IntPtr onUploadStatusCB;
            public IntPtr onUploadCancelCB;

            delegate void onUploadSuccessDelegate(string file,string url);
			delegate void onUploadFailedDelegate(string file, int errorcode, string msg);
            delegate void onUploadProgressDelegate(string file, float percentindec);
            delegate void onUploadStatusDelegate(string file, int statuscode);
            delegate void onUploadCancelDelegate(string file);

            public UploadCallBack() 
			{
				onUploadSuccessDelegate sudel = new onUploadSuccessDelegate (onUploadSuccess);
				onUploadSuccessCB = Marshal.GetFunctionPointerForDelegate(sudel);

				onUploadFailedDelegate fadel = new onUploadFailedDelegate (onUploadFailed);
				onUploadFailedCB = Marshal.GetFunctionPointerForDelegate(fadel);

                onUploadProgressDelegate prodel = new onUploadProgressDelegate(onUploadProgress);
                onUploadProgressCB = Marshal.GetFunctionPointerForDelegate(prodel);

                onUploadStatusDelegate stadel = new onUploadStatusDelegate(onUploadStatus);
                onUploadStatusCB = Marshal.GetFunctionPointerForDelegate(stadel);

                onUploadCancelDelegate candel = new onUploadCancelDelegate(onUploadCancel);
                onUploadCancelCB = Marshal.GetFunctionPointerForDelegate(candel);
            }

            [MonoPInvokeCallback(typeof(onUploadSuccessDelegate))]
            static public void onUploadSuccess(string file, string url)
			{
				if(_uploadListener != null)
                {
                    TwoAction action = new TwoAction((object t, object t1) => { _uploadListener.onUploadSuccess((string)t, (string)t1); }, file, url);
                    Loom.DispatchToMainThread(action);
                }
			}

            [MonoPInvokeCallback(typeof(onUploadFailedDelegate))]
            static public void onUploadFailed(string file, int errorcode, string msg)
			{
                if (_uploadListener != null)
                {
                    ThreeAction action = new ThreeAction((object t, object t1, object t2) => { _uploadListener.onUploadFailed((string)t, (int)t1, (string)t2); }, file, errorcode, msg);
                    Loom.DispatchToMainThread(action);
                }
			}

            [MonoPInvokeCallback(typeof(onUploadProgressDelegate))]
            static public void onUploadProgress(string file, float percentindec)
            {
                if (_uploadListener != null)
                {
                    TwoAction action = new TwoAction((object t, object t1) => { _uploadListener.onUploadProgress((string)t, (float)t1); }, file, percentindec);
                    Loom.DispatchToMainThread(action);
                }
            }

            [MonoPInvokeCallback(typeof(onUploadStatusDelegate))]
            static public void onUploadStatus(string file, int statuscode)
            {
                if (_uploadListener != null)
                {
                    TwoAction action = new TwoAction((object t, object t1) => { _uploadListener.onUploadStatus((string)t, (int)t1); }, file, statuscode);
                    Loom.DispatchToMainThread(action);
                }
            }

            [MonoPInvokeCallback(typeof(onUploadCancelDelegate))]
            static public void onUploadCancel(string file)
            {
                if (_uploadListener != null)
                {
                    OneAction action = new OneAction((object t) => { _uploadListener.onUploadCancel((string)t); }, file);
                    Loom.DispatchToMainThread(action);
                }
            }
        }

		[StructLayout(LayoutKind.Sequential)]
		protected class DownloadCallBack
		{
			public IntPtr onDownloadSuccessCB;
			public IntPtr onDownloadFailedCB;
            public IntPtr onDownloadProgressCB;
            public IntPtr onDownloadStatusCB;
            public IntPtr onDownloadCancelCB;

            delegate void onDownloadSuccessDelegate(string url,string file);
			delegate void onDownloadFailedDelegate(string url, int errorcode, string msg);
            delegate void onDownloadProgressDelegate(string url, float percentindec);
            delegate void onDownloadStatusDelegate(string url, int statuscode);
            delegate void onDownloadCancelDelegate(string url);

            public DownloadCallBack()
			{
				onDownloadSuccessDelegate sudel = new onDownloadSuccessDelegate (onDownloadSuccess);
				onDownloadSuccessCB = Marshal.GetFunctionPointerForDelegate(sudel);

				onDownloadFailedDelegate fadel = new onDownloadFailedDelegate (onDownloadFailed);
				onDownloadFailedCB = Marshal.GetFunctionPointerForDelegate(fadel);

                onDownloadProgressDelegate prodel = new onDownloadProgressDelegate(onDownloadProgress);
                onDownloadProgressCB = Marshal.GetFunctionPointerForDelegate(prodel);

                onDownloadStatusDelegate stadel = new onDownloadStatusDelegate(onDownloadStatus);
                onDownloadStatusCB = Marshal.GetFunctionPointerForDelegate(stadel);

                onDownloadCancelDelegate candel = new onDownloadCancelDelegate(onDownloadCancel);
                onDownloadCancelCB = Marshal.GetFunctionPointerForDelegate(candel);
            }

			[MonoPInvokeCallback(typeof(onDownloadSuccessDelegate))]
			static public void onDownloadSuccess(string url,string file)
			{
                if (_downloadListener != null)
                {
                    TwoAction action = new TwoAction((object t, object t1) => { _downloadListener.onDownloadSuccess((string)t, (string)t1); }, url, file);
                    Loom.DispatchToMainThread(action);
                }
			}

			[MonoPInvokeCallback(typeof(onDownloadFailedDelegate))]
			static public void onDownloadFailed(string url, int errorcode, string msg)
			{
                if (_downloadListener != null)
                {
                    ThreeAction action = new ThreeAction((object t, object t1, object t2) => { _downloadListener.onDownloadFailed((string)t, (int)t1, (string)t2); }, url, errorcode, msg);
                    Loom.DispatchToMainThread(action);
                }
			}

            [MonoPInvokeCallback(typeof(onDownloadProgressDelegate))]
            static public void onDownloadProgress(string url, float percentindec)
            {
                if (_downloadListener != null)
                {
                    TwoAction action = new TwoAction((object t, object t1) => { _downloadListener.onDownloadProgress((string)t, (float)t1); }, url, percentindec);
                    Loom.DispatchToMainThread(action);
                }
            }

            [MonoPInvokeCallback(typeof(onDownloadStatusDelegate))]
            static public void onDownloadStatus(string url, int statuscode)
            {
                if (_downloadListener != null)
                {
                    TwoAction action = new TwoAction((object t, object t1) => { _downloadListener.onDownloadStatus((string)t, (int)t1); }, url, statuscode);
                    Loom.DispatchToMainThread(action);
                }
            }

            [MonoPInvokeCallback(typeof(onDownloadCancelDelegate))]
            static public void onDownloadCancel(string url)
            {
                if (_downloadListener != null)
                {
                    OneAction action = new OneAction((object t) => { _downloadListener.onDownloadCancel((string)t); }, url);
                    Loom.DispatchToMainThread(action);
                }
            }
        }

		public GCloudIOS()
		{
			GCloud_Constructer ();		
		}

		/**初始化SDK*/
		public virtual bool Initialize(int GID, int ZID, long UID, string clientName, string clientVer)
		{
		   return GCloud_Initialize(GID,ZID,UID,clientName,clientVer);
		}


        public virtual bool Destroy()
        {
           return GCloud_Destroy ();
        }

		/** Log 输出开关 (默认开启)YES:显示；NO:不显示 */
        public void SetLogDebug(bool flag)
        {
            GCloud_SetLogDebug(flag);
        }

       /** 切换服务器 */
        public void SwitchServer(int GID, int ZID, long UID, string appName, string appVer)
        {
			GCloud_SwitchServer(GID, ZID, UID, appName, appVer);
        }

        /// <summary>
		/// 判断GCloud下的文件是否存在
		/// file : 文件名、本地路径、URL
        /// </summary>
        public bool IsGCloudFileExist(string file)
        {
			return GCloud_IsGCloudFileExist(file);
        }

        /// <summary>
		/// 删除GCloud下对应的单个文件
		/// file : 文件名、本地路径、URL
        /// </summary>
        public bool DeleteGCloudFile(string file)
        {
			return GCloud_DeleteGCloudFile(file);
        }

        // <summary>
		/// 检测文件是否存在
        /// </summary>
        public bool IsFileExist(string file)
        {
			return GCloud_IsFileExist(file);
        }

        /// <summary>
		/// 删除文件
        /// </summary>
        public bool DeleteFile(string file)
        {
			return GCloud_DeleteFile(file);
        }

        /// <summary>
		/// 删除GCloud里面所有文件
        /// </summary>
        public void DeleteAllGCloudFile()
        {
			GCloud_DeleteAllGCloudFile ();
        }

		/** 上传回调 */
        public void SetUploaderListener(IUploaderListener listener)
        {
            _uploadListener = listener;
			GCloud_SetUploadListener(_uploadCallback);
        }

		/**发送录音文件到服务器*/
        public void UploadFile(string filename)
        {
            GCloud_UploadFile(filename);
        }

		/** 下载回调 */
        public void SetDownloaderListener(IDownloaderListener listener)
        {
            _downloadListener = listener;
			GCloud_SetDownloadListener(_downloadCallback);
        }

		/**根据声音voiceUrl下载录音文件*/
        public void DownloadFile(string url)
        {
            GCloud_DownloadFile(url);
        }

		/** 发送二进制数据文件到服务器 */
		public string UploadBinary(byte[] buffer, string extension)
		{
			IntPtr pbuffer = Marshal.AllocHGlobal(buffer.Length);
			Marshal.Copy(buffer, 0, pbuffer, buffer.Length);
			IntPtr pstr = GCloud_UploadBinary (pbuffer,buffer.Length, extension);
			string retstr = Marshal.PtrToStringAuto(pstr);
			Marshal.FreeHGlobal(pbuffer);
			return retstr;
		}
		
		/** 发送文件到服务器 永久存储*/
		public void UploadPersistFile(string fileDir)
		{
			GCloud_UploadPersistFile(fileDir);
		}
		
		/** 发送二进制数据文件到服务器 永久存储*/
		public string  UploadPersistBinary(byte[] buffer, string extension)
		{
			IntPtr pbuffer = Marshal.AllocHGlobal(buffer.Length);
			Marshal.Copy(buffer, 0, pbuffer, buffer.Length);
			IntPtr pstr = GCloud_UploadPersistBinary(pbuffer,buffer.Length,extension);
			string retstr = Marshal.PtrToStringAuto(pstr);
			Marshal.FreeHGlobal(pbuffer);
			return retstr;
		}
		
		/** 取消上传 */
		public void CancelFileUpload(string file)
		{
			GCloud_CancelFileUpload(file);
		}
		
		/** 设置服务器*/
		public void SetHttpServer(string httpsrv)
		{
			GCloud_SetHttpServer(httpsrv);
		}
		
		/** 暂停 可以恢复*/
		public void PauseDownload(string url)
		{
			GCloud_PauseDownload(url);
		}
		
		/** 暂停后继续下载 */
		public void ResumeDownload(string url)
		{
			GCloud_ResumeDownload(url);
		}
		
		/** 取消 不可以恢复*/
		public void CancelDownload(string url)
		{
			GCloud_CancelDownload(url);
		}





    }
}
#endif
