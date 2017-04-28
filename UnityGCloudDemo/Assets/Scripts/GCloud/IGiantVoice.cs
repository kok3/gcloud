using UnityEngine;
using System.Collections;

namespace GiantSDK.Voice
{
	public interface   IGiantVoice
	{
		
		//初始化sdk(appid ,appkey, 可以联系我们，userName,serverurl,xif)
		void InitGASDK (string appId, string appKey, string  userName, string serverUrl, string xfid,string customip);

		#region 实时语音

		//进入房间（roomid:房间id）
		void JoinRoom (string roomId);
		 
		//退出房间  
		void QuitRoom ();
	 
		//打开扬声器外放功能
		void OpenLoudSpeaker ();
		 
		//关闭扬声器外放功能
		void CloseLoudSpeaker ();
		 
		//设置语音外放音量调节（取值范围为0-10的整数，最大值请根据回音情况做调节）
		void SpeakerVolum (float volume);

		//打开输入方向麦克风（对方听到自己的声音）
		void OpenMic ();

		//关闭输入方向麦克风（对方听不到自己声音）
		void CloseMic ();

		#endregion

		#region IM留言消息

		//开始IM消息
		void StartRecordVoice (bool needConvertWord);

		//停止录制IM消息
		void StopRecordVoice ();

		//开始播放URL对应的IM消息
		void StartPlayVoice (string voiceUrl);

		//停止正在播放的IM消息
		void StopPlayVoice ();

		//取消录音
		void CancelRecordVoice ();

		//改变语音聊天登录用户信息
		void SetUserInfo (string username, string userkey);

		#endregion

		//自定义连接平台服务器URL
		void CustomRoomServer (string urlserver);
	}
}

