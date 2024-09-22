using System;
using HarmonyLib;
using Exiled.CustomRoles.API;
using Exiled.API.Features;

namespace SCP999;
public class Plugin : Plugin<Config, Translation>
{
    public override string Name => "SCP999";
    public override string Author => "AleRabo && RisottoMan";
    public override Version Version => new(1, 0, 0);
    public override Version RequiredExiledVersion => new(8, 11, 0);
    
    private Harmony _harmony;
    private ServerHandler _serverHandler;
    private PlayerHandler _playerHandler;
    public static Plugin Singleton;

    public override void OnEnabled()
    {
        Singleton = this;
        _serverHandler = new ServerHandler();
        _playerHandler = new PlayerHandler();

        Config.Scp999RoleConfig.Register();

        _harmony = new Harmony($"SCP999 - {DateTime.Now}");
        _harmony.PatchAll();

        Exiled.Events.Handlers.Warhead.Stopping += _serverHandler.OnWarheadStop;
        Exiled.Events.Handlers.Scp096.Enraging += _serverHandler.OnScpEnraging;
        Exiled.Events.Handlers.Scp096.AddingTarget += _serverHandler.OnAddingTarget;
        Exiled.Events.Handlers.Player.SpawningRagdoll += _serverHandler.OnSpawningRagdoll;
        Exiled.Events.Handlers.Player.EnteringPocketDimension += _serverHandler.OnEnteringPocketDimension;
        Exiled.Events.Handlers.Server.RoundStarted += _playerHandler.OnRoundStarted;
        Exiled.Events.Handlers.Player.SearchingPickup += _playerHandler.OnSeachingPickup;
        Exiled.Events.Handlers.Player.DroppingItem += _playerHandler.OnDroppingItem;
        Exiled.Events.Handlers.Player.Hurting += _playerHandler.OnPlayerHurting;
        Exiled.Events.Handlers.Player.UsingItem += _playerHandler.OnUsingItem;
        Exiled.Events.Handlers.Player.UsingItem += _playerHandler.OnUsingItem;
        Exiled.Events.Handlers.Player.InteractingDoor += _playerHandler.OnInteractingDoor;
        Exiled.Events.Handlers.Player.Dying += _playerHandler.OnPlayerDying;
        Exiled.Events.Handlers.Player.Left += _playerHandler.OnPlayerLeft;
        Exiled.Events.Handlers.Player.Spawning += _playerHandler.OnPlayerSpawning;
        Exiled.Events.Handlers.Player.ChangingRole += _playerHandler.OnChangingRole;
        
        base.OnEnabled();
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Warhead.Stopping -= _serverHandler.OnWarheadStop;
        Exiled.Events.Handlers.Scp096.Enraging -= _serverHandler.OnScpEnraging;
        Exiled.Events.Handlers.Scp096.AddingTarget -= _serverHandler.OnAddingTarget;
        Exiled.Events.Handlers.Player.SpawningRagdoll -= _serverHandler.OnSpawningRagdoll;
        Exiled.Events.Handlers.Player.EnteringPocketDimension -= _serverHandler.OnEnteringPocketDimension;
        Exiled.Events.Handlers.Server.RoundStarted -= _playerHandler.OnRoundStarted;
        Exiled.Events.Handlers.Player.SearchingPickup -= _playerHandler.OnSeachingPickup;
        Exiled.Events.Handlers.Player.DroppingItem -= _playerHandler.OnDroppingItem;
        Exiled.Events.Handlers.Player.Hurting -= _playerHandler.OnPlayerHurting;
        Exiled.Events.Handlers.Player.UsingItem -= _playerHandler.OnUsingItem;
        Exiled.Events.Handlers.Player.UsingItem -= _playerHandler.OnUsingItem;
        Exiled.Events.Handlers.Player.InteractingDoor -= _playerHandler.OnInteractingDoor;
        Exiled.Events.Handlers.Player.Dying -= _playerHandler.OnPlayerDying;
        Exiled.Events.Handlers.Player.Left -= _playerHandler.OnPlayerLeft;
        Exiled.Events.Handlers.Player.Spawning -= _playerHandler.OnPlayerSpawning;
        Exiled.Events.Handlers.Player.ChangingRole -= _playerHandler.OnChangingRole;
        
        Config.Scp999RoleConfig.Unregister();
        _harmony.UnpatchAll();

        _playerHandler = null;
        _serverHandler = null;
        Singleton = null;
        base.OnDisabled();
    }
}