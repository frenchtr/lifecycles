namespace TravisRFrench.Lifecycles.Runtime
{
    public interface IHasManagedLifeCycle
    {
        bool IsAlive { get; }
        bool IsEnabled { get; }

        LifecyclePhase Phase { get; }

        // Setup
        void OnLifeCycleCompose();
        void OnLifeCycleVerifyConfiguration();
        void OnLifeCycleRegisterForDiscovery();
        void OnLifeCycleWireEndpoints();
        void OnLifeCycleInitialize();
        void OnLifeCycleSubscribeToExternalEvents();
        void OnLifeCycleActivate();

        // Teardown
        void OnLifeCycleDeactivate();
        void OnLifeCycleUnsubscribeFromExternalEvents();
        void OnLifeCycleUnwireEndpoints();
        void OnLifeCycleUnregisterFromDiscovery();
        void OnLifeCycleDispose();
    }
}
