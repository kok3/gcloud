package com.ztgame.audio;

import android.util.Log;

import com.ztgame.audio.receiver.AudioReceiver;
import com.ztgame.audio.sender.AudioRecorder;

import com.unity3d.player.UnityPlayer;

public class AudioManager {

	private AudioRecorder audioRecorder;
	private AudioReceiver audioReceiver;
	public long playerID;
	public boolean isPausePlay = false;
	public boolean isLiving = false;
	public boolean isRecordSpeak = false;
	public boolean isRegisterMic = false;
	public boolean isCheckMicOpen = false;
	public boolean isOpenAudioProcess = false;

	private static AudioManager instance;

	private AudioManager() {
	}

	public static AudioManager getInstance() {
		if (null == instance) {
			Log.d("@@@@@", "getInstance: AudioManager");
			instance = new AudioManager();
		}
		return instance;
	}

	public void startRecord() {
		Log.d("@@@@@", "startRecord");
		com.ztgame.audio.AudioManager.getInstance().isRegisterMic = true;

		if (null == audioRecorder) {
			audioRecorder = new AudioRecorder();
		}
		audioRecorder.startRecording();
	}

	public void stopRecord() {
		Log.d("@@@@@", "xxxxxxxxxxxxxxxyyyyyyyyyyyyyystopRecord");
		if (audioRecorder != null)
			audioRecorder.stopRecording();

		UnityPlayer.UnitySendMessage("Main Camera","messgae","@@@fro java message");
	}

	public void startListen() {
		Log.d("@@@@@", "startListen");
		com.ztgame.audio.AudioManager.getInstance().isPausePlay = false;
		if (null == audioReceiver) {
			audioReceiver = new AudioReceiver();
		}
		audioReceiver.startRecieving();
	}

	public void stopListen() {
		Log.d("@@@@@", "stopListen");
		com.ztgame.audio.AudioManager.getInstance().isPausePlay = true;
		if (audioReceiver != null)
			audioReceiver.stopRecieving();
	}
	
}
