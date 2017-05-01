package com.ztgame.audio.receiver;

import java.io.File;
import java.io.FileNotFoundException;
import java.io.FileOutputStream;
import java.io.IOException;
import java.util.Collections;
import java.util.LinkedList;
import java.util.List;

import com.unity3d.player.UnityPlayer;
import com.ztgame.audio.AudioConfig;
import com.ztgame.audio.data.AudioData;

import android.media.AudioFormat;
import android.media.AudioManager;
import android.media.AudioRecord;
import android.media.AudioTrack;
import android.util.Log;

public class AudioPlayer implements Runnable {
	String LOG = "AudioPlayer ";
	private static AudioPlayer player;

	private List<AudioData> dataList = null;
	private AudioData playData;
	private boolean isPlaying = false;

	private AudioTrack audioTrack;

	//
	private File file;
	private FileOutputStream fos;

	private AudioPlayer() {
		dataList = Collections.synchronizedList(new LinkedList<AudioData>());

//		file = new File("/sdcard/audio/decode.amr");
//		try {
//			if (!file.exists())
//				try {
//					file.createNewFile();
//				} catch (IOException e) {
//					e.printStackTrace();
//				}
//			fos = new FileOutputStream(file);
//		} catch (FileNotFoundException e) {
//			e.printStackTrace();
//		}
	}

	public static AudioPlayer getInstance() {
		if (player == null) {
			player = new AudioPlayer();
		}
		return player;
	}

	public void addData(byte[] rawData, int size) {
		if(rawData!=null&&rawData.length>=size)
		{
			AudioData decodedData = new AudioData();
			decodedData.setSize(size);
			byte[] tempData = new byte[size];
			System.arraycopy(rawData, 0, tempData, 0, size);
			decodedData.setRealData(tempData);
			dataList.add(decodedData);
			//Log.e(LOG, "Player添加一次数据 " + dataList.size());
		}
	}

	/*
	 * init Player parameters
	 */
	private boolean initAudioTrack() {
		int bufferSize = AudioTrack.getMinBufferSize(AudioConfig.SAMPLERATE,
				AudioFormat.CHANNEL_OUT_MONO,
				AudioConfig.AUDIO_FORMAT);
		if (bufferSize < 0) {
			Log.e(LOG, LOG + "initialize error!");
			return false;
		}
		Log.i(LOG, "Player初始化的 buffersize是 " + bufferSize);
		try
		{
		    audioTrack = new AudioTrack(AudioManager.STREAM_MUSIC,
	                AudioConfig.SAMPLERATE, AudioFormat.CHANNEL_OUT_MONO,
	                AudioConfig.AUDIO_FORMAT, bufferSize, AudioTrack.MODE_STREAM);
		}
		catch(Exception e)
		{
		    Log.e(LOG, LOG + "AudioTrack init error:"+e);
		}
	
		try{
			if(audioTrack!=null&&audioTrack.getState() == AudioTrack.STATE_INITIALIZED)
			{
				//audioTrack.setStereoVolume(1.0f, 1.0f);
				audioTrack.play();
			}
		}catch(Exception e)
		{
			e.printStackTrace();
		}
		
		return true;
	}

	private void playFromList() throws IOException {
		try{
			while (isPlaying) {
				Log.e(LOG, "播playFromList--isPlaying 111");
				while (dataList.size() > 0) {
					playData = dataList.remove(0);
					if(playData!=null)
					{	Log.e(LOG, "播放一次数据 222: " + playData.getSize());
						if(!com.ztgame.audio.AudioManager.getInstance().isPausePlay&&com.ztgame.audio.AudioManager.getInstance().isRegisterMic==true)
						{
							Log.e(LOG, "播放一次数据 333" + dataList.size());
							if(audioTrack!=null)
							{
								audioTrack.write(playData.getRealData(), 0, playData.getSize());
							}
						}
						
						// fos.write(playData.getRealData(), 0, playData.getSize());
						// fos.flush();
						try{
							//UnityPlayer.UnitySendMessage("Main Camera","PlaySamples","play");
						}catch(Exception e)
						{
							e.printStackTrace();
						}
					}
					
				}
				try {
					Thread.sleep(20);
				} catch (InterruptedException e) {
				}
			}
		}catch(Exception e)
		{
			
		}
	}

	public void startPlaying() {
		if (isPlaying) {
			Log.e(LOG, "验证播放器是否打开" + isPlaying);
			return;
		}
		new Thread(this).start();
	}

	public void run() {
		this.isPlaying = true;
		if (!initAudioTrack()) {
			Log.i(LOG, "播放器初始化失败");
			return;
		}
		Log.e(LOG, "开始播放");
		try {
			playFromList();
		} catch (IOException e) {
			e.printStackTrace();
		}
		// while (isPlaying) {
		// if (dataList.size() > 0) {
		// playFromList();
		// } else {
		//
		// }
		// }
		
		try{
			if (this.audioTrack != null) {
				if (this.audioTrack.getPlayState() == AudioTrack.PLAYSTATE_PLAYING) {
					this.audioTrack.stop();
					this.audioTrack.release();
				}
			}
		}catch(Exception e)
		{
			e.printStackTrace();
		}
		
		
		Log.d(LOG, LOG + "end playing");
	}

	public void stopPlaying() {
		this.isPlaying = false;
	}
}
