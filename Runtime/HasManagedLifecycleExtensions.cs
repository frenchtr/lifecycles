namespace TravisRFrench.Lifecycles.Runtime
{
	public static class HasManagedLifecycleExtensions
	{
		public static bool IsComposed(this IHasManagedLifeCycle instance)
		{
			return (instance.Phase >= LifecyclePhase.Composed);
		}

		public static bool IsRegistered(this IHasManagedLifeCycle instance)
		{
			return (instance.Phase >= LifecyclePhase.Registered &&
			        instance.Phase < LifecyclePhase.Unregistered);
		}
		
		public static bool IsInitialized(this IHasManagedLifeCycle instance)
		{
			return (instance.Phase >= LifecyclePhase.Initialized &&
			        instance.Phase < LifecyclePhase.Disposed);
		}
		
		public static bool IsBound(this IHasManagedLifeCycle instance)
		{
			return (instance.Phase >= LifecyclePhase.Bound &&
			        instance.Phase < LifecyclePhase.Unbound);
		}
		
		public static bool IsActive(this IHasManagedLifeCycle instance)
		{
			return (instance.Phase >= LifecyclePhase.Activated &&
			        instance.Phase < LifecyclePhase.Deactivated);
		}
	}
}
