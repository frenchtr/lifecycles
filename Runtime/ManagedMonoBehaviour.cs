using UnityEngine;

namespace TravisRFrench.Lifecycles.Runtime
{
	public class ManagedMonoBehaviour : MonoBehaviour, IHasManagedLifeCycle
	{
		private static ILifeCycleService lifeCycleService;

		internal static void ProvideService(ILifeCycleService lifeCycleService)
		{
			ManagedMonoBehaviour.lifeCycleService = lifeCycleService;
		}

		bool IHasManagedLifeCycle.IsAlive => this != null;
		bool IHasManagedLifeCycle.IsEnabled => this.enabled;

		public LifecyclePhase Phase { get; private set; }

		// -----------------
		// Overridable hooks
		// -----------------

		protected virtual void OnLifeCycleCompose() { }
		protected virtual void OnLifeCycleVerifyConfiguration() { }
		protected virtual void OnLifeCycleRegisterForDiscovery() { }
		protected virtual void OnLifeCycleWireEndpoints() { }
		protected virtual void OnLifeCycleSubscribeToExternalEvents() { }
		protected virtual void OnLifeCycleInitialize() { }
		protected virtual void OnLifeCycleActivate() { }

		protected virtual void OnLifeCycleDeactivate() { }
		protected virtual void OnLifeCycleUnsubscribeFromExternalEvents() { }
		protected virtual void OnLifeCycleUnwireEndpoints() { }
		protected virtual void OnLifeCycleUnregisterFromDiscovery() { }
		protected virtual void OnLifeCycleDispose() { }

		// -----------------
		// Explicit interface
		// -----------------

		void IHasManagedLifeCycle.OnLifeCycleCompose()
		{
			this.OnLifeCycleCompose();
			this.Phase = LifecyclePhase.Composed;
		}

		void IHasManagedLifeCycle.OnLifeCycleVerifyConfiguration()
		{
			this.OnLifeCycleVerifyConfiguration();
			this.Phase = LifecyclePhase.Verified;
		}

		void IHasManagedLifeCycle.OnLifeCycleRegisterForDiscovery()
		{
			this.OnLifeCycleRegisterForDiscovery();
			this.Phase = LifecyclePhase.Registered;
		}

		void IHasManagedLifeCycle.OnLifeCycleWireEndpoints()
		{
			this.OnLifeCycleWireEndpoints();
			this.Phase = LifecyclePhase.Wired;
		}

		void IHasManagedLifeCycle.OnLifeCycleSubscribeToExternalEvents()
		{
			this.OnLifeCycleSubscribeToExternalEvents();
			this.Phase = LifecyclePhase.Subscribed;
		}
		
		void IHasManagedLifeCycle.OnLifeCycleInitialize()
		{
			this.OnLifeCycleInitialize();
			this.Phase = LifecyclePhase.Initialized;
		}

		void IHasManagedLifeCycle.OnLifeCycleActivate()
		{
			this.OnLifeCycleActivate();
			this.Phase = LifecyclePhase.Activated;
		}

		void IHasManagedLifeCycle.OnLifeCycleDeactivate()
		{
			this.OnLifeCycleDeactivate();
			this.Phase = LifecyclePhase.Deactivated;
		}

		void IHasManagedLifeCycle.OnLifeCycleUnsubscribeFromExternalEvents()
		{
			this.OnLifeCycleUnsubscribeFromExternalEvents();
			this.Phase = LifecyclePhase.Unsubscribed;
		}

		void IHasManagedLifeCycle.OnLifeCycleUnwireEndpoints()
		{
			this.OnLifeCycleUnwireEndpoints();
			this.Phase = LifecyclePhase.Unwired;
		}

		void IHasManagedLifeCycle.OnLifeCycleUnregisterFromDiscovery()
		{
			this.OnLifeCycleUnregisterFromDiscovery();
			this.Phase = LifecyclePhase.Unregistered;
		}

		void IHasManagedLifeCycle.OnLifeCycleDispose()
		{
			this.OnLifeCycleDispose();
			this.Phase = LifecyclePhase.Disposed;
		}

		// -----------------
		// Unity forwarding
		// -----------------

		private void Awake()
		{
			lifeCycleService.Manage(this);
			lifeCycleService.RequestAwaken(this);
		}

		private void OnEnable()
		{
			lifeCycleService.RequestEnable(this);
		}

		private void OnDisable()
		{
			lifeCycleService.RequestDisable(this);
		}

		private void OnDestroy()
		{
			lifeCycleService.RequestDestroy(this);
		}
	}
}
