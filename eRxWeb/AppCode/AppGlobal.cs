using System;
using System.Collections;
//namespace StaticAppGlobals
//{
namespace eRxWeb
{
	public class AppGlobal
	{
		private static volatile AppGlobal instance = null;
		private static object syncRoot = new object();
		// make the default constructor private, we never
		// explicitly create an instance with "new"
		private AppGlobal()
		{
		}
		// public property that gets the single instance of this class.
		public static AppGlobal Instance
		{
			get 
			{
				// only create a new instance if one doesn't already exist.
				if (instance == null)
				{
					// use lock to ensure that only one thread can access
					// this block of code at once. You can invoke lock on anything,
					// syncRoot is just a convenience object instance.
					lock(syncRoot)
					{
						if (instance == null)
							instance = new AppGlobal();
					}
				}
				return instance;
			}
		}
		private	static string mySampleProperty=String.Empty;
		public static string MySampleProperty 
		{
			get {return mySampleProperty;}
			set {mySampleProperty=value;}
		}

		public static Hashtable MySettings = new Hashtable();
	}
//}
}