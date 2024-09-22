using System;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.CustomRoles.API.Features;
using Exiled.Events.EventArgs.Player;
using PluginAPI.Events;
using SCP999.Abilities;

namespace SCP999;
public class PlayerHandler
{
    /// <summary>
    /// The logic of choosing SCP-999 if the round is started
    /// </summary>
    public void OnRoundStarted()
    {
        Scp999Role customRole = CustomRole.Get(typeof(Scp999Role)) as Scp999Role;
        
        foreach (Player player in Player.List)
        {
            // If there is no SCP-999 in the game, then add
            if (customRole.TrackedPlayers.Count >= customRole.SpawnProperties.Limit)
                return;
            
            // The player already has a role
            if (customRole.Check(player))
                return;

            // The player is an NPC
            if (player.IsNPC && player.Nickname != "SCP-999")
                return;
            
            // Checking the chance to spawn
            if (UnityEngine.Random.Range(0, 100) > customRole.SpawnChance)
                return;
            
            customRole.AddRole(player);
        }
    }
    
    /// <summary>
    /// Allow the use of abilities for SCP-999
    /// </summary>
    public void OnUsingItem(UsingItemEventArgs ev)
    {
        if (CustomRole.Get(typeof(Scp999Role)).Check(ev.Player))
        {
            // todo something
        }
    }
    
    /// <summary>
    /// The mechanics of opening doors - todo
    /// </summary>
    public void OnInteractingDoor(InteractingDoorEventArgs ev)
    {
        if (!CustomRole.Get(typeof(Scp999Role)).Check(ev.Player))
            return;
        
        switch (ev.Door.Type)
        {
            case DoorType.GateA:
            case DoorType.GateB:
            case DoorType.LczArmory:
            case DoorType.Scp049Armory:
            case DoorType.HID:
            { break; }
            default:
            {
                ev.IsAllowed = true;
                break;
            }
        }
    }
    
    /// <summary>
    /// Allow the attacker to show the hitmarker
    /// </summary>
    public void OnPlayerHurting(HurtingEventArgs ev)
    {
        if (CustomRole.Get(typeof(Scp999Role)).Check(ev.Player))
        {
            if (ev.Amount < CustomRole.Get(typeof(Scp999Role)).MaxHealth)
            {
                ev.IsAllowed = false;
                ev.Attacker?.ShowHitMarker();
                ev.Player.Health -= 100;
            }
        }
    }
    
    /// <summary>
    /// Does not allow SCP-999 to pick up items
    /// </summary>
    public void OnSeachingPickup(SearchingPickupEventArgs ev)
    {
        if (CustomRole.Get(typeof(Scp999Role)).Check(ev.Player))
        {
            ev.IsAllowed = false;
        }
    }

    /// <summary>
    /// Does not allow SCP-999 to drop items
    /// </summary>
    public void OnDroppingItem(DroppingItemEventArgs ev)
    {
        if (CustomRole.Get(typeof(Scp999Role)).Check(ev.Player))
        {
            ev.IsAllowed = false;
        }
    }
    
    /// <summary>
    /// Clearing the inventory if the SCP-999 dies
    /// </summary>
    public void OnPlayerDying(DyingEventArgs ev)
    {
        if (CustomRole.Get(typeof(Scp999Role)).Check(ev.Player))
        {
            ev.Player.ClearInventory();
        }
    }
    
    /// <summary>
    /// If the player leaves the game, then delete his instance
    /// </summary>
    public void OnPlayerLeft(LeftEventArgs ev)
    {
        if (CustomRole.Get(typeof(Scp999Role)).Check(ev.Player))
        {
            CustomRole.Get(typeof(Scp999Role)).RemoveRole(ev.Player);
        }
    }
    
    /// <summary>
    /// If a player has spawned with a new role, then delete his instance
    /// </summary>
    public void OnPlayerSpawning(SpawningEventArgs ev)
    {
        if (CustomRole.Get(typeof(Scp999Role)).Check(ev.Player))
        {
            CustomRole.Get(typeof(Scp999Role)).RemoveRole(ev.Player);
        }
    }
    
    /// <summary>
    /// If a player has changed his class, then delete his instance
    /// </summary>
    public void OnChangingRole(ChangingRoleEventArgs ev)
    {
        if (CustomRole.Get(typeof(Scp999Role)).Check(ev.Player))
        {
            CustomRole.Get(typeof(Scp999Role)).RemoveRole(ev.Player);
        }
    }
}