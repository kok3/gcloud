package com.ztgame.audio.receiver;

import java.io.IOException;
import java.net.DatagramPacket;
import java.net.DatagramSocket;
import java.net.SocketException;
import java.nio.ByteBuffer;
import java.nio.ByteOrder;

import com.unity3d.player.UnityPlayer;
import com.ztgame.audio.net.P2PManager;

import android.util.Log;

public class AudioReceiver implements Runnable {
	String LOG = "AudioReceiver";
	DatagramSocket socket;
	DatagramPacket packet;
	boolean isRunning = false;

	private byte[] packetBuf = new byte[2048];
	private int packetSize = 2048;

	/*
	 * 开始接收数据
	 */
	public void startRecieving() {
		//if (socket == null) {
			//try {
				//socket = new DatagramSocket(port);
				packet = new DatagramPacket(packetBuf, packetSize);
			//} catch (SocketException e) {
			//}
		//}
		new Thread(this).start();
	}

	/*
	 * 停止接收数据
	 */
	public void stopRecieving() {
		isRunning = false;
	}

	/*
	 * 释放资源
	 */
	private void release() {
		if (packet != null) {
			packet = null;
		}
		if (socket != null) {
			socket.close();
			socket = null;
		}
	}

	public void run() {
		// 在接收前，要先启动解码器
		AudioDecoder decoder = AudioDecoder.getInstance();
		if(decoder!=null)
		{
			decoder.startDecoding();
		}
		

		isRunning = true;
		try {
			while (isRunning) {
				if(P2PManager.getInstance().socket!=null)
				{
					P2PManager.getInstance().socket.receive(packet);
					//Log.i(LOG, "收到一个包..." + packet.getLength());
					// 每接收一个UDP包，就交给解码器，等待解码
					if(packet.getLength()<2)
					{
						return;
					}
					
					byte[] packetData = packet.getData();
                    if (packetData != null && packetData.length > 0)
					{
                    	Byte prototol = 0;
                        ByteBuffer buffer = ByteBuffer.wrap(packetData);
                        if(buffer!=null)
                        {
                        	buffer.order(ByteOrder.LITTLE_ENDIAN);
                        	prototol = buffer.get();
                        }
                        
                        if(prototol == P2PManager.MicSamples2)
                        {
                            if(packet!=null&&packet.getLength()>1)
                            {
                                //buffer.getLong();
                                byte[] temp = new byte[packet.getLength()-1];
                                buffer.get(temp);
                                if(decoder!=null)
                                {
                                	decoder.addData(temp, packet.getLength()-1);
                                }
                                
                                //Log.d("P2PManager.recvMsg", "MicSamples: "+String.valueOf(packet.getLength()-1));
                            }
                        }else if(prototol == P2PManager.MESSAGE_VOICE_ANCHOR)
                        {
                        	if(!com.ztgame.audio.AudioManager.getInstance().isRecordSpeak&&packet!=null&&packet.getLength()>1)
                            {
                                //buffer.getLong();
                                byte[] temp = new byte[packet.getLength()-1];
                                buffer.get(temp);
                                if(decoder!=null)
                                {
                                	decoder.addData(temp, packet.getLength()-1);
                                }
                                
                                //Log.d("P2PManager.recvMsg", "MicSamples: "+String.valueOf(packet.getLength()-1));
                            }
                        }else{
                        	UnityPlayer.UnitySendMessage("Main Camera","MicReceive",String.valueOf(prototol));
                            Log.d("P2PManager", "recvMsg: " + String.valueOf(prototol));
                        }
					}
				}
			}

		} catch (IOException e) {
			Log.e(LOG, "RECIEVE ERROR!");
		}
		// 接收完成，停止解码器，释放资源
		if(decoder!=null)
		{
			decoder.stopDecoding();
		}
		release();
		Log.e(LOG, "stop recieving");
	}
}
