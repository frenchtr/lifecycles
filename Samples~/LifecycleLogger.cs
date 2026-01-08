// =========================
// File: LifecycleLogger.cs
// Runtime MonoBehaviour that logs every lifecycle callback.
// =========================

using TravisRFrench.Lifecycles.Runtime;
using UnityEngine;

namespace TravisRFrench.Lifecycles.lifecycles.Samples
{
	/// <summary>
	/// Logs lifecycle callbacks as they occur.
	/// Useful for validating ordering, barriers, and enable/disable behavior.
	/// </summary>
	public sealed class LifecycleLogger : ManagedMonoBehaviour
	{
		[SerializeField] private bool includeFrameCount = true;
		[SerializeField] private bool includeGameObjectPath = false;

		private string Prefix
		{
			get
			{
				var frame = this.includeFrameCount ? $"[frame:{Time.frameCount}] " : string.Empty;
				var who = this.includeGameObjectPath ? $"{GetHierarchyPath(this.transform)}" : $"{this.gameObject.name}";
				return $"{frame}[{who}] ";
			}
		}

		private void Log(string callbackName)
		{
			Debug.Log($"{this.Prefix}{callbackName}");
		}
		
		protected override void OnLifecycleCompose() => this.Log(nameof(this.OnLifecycleCompose));
		protected override void OnLifecycleVerifyComposition() => this.Log(nameof(this.OnLifecycleVerifyComposition));
		protected override void OnLifecycleRegister() => this.Log(nameof(this.OnLifecycleRegister));
		protected override void OnLifecycleSetup() => this.Log(nameof(this.OnLifecycleSetup));

		protected override void OnLifecycleInitializeEnable() => this.Log(nameof(this.OnLifecycleInitializeEnable));
		protected override void OnLifecycleSubscribe() => this.Log(nameof(this.OnLifecycleSubscribe));
		protected override void OnLifecycleActivate() => this.Log(nameof(this.OnLifecycleActivate));
		protected override void OnLifecycleFirstActivation() => this.Log(nameof(this.OnLifecycleFirstActivation));

		protected override void OnLifecycleDeactivate() => this.Log(nameof(this.OnLifecycleDeactivate));
		protected override void OnLifecycleUnsubscribe() => this.Log(nameof(this.OnLifecycleUnsubscribe));
		protected override void OnLifecycleFinalizeDisable() => this.Log(nameof(this.OnLifecycleFinalizeDisable));

		protected override void OnLifecycleTeardown() => this.Log(nameof(this.OnLifecycleTeardown));
		protected override void OnLifecycleUnregister() => this.Log(nameof(this.OnLifecycleUnregister));
		protected override void OnLifecycleDispose() => this.Log(nameof(this.OnLifecycleDispose));

		private static string GetHierarchyPath(Transform t)
		{
			if (t == null)
			{
				return "<null>";
			}

			var path = t.name;
			while (t.parent != null)
			{
				t = t.parent;
				path = $"{t.name}/{path}";
			}
			return path;
		}
	}
}
