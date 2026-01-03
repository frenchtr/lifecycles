using System;
using UnityEngine;

namespace TravisRFrench.Lifecycles.Runtime
{
	[DefaultExecutionOrder(-999999)]
	public class LifeCycleManager : MonoBehaviour
	{
		private static LifeCycleManager singleton;
		
		private void EnsureServiceWasProvided()
		{
			if (Lifecycle.Service == null)
			{
				Debug.LogError($"The lifecycle service was not provided. (Did you call {nameof(Lifecycle.ProvideService)}?)");
				throw new InvalidOperationException();
			}
		}
		
		private void Awake()
		{
			if (singleton == null)
			{
				singleton = this;
			}

			if (singleton != this)
			{
				Destroy(this.gameObject);
				return;
			}

			this.EnsureServiceWasProvided();
			Lifecycle.Service.AwakenAll();
		}

		private void OnEnable()
		{
			if (singleton != this)
			{
				return;
			}
			
			this.EnsureServiceWasProvided();
			Lifecycle.Service.EnableAll();
		}

		private void OnDisable()
		{
			if (singleton != this)
			{
				return;
			}

			this.EnsureServiceWasProvided();
			Lifecycle.Service.DisableAll();
		}

		private void OnDestroy()
		{
			if (singleton != this)
			{
				return;
			}

			this.EnsureServiceWasProvided();
			Lifecycle.Service.DestroyAll();
		}

		private void LateUpdate()
		{
			this.EnsureServiceWasProvided();
			Lifecycle.Service.Tick();
		}
	}
}
