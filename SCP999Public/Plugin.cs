using Exiled.API.Features;
using System;
using PlayerHandlers = Exiled.Events.Handlers.Player;
using Exiled.CustomRoles.API;
using HarmonyLib;

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
        internal Harmony harmony;

        /// <summary>
        /// The event handlers <see cref="SCP999.EventHandlers"/> class.
        /// </summary>
        private EventHandlers EventHandlers;

        public override void OnEnabled()
        {
            Singleton = this;
            EventHandlers = new EventHandlers();

            Config.Scp999RoleConfig.Register();

            harmony = new Harmony($"SCP999 - {DateTime.Now}");
            harmony.PatchAll();

            PlayerHandlers.Spawned += EventHandlers.Spawned;
            PlayerHandlers.UsingItem += EventHandlers.OnUsingItem;
            PlayerHandlers.ChangingRole += EventHandlers.RoleChanged;
            PlayerHandlers.SearchingPickup += EventHandlers.PickingUpItem;
            PlayerHandlers.DroppingItem += EventHandlers.Dropping;
            PlayerHandlers.EnteringPocketDimension += EventHandlers.EnterPocket;
            PlayerHandlers.FailingEscapePocketDimension += EventHandlers.ExitPocket;
            PlayerHandlers.Hurting += EventHandlers.Hurting;

            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            PlayerHandlers.Spawned -= EventHandlers.Spawned;
            PlayerHandlers.UsingItem -= EventHandlers.OnUsingItem;
            PlayerHandlers.ChangingRole -= EventHandlers.RoleChanged;
            PlayerHandlers.DroppingItem -= EventHandlers.Dropping;
            PlayerHandlers.EnteringPocketDimension -= EventHandlers.EnterPocket;
            PlayerHandlers.FailingEscapePocketDimension -= EventHandlers.ExitPocket;
            PlayerHandlers.Hurting -= EventHandlers.Hurting;

            EventHandlers = null;
            Config.Scp999RoleConfig.Unregister();
            harmony.UnpatchAll();

            Singleton = null;
            base.OnDisabled();
        }
    }
}