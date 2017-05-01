package com.ztgame.audio.net;

import java.io.IOException;
import java.net.DatagramPacket;
import java.net.DatagramSocket;
import java.net.InetAddress;
import java.net.SocketException;
import java.net.UnknownHostException;

import com.ztgame.audio.AudioManager;
import com.ztgame.audio.receiver.AudioDecoder;

import android.util.Log;


public class P2PManager {
	
	 public static byte Null = 0;

	 public static byte RegisterMic = 1;
	 public static byte MicSamples = 2;
	 public static byte DegisterMic = 3;

	 public static byte MicSamples2 = 4;
	 
	 public static byte MESSAGE_VOICE_ANCHOR = 7;
	 public static byte MESSAGE_SHIELD_VOIVE = 8;
	 public static byte MESSAGE_BACK_SHIELD_VOIVE = 9;
	
	
	public DatagramSocket socket = null;
	public DatagramPacket dataPacket = null;
	private InetAddress ip;
	private int port = 0;
	//public int myPort = 11410;
	public int myPort = 5757;
	public boolean isAutoPort = false;
	
	private static P2PManager instance = null;
	
	public static P2PManager getInstance() {
		if (null == instance) {
			instance = new P2PManager();
		}
		return instance;
	}
	
	public P2PManager()
	{
		
	}
	
	public void InitNet(String ipStr,int portInt)
	{
		try {
			ip = InetAddress.getByName(ipStr);
			port = portInt;
			
		} catch (UnknownHostException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
	}
	
	public void InitSocket()
	{
		if(socket!=null)
		{
			socket.close();
			//socket = null;
		}

		for(int i=0;i<1;i++)
		//for(int i=0;i<100;i++)
		{
			try {
				//if(socket==null){
					if(isAutoPort)
					{
						socket = new DatagramSocket();
					}else{
						socket = new DatagramSocket(myPort);
					}
					
					++myPort;
				//}
				break;
				
			} catch (SocketException e) {
				Log.d("InitNet", "端口被占用: "+myPort);
				++myPort;
				// TODO Auto-generated catch block
				e.printStackTrace();
			}
		}
	}
	
	public void SendMsg(byte[] data,int size)
	{
		if(ip!=null)
		{
			dataPacket = new DatagramPacket(data, size, ip, port);
			dataPacket.setData(data);

			Log.d("SendMsg", "@@@addData");
            //xxx for test
            //AudioDecoder.getInstance().addData(data, size);
		}
		
		//Log.e("SendMsg", "发送一段数据 " + data.length);
		try {
			if(socket!=null&&dataPacket!=null)
			{
				socket.send(dataPacket);
			}
		} catch (IOException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
	}
	
	public void RecvMsg()
	{
		
	}
}
