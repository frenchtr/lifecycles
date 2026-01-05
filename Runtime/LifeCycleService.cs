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
			this.managedInstances = managedInstances?.ToList() ?? new List<IHasManagedLifeCycle>();
			this.awakenQueue = new List<IHasManagedLifeCycle>();
			this.enableQueue = new List<IHasManagedLifeCycle>();
			this.disableQueue = new List<IHasManagedLifeCycle>();
			this.destroyQueue = new List<IHasManagedLifeCycle>();
		}

		public void Manage(IHasManagedLifeCycle instance)
		{
			if (this.managedInstances.Contains(instance))
			{
				return;
			}

			this.managedInstances.Add(instance);
		}

		public void Unmanage(IHasManagedLifeCycle instance)
		{
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
			if (this.enableQueue.Contains(instance))
			{
				return;
			}

			this.disableQueue.Remove(instance);

			this.enableQueue.Add(instance);
		}

		public void RequestDisable(IHasManagedLifeCycle instance)
		{
			if (this.disableQueue.Contains(instance))
			{
				return;
			}

			this.enableQueue.Remove(instance);

			this.disableQueue.Add(instance);
		}

		public void RequestDestroy(IHasManagedLifeCycle instance)
		{
			if (this.destroyQueue.Contains(instance))
			{
				return;
			}

			this.awakenQueue.Remove(instance);
			this.enableQueue.Remove(instance);
			this.disableQueue.Remove(instance);

			this.destroyQueue.Add(instance);

			this.DestroyImmediate(instance);
		}

		public void Tick()
		{
			this.AwakenEnqueued();
			this.awakenQueue.Clear();

			this.EnableEnqueued();
			this.enableQueue.Clear();

			this.DisableEnqueued();
			this.disableQueue.Clear();

			this.DestroyEnqueued();
			this.destroyQueue.Clear();
		}

		private void AwakenEnqueued()
		{
			var instances = this.awakenQueue.ToList();

			// 1) Compose
			foreach (var instance in instances)
			{
				if (!instance.IsAlive)
				{
					continue;
				}

				try
				{
					instance.OnLifeCycleCompose();
				}
				catch (Exception)
				{
					Debug.LogError($"An error occurred in the {nameof(IHasManagedLifeCycle.OnLifeCycleCompose)} method of {instance}.");
					throw;
				}
			}

			// 2) Verify
			foreach (var instance in instances)
			{
				if (!instance.IsAlive)
				{
					continue;
				}

				try
				{
					instance.OnLifeCycleVerifyConfiguration();
				}
				catch (Exception)
				{
					Debug.LogError($"An error occurred in the {nameof(IHasManagedLifeCycle.OnLifeCycleVerifyConfiguration)} method of {instance}.");
					throw;
				}
			}

			// 3) Register for discovery
			foreach (var instance in instances)
			{
				if (!instance.IsAlive)
				{
					continue;
				}

				try
				{
					instance.OnLifeCycleRegisterForDiscovery();
				}
				catch (Exception)
				{
					Debug.LogError($"An error occurred in the {nameof(IHasManagedLifeCycle.OnLifeCycleRegisterForDiscovery)} method of {instance}.");
					throw;
				}
			}

			// 4) Wire endpoints (initial binding)
			foreach (var instance in instances)
			{
				if (!instance.IsAlive)
				{
					continue;
				}

				try
				{
					instance.OnLifeCycleWireEndpoints();
				}
				catch (Exception)
				{
					Debug.LogError($"An error occurred in the {nameof(IHasManagedLifeCycle.OnLifeCycleWireEndpoints)} method of {instance}.");
					throw;
				}
			}
			
			// 5) Initialize (domain-specific readiness before any subscriptions/activation)
			foreach (var instance in instances)
			{
				if (!instance.IsAlive)
				{
					continue;
				}

				try
				{
					instance.OnLifeCycleInitialize();
				}
				catch (Exception)
				{
					Debug.LogError($"An error occurred in the {nameof(IHasManagedLifeCycle.OnLifeCycleInitialize)} method of {instance}.");
					throw;
				}
			}
		}

		private void EnableEnqueued()
		{
			var instances = this.enableQueue.ToList();

			// 6) Subscribe to external events
			foreach (var instance in instances)
			{
				if (!instance.IsAlive || !instance.IsEnabled)
				{
					continue;
				}

				try
				{
					instance.OnLifeCycleSubscribeToExternalEvents();
				}
				catch (Exception)
				{
					Debug.LogError($"An error occurred in the {nameof(IHasManagedLifeCycle.OnLifeCycleSubscribeToExternalEvents)} method of {instance}.");
					throw;
				}
			}

			// 7) Activate
			foreach (var instance in instances)
			{
				if (!instance.IsAlive || !instance.IsEnabled)
				{
					continue;
				}

				try
				{
					instance.OnLifeCycleActivate();
				}
				catch (Exception)
				{
					Debug.LogError($"An error occurred in the {nameof(IHasManagedLifeCycle.OnLifeCycleActivate)} method of {instance}.");
					throw;
				}
			}
		}

		private void DisableEnqueued()
		{
			var instances = this.disableQueue.ToList();

			// 8) Deactivate
			foreach (var instance in instances)
			{
				if (!instance.IsAlive)
				{
					continue;
				}

				try
				{
					instance.OnLifeCycleDeactivate();
				}
				catch (Exception)
				{
					Debug.LogError($"An error occurred in the {nameof(IHasManagedLifeCycle.OnLifeCycleDeactivate)} method of {instance}.");
					throw;
				}
			}

			// 9) Unsubscribe
			foreach (var instance in instances)
			{
				if (!instance.IsAlive)
				{
					continue;
				}

				try
				{
					instance.OnLifeCycleUnsubscribeFromExternalEvents();
				}
				catch (Exception)
				{
					Debug.LogError($"An error occurred in the {nameof(IHasManagedLifeCycle.OnLifeCycleUnsubscribeFromExternalEvents)} method of {instance}.");
					throw;
				}
			}

			// 10) Unwire
			foreach (var instance in instances)
			{
				if (!instance.IsAlive)
				{
					continue;
				}

				try
				{
					instance.OnLifeCycleUnwireEndpoints();
				}
				catch (Exception)
				{
					Debug.LogError($"An error occurred in the {nameof(IHasManagedLifeCycle.OnLifeCycleUnwireEndpoints)} method of {instance}.");
					throw;
				}
			}
		}

		private void DestroyEnqueued()
		{
			var instances = this.destroyQueue.ToList();

			// 11) Unregister
			foreach (var instance in instances)
			{
				if (!instance.IsAlive)
				{
					continue;
				}

				try
				{
					instance.OnLifeCycleUnregisterFromDiscovery();
				}
				catch (Exception)
				{
					Debug.LogError($"An error occurred in the {nameof(IHasManagedLifeCycle.OnLifeCycleUnregisterFromDiscovery)} method of {instance}.");
					throw;
				}
			}

			// 12) Dispose
			foreach (var instance in instances)
			{
				if (!instance.IsAlive)
				{
					continue;
				}

				try
				{
					instance.OnLifeCycleDispose();
				}
				catch (Exception)
				{
					Debug.LogError($"An error occurred in the {nameof(IHasManagedLifeCycle.OnLifeCycleDispose)} method of {instance}.");
					throw;
				}
			}

			foreach (var instance in instances)
			{
				try
				{
					this.Unmanage(instance);
				}
				catch (Exception)
				{
					Debug.LogError($"An error occurred when unmanaging instance {instance}.");
					throw;
				}
			}
		}

		private void DestroyImmediate(IHasManagedLifeCycle instance)
		{
			// Best-effort teardown ordering:
			// Deactivate -> Unsubscribe -> Unwire -> Unregister -> Dispose -> Unmanage
			try
			{
				if (instance.IsAlive)
				{
					instance.OnLifeCycleDeactivate();
				}
			}
			catch (Exception)
			{
				Debug.LogError($"An error occurred in the {nameof(IHasManagedLifeCycle.OnLifeCycleDeactivate)} method of {instance}.");
				throw;
			}

			try
			{
				if (instance.IsAlive)
				{
					instance.OnLifeCycleUnsubscribeFromExternalEvents();
				}
			}
			catch (Exception)
			{
				Debug.LogError($"An error occurred in the {nameof(IHasManagedLifeCycle.OnLifeCycleUnsubscribeFromExternalEvents)} method of {instance}.");
				throw;
			}

			try
			{
				if (instance.IsAlive)
				{
					instance.OnLifeCycleUnwireEndpoints();
				}
			}
			catch (Exception)
			{
				Debug.LogError($"An error occurred in the {nameof(IHasManagedLifeCycle.OnLifeCycleUnwireEndpoints)} method of {instance}.");
				throw;
			}

			try
			{
				if (instance.IsAlive)
				{
					instance.OnLifeCycleUnregisterFromDiscovery();
				}
			}
			catch (Exception)
			{
				Debug.LogError($"An error occurred in the {nameof(IHasManagedLifeCycle.OnLifeCycleUnregisterFromDiscovery)} method of {instance}.");
				throw;
			}

			try
			{
				if (instance.IsAlive)
				{
					instance.OnLifeCycleDispose();
				}
			}
			catch (Exception)
			{
				Debug.LogError($"An error occurred in the {nameof(IHasManagedLifeCycle.OnLifeCycleDispose)} method of {instance}.");
				throw;
			}

			try
			{
				this.Unmanage(instance);
			}
			catch (Exception)
			{
				Debug.LogError($"An error occurred when unmanaging instance {instance}.");
				throw;
			}
		}
	}
}
