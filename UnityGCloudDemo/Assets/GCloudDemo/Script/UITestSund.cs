using UnityEngine;
using System.Collections;
using UnityEngine.UI;

using GiantSDK.Voice;
public class UITestSund : MonoBehaviour {

    private static AndroidJavaClass androidJavaClass = null;
    protected AndroidJavaObject gCloudJavaObject = null;

    //public GameObject Text;
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

        if (gCloudJavaObject != null)
            gCloudJavaObject.Call("stopRecord");

    }
    //退出房间
    public void onQuitRoom()
    {
        Debug.LogError("@@@onQuitRoom");

        setTip("onQuitRoom");
    }
    //开启麦克风
    public void onStartMic()
    {
        //Debug.Log("@@@onStartMic");
        if (gCloudJavaObject != null)
            gCloudJavaObject.Call("startRecord");
    }
    //关闭麦克风
    public void onStopMic()
    {
        //Debug.Log("@@@onStopMic");
        if (gCloudJavaObject != null)
        {
            Debug.Log("@@@onStopMic");
            gCloudJavaObject.Call("stopRecord");
        }
    }
    //开启扬声器
    public void onStartSpeaker()
    {
        //Debug.Log("@@@onStartSpeaker");
        if (gCloudJavaObject != null)
            gCloudJavaObject.Call("startListen");
    }
    //关闭扬声器
    public void onStopSpeaker()
    {
        //Debug.Log("@@@onStopSpeaker");
        //Debug.LogError("@@@onStartSpeaker");
        if (gCloudJavaObject != null)
            gCloudJavaObject.Call("stopListen");
    }


    public void onQuit()
    {
        Application.Quit();
    }
	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
