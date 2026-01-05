namespace TravisRFrench.Lifecycles.Runtime
{
	public static class HasManagedLifecycleExtensions
	{
		public static bool IsComposed(this IHasManagedLifeCycle instance)
		{
			return (instance.Phase >= LifecyclePhase.Composed);
		}

		public static bool IsVerified(this IHasManagedLifeCycle instance)
		{
			return (instance.Phase >= LifecyclePhase.Verified);
		}
		
		public static bool IsRegistered(this IHasManagedLifeCycle instance)
		{
			return instance.Phase is >= LifecyclePhase.Registered and < LifecyclePhase.Unregistered;
		}

		public static bool IsWired(this IHasManagedLifeCycle instance)
		{
			return instance.Phase is >= LifecyclePhase.Wired and < LifecyclePhase.Unwired;
		}

		public static bool IsSubscribed(this IHasManagedLifeCycle instance)
		{
			return instance.Phase is >= LifecyclePhase.Subscribed and < LifecyclePhase.Unsubscribed;
		}
		
		public static bool IsInitialized(this IHasManagedLifeCycle instance)
		{
			return instance.Phase is >= LifecyclePhase.Initialized and < LifecyclePhase.Disposed;
		}
		
		public static bool IsActivated(this IHasManagedLifeCycle instance)
		{
			return instance.Phase is >= LifecyclePhase.Activated and < LifecyclePhase.Deactivated;
		}
	}
}
