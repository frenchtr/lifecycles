// =========================
// File: ManagedMonoBehaviour.cs
// =========================

using System;
using UnityEngine;

namespace TravisRFrench.Lifecycles.Runtime
{
	/// <summary>
	/// Base class that routes Unity callbacks into lifecycle requests.
	/// Execution is performed by LifeCycleService.Tick().
	///
	/// NOTE: runtime debug state is NOT serialized; it is mirrored from the lifecycle runtime.
	/// </summary>
	public abstract class ManagedMonoBehaviour : MonoBehaviour, ILifeCycleManaged, ILifecycleDebugStateSink
	{
		private static ILifeCycleService LifeCycle => LifeCycleRuntime.Service;
		
		[NonSerialized] private LifecyclePhase phase = LifecyclePhase.None;
		[NonSerialized] private bool isActive;
		[NonSerialized] private bool isFaulted;
		[NonSerialized] private string faultSummary;

		public bool IsAlive { get; private set; } = true;

		public LifecyclePhase Phase => this.phase;
		public bool IsActive => this.isActive;
		public bool IsFaulted => this.isFaulted;
		public string FaultSummary => this.faultSummary;
		
		void ILifeCycleManaged.Compose() => this.OnLifecycleCompose();
		void ILifeCycleManaged.VerifyComposition() => this.OnLifecycleVerifyComposition();
		void ILifeCycleManaged.Register() => this.OnLifecycleRegister();
		void ILifeCycleManaged.Setup() => this.OnLifecycleSetup();

		void ILifeCycleManaged.InitializeEnable() => this.OnLifecycleInitializeEnable();

		void ILifeCycleManaged.Subscribe() => this.OnLifecycleSubscribe();

		void ILifeCycleManaged.Activate() => this.OnLifecycleActivate();

		void ILifeCycleManaged.FirstActivation() => this.OnLifecycleFirstActivation();

		void ILifeCycleManaged.Deactivate() => this.OnLifecycleDeactivate();

		void ILifeCycleManaged.Unsubscribe() => this.OnLifecycleUnsubscribe();

		void ILifeCycleManaged.FinalizeDisable() => this.OnLifecycleFinalizeDisable();

		void ILifeCycleManaged.Teardown() => this.OnLifecycleTeardown();

		void ILifeCycleManaged.Unregister() => this.OnLifecycleUnregister();

		void ILifeCycleManaged.Dispose() => this.OnLifecycleDispose();

		private void Awake()
		{
			LifeCycle.Manage(this);
			LifeCycle.RequestAwake(this);
		}

		private void OnEnable()
		{
			LifeCycle.RequestEnable(this);
		}

		private void OnDisable()
		{
			LifeCycle.RequestDisable(this);
		}

		private void OnDestroy()
		{
			this.IsAlive = false;
			LifeCycle.RequestDestroy(this);
		}

		void ILifecycleDebugStateSink.__SetPhase(LifecyclePhase p) => this.phase = p;
		void ILifecycleDebugStateSink.__SetIsActive(bool a) => this.isActive = a;
		void ILifecycleDebugStateSink.__SetIsFaulted(bool f) => this.isFaulted = f;
		void ILifecycleDebugStateSink.__SetFaultSummary(string s) => this.faultSummary = s;

		// Awake band
		protected virtual void OnLifecycleCompose() { }
		protected virtual void OnLifecycleVerifyComposition() { }
		protected virtual void OnLifecycleRegister() { }
		protected virtual void OnLifecycleSetup() { }
		
		// Enable band
		protected virtual void OnLifecycleInitializeEnable() { }
		protected virtual void OnLifecycleSubscribe() { }
		protected virtual void OnLifecycleActivate() { }
		protected virtual void OnLifecycleFirstActivation() { }
		
		// Disable band
		protected virtual void OnLifecycleDeactivate() { }
		protected virtual void OnLifecycleUnsubscribe() { }
		protected virtual void OnLifecycleFinalizeDisable() { }
		
		// Destroy band
		protected virtual void OnLifecycleTeardown() { }
		protected virtual void OnLifecycleUnregister() { }
		protected virtual void OnLifecycleDispose() { }
	}
}
