using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TravisRFrench.Lifecycles.Runtime
{
	public class LifeCycleService : ILifeCycleService
	{
		private readonly List<IHasManagedLifeCycle> managedInstances;
		private readonly List<IHasManagedLifeCycle> awakenQueue;
		private readonly List<IHasManagedLifeCycle> enableQueue;
		private readonly List<IHasManagedLifeCycle> disableQueue;
		private readonly List<IHasManagedLifeCycle> destroyQueue;

		public LifeCycleService(IEnumerable<IHasManagedLifeCycle> managedInstances = null)
		{
			this.managedInstances = managedInstances?
				.Where(i => i != null)
				.Distinct()
				.ToList()
				?? new List<IHasManagedLifeCycle>();

			this.awakenQueue = new List<IHasManagedLifeCycle>();
			this.enableQueue = new List<IHasManagedLifeCycle>();
			this.disableQueue = new List<IHasManagedLifeCycle>();
			this.destroyQueue = new List<IHasManagedLifeCycle>();
		}

		public void Manage(IHasManagedLifeCycle instance)
		{
			if (instance == null)
			{
				Debug.LogError(LifecycleLogMessages.FormatNullArgument(nameof(Manage)));
				return;
			}

			if (this.managedInstances.Contains(instance))
			{
				return;
			}

			this.managedInstances.Add(instance);
		}

		public void Unmanage(IHasManagedLifeCycle instance)
		{
			if (instance == null)
			{
				return;
			}

			if (!this.managedInstances.Contains(instance))
			{
				return;
			}

			this.managedInstances.Remove(instance);

			this.awakenQueue.Remove(instance);
			this.enableQueue.Remove(instance);
			this.disableQueue.Remove(instance);
			this.destroyQueue.Remove(instance);
		}

		public void AwakenAll()
		{
			foreach (var instance in this.managedInstances.ToList())
			{
				this.RequestAwaken(instance);
			}
		}

		public void EnableAll()
		{
			foreach (var instance in this.managedInstances.ToList())
			{
				this.RequestEnable(instance);
			}
		}

		public void DisableAll()
		{
			foreach (var instance in this.managedInstances.ToList())
			{
				this.RequestDisable(instance);
			}
		}

		public void DestroyAll()
		{
			foreach (var instance in this.managedInstances.ToList())
			{
				this.RequestDestroy(instance);
			}
		}

		public void RequestAwaken(IHasManagedLifeCycle instance)
		{
			if (instance == null)
			{
				Debug.LogError(LifecycleLogMessages.FormatNullArgument(nameof(RequestAwaken)));
				return;
			}

			if (this.awakenQueue.Contains(instance))
			{
				return;
			}

			// If it was previously queued for destruction, revive the request.
			this.destroyQueue.Remove(instance);

			this.awakenQueue.Add(instance);
		}

		public void RequestEnable(IHasManagedLifeCycle instance)
		{
			if (instance == null)
			{
				Debug.LogError(LifecycleLogMessages.FormatNullArgument(nameof(RequestEnable)));
				return;
			}

			if (this.enableQueue.Contains(instance))
			{
				return;
			}

			this.disableQueue.Remove(instance);
			this.enableQueue.Add(instance);
		}

		public void RequestDisable(IHasManagedLifeCycle instance)
		{
			if (instance == null)
			{
				Debug.LogError(LifecycleLogMessages.FormatNullArgument(nameof(RequestDisable)));
				return;
			}

			if (this.disableQueue.Contains(instance))
			{
				return;
			}

			this.enableQueue.Remove(instance);
			this.disableQueue.Add(instance);
		}

		public void RequestDestroy(IHasManagedLifeCycle instance)
		{
			if (instance == null)
			{
				Debug.LogError(LifecycleLogMessages.FormatNullArgument(nameof(RequestDestroy)));
				return;
			}

			// Remove from other queues first.
			this.awakenQueue.Remove(instance);
			this.enableQueue.Remove(instance);
			this.disableQueue.Remove(instance);

			if (!this.destroyQueue.Contains(instance))
			{
				this.destroyQueue.Add(instance);
			}

			this.DestroyImmediate(instance);
		}

		public void Tick()
		{
			try { this.AwakenEnqueued(); }
			finally { this.awakenQueue.Clear(); }

			try { this.EnableEnqueued(); }
			finally { this.enableQueue.Clear(); }

			try { this.DisableEnqueued(); }
			finally { this.disableQueue.Clear(); }

			try { this.DestroyEnqueued(); }
			finally { this.destroyQueue.Clear(); }
		}

		private void AwakenEnqueued()
		{
			var instances = this.awakenQueue.Where(i => i != null).ToList();

			// Compose
			foreach (var instance in instances)
			{
				if (!instance.IsAlive) continue;
				LifecycleInvoker.SafeInvoke(instance, LifecyclePhase.Composed, nameof(IHasManagedLifeCycle.OnLifeCycleCompose), instance.OnLifeCycleCompose);
			}

			// Verify
			foreach (var instance in instances)
			{
				if (!instance.IsAlive) continue;
				LifecycleInvoker.SafeInvoke(instance, LifecyclePhase.Verified, nameof(IHasManagedLifeCycle.OnLifeCycleVerifyConfiguration), instance.OnLifeCycleVerifyConfiguration);
			}

			// Register for discovery
			foreach (var instance in instances)
			{
				if (!instance.IsAlive) continue;
				LifecycleInvoker.SafeInvoke(instance, LifecyclePhase.Registered, nameof(IHasManagedLifeCycle.OnLifeCycleRegisterForDiscovery), instance.OnLifeCycleRegisterForDiscovery);
			}

			// Wire endpoints (initial binding)
			foreach (var instance in instances)
			{
				if (!instance.IsAlive) continue;
				LifecycleInvoker.SafeInvoke(instance, LifecyclePhase.Wired, nameof(IHasManagedLifeCycle.OnLifeCycleWireEndpoints), instance.OnLifeCycleWireEndpoints);
			}

			// Initialize
			foreach (var instance in instances)
			{
				if (!instance.IsAlive) continue;
				LifecycleInvoker.SafeInvoke(instance, LifecyclePhase.Initialized, nameof(IHasManagedLifeCycle.OnLifeCycleInitialize), instance.OnLifeCycleInitialize);
			}
		}

		private void EnableEnqueued()
		{
			var instances = this.enableQueue.Where(i => i != null).ToList();

			// Subscribe to external events
			foreach (var instance in instances)
			{
				if (!instance.IsAlive || !instance.IsEnabled) continue;
				LifecycleInvoker.SafeInvoke(instance, LifecyclePhase.Subscribed, nameof(IHasManagedLifeCycle.OnLifeCycleSubscribeToExternalEvents), instance.OnLifeCycleSubscribeToExternalEvents);
			}

			// Activate
			foreach (var instance in instances)
			{
				if (!instance.IsAlive || !instance.IsEnabled) continue;
				LifecycleInvoker.SafeInvoke(instance, LifecyclePhase.Activated, nameof(IHasManagedLifeCycle.OnLifeCycleActivate), instance.OnLifeCycleActivate);
			}
		}

		private void DisableEnqueued()
		{
			var instances = this.disableQueue.Where(i => i != null).ToList();

			// Deactivate
			foreach (var instance in instances)
			{
				if (!instance.IsAlive) continue;
				LifecycleInvoker.SafeInvoke(instance, LifecyclePhase.Deactivated, nameof(IHasManagedLifeCycle.OnLifeCycleDeactivate), instance.OnLifeCycleDeactivate);
			}

			// Unsubscribe
			foreach (var instance in instances)
			{
				if (!instance.IsAlive) continue;
				LifecycleInvoker.SafeInvoke(instance, LifecyclePhase.Unsubscribed, nameof(IHasManagedLifeCycle.OnLifeCycleUnsubscribeFromExternalEvents), instance.OnLifeCycleUnsubscribeFromExternalEvents);
			}

			// Unwire
			foreach (var instance in instances)
			{
				if (!instance.IsAlive) continue;
				LifecycleInvoker.SafeInvoke(instance, LifecyclePhase.Unwired, nameof(IHasManagedLifeCycle.OnLifeCycleUnwireEndpoints), instance.OnLifeCycleUnwireEndpoints);
			}
		}

		private void DestroyEnqueued()
		{
			var instances = this.destroyQueue.Where(i => i != null).ToList();

			// Unregister
			foreach (var instance in instances)
			{
				if (!instance.IsAlive) continue;
				LifecycleInvoker.SafeInvoke(instance, LifecyclePhase.Unregistered, nameof(IHasManagedLifeCycle.OnLifeCycleUnregisterFromDiscovery), instance.OnLifeCycleUnregisterFromDiscovery);
			}

			// Dispose
			foreach (var instance in instances)
			{
				if (!instance.IsAlive) continue;
				LifecycleInvoker.SafeInvoke(instance, LifecyclePhase.Disposed, nameof(IHasManagedLifeCycle.OnLifeCycleDispose), instance.OnLifeCycleDispose);
			}

			// Unmanage (not an official phase)
			foreach (var instance in instances)
			{
				try
				{
					this.Unmanage(instance);
				}
				catch (Exception ex)
				{
					LifecycleErrorReporter.ReportNonPhaseFailure(instance, nameof(Unmanage), ex);
				}
			}
		}

		private void DestroyImmediate(IHasManagedLifeCycle instance)
		{
			try
			{
				if (!this.destroyQueue.Contains(instance))
				{
					this.destroyQueue.Add(instance);
				}

				this.DestroyEnqueued();
			}
			finally
			{
				this.destroyQueue.Clear();
			}
		}
	}

	internal static class LifecycleInvoker
	{
		public static void SafeInvoke(
			IHasManagedLifeCycle instance,
			LifecyclePhase phase,
			string operation,
			Action action)
		{
			if (instance == null || action == null)
			{
				return;
			}

			try
			{
				action.Invoke();
			}
			catch (Exception ex)
			{
				LifecycleErrorReporter.ReportPhaseFailure(instance, phase, operation, ex);
			}
		}
	}

	internal static class LifecycleErrorReporter
	{
		public static void ReportPhaseFailure(
			IHasManagedLifeCycle instance,
			LifecyclePhase phase,
			string operation,
			Exception exception)
		{
			if (exception == null) return;

			var message = LifecycleLogMessages.FormatPhaseFailure(phase, operation, instance);
			Log(message, instance, exception);
		}

		public static void ReportNonPhaseFailure(
			IHasManagedLifeCycle instance,
			string operation,
			Exception exception)
		{
			if (exception == null) return;

			var message = LifecycleLogMessages.FormatNonPhaseFailure(operation, instance);
			Log(message, instance, exception);
		}

		private static void Log(string message, IHasManagedLifeCycle instance, Exception exception)
		{
			// Preserve original stack trace / double-click location by logging the ORIGINAL exception object.
			if (instance is UnityEngine.Object context)
			{
				Debug.LogError(message, context);
				Debug.LogException(exception, context);
				return;
			}

			Debug.LogError(message);
			Debug.LogException(exception);
		}
	}

	/// <summary>
	/// Centralized log message templates and formatting for lifecycle diagnostics.
	/// Keep ALL lifecycle-related message strings here.
	/// </summary>
	internal static class LifecycleLogMessages
	{
		// Templates (single source of truth)
		private const string NullArgumentTemplate = "[LifeCycleService] Tried to {0} a null instance.";
		private const string PhaseFailureTemplate = "[Lifecycle:{0}] Operation: {1} Instance: {2}";
		private const string NonPhaseFailureTemplate = "[Lifecycle] Operation: {0} Instance: {1}";

		public static string FormatNullArgument(string operation)
		{
			return string.Format(NullArgumentTemplate, operation);
		}

		public static string FormatPhaseFailure(LifecyclePhase phase, string operation, IHasManagedLifeCycle instance)
		{
			return string.Format(PhaseFailureTemplate, phase, operation, GetInstanceTypeName(instance));
		}

		public static string FormatNonPhaseFailure(string operation, IHasManagedLifeCycle instance)
		{
			return string.Format(NonPhaseFailureTemplate, operation, GetInstanceTypeName(instance));
		}

		private static string GetInstanceTypeName(IHasManagedLifeCycle instance)
		{
			return instance?.GetType().FullName ?? "<null>";
		}
	}
}
