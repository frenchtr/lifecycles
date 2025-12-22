using System;
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

		protected virtual void OnCompose()
		{
		}

		protected virtual void OnRegister()
		{
		}

		protected virtual void OnInitialize()
		{
		}

		protected virtual void OnBind()
		{
		}

		protected virtual void OnActivate()
		{
		}

		protected virtual void OnDeactivate()
		{
		}
		
		protected virtual void OnUnbind()
		{
		}

		protected virtual void OnUnregister()
		{
		}

		protected virtual void OnDispose()
		{
		}
		
		void IHasManagedLifeCycle.OnCompose()
		{
			this.OnCompose();
			this.Phase = LifecyclePhase.Composed;
		}

		void IHasManagedLifeCycle.OnRegister()
		{
			this.OnRegister();
			this.Phase = LifecyclePhase.Registered;
		}

		void IHasManagedLifeCycle.OnInitialize()
		{
			this.OnInitialize();
			this.Phase = LifecyclePhase.Initialized;
		}

		void IHasManagedLifeCycle.OnBind()
		{
			this.OnBind();
			this.Phase = LifecyclePhase.Bound;
		}

		void IHasManagedLifeCycle.OnActivate()
		{
			this.OnActivate();
			this.Phase = LifecyclePhase.Activated;
		}

		void IHasManagedLifeCycle.OnDeactivate()
		{
			OnDeactivate();
			Phase = LifecyclePhase.Deactivated;
		}

		void IHasManagedLifeCycle.OnUnbind()
		{
			this.OnUnbind();
			this.Phase = LifecyclePhase.Unbound;
		}

		void IHasManagedLifeCycle.OnUnregister()
		{
			this.OnUnregister();
			this.Phase = LifecyclePhase.Unregistered;
		}

		void IHasManagedLifeCycle.OnDispose()
		{
			this.OnDispose();
			this.Phase = LifecyclePhase.Disposed;
		}

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
