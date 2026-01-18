// =========================
// File: LifecycleLogMessages.cs
// =========================

namespace TravisRFrench.Lifecycles.Runtime
{
	internal static class LifecycleLogMessages
	{
		private const string NullArgumentTemplate = "[LifeCycleService] Tried to {0} a null instance.";
		private const string PhaseFailureTemplate = "[Lifecycle:{0}] Operation: {1} Instance: {2}";
		private const string NonPhaseFailureTemplate = "[Lifecycle] Operation: {0} Instance: {1}";

		public static string FormatNullArgument(string operation)
			=> string.Format(NullArgumentTemplate, operation);

		public static string FormatPhaseFailure(LifecyclePhase phase, string operation, ILifeCycleManaged instance)
			=> string.Format(PhaseFailureTemplate, phase, operation, GetInstanceTypeName(instance));

		public static string FormatNonPhaseFailure(string operation, ILifeCycleManaged instance)
			=> string.Format(NonPhaseFailureTemplate, operation, GetInstanceTypeName(instance));

		private static string GetInstanceTypeName(ILifeCycleManaged instance)
			=> instance?.GetType().FullName ?? "<null>";
	}
}
