using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.CustomRoles.API.Features;
using Exiled.Events.EventArgs.Player;
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

        foreach (Type type in Assembly.GetExecutingAssembly().GetTypes()) // Use GetExecutingAssembly
        {
            try
            {
                // Check if the type implements IAbility
                if (typeof(IAbility).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
                {
                    Log.Info("Before declaring activator");
                    var activator = Activator.CreateInstance(type) as IAbility;
                    if (activator != null) // Check if the activator is successfully created
                    {
                        Log.Info(activator.GetType().Name);
                        _abilityList.Add(activator);
                    }
                    else
                    {
                        Log.Warn($"Activator for type {type} returned null.");
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Error creating instance of {type}: {ex}");
            }
        }

        Log.Info($"Ability list count after initialization: {_abilityList.Count}");
    }

    /// <summary>
    /// Using an item
    /// </summary>
    public void OnUsingItem(UsingItemEventArgs ev)
    {
        if (CustomRole.Get(typeof(Scp999Role)).Check(ev.Player))
        {
            ev.IsAllowed = false;

            Log.Info("Before sending ability event");
            var ability = _abilityList.FirstOrDefault(r => r.ItemType == ev.Item.Type);
            if (ability != null)
            {
                ability.Invoke(ev);
                Log.Info($"Invoked ability: {ability.GetType().Name}");
            }
            else
            {
                Log.Warn("No matching ability found for item type: " + ev.Item.Type);
            }
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
            case DoorType.HczArmory:
            case DoorType.Scp106Primary:
            case DoorType.Scp330:
            case DoorType.Scp096:
            case DoorType.Scp914Gate:
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