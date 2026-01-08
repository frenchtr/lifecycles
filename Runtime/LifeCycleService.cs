// =========================
// File: LifeCycleService.cs
// =========================
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TravisRFrench.Lifecycles.Runtime
{
	/// <summary>
	/// Deterministic lifecycle runtime that executes framework-owned phases as barriers.
	/// Unity callbacks are treated as requests; execution occurs in Tick().
	/// </summary>
	public sealed class LifeCycleService : ILifeCycleService
	{
		private readonly List<ILifeCycleManaged> managed = new();
		private readonly Dictionary<ILifeCycleManaged, LifecycleState> state = new();

		private readonly List<ILifeCycleManaged> awakeQueue = new();
		private readonly List<ILifeCycleManaged> enableQueue = new();
		private readonly List<ILifeCycleManaged> disableQueue = new();
		private readonly List<ILifeCycleManaged> destroyQueue = new();

		// Deferred unmanage to preserve barrier processing
		private readonly HashSet<ILifeCycleManaged> pendingRemoval = new();

		public void Manage(ILifeCycleManaged instance)
		{
			if (instance == null)
			{
				Debug.LogError(LifecycleLogMessages.FormatNullArgument(nameof(this.Manage)));
				return;
			}

			if (this.managed.Contains(instance))
			{
				return;
			}

			this.managed.Add(instance);
			this.state[instance] = new LifecycleState();

			this.MirrorAll(instance);
		}

		public void Unmanage(ILifeCycleManaged instance)
		{
			if (instance == null)
			{
				return;
			}

			this.managed.Remove(instance);
			this.state.Remove(instance);

			this.awakeQueue.Remove(instance);
			this.enableQueue.Remove(instance);
			this.disableQueue.Remove(instance);
			this.destroyQueue.Remove(instance);
			this.pendingRemoval.Remove(instance);
		}

		public void RequestAwake(ILifeCycleManaged instance)
		{
			if (!this.GuardRequest(instance, nameof(this.RequestAwake)))
			{
				return;
			}

			if (!this.awakeQueue.Contains(instance))
			{
				this.destroyQueue.Remove(instance);
				this.awakeQueue.Add(instance);
			}
		}

		public void RequestEnable(ILifeCycleManaged instance)
		{
			if (!this.GuardRequest(instance, nameof(this.RequestEnable)))
			{
				return;
			}

			if (!this.enableQueue.Contains(instance))
			{
				this.disableQueue.Remove(instance);
				this.enableQueue.Add(instance);
			}
		}

		public void RequestDisable(ILifeCycleManaged instance)
		{
			if (!this.GuardRequest(instance, nameof(this.RequestDisable)))
			{
				return;
			}

			if (!this.disableQueue.Contains(instance))
			{
				this.enableQueue.Remove(instance);
				this.disableQueue.Add(instance);
			}
		}

		public void RequestDestroy(ILifeCycleManaged instance)
		{
			if (!this.GuardRequest(instance, nameof(this.RequestDestroy)))
			{
				return;
			}

			this.awakeQueue.Remove(instance);
			this.enableQueue.Remove(instance);
			this.disableQueue.Remove(instance);

			if (!this.destroyQueue.Contains(instance))
			{
				this.destroyQueue.Add(instance);
			}

			// Immediate destroy semantics for this instance:
			// - run destroy pass now for this instance
			// - defer unmanage until end-of-tick
			this.DestroyImmediate(instance);
		}

		public void Tick()
		{
			try { this.DrainAwake(); }
			finally { this.awakeQueue.Clear(); }

			try { this.DrainEnable(); }
			finally { this.enableQueue.Clear(); }

			try { this.DrainDisable(); }
			finally { this.disableQueue.Clear(); }

			try { this.DrainDestroy(); }
			finally { this.destroyQueue.Clear(); }

			if (this.pendingRemoval.Count > 0)
			{
				foreach (var inst in this.pendingRemoval.ToList())
				{
					this.Unmanage(inst);
				}

				this.pendingRemoval.Clear();
			}
		}

		// -------------------
		// Barrier processing
		// -------------------

		private void DrainAwake()
		{
			var instances = Snapshot(this.awakeQueue);

			foreach (var inst in instances)
			{
				if (!this.CanRun(inst))
				{
					continue;
				}

				this.Advance(inst, LifecyclePhase.Assembled, nameof(ILifeCycleManaged.Compose), inst.Compose);
			}

			foreach (var inst in instances)
			{
				if (!this.CanRun(inst))
				{
					continue;
				}

				this.Advance(inst, LifecyclePhase.VerifiedStructure, nameof(ILifeCycleManaged.VerifyComposition), inst.VerifyComposition);
			}

			foreach (var inst in instances)
			{
				if (!this.CanRun(inst))
				{
					continue;
				}

				this.Advance(inst, LifecyclePhase.Registered, nameof(ILifeCycleManaged.Register), inst.Register);
			}

			foreach (var inst in instances)
			{
				if (!this.CanRun(inst))
				{
					continue;
				}

				this.Advance(inst, LifecyclePhase.Setup, nameof(ILifeCycleManaged.Setup), inst.Setup);
			}
		}

		private void DrainEnable()
		{
			var instances = Snapshot(this.enableQueue);

			foreach (var inst in instances)
			{
				if (!this.CanRun(inst))
				{
					continue;
				}

				if (!this.HasReached(inst, LifecyclePhase.Setup))
				{
					continue;
				}

				this.Advance(inst, LifecyclePhase.InitializedEnable, nameof(ILifeCycleManaged.InitializeEnable), inst.InitializeEnable);
			}

			foreach (var inst in instances)
			{
				if (!this.CanRun(inst))
				{
					continue;
				}

				if (!this.HasReached(inst, LifecyclePhase.InitializedEnable))
				{
					continue;
				}

				this.Advance(inst, LifecyclePhase.Subscribed, nameof(ILifeCycleManaged.Subscribe), inst.Subscribe);
			}

			foreach (var inst in instances)
			{
				if (!this.CanRun(inst))
				{
					continue;
				}

				if (!this.HasReached(inst, LifecyclePhase.Subscribed))
				{
					continue;
				}

				this.Advance(inst, LifecyclePhase.Activated, nameof(ILifeCycleManaged.Activate), inst.Activate);
				if (!this.IsFaulted(inst))
				{
					this.SetActive(inst, true);
				}
			}

			foreach (var inst in instances)
			{
				if (!this.CanRun(inst))
				{
					continue;
				}

				if (!this.HasReached(inst, LifecyclePhase.Activated))
				{
					continue;
				}

				var s = this.state[inst];
				if (s.HasFirstActivationRun)
				{
					continue;
				}

				this.Advance(inst, LifecyclePhase.FirstActivation, nameof(ILifeCycleManaged.FirstActivation), inst.FirstActivation);
				if (!this.IsFaulted(inst))
				{
					s.MarkFirstActivationRun();
				}

				this.MirrorAll(inst);
			}
		}

		private void DrainDisable()
		{
			var instances = Snapshot(this.disableQueue);

			foreach (var inst in instances)
			{
				if (!this.CanRun(inst))
				{
					continue;
				}

				if (!this.HasReached(inst, LifecyclePhase.Activated))
				{
					continue;
				}

				this.Advance(inst, LifecyclePhase.Deactivated, nameof(ILifeCycleManaged.Deactivate), inst.Deactivate);
				if (!this.IsFaulted(inst))
				{
					this.SetActive(inst, false); // at end of deactivate
				}
			}

			foreach (var inst in instances)
			{
				if (!this.CanRun(inst))
				{
					continue;
				}

				if (!this.HasReached(inst, LifecyclePhase.Subscribed))
				{
					continue;
				}

				this.Advance(inst, LifecyclePhase.Unsubscribed, nameof(ILifeCycleManaged.Unsubscribe), inst.Unsubscribe);
			}

			foreach (var inst in instances)
			{
				if (!this.CanRun(inst))
				{
					continue;
				}

				if (!this.HasReached(inst, LifecyclePhase.InitializedEnable))
				{
					continue;
				}

				this.Advance(inst, LifecyclePhase.FinalizedDisable, nameof(ILifeCycleManaged.FinalizeDisable), inst.FinalizeDisable);
			}
		}

		private void DrainDestroy()
		{
			var instances = Snapshot(this.destroyQueue);

			foreach (var inst in instances)
			{
				if (!this.CanRunEvenIfFaulted(inst))
				{
					continue;
				}

				if (this.HasReached(inst, LifecyclePhase.Setup))
				{
					this.AdvanceEvenIfFaulted(inst, LifecyclePhase.Teardown, nameof(ILifeCycleManaged.Teardown), inst.Teardown);
				}
			}

			foreach (var inst in instances)
			{
				if (!this.CanRunEvenIfFaulted(inst))
				{
					continue;
				}

				if (this.HasReached(inst, LifecyclePhase.Registered))
				{
					this.AdvanceEvenIfFaulted(inst, LifecyclePhase.Unregistered, nameof(ILifeCycleManaged.Unregister), inst.Unregister);
				}
			}

			foreach (var inst in instances)
			{
				if (!this.CanRunEvenIfFaulted(inst))
				{
					continue;
				}

				this.AdvanceEvenIfFaulted(inst, LifecyclePhase.Disposed, nameof(ILifeCycleManaged.Dispose), inst.Dispose);

				this.pendingRemoval.Add(inst);
			}
		}

		private void DestroyImmediate(ILifeCycleManaged instance)
		{
			try
			{
				this.destroyQueue.Clear();
				this.destroyQueue.Add(instance);
				this.DrainDestroy();
			}
			finally
			{
				this.destroyQueue.Clear();
			}
		}

		// -------------------
		// Helpers
		// -------------------

		private static List<ILifeCycleManaged> Snapshot(List<ILifeCycleManaged> queue)
			=> queue
				.Where(i => i != null)
				.Distinct()
				.ToList();

		private bool GuardRequest(ILifeCycleManaged instance, string operation)
		{
			if (instance == null)
			{
				Debug.LogError(LifecycleLogMessages.FormatNullArgument(operation));
				return false;
			}

			if (!this.managed.Contains(instance))
			{
				this.Manage(instance);
			}

			return true;
		}

		private bool CanRun(ILifeCycleManaged instance)
		{
			if (instance == null)
			{
				return false;
			}

			if (!this.state.TryGetValue(instance, out var s))
			{
				return false;
			}

			if (s.IsFaulted)
			{
				return false;
			}

			if (!instance.IsAlive)
			{
				return false;
			}

			return true;
		}

		private bool CanRunEvenIfFaulted(ILifeCycleManaged instance)
		{
			if (instance == null)
			{
				return false;
			}

			if (!this.state.ContainsKey(instance))
			{
				return false;
			}

			return true; // allow destroy band despite faults
		}

		private bool HasReached(ILifeCycleManaged instance, LifecyclePhase phase)
		{
			if (!this.state.TryGetValue(instance, out var s))
			{
				return false;
			}

			return s.Phase >= phase && s.Phase != LifecyclePhase.Faulted;
		}

		private bool IsFaulted(ILifeCycleManaged instance)
		{
			return this.state.TryGetValue(instance, out var s) && s.IsFaulted;
		}

		private void SetActive(ILifeCycleManaged instance, bool active)
		{
			var s = this.state[instance];
			s.SetActive(active);

			if (instance is ILifecycleDebugStateSink sink)
			{
				sink.__SetIsActive(active);
			}
		}

		private void MarkFaulted(ILifeCycleManaged instance, Exception ex, LifecyclePhase phase, string op)
		{
			var summary = LifecycleLogMessages.FormatPhaseFailure(phase, op, instance);
			this.state[instance].MarkFaulted(ex, summary);

			if (instance is ILifecycleDebugStateSink sink)
			{
				sink.__SetIsFaulted(true);
				sink.__SetFaultSummary(summary);
				sink.__SetPhase(LifecyclePhase.Faulted);
			}
		}

		private void Advance(ILifeCycleManaged instance, LifecyclePhase phase, string operation, Action action)
		{
			if (!this.state.TryGetValue(instance, out var s))
			{
				return;
			}

			if (s.IsFaulted)
			{
				return;
			}

			// already at or past
			if (s.Phase >= phase)
			{
				return;
			}

			if (!LifecycleInvoker.SafeInvoke(instance, phase, operation, action, out var ex))
			{
				this.MarkFaulted(instance, ex, phase, operation);
				return;
			}

			s.SetPhase(phase);

			if (instance is ILifecycleDebugStateSink sink)
			{
				sink.__SetPhase(phase);
			}
		}

		private void AdvanceEvenIfFaulted(ILifeCycleManaged instance, LifecyclePhase phase, string operation, Action action)
		{
			// Allow destroy band even if faulted; still keep idempotency.
			if (!this.state.TryGetValue(instance, out var s))
			{
				return;
			}

			if (s.Phase >= phase)
			{
				return;
			}

			if (!LifecycleInvoker.SafeInvoke(instance, phase, operation, action, out var ex))
			{
				// Do not overwrite prior fault marker; just mirror a summary if we have none.
				if (s.LastException == null)
				{
					this.MarkFaulted(instance, ex, phase, operation);
				}

				return;
			}

			s.SetPhase(phase);

			if (instance is ILifecycleDebugStateSink sink)
			{
				sink.__SetPhase(phase);
			}
		}

		private void MirrorAll(ILifeCycleManaged instance)
		{
			if (!this.state.TryGetValue(instance, out var s))
			{
				return;
			}

			if (instance is not ILifecycleDebugStateSink sink)
			{
				return;
			}

			sink.__SetPhase(s.Phase);
			sink.__SetIsActive(s.IsActive);
			sink.__SetIsFaulted(s.IsFaulted);
			sink.__SetFaultSummary(s.FaultSummary);
		}
	}
}
