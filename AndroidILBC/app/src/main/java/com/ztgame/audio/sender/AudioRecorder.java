package com.ztgame.audio.sender;
//*****
//import com.media.audio.module.AudioProcessor;
import com.unity3d.player.UnityPlayer;
import com.ztgame.audio.AudioConfig;
import com.ztgame.audio.AudioManager;

import android.media.AudioRecord;
import android.util.Log;

public class AudioRecorder implements Runnable {

	String LOG = "Recorder ";

	private boolean isRecording = false;
	private AudioRecord audioRecord;

	private static final int BUFFER_FRAME_SIZE = 480;
	private int audioBufSize = 0;

	//
	private byte[] samples;// data
	// the size of audio read from recorder
	private int bufferRead = 0;
	// samples size
	private int bufferSize = 0;

	/*
	 * start recording
	 */
	public void startRecording() {
		bufferSize = BUFFER_FRAME_SIZE;

		audioBufSize = AudioRecord.getMinBufferSize(AudioConfig.SAMPLERATE,
				AudioConfig.RECORDER_CHANNEL_CONFIG, AudioConfig.AUDIO_FORMAT);
		if (audioBufSize == AudioRecord.ERROR_BAD_VALUE) {
			Log.e(LOG, "audioBufSize error");
			return;
		}
		samples = new byte[audioBufSize];
		try
		{
		      // 初始化recorder
	        if (null == audioRecord) {
	            audioRecord = new AudioRecord(AudioConfig.AUDIO_RESOURCE,
	                    AudioConfig.SAMPLERATE,
	                    AudioConfig.RECORDER_CHANNEL_CONFIG,
	                    AudioConfig.AUDIO_FORMAT, audioBufSize);
	        }
        }
        catch (Exception e)
        {
            Log.e(LOG, "AudioRecord init error :"+e);
        }

		new Thread(this).start();
	}

	/*
	 * stop
	 */
	public void stopRecording() {
		this.isRecording = false;
	}

	public boolean isRecording() {
		return isRecording;
	}
	
	private boolean isCheckOpenMic = false;
	private void CheckOpenMicTip()
	{
		if(!AudioManager.getInstance().isCheckMicOpen)return;
		if(isCheckOpenMic)
		{
			return;
		}
		isCheckOpenMic = true;
		try{
			UnityPlayer.UnitySendMessage("Main Camera","CheckOpenMicTip","");
		}catch(Exception e)
		{
			e.printStackTrace();
		}
	}

	public void run() {
		// start encoder before recording
		isCheckOpenMic = false;
		AudioEncoder encoder = AudioEncoder.getInstance();
		encoder.startEncoding();
		System.out.println(LOG + "audioRecord startRecording()");
		try
		{
            if (audioRecord != null && audioRecord.getState() == AudioRecord.STATE_INITIALIZED)
	        {
	            audioRecord.startRecording();
	        }
		}
		catch(Exception e)
		{
			CheckOpenMicTip();
		    System.out.println(LOG + "start record error!!!");
		    return;
		}
		
		System.out.println(LOG + "start recording");
		try{
			this.isRecording = true;
			while (isRecording) {
				if(audioRecord!=null)
				{
					bufferRead = audioRecord.read(samples, 0, bufferSize);
				}
				
				if(bufferRead<=0)
				{
					CheckOpenMicTip();
				}
				
				if (bufferRead > 0) {
					if(AudioManager.getInstance().isOpenAudioProcess)
					{
						System.out.println(LOG + "111111111111111111111111");
//						byte[] audioData = AudioProcessor.getInstance().processAudio(samples, bufferRead / 2);
//						if(audioData != null)
//						{
//							// add data to encoder
//							encoder.addData(audioData, bufferRead);
//						}
						encoder.addData(samples, bufferRead);
					}else{
						System.out.println(LOG + "22222222222222222222222222");
						encoder.addData(samples, bufferRead);
					}
					// add data to encoder
					
				}
				/*
				if(samples!=null&&samples.length>0)
				{
					try{
						UnityPlayer.UnitySendMessage("Main Camera","RecordSamples",String.valueOf(GetVolume(samples)));
						Log.d("bufferRead", String.valueOf(GetVolume(samples)));
					}catch(Exception e)
					{
						e.printStackTrace();
					}
				}*/
				
				try {
					Thread.sleep(10);
				} catch (InterruptedException e) {
					e.printStackTrace();
				}
			}
		}catch(Exception e)
		{
			
		}
		
		System.out.println(LOG + "end recording");
		
		try{
			if (audioRecord != null && audioRecord.getState() == AudioRecord.STATE_INITIALIZED)
	        {
				audioRecord.stop();
	        }
		}catch(Exception e)
		{
			System.out.println(LOG + "stop record error!!!");
		}
		
		if(encoder!=null)
		{
			encoder.stopEncoding();
		}
	}
	
	
	public float GetVolume(byte[] samples)
    {
        float s = 0;
        int a = 0;
        for (a = 0; a < 30 && a < samples.length; a++)
        {
            if (s < Math.abs(samples[a]))
                s = Math.abs(samples[a]);
        }
        return s;
    }
}
