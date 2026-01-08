// =========================
// File: LifecycleState.cs (internal)
// =========================
using System;

namespace TravisRFrench.Lifecycles.Runtime
{
	internal sealed class LifecycleState
	{
		public LifecyclePhase Phase { get; private set; } = LifecyclePhase.None;
		public bool IsActive { get; private set; }
		public bool HasFirstActivationRun { get; private set; }
		public bool IsFaulted => this.Phase == LifecyclePhase.Faulted;

		public Exception LastException { get; private set; }
		public string FaultSummary { get; private set; }

		public void SetPhase(LifecyclePhase phase) => this.Phase = phase;

		public void SetActive(bool active) => this.IsActive = active;

		public void MarkFirstActivationRun() => this.HasFirstActivationRun = true;

		public void MarkFaulted(Exception ex, string summary)
		{
			this.LastException = ex;
			this.FaultSummary = summary;
			this.Phase = LifecyclePhase.Faulted;
		}
	}
}
