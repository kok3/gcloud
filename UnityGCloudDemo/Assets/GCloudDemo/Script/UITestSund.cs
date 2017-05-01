using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UITestSund : MonoBehaviour {

    //public GameObject Text;
    public Text tipText;


    public void setTip(string tip)
    {
        tipText.text = tip;
    }
    //连接服务器
    public void onConnectSrv()
    {
        Debug.LogError("@@@onConnectSrv");

        setTip("onConnectSrv");

    }
    //进入房间
    public void onEnterRoom()
    {
        Debug.LogError("@@@onEnterRoom");

        setTip("onEnterRoom");

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
        Debug.LogError("@@@onStartMic");
    }
    //关闭麦克风
    public void onStopMic()
    {
        Debug.LogError("@@@onStopMic");
    }
    //开启扬声器
    public void onStartSpeaker()
    {
        Debug.LogError("@@@onStartSpeaker");
    }
    //开启扬声器
    public void onStopSpeaker()
    {
        Debug.LogError("@@@onStopSpeaker");
    }
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
