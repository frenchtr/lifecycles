namespace TravisRFrench.Lifecycles.Runtime
{
	public interface IHasManagedLifeCycle
	{
		bool IsAlive { get; }
		bool IsEnabled { get; }
		
		LifecyclePhase Phase { get; }
		
		void OnCompose();
		void OnRegister();
		void OnInitialize();
		void OnBind();
		void OnActivate();
		void OnDeactivate();
		void OnUnbind();
		void OnUnregister();
		void OnDispose();
	}
}
