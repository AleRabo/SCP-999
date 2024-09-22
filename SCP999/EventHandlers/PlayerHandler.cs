using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Exiled.API.Enums;
using Exiled.CustomRoles.API.Features;
using Exiled.Events.EventArgs.Player;
using PluginAPI.Core;
using SCP999.Interfaces;

namespace SCP999;
public class PlayerHandler
{
    private List<IAbility> _abilityList;
    
    /// <summary>
    /// Activate all ability classes and save to the list
    /// </summary>
    public void OnWaitingRound()
    {
        _abilityList = new List<IAbility>();
        
        foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
        {
            try
            {
                if (type.IsInterface || !type.GetInterfaces().Contains(typeof(IAbility)))
                    continue;

                var activator = Activator.CreateInstance(type) as IAbility;
                if (activator != null)
                {
                    _abilityList.Add(activator);
                }
            }
            catch (Exception ex)
            {
                Log.Error("OnWaitingRound error in IAbility:" + ex.Message);
            }
        }
    }
    
    /// <summary>
    /// Allow the use of abilities for SCP-999
    /// </summary>
    public void OnUsingItem(UsingItemEventArgs ev)
    {
        if (CustomRole.Get(typeof(Scp999Role)).Check(ev.Player))
        {
            ev.IsAllowed = false;
            
            _abilityList.FirstOrDefault(r => r.ItemType == ev.Item.Type)?.Invoke(ev);
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