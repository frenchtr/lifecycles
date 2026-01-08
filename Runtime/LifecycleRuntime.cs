// =========================
// File: LifeCycleRuntime.cs
// =========================

namespace TravisRFrench.Lifecycles.Runtime
{
	/// <summary>
	/// Minimal runtime entry point.
	/// Replace with your DI bootstrapper / installers as needed.
	/// </summary>
	public static class LifeCycleRuntime
	{
		private static ILifeCycleService service;

		public static ILifeCycleService Service
		{
			get
			{
				service ??= new LifeCycleService();
				return service;
			}
			set => service = value;
		}
	}
}