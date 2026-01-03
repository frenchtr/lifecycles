using UnityEngine;

namespace TravisRFrench.Lifecycles.Runtime
{
	public static class Bootstrapper
	{
		public static bool AutoProvideService { get; set; } = true;
		
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void BeforeSceneLoad()
		{
			if (Lifecycle.Service == null)
			{
				if (AutoProvideService)
				{
					var service = new LifeCycleService();
					Lifecycle.ProvideService(service);
					ManagedMonoBehaviour.ProvideService(service);
				}
			}

			var gameObject = new GameObject()
			{
				name = "Lifecycle Manager",
			};
			
			var manager = gameObject.AddComponent<LifeCycleManager>();
			Object.DontDestroyOnLoad(gameObject);
		}
	}
}
