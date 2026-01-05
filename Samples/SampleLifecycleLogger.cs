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

		protected override void OnLifeCycleCompose()
		{
			Debug.Log($"Running {nameof(this.OnLifeCycleCompose)} from {this.gameObject.name}");
		}

		protected override void OnLifeCycleVerifyConfiguration()
		{
			Debug.Log($"Running {nameof(this.OnLifeCycleVerifyConfiguration)} from {this.gameObject.name}");
		}

		protected override void OnLifeCycleRegisterForDiscovery()
		{
			Debug.Log($"Running {nameof(this.OnLifeCycleRegisterForDiscovery)} from {this.gameObject.name}");
		}

		protected override void OnLifeCycleWireEndpoints()
		{
			Debug.Log($"Running {nameof(this.OnLifeCycleWireEndpoints)} from {this.gameObject.name}");
		}

		protected override void OnLifeCycleSubscribeToExternalEvents()
		{
			Debug.Log($"Running {nameof(this.OnLifeCycleSubscribeToExternalEvents)} from {this.gameObject.name}");
		}

		protected override void OnLifeCycleInitialize()
		{
			Debug.Log($"Running {nameof(this.OnLifeCycleInitialize)} from {this.gameObject.name}");
		}

		protected override void OnLifeCycleActivate()
		{
			Debug.Log($"Running {nameof(this.OnLifeCycleActivate)} from {this.gameObject.name}");
		}

		protected override void OnLifeCycleDeactivate()
		{
			Debug.Log($"Running {nameof(this.OnLifeCycleDeactivate)} from {this.gameObject.name}");
		}

		protected override void OnLifeCycleUnsubscribeFromExternalEvents()
		{
			Debug.Log($"Running {nameof(this.OnLifeCycleUnsubscribeFromExternalEvents)} from {this.gameObject.name}");
		}

		protected override void OnLifeCycleUnwireEndpoints()
		{
			Debug.Log($"Running {nameof(this.OnLifeCycleUnwireEndpoints)} from {this.gameObject.name}");
		}

		protected override void OnLifeCycleUnregisterFromDiscovery()
		{
			Debug.Log($"Running {nameof(this.OnLifeCycleUnregisterFromDiscovery)} from {this.gameObject.name}");
		}

		protected override void OnLifeCycleDispose()
		{
			Debug.Log($"Running {nameof(this.OnLifeCycleDispose)} from {this.gameObject.name}");
		}
	}
}
