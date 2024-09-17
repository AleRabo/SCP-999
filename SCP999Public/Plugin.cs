using Exiled.API.Features;
using Exiled.CustomRoles.API.Features;
using System;
using PlayerHandlers = Exiled.Events.Handlers.Player;
using Exiled.CustomRoles.API;

namespace SCP999
{
    public class Plugin : Plugin<Config, Translation>
    {

        public static Plugin Singleton;

        // Plugin properties
        public override string Name => "SCP999";
        public override string Prefix => "SCP999";
        public override string Author => "AleRabo";
        public override Version Version => new Version(1, 0, 0);
        public override Version RequiredExiledVersion => new Version(8, 11, 0);

        /// <summary>
        /// The event handlers <see cref="SCP999.EventHandlers"/> class.
        /// </summary>
        private EventHandlers EventHandlers;

        public override void OnEnabled()
        {
            Singleton = this;
            EventHandlers = new EventHandlers();

            Config.Scp999RoleConfig.Register();

            PlayerHandlers.Spawned += EventHandlers.Spawned;
            PlayerHandlers.UsingItem += EventHandlers.OnUsingItem;
            PlayerHandlers.ChangingRole += EventHandlers.RoleChanged;
            PlayerHandlers.SearchingPickup += EventHandlers.PickingUpItem;
            PlayerHandlers.DroppingItem += EventHandlers.Dropping;

            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            PlayerHandlers.Spawned -= EventHandlers.Spawned;
            PlayerHandlers.UsingItem -= EventHandlers.OnUsingItem;
            PlayerHandlers.ChangingRole -= EventHandlers.RoleChanged;
            PlayerHandlers.DroppingItem -= EventHandlers.Dropping;

            EventHandlers = null;
            Config.Scp999RoleConfig.Unregister();

            Singleton = null;
            base.OnDisabled();
        }
    }
}