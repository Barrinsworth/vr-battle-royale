using System;
using Improbable.Gdk.Core;
using Improbable.Gdk.PlayerLifecycle;
using Improbable.Gdk.GameObjectCreation;
using Improbable.Worker.CInterop;
using UnityEngine;

namespace VRBattleRoyale.Multiplayer
{
    public class UnityClientConnector : WorkerConnector
    {
        public const string WorkerType = "UnityClient";

        [SerializeField] private GameObject level;

        private GameObject levelInstance;
        private AdvancedEntityPipeline entityPipeline;

        public event Action OnLostPlayerEntity;

        #region Unity Life Cycle
        private async void Start()
        {
            var connParams = CreateConnectionParameters(WorkerType);
            connParams.Network.ConnectionType = NetworkConnectionType.Kcp;

            var builder = new SpatialOSConnectionHandlerBuilder().SetConnectionParameters(connParams);

            if (!Application.isEditor)
            {
                var initializer = new CommandLineConnectionFlowInitializer();
                switch (initializer.GetConnectionService())
                {
                    case ConnectionService.Receptionist:
                        builder.SetConnectionFlow(new ReceptionistFlow(CreateNewWorkerId(WorkerType), initializer));
                        break;
                    case ConnectionService.Locator:
                        builder.SetConnectionFlow(new LocatorFlow(initializer));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            else
            {
                builder.SetConnectionFlow(new ReceptionistFlow(CreateNewWorkerId(WorkerType)));
            }

            await Connect(builder, new ForwardingDispatcher()).ConfigureAwait(false);
        }
        #endregion

        #region Overrides
        protected override void HandleWorkerConnectionEstablished()
        {
            PlayerLifecycleHelper.AddClientSystems(Worker.World);
            PlayerLifecycleConfig.MaxPlayerCreationRetries = 0;

            entityPipeline = new AdvancedEntityPipeline(Worker, GetAuthPlayerPrefabPath(), GetNonAuthPlayerPrefabPath());
            entityPipeline.OnRemovedAuthoritativePlayer += RemovingAuthoritativePlayer;

            GameObjectCreationHelper.EnableStandardGameObjectCreation(Worker.World, entityPipeline, gameObject);

            if (level != null)
            {
                levelInstance = Instantiate(level, transform.position, transform.rotation);
            }
        }

        public override void Dispose()
        {
            if (levelInstance != null)
            {
                Destroy(levelInstance);
            }

            base.Dispose();
        }
        #endregion

        protected virtual string GetAuthPlayerPrefabPath()
        {
            return "Prefabs/UnityClient/Authoritative/Player";
        }

        protected virtual string GetNonAuthPlayerPrefabPath()
        {
            return "Prefabs/UnityClient/NonAuthoritative/Player";
        }

        private void RemovingAuthoritativePlayer()
        {
            Debug.LogError($"Player entity got removed while still being connected. Disconnecting...");
            OnLostPlayerEntity?.Invoke();
        }
    }
}
