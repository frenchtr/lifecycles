// =========================
// File: LifecycleInvoker.cs
// =========================
using System;

namespace TravisRFrench.Lifecycles.Runtime
{
	internal static class LifecycleInvoker
	{
		public static bool SafeInvoke(ILifeCycleManaged instance, LifecyclePhase phase, string operation, Action action, out Exception caught)
		{
			caught = null;

			if (instance == null || action == null)
			{
				return false;
			}

			try
			{
				action.Invoke();
				return true;
			}
			catch (Exception ex)
			{
				caught = ex;
				LifecycleErrorReporter.ReportPhaseFailure(instance, phase, operation, ex);
				return false;
			}
		}
	}
}
