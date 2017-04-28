/*
 * Copyright (C) 2016年 Giant. All rights reserved.
 *
 * GCloudWindows.cs
 * 
 * Created by wangyanqing.
 */

using UnityEngine;
using System.Collections;

#if (UNITY_STANDALONE_WIN)
using AOT;
using System.Runtime.InteropServices;
using System;

namespace GCloudSDK
{
	public class GCloudWindows 
    {
        static protected IUploaderListener _uploadListener = null;
		static protected UploadCallBack _uploadCallback = new UploadCallBack();

		static protected IDownloaderListener _downloadListener = null;
		static protected DownloadCallBack _downloadCallback = new DownloadCallBack();

		[DllImport("GCloud")]
		protected static extern void GCloud_Constructer();

		[DllImport("GCloud")]
		protected static extern bool GCloud_Initialize(int GID, int ZID, long UID, string clientName, string clientVer);

		[DllImport("GCloud")]
		protected static extern bool GCloud_Destroy ();

		[DllImport("GCloud")]
		protected static extern void GCloud_SetLogDebug(bool flag);

		[DllImport("GCloud")]
		protected static extern void GCloud_SwitchServer(int GID, int ZID, long UID, string clientName, string clientVer);

		[DllImport("GCloud")]
		protected static extern bool GCloud_UploadFile(string filename);

		[DllImport("GCloud")]
		protected static extern bool GCloud_DownloadFile(string url);

		[DllImport("GCloud")]
		protected static extern void GCloud_SetUploadListener(UploadCallBack listener);

		[DllImport("GCloud")]
		protected static extern void GCloud_SetDownloadListener(DownloadCallBack  listener);

		[DllImport("GCloud")]
		protected static extern void GCloud_DeleteAllGCloudFile();

		[DllImport("GCloud")]
		protected static extern bool GCloud_IsGCloudFileExist(string file);

		[DllImport("GCloud")]
		protected static extern bool GCloud_DeleteGCloudFile(string file);

		[DllImport("GCloud")]
		protected static extern bool GCloud_IsFileExist(string filePath);

		[DllImport("GCloud")]
		protected static extern bool GCloud_DeleteFile(string filePath);

        [DllImport("GCloud")]
        protected static extern void GCloud_SetHttpServer(string httpsrv);

        [DllImport("GCloud")]
        protected static extern IntPtr GCloud_UploadBinary(IntPtr buffer, int len, string extension);

        [DllImport("GCloud")]
        protected static extern IntPtr GCloud_UploadPersistBinary(IntPtr buffer, int len, string extension);

        [DllImport("GCloud")]
        protected static extern void GCloud_UploadPersistFile(string url);

        [DllImport("GCloud")]
        protected static extern void GCloud_RemovePersistFile(string url);

        [DllImport("GCloud")]
        protected static extern void GCloud_CancelFileUpload(string file);

        [DllImport("GCloud")]
        protected static extern void GCloud_CancelFileDownload(string url);


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

		public GCloudWindows()
		{
			GCloud_Constructer();
		}

		/**初始化SDK*/
		public virtual bool Initialize(int GID, int ZID, long UID, string clientName, string clientVer)
		{
            return GCloud_Initialize(GID,ZID,UID,clientName,clientVer);
		}

        public virtual void Destroy()
        {
            GCloud_Destroy ();
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
        public bool UploadFile(string filename)
        {
            return GCloud_UploadFile(filename);
        }

		/** 下载回调 */
        public void SetDownloaderListener(IDownloaderListener listener)
        {
            _downloadListener = listener;
			GCloud_SetDownloadListener(_downloadCallback);
        }

		/**根据声音voiceUrl下载录音文件*/
        public bool DownloadFile(string url)
        {
            return GCloud_DownloadFile(url);
        }

        public void SetHttpServer(string httpsrv)
        {
            GCloud_SetHttpServer(httpsrv);
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
            IntPtr pbuffer = Marshal.AllocHGlobal(buffer.Length);
            Marshal.Copy(buffer, 0, pbuffer, buffer.Length);
            IntPtr pstr = GCloud_UploadBinary(pbuffer, buffer.Length, extension);
            string retstr = Marshal.PtrToStringAuto(pstr);
            Marshal.FreeHGlobal(pbuffer);
            return retstr;
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
            IntPtr pbuffer = Marshal.AllocHGlobal(buffer.Length);
            Marshal.Copy(buffer, 0, pbuffer, buffer.Length);
            IntPtr pstr = GCloud_UploadPersistBinary(pbuffer, buffer.Length, extension);
            string retstr = Marshal.PtrToStringAuto(pstr);
            Marshal.FreeHGlobal(pbuffer);
            return retstr;
        }

        /// <summary>
        /// 上传永久存储的文件
        /// </summary>
        /// <param name="file"></param>
        public void UploadPersistFile(string file)
        {
            GCloud_UploadPersistFile(file);
        }

        /// <summary>
        /// 根据url删除永久存储的文件
        /// </summary>
        /// <param name="url"></param>
        public void RemovePersistFile(string url)
        {
            GCloud_RemovePersistFile(url);
        }

        public void CancelFileUpload(string file)
        {
            GCloud_CancelFileUpload(file);
        }

        public void CancelFileDownload(string url)
        {
            GCloud_CancelFileDownload(url);
        }
    }
}
#endif

