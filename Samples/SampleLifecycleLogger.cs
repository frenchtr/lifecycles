using TravisRFrench.Lifecycles.Runtime;
using UnityEngine;

namespace TravisRFrench.Lifecycles.lifecycles.Samples
{
	public class SampleLifecycleLogger : ManagedMonoBehaviour
	{
		[ContextMenu(nameof(DestroySelf))]
		private void DestroySelf()
		{
			Destroy(this);
		}
		
		protected override void OnCompose()
		{
			Debug.Log($"Running {nameof(this.OnCompose)} from {this.gameObject.name}");
		}

		protected override void OnRegister()
		{
			Debug.Log($"Running {nameof(this.OnRegister)} from {this.gameObject.name}");
		}

		protected override void OnInitialize()
		{
			Debug.Log($"Running {nameof(this.OnInitialize)} from {this.gameObject.name}");
		}

		protected override void OnBind()
		{
			Debug.Log($"Running {nameof(this.OnBind)} from {this.gameObject.name}");
		}

		protected override void OnActivate()
		{
			Debug.Log($"Running {nameof(this.OnActivate)} from {this.gameObject.name}");
		}

		protected override void OnDeactivate()
		{
			Debug.Log($"Running {nameof(this.OnDeactivate)} from {this.gameObject.name}");
		}

		protected override void OnUnbind()
		{
			Debug.Log($"Running {nameof(this.OnUnbind)} from {this.gameObject.name}");
		}

		protected override void OnUnregister()
		{
			Debug.Log($"Running {nameof(this.OnUnregister)} from {this.gameObject.name}");
		}

		protected override void OnDispose()
		{
			Debug.Log($"Running {nameof(this.OnDispose)} from {this.gameObject.name}");
		}
	}
}
