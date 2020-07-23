using UnityEngine;

namespace SPTarkov.Common.Utils.Hook
{
	public class Loader<T> where T : MonoBehaviour
	{
		public static GameObject HookObject
		{
			get
			{
				GameObject result = GameObject.Find("SPTarkov Instance");

				if (result == null)
				{
					result = new GameObject("SPTarkov Instance");
					Object.DontDestroyOnLoad(result);
				}

				return result;
			}
		}

		public static T Load()
		{
			return HookObject.GetOrAddComponent<T>();
		}
	}
}
