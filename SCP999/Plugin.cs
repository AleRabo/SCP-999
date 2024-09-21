using System;
using HarmonyLib;
using Exiled.CustomRoles.API;
using Exiled.API.Features;
using SCP999.Abilities;
using SCP999.Interfaces;

namespace SCP999;
public class Plugin : Plugin<Config, Translation>
{
    public override string Name => "SCP999";
    public override string Author => "AleRabo && RisottoMan";
    public override Version Version => new(1, 0, 0);
    public override Version RequiredExiledVersion => new(8, 11, 0);
    
    private Harmony _harmony;
    private EventHandler _eventHandler;
    public static Plugin Singleton;

    public override void OnEnabled()
    {
        Singleton = this;
        _eventHandler = new EventHandler();

        Config.Scp999RoleConfig.Register();

        _harmony = new Harmony($"SCP999 - {DateTime.Now}");
        _harmony.PatchAll();

        // Enable abilities - a temporary solution
        Heal heal = new Heal();
        
        Exiled.Events.Handlers.Player.Spawned += _eventHandler.Spawned;
        Exiled.Events.Handlers.Player.UsingItem += _eventHandler.OnUsingItem;
        Exiled.Events.Handlers.Player.ChangingRole += _eventHandler.RoleChanged;
        Exiled.Events.Handlers.Player.SearchingPickup += _eventHandler.PickingUpItem;
        Exiled.Events.Handlers.Player.DroppingItem += _eventHandler.Dropping;
        Exiled.Events.Handlers.Player.EnteringPocketDimension += _eventHandler.EnterPocket;
        Exiled.Events.Handlers.Player.FailingEscapePocketDimension += _eventHandler.ExitPocket;
        Exiled.Events.Handlers.Player.Hurting += _eventHandler.Hurting;
        
        base.OnEnabled();
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Player.Spawned -= _eventHandler.Spawned;
        Exiled.Events.Handlers.Player.UsingItem -= _eventHandler.OnUsingItem;
        Exiled.Events.Handlers.Player.ChangingRole -= _eventHandler.RoleChanged;
        Exiled.Events.Handlers.Player.DroppingItem -= _eventHandler.Dropping;
        Exiled.Events.Handlers.Player.EnteringPocketDimension -= _eventHandler.EnterPocket;
        Exiled.Events.Handlers.Player.FailingEscapePocketDimension -= _eventHandler.ExitPocket;
        Exiled.Events.Handlers.Player.Hurting -= _eventHandler.Hurting;
        
        // Disable abilities - a temporary solution
        Ability.DisableAll();
        
        Config.Scp999RoleConfig.Unregister();
        _eventHandler = null;
        _harmony.UnpatchAll();

        Singleton = null;
        base.OnDisabled();
    }
}