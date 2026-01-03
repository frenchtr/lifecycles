namespace TravisRFrench.Lifecycles.Runtime
{
	public static class Lifecycle
	{
		public static ILifeCycleService Service { get; private set; }
		
		public static void ProvideService(ILifeCycleService lifeCycleService)
		{
			Service = lifeCycleService;
			ManagedMonoBehaviour.ProvideService(lifeCycleService);
		}
	}
}
