// =========================
// File: LifecycleErrorReporter.cs
// =========================
using System;
using UnityEngine;

namespace TravisRFrench.Lifecycles.Runtime
{
	internal static class LifecycleErrorReporter
	{
		public static void ReportPhaseFailure(ILifeCycleManaged instance, LifecyclePhase phase, string operation, Exception exception)
		{
			if (exception == null)
			{
				return;
			}

			var msg = LifecycleLogMessages.FormatPhaseFailure(phase, operation, instance);
			Log(msg, instance, exception);
		}

		public static void ReportNonPhaseFailure(ILifeCycleManaged instance, string operation, Exception exception)
		{
			if (exception == null)
			{
				return;
			}

			var msg = LifecycleLogMessages.FormatNonPhaseFailure(operation, instance);
			Log(msg, instance, exception);
		}

		private static void Log(string message, ILifeCycleManaged instance, Exception exception)
		{
			if (instance is UnityEngine.Object context)
			{
				Debug.LogError(message, context);
				Debug.LogException(exception, context); // preserves stack trace / double-click
				return;
			}

			Debug.LogError(message);
			Debug.LogException(exception);
		}
	}
}
