/*
 * Copyright (C) 2016年 Giant. All rights reserved.
 *
 * Loom.cs
 * 
 * Created by wangyanqing.
 */

namespace GCloudSDK
{
	using UnityEngine;  
	//using System.Collections;  
	using System.Collections.Generic;  
	using System;  
	using System.Threading;  
	//using System.Linq;  

	public interface IAction
	{
		void excAction();
	}

	public class VoidAction : IAction
	{
		Action action;
		
		public VoidAction(Action act)
		{
			action = act;
		}
		
		public void excAction()
		{
			action();
		}
	}

	public class OneAction : IAction
	{
		Action<object> action;
		object param;
		
		public OneAction(Action<object> act, object par)
		{
			action = act;
			param = par;
		}
		
		public void excAction()
		{
			action(param);
		}
	}

	public class TwoAction : IAction
	{
		Action<object, object> action;
		object[] param;
		
		public TwoAction(Action<object, object> act, params object[] pars)
		{
			action = act;
			param = pars;
		}
		
		public void excAction()
		{
			action(param[0], param[1]);
		}
	}

	public class ThreeAction : IAction
	{
		Action<object, object, object> action;
		object[] param;
		
		public ThreeAction(Action<object, object, object> act, params object[] pars)
		{
			action = act;
			param = pars;
		}
		
		public void excAction()
		{
			action(param[0], param[1], param[2]);
		}
	}
	  
	public class Loom
	{  
		static private Thread mainThread = null;
		//public int maxThreads = 8;  
		//private int numThreads;  
		  
		//static private Loom _current;  
		////private int _count;  
		//static public Loom Current  
		//{  
		//	get  
		//	{  
		//		//Initialize();  
		//		return _current;  
		//	}  
		//}  
		  
		static private bool initialized;  
		  
		static public void Initialize()  
		{  
			if (!initialized)  
			{
                //Debug.Log("Initialize Loom");
				if(!Application.isPlaying)  
					return;  
				initialized = true;  
				//var g = new GameObject("GiantVoice");  
				//_current = g.AddComponent<Loom>();  
				
				if (mainThread == null)
				{
					Thread currentThread = Thread.CurrentThread;
					if(currentThread.GetApartmentState() == ApartmentState.MTA || currentThread.ManagedThreadId > 1 || currentThread.IsThreadPoolThread)
					{
						Debug.Log("Trying to Init a WorkerThread as the MainThread! ");
					}
					else
					{
						mainThread = currentThread;
					}
				}
			}        
		}  
		
		static public bool CheckIfMainThread()
		{
			return Thread.CurrentThread == mainThread;
		}
		  
		static private List<IAction> _actions = new List<IAction>();  
		/* public struct DelayedQueueItem  
		{  
			public float time;  
			public Action action;  
		}  
		private List<DelayedQueueItem> _delayed = new  List<DelayedQueueItem>();  
	  
		List<DelayedQueueItem> _currentDelayed = new List<DelayedQueueItem>();  
		  
		public void QueueOnMainThread(Action action)  
		{  
			QueueOnMainThread( action, 0f);  
		}  
		public void QueueOnMainThread(Action action, float time)  
		{  
			if(time != 0)  
			{  
				lock(Current._delayed)  
				{  
					Current._delayed.Add(new DelayedQueueItem { time = Time.time + time, action = action});  
				}  
			}  
			else  
			{  
				lock (Current._actions)  
				{  
					Current._actions.Add(action);  
				}  
			}  
		}   */
		
		/* public static object DispatchToMainThreadReturn(ThreadDispatchDelegateArgReturn dispatchCall, object dispatchArgument, bool safeMode = true)
		{
			if (CheckIfMainThread())
			{
				if (dispatchCall != null)
					return dispatchCall(dispatchArgument);
			}
			else
			{
				ThreadDispatchAction action = new ThreadDispatchAction();
				lock (dispatchActions) { dispatchActions.Add(action); }
				action.Init(dispatchCall, dispatchArgument, safeMode); //Puts the Thread to sleep while waiting for the action to be invoked.
				return action.dispatchExecutionResult;
			}
			return null;
		} */
		
		public static void DispatchToMainThread(IAction action)
		{
			if (CheckIfMainThread())
			{
				if (action != null)
					action.excAction(); 
			}
			else
			{
				lock (_actions)  
				{  
					_actions.Add(action);  
				}  
			}
		}
		  
		/* public Thread RunAsync(Action a)  
		{  
			Initialize();  
			while(numThreads >= maxThreads)  
			{  
				Thread.Sleep(1);  
			}  
			Interlocked.Increment(ref numThreads);  
			ThreadPool.QueueUserWorkItem(RunAction, a);  
			return null;  
		}  
		  
		private void RunAction(object action)  
		{  
			try  
			{  
				((Action)action)();  
			}  
			catch  
			{  
			}  
			finally  
			{  
				Interlocked.Decrement(ref numThreads);  
			}  
				  
		}  
		  
		  
		void OnDisable()  
		{  
			if (_current == this)  
			{  
				_current = null;  
			}  
		}   */

		static List<IAction> _currentActions = new List<IAction>();  
		  
		// Update is called once per frame  
		public static void Update()  
		{  
			lock (_actions)  
			{  
                if (_actions.Count > 0)
                {
                    _currentActions.AddRange(_actions);
                    _actions.Clear();
                }
			}

            if (_currentActions.Count > 0)
            {
                foreach (var a in _currentActions)
                {
                    a.excAction();
                }
                _currentActions.Clear();
            }
		}  
	}  
}