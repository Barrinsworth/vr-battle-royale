using System;
using Improbable.Gdk.Core;
using Improbable.Gdk.Mobile;
using Improbable.Gdk.PlayerLifecycle;
using Improbable.Gdk.GameObjectCreation;
using UnityEngine;

namespace VRBattleRoyale.Multiplayer
{
    public class MobileClientWorkerConnector : UnityClientConnector, MobileConnectionFlowInitializer.IMobileSettingsProvider
    {
#pragma warning disable 649
        [SerializeField] private string ipAddress;
#pragma warning restore 649

        public new const string WorkerType = "MobileClient";

        #region Unity Life Cycle
        private async void Start()
        {
            var connParams = CreateConnectionParameters(WorkerType, new MobileConnectionParametersInitializer());

            var flowInitializer = new MobileConnectionFlowInitializer(
                new MobileConnectionFlowInitializer.CommandLineSettingsProvider(),
                new MobileConnectionFlowInitializer.PlayerPrefsSettingsProvider(),
                this);

            var builder = new SpatialOSConnectionHandlerBuilder()
                .SetConnectionParameters(connParams);

            switch (flowInitializer.GetConnectionService())
            {
                case ConnectionService.Receptionist:
                    builder.SetConnectionFlow(new ReceptionistFlow(CreateNewWorkerId(WorkerType),
                        flowInitializer));
                    break;
                case ConnectionService.Locator:
                    builder.SetConnectionFlow(new LocatorFlow(flowInitializer));
                    break;
                default:
                    throw new ArgumentException("Received unsupported connection service.");
            }

            await Connect(builder, new ForwardingDispatcher()).ConfigureAwait(false);
        }
        #endregion

        #region Overrides
        protected override string GetAuthPlayerPrefabPath()
        {
            return "Prefabs/MobileClient/Authoritative/Player";
        }

        protected override string GetNonAuthPlayerPrefabPath()
        {
            return "Prefabs/MobileClient/NonAuthoritative/Player";
        }
        #endregion

        public Option<string> GetReceptionistHostIp()
        {
            return string.IsNullOrEmpty(ipAddress) ? Option<string>.Empty : new Option<string>(ipAddress);
        }

        public Option<string> GetDevAuthToken()
        {
            var token = Resources.Load<TextAsset>("DevAuthToken")?.text.Trim();
            return token ?? Option<string>.Empty;
        }

        public Option<ConnectionService> GetConnectionService()
        {
            return Option<ConnectionService>.Empty;
        }
    }
}