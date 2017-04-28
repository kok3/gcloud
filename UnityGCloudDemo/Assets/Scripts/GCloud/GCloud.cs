using UnityEngine;
using System.Collections;

namespace GCloudSDK
{
#if (!UNITY_ANDROID || UNITY_EDITOR)
    public class AndroidJavaProxy
    {
        public AndroidJavaProxy(string str) { }
    }
//
//	public class AndroidJavaObject
//	{
//		public AndroidJavaObject(string str) { }
//	}
//
//	public class AndroidJavaClass
//	{
//		public AndroidJavaClass(string str) { }
//	}
#endif

    public class GCloudLog
    {
		//protected static int LOG_WIDTH_LENGTH = 70;

		//protected static string sTag = "GCloud";
        static bool sIsDebug = true;

        public static bool isDebug()
        {
            return sIsDebug;
        }

        public static void setDebug(bool isDebug)
        {
            sIsDebug = isDebug;
        }

        public static void d(string msg)
        {
            if (sIsDebug)
            {
                Debug.Log("UnityGCloud:" + msg);
            }
        }

        public static void Log(string msg)
        {
            if (sIsDebug)
            {
                Debug.Log("UnityGCloud:" + msg);
            }
        }
    }

#if !UNITY_EDITOR
    #if UNITY_ANDROID
                public class GCloud : GCloudAndroid
    #elif (UNITY_IOS || UNITY_IPHONE)
	            public class GCloud : GCloudIOS
    #elif (UNITY_STANDALONE_WIN)
	            public class GCloud : GCloudWindows
    #else
	            public class GCloud : GCloudBlank
    #endif
#else
    #if UNITY_STANDALONE_WIN
    public class GCloud : GCloudWindows
    #else
    public class GCloud : GCloudBlank
    #endif
#endif
    {
        public void Update()
        {
            Loom.Update();
        }
    }
}
