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

			if (this.awakenQueue.Contains(instance))
			{
				this.awakenQueue.Remove(instance);				
			}
			
			if (this.enableQueue.Contains(instance))
			{
				this.enableQueue.Remove(instance);				
			}
			
			if (this.disableQueue.Contains(instance))
			{
				this.disableQueue.Remove(instance);				
			}
			
			if (this.destroyQueue.Contains(instance))
			{
				this.destroyQueue.Remove(instance);				
			}
		}

		public void AwakenAll()
		{
			var instancesToAwaken = this.managedInstances.ToList();
			foreach (var instance in instancesToAwaken)
			{
				this.RequestAwaken(instance);
			}
		}

		public void EnableAll()
		{
			var instancesToEnable = this.managedInstances.ToList();
			foreach (var instance in instancesToEnable)
			{
				this.RequestEnable(instance);
			}
		}

		public void DisableAll()
		{
			var instancesToDisable = this.managedInstances.ToList();
			foreach (var instance in instancesToDisable)
			{
				this.RequestDisable(instance);
			}
		}

		public void DestroyAll()
		{
			var instancesToDestroy = this.managedInstances.ToList();
			foreach (var instance in instancesToDestroy)
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

			if (this.destroyQueue.Contains(instance))
			{
				this.destroyQueue.Remove(instance);
			}

			this.awakenQueue.Add(instance);
		}

		public void RequestEnable(IHasManagedLifeCycle instance)
		{
			if (this.enableQueue.Contains(instance))
			{
				return;
			}
			
			if (this.disableQueue.Contains(instance))
			{
				this.disableQueue.Remove(instance);
			}

			this.enableQueue.Add(instance);
		}

		public void RequestDisable(IHasManagedLifeCycle instance)
		{
			if (this.disableQueue.Contains(instance))
			{
				return;
			}
			
			if (this.enableQueue.Contains(instance))
			{
				this.enableQueue.Remove(instance);
			}

			this.disableQueue.Add(instance);
		}

		public void RequestDestroy(IHasManagedLifeCycle instance)
		{
			if (this.destroyQueue.Contains(instance))
			{
				return;
			}
			
			if (this.awakenQueue.Contains(instance))
			{
				this.awakenQueue.Remove(instance);
			}

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
			var instancesToAwaken = this.awakenQueue.ToList();
			foreach (var instance in instancesToAwaken)
			{
				if (!instance.IsAlive)
				{
					continue;
				}
				
				try
				{
					instance.OnCompose();
				}
				catch (Exception exception)
				{
					Debug.LogError($"An error occurred in the {nameof(IHasManagedLifeCycle.OnCompose)} method of {instance}.");
					throw;
				}
			}
			
			foreach (var instance in instancesToAwaken)
			{
				if (!instance.IsAlive)
				{
					continue;
				}
				
				try
				{
					instance.OnRegister();
				}
				catch (Exception exception)
				{
					Debug.LogError($"An error occurred in the {nameof(IHasManagedLifeCycle.OnRegister)} method of {instance}.");
					throw;
				}
			}
			
			foreach (var instance in instancesToAwaken)
			{
				if (!instance.IsAlive)
				{
					continue;
				}

				if (!instance.IsEnabled)
				{
					continue;
				}
				
				try
				{
					instance.OnInitialize();
				}
				catch (Exception exception)
				{
					Debug.LogError($"An error occurred in the {nameof(IHasManagedLifeCycle.OnInitialize)} method of {instance}.");
					throw;
				}
			}
		}
		
		private void EnableEnqueued()
		{
			var instancesToEnable = this.enableQueue.ToList();
			foreach (var instance in instancesToEnable)
			{
				if (!instance.IsAlive)
				{
					continue;
				}

				if (!instance.IsEnabled)
				{
					continue;
				}

				try
				{
					instance.OnBind();
				}
				catch (Exception exception)
				{
					Debug.LogError($"An error occurred in the {nameof(IHasManagedLifeCycle.OnBind)} method of {instance}.");
					throw;
				}
			}
			
			foreach (var instance in instancesToEnable)
			{
				if (!instance.IsAlive)
				{
					continue;
				}

				if (!instance.IsEnabled)
				{
					continue;
				}
				
				try
				{
					instance.OnActivate();
				}
				catch (Exception exception)
				{
					Debug.LogError($"An error occurred in the {nameof(IHasManagedLifeCycle.OnActivate)} method of {instance}.");
					throw;
				}
			}
		}
		
		private void DisableEnqueued()
		{
			var instancesToDisable = this.disableQueue.ToList();
			foreach (var instance in instancesToDisable)
			{
				if (instance is null)
				{
					continue;
				}
				
				try
				{
					instance.OnDeactivate();
				}
				catch (Exception exception)
				{
					Debug.LogError($"An error occurred in the {nameof(IHasManagedLifeCycle.OnDeactivate)} method of {instance}.");
					throw;
				}
			}
			
			foreach (var instance in instancesToDisable)
			{
				if (instance is null)
				{
					continue;
				}
				
				try
				{
					instance.OnUnbind();
				}
				catch (Exception exception)
				{
					Debug.LogError($"An error occurred in the {nameof(IHasManagedLifeCycle.OnUnbind)} method of {instance}.");
					throw;
				}
			}
		}
		
		private void DestroyEnqueued()
		{
			var instancesToDestroy = this.destroyQueue.ToList();
			foreach (var instance in instancesToDestroy)
			{
				if (instance is null)
				{
					continue;
				}
				
				try
				{
					instance.OnUnregister();
				}
				catch (Exception exception)
				{
					Debug.LogError($"An error occurred in the {nameof(IHasManagedLifeCycle.OnUnregister)} method of {instance}.");
					throw;
				}
			}
			
			foreach (var instance in instancesToDestroy)
			{
				if (instance is null)
				{
					continue;
				}
				
				try
				{
					instance.OnDispose();
				}
				catch (Exception exception)
				{
					Debug.LogError($"An error occurred in the {nameof(IHasManagedLifeCycle.OnDispose)} method of {instance}.");
					throw;
				}
			}

			foreach (var instance in instancesToDestroy)
			{
				try
				{
					this.Unmanage(instance);
				}
				catch (Exception exception)
				{
					Debug.LogError($"An error occurred when unmanaging instance {instance}.");
					throw;
				}
			}
		}
		
		

		private void DestroyImmediate(IHasManagedLifeCycle instance)
		{
			try
			{
				instance.OnDeactivate();
			}
			catch (Exception exception)
			{
				Debug.LogError($"An error occurred in the {nameof(IHasManagedLifeCycle.OnDeactivate)} method of {instance}.");
				throw;
			}
			
			try
			{
				instance.OnUnbind();
			}
			catch (Exception exception)
			{
				Debug.LogError($"An error occurred in the {nameof(IHasManagedLifeCycle.OnUnbind)} method of {instance}.");
				throw;
			}
			
			try
			{
				instance.OnUnregister();
			}
			catch (Exception exception)
			{
				Debug.LogError($"An error occurred in the {nameof(IHasManagedLifeCycle.OnUnregister)} method of {instance}.");
				throw;
			}
			
			try
			{
				instance.OnDispose();
			}
			catch (Exception exception)
			{
				Debug.LogError($"An error occurred in the {nameof(IHasManagedLifeCycle.OnDispose)} method of {instance}.");
				throw;
			}

			try
			{
				this.Unmanage(instance);
			}
			catch (Exception exception)
			{
				Debug.LogError($"An error occurred when unmanaging instance {instance}.");
				throw;
			}
		}
	}
}
