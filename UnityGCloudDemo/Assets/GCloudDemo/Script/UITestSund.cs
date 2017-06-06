using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Runtime.InteropServices;

public class UITestSund : MonoBehaviour {

	const string SDK_DLL = "__Internal";

	#if UNITY_IOS && !UNITY_EDITOR

		[DllImport(SDK_DLL)]

		public static extern void cStartSpeaker();

		[DllImport(SDK_DLL)]

		public static extern void cStopSpeaker();

		[DllImport(SDK_DLL)]

		public static extern void cStartMic();

		[DllImport(SDK_DLL)]

		public static extern void cStopMic();

		[DllImport(SDK_DLL)]

		public static extern void cEnterRoom();

		[DllImport(SDK_DLL)]

		public static extern void cQuitRoom();
		 
	#endif

    private static AndroidJavaClass androidJavaClass = null;
    protected AndroidJavaObject gCloudJavaObject = null;
	public AndroidJavaClass channelClass = null;

    //public GameObject Text;s
    public Text tipText;


    public void setTip(string tip)
    {
        tipText.text = tip;
    }
    //连接服务器
    public void onConnectSrv()
    {
        //Debug.LogError("@@@onConnectSrv");


        setTip("onConnectSrv");

        if (gCloudJavaObject == null)
        {
            gCloudJavaObject = new AndroidJavaObject("com.ztgame.audio.AudioManager");
            //gCloudJavaObject = new AndroidJavaObject("com.ztgame.audio.AudioManager");
            //gCloudJavaObject.CallStatic<AndroidJavaObject>("getInstance");
            //gCloudJavaObject = androidJavaClass.CallStatic<AndroidJavaObject>("getInstance");
        }


        //GiantVoiceManager.Instance.TestInvokeJar();
    }

    public void TestJar()
    {

    }
    //进入房间
    public void onEnterRoom()
    {
        //Debug.LogError("@@@onEnterRoom");

        setTip("onEnterRoom");
	
		#if UNITY_ANDROID && !UNITY_EDITOR
        if (gCloudJavaObject != null)
            gCloudJavaObject.Call("stopRecord");
		#elif UNITY_IOS && !UNITY_EDITOR
			cEnterRoom();
		#endif

    }
    //退出房间
    public void onQuitRoom()
    {
        Debug.LogError("@@@onQuitRoom");

        setTip("onQuitRoom");

		#if UNITY_ANDROID && !UNITY_EDITOR

		#elif UNITY_IOS && !UNITY_EDITOR
			cQuitRoom();
		#endif

    }
    //开启麦克风
    public void onStartMic()
    {
        //Debug.Log("@@@onStartMic");
		#if UNITY_ANDROID && !UNITY_EDITOR
	        if (gCloudJavaObject != null)
	            gCloudJavaObject.Call("startRecord");
		#elif UNITY_IOS && !UNITY_EDITOR
			cStartMic();
		#endif
    }
    //关闭麦克风
    public void onStopMic()
    {
        //Debug.Log("@@@onStopMic");
		#if UNITY_ANDROID && !UNITY_EDITOR
	        if (gCloudJavaObject != null)
	        {
	            Debug.Log("@@@onStopMic");
	            gCloudJavaObject.Call("stopRecord");
	        }
		#elif UNITY_IOS && !UNITY_EDITOR
			cStopMic();
		#endif
    }
    //开启扬声器
    public void onStartSpeaker()
    {
        //Debug.Log("@@@onStartSpeaker");
		#if UNITY_ANDROID && !UNITY_EDITOR
	        if (gCloudJavaObject != null)
	            gCloudJavaObject.Call("startListen");
		#elif UNITY_IOS && !UNITY_EDITOR
			cStartSpeaker();
		#endif
    }
    //关闭扬声器
    public void onStopSpeaker()
    {
        //Debug.Log("@@@onStopSpeaker");
        //Debug.LogError("@@@onStartSpeaker");
		#if UNITY_ANDROID && !UNITY_EDITOR
	        if (gCloudJavaObject != null)
	            gCloudJavaObject.Call("stopListen");
		#elif UNITY_IOS && !UNITY_EDITOR
			cStopSpeaker();
		#endif
    }


    public void onQuit()
    {
		AndroidJavaClass context = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
		AndroidJavaObject obj = context.GetStatic<AndroidJavaObject>("currentActivity") ;
		if (context != null && obj != null) {
			tipText.text = channelClass.CallStatic<string> ("getChannel", obj);
			Debug.Log ("@@@" + tipText.text);
		} else {
			Debug.Log ("@@@ error");
			tipText.text = "Error";
		}
	}
        //Application.Quit();
   
	// Use this for initialization
	void Start () {
		Debug.Log ("@@@ Start");

		channelClass = new AndroidJavaClass ("com.czt.util.ChannelUtil");

		if (channelClass != null)
			Debug.Log ("@@@ channelClass OK!");
		else
			Debug.Log ("@@@ channelClass Wrong!");
	}

	
	// Update is called once per frame
	void Update () {
	
	}
}
