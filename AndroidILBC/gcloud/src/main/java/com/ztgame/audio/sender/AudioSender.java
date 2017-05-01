package com.ztgame.audio.sender;

import java.io.IOException;
import java.net.DatagramPacket;
import java.net.DatagramSocket;
import java.net.InetAddress;
import java.net.SocketException;
import java.net.UnknownHostException;
import java.nio.ByteBuffer;
import java.util.Collections;
import java.util.LinkedList;
import java.util.List;

import com.ztgame.audio.AudioManager;
import com.ztgame.audio.data.AudioData;
import com.ztgame.audio.net.P2PManager;

import android.util.Log;

import com.ztgame.audio.NetConfig;

public class AudioSender implements Runnable {
	String LOG = "AudioSender ";

	private boolean isSendering = false;
	private List<AudioData> dataList;

	//DatagramSocket socket;
	DatagramPacket dataPacket;
	//private InetAddress ip;
	//private int port;

	public AudioSender() {
		dataList = Collections.synchronizedList(new LinkedList<AudioData>());
		/*
		try {
			try {
				ip = InetAddress.getByName(NetConfig.SERVER_HOST);
				Log.e(LOG, "服务端地址是 " + ip.toString());
				port = NetConfig.SERVER_PORT;
				socket = new DatagramSocket();
			} catch (UnknownHostException e) {
				e.printStackTrace();
			}
		} catch (SocketException e) {
			e.printStackTrace();
		}*/
		P2PManager.getInstance().InitNet(NetConfig.SERVER_HOST, NetConfig.SERVER_PORT);
		P2PManager.getInstance().InitSocket();
	}

	public void addData(byte[] data, int size) {
		if(data!=null&&data.length>=size)
		{
			AudioData encodedData = new AudioData();
			encodedData.setSize(size);
			byte[] tempData = new byte[size];
			System.arraycopy(data, 0, tempData, 0, size);
			encodedData.setRealData(tempData);
			dataList.add(encodedData);
		}
	}

	/*
	 * send data to server
	 */
	private void sendData(byte[] data, int size) {
		ByteBuffer buffer = ByteBuffer.allocate(data.length+1);
		if(AudioManager.getInstance().isLiving)
		{
			buffer.put(P2PManager.MESSAGE_VOICE_ANCHOR);
		}else{
			buffer.put(P2PManager.MicSamples2);
		}
		
		//buffer.putLong(AudioManager.getInstance().playerID);
		buffer.put(data);
		P2PManager.getInstance().SendMsg(buffer.array(), size+1);
		
		/*
		try {
			dataPacket = new DatagramPacket(data, size, ip, port);
			dataPacket.setData(data);
			Log.e(LOG, "发送一段数据 " + data.length);
			socket.send(dataPacket);
		} catch (IOException e) {
			e.printStackTrace();
		}*/
	}

	/*
	 * start sending data
	 */
	public void startSending() {
		new Thread(this).start();
	}

	/*
	 * stop sending data
	 */
	public void stopSending() {
		this.isSendering = false;
	}

	// run
	public void run() {
		this.isSendering = true;
		System.out.println(LOG + "start....");
		try{
			while (isSendering) {
				if (dataList.size() > 0) {
					AudioData encodedData = dataList.remove(0);
					sendData(encodedData.getRealData(), encodedData.getSize());
				}
			}
		}catch(Exception e)
		{
			
		}
		System.out.println(LOG + "stop!!!!");
	}
}