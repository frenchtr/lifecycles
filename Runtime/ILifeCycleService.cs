namespace TravisRFrench.Lifecycles.Runtime
{
	public interface ILifeCycleService
	{
		void Manage(IHasManagedLifeCycle instance);
		void AwakenAll();
		void EnableAll();
		void DisableAll();
		void DestroyAll();
		void RequestAwaken(IHasManagedLifeCycle instance);
		void RequestEnable(IHasManagedLifeCycle instance);
		void RequestDisable(IHasManagedLifeCycle instance);
		void RequestDestroy(IHasManagedLifeCycle instance);
		void Tick();
	}
}
