using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Net;
using System;
using AOT;

namespace GiantSDK.Voice
{
	
	public delegate void VoiceRecordCompleteHandler (int retCode, string url, string text, float duration);
	public delegate void PlayFinishCompleteHandler (int retCode);
	public delegate void JoinRoomCompleteHandler (int retCode);
	public delegate void QuitRoomCompleteHandler (int retCode);

	public enum GAVoiceCBType
	{
		GAVoice_RecordOver = 25,
		//录音完成
		GAVoice_PlayOver = 35,
		//播放完成
		GAVoice_JoinRoom = 7,
		//加入房间
		GAVoice_QuitRoom = 16,
		//退出房间

	}
	[Serializable]
	public class GAVoiceMessage
	{
		public string url;
		public string duration;
		public string text;
	}


	public class GiantVoiceManager// : GiantVoiceSingletonForMono<GiantVoiceManager>,IGiantVoice
	{
        public static GiantVoiceManager _instance = null;
        public static GiantVoiceManager Instance
        {
            get
            {
                if (null == _instance)
                {
                    _instance = new GiantVoiceManager();
                }

                return _instance;
            }
        }


		#if UNITY_STANDALONE_WIN || UNITY_EDITOR

		public const string LibName = "rtchatsdk";

		#elif UNITY_IPHONE
	
	    public const string LibName = "__Internal";
		
		#elif UNITY_ANDROID
	
	    public const string LibName = "rtchatsdk";

	    #endif
	 
	

		public delegate void SdkCallBackDelegate (int cmdType, int errorCode, string msgstr);

		public static event JoinRoomCompleteHandler OnJoinRoomComplete;
		public static event QuitRoomCompleteHandler OnQuitRoomComplete;
		public static event VoiceRecordCompleteHandler OnVoiceRecordComplete;
		public static event PlayFinishCompleteHandler OnPlayOverComplete;


	    public void TestInvokeJar()
        {
            Debug.LogError("TestInvokeJar");
        }

		public  void InitGASDK (string appId, string appKey, string  userName, string serverUrl, string xfid,string customip)
		{

			#if RTCHAT_ENABLE

			#if UNITY_ANDROID && !UNITY_EDITOR
			      
					RegisterAnd ();

			#endif

			
			#endif
		}

		public   void JoinRoom (string roomId)
		{
			#if RTCHAT_ENABLE
				
				//requestJoinPlatformRoom (roomId);
			
			#endif
		}

		public   void QuitRoom ()
		{
			#if RTCHAT_ENABLE
			//requestLeavePlatformRoom ();
			#endif

		}

		public   void OpenLoudSpeaker ()
		{
			#if RTCHAT_ENABLE

			//setLoudSpeaker (true);

			#endif	
		}

		public   void CloseLoudSpeaker ()
		{
			#if RTCHAT_ENABLE

			//setLoudSpeaker (false);

			#endif	
		}

		public   void SpeakerVolum (float volume)
		{
			#if RTCHAT_ENABLE
			//adjustSpeakerVolume (volume);

			#endif
		}
		
		public   void OpenMic ()
		{
			#if RTCHAT_ENABLE

			//setSendVoice (true);
			#endif
		}

		public   void CloseMic ()
		{
			#if RTCHAT_ENABLE
     	
			//setSendVoice (false);

			#endif
		}

		public   void StartRecordVoice (bool needConvertWord)
		{
            //#if RTCHAT_ENABLE
            //bool isstart =	startRecordVoice (needConvertWord);
            //Debug.Log (" c# 开始录制 ：" + isstart);
            //#endif
		}

		public   void StopRecordVoice ()
		{
            //#if RTCHAT_ENABLE
            //bool isstop =	stopRecordVoice ();
            //Debug.Log (" c# 停止录制 ：" + isstop);

            //#endif

		}

		public   void StartPlayVoice (string voiceUrl)
        {
        //    #if RTCHAT_ENABLE
        //    bool isstartPlayLocal = startPlayLocalVoice (voiceUrl);
        //    Debug.Log (" c# 开始播放本地 ：" + isstartPlayLocal);
        //    #endif

		}

		public   void StopPlayVoice ()
		{
            //#if RTCHAT_ENABLE

            //bool isstopPlay = stopPlayLocalVoice ();
            //Debug.Log (" c# 停止播放本地 ：" + isstopPlay);

            //#endif


		}
		//取消录音
		public   void CancelRecordVoice ()
		{
            //#if RTCHAT_ENABLE

            //bool iscancelVoice = cancelRecordedVoice ();
            //Debug.Log (" c# 取消录音 ：" + iscancelVoice);

            //#endif

		}
		//改变语音聊天登录用户信息
		public   void SetUserInfo (string username, string userkey)
		{
            //#if RTCHAT_ENABLE
            //int userinfo = setUserInfo (username, userkey);
            //Debug.Log (" c# setuserinfo ：" + userinfo);
            //#endif

		}

		public   void CustomRoomServer (string urlserver)
		{
            //#if RTCHAT_ENABLE

            //customRoomServerAddr (urlserver);

            //#endif
		}

		[MonoPInvokeCallback (typeof(SdkCallBackDelegate))]
		static void SdkCallBackFunc (int cmdType, int errorCode, string msgstr)
		{
			Debug.Log ("SdkCallBackFunc:msg= " + msgstr + ",errorcode = " + errorCode + ",cmdType = " + cmdType);

			#if RTCHAT_ENABLE

			switch (cmdType) {
			case (int)GAVoiceCBType.GAVoice_RecordOver:    //录音回调
				 
				if (msgstr == null || msgstr.Length <= 0) {
					if (OnVoiceRecordComplete != null) {
						OnVoiceRecordComplete (errorCode, null, null, 0);
					}

				} else {

					try {
						GAVoiceMessage gaMsg = JsonUtility.FromJson<GAVoiceMessage> (msgstr);	
						if (OnVoiceRecordComplete != null) {
							OnVoiceRecordComplete (errorCode, gaMsg.url, gaMsg.text, float.Parse (gaMsg.duration));
						}
						
					} catch {
						Debug.Log ("录音回调异常");
						if (OnVoiceRecordComplete != null) {
							OnVoiceRecordComplete (errorCode, null, null, 0);
						}	
						
					}

				}

				break;
			case (int)GAVoiceCBType.GAVoice_PlayOver:    //播放完成回调
				if (OnPlayOverComplete != null) {
					OnPlayOverComplete (errorCode);
				}
				break;
			case (int)GAVoiceCBType.GAVoice_JoinRoom://加入房间
				if (OnJoinRoomComplete != null) {
					OnJoinRoomComplete (errorCode);
				}
				break;
			case (int)GAVoiceCBType.GAVoice_QuitRoom://退出房间
				if (OnQuitRoomComplete != null) {
					OnQuitRoomComplete (errorCode);
				}
				
				break;

			}

			#endif

		
		}

		#if UNITY_ANDROID
	


		private AndroidJavaClass _androidManager = new AndroidJavaClass ("com.ztgame.voice.GARTCManager");

		public void RegisterAnd ()
		{
			if (_androidManager != null) {
				_androidManager.CallStatic ("register");	
			}
		}

		public void UnregisterAnd ()
		{
			if (_androidManager != null) {
					
				_androidManager.CallStatic ("unRegister");	
			}
		}

		#endif
	 
	 
	}


}
