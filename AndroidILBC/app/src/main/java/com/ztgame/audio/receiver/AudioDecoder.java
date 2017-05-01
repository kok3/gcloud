package com.ztgame.audio.receiver;

import java.util.Collections;
import java.util.LinkedList;
import java.util.List;

import xmu.swordbearer.audio.AudioCodec;

import com.ztgame.audio.data.AudioData;

import android.util.Log;

public class AudioDecoder implements Runnable {

	String LOG = "AudioDecoder";
	private static AudioDecoder decoder;

	private static final int MAX_BUFFER_SIZE = 2048;

	private boolean isDecoding = false;
	private List<AudioData> dataList = null;

	public static AudioDecoder getInstance() {
		if (decoder == null) {
			decoder = new AudioDecoder();
		}
		return decoder;
	}

	private AudioDecoder() {
		this.dataList = Collections
				.synchronizedList(new LinkedList<AudioData>());
	}

	/*
	 * add Data to be decoded
	 * 
	 * @ data:the data recieved from server
	 * 
	 * @ size:data size
	 */
	public void addData(byte[] data, int size) {
		if(data!=null&&data.length>=size)
		{
			AudioData adata = new AudioData();
			adata.setSize(size);
			byte[] tempData = new byte[size];
			System.arraycopy(data, 0, tempData, 0, size);
			adata.setRealData(tempData);
			dataList.add(adata);
		}
		// Log.e(LOG, "添加一次数据 " + dataList.size());

	}

	/*
	 * start decode AMR data
	 */
	public void startDecoding() {
		System.out.println(LOG + "开始解码");
		if (isDecoding) {
			return;
		}
		new Thread(this).start();
	}

	public void run() {
		// start player first
		AudioPlayer player = AudioPlayer.getInstance();
		if(player!=null)
		{
			player.startPlaying();
		}
		//
		this.isDecoding = true;
		// init ILBC parameter:30 ,20, 15
		AudioCodec.audio_codec_init(30);

		Log.d(LOG, LOG + "initialized decoder");
		int decodeSize = 0;
		try{
			while (isDecoding) {
				while (dataList.size() > 0) {
					AudioData encodedData = dataList.remove(0);
					if(encodedData!=null)
					{
						byte[] decodedData = new byte[MAX_BUFFER_SIZE];
						byte[] data = encodedData.getRealData();
		                if (data != null && data.length > 0&&data.length == 50)
						{
		                    decodeSize = AudioCodec.audio_decode(data, 0,
		                            50, decodedData, 0);
		                    Log.e(LOG, "解码一次 " + data.length + " 解码后的长度 " + decodeSize);
		                    if (decodeSize > 0) {
		                        // add decoded audio to player
		                        player.addData(decodedData, decodeSize);
		                    }
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
		
		System.out.println(LOG + "stop decoder");
		// stop playback audio
		if(player!=null)
		{
			player.stopPlaying();
		}
	}

	public void stopDecoding() {
		this.isDecoding = false;
	}
}