using System.Collections.Generic;
using Exiled.API.Features;
using Exiled.CustomRoles.API;
using Exiled.CustomRoles.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;

namespace SCP999.Interfaces;
public abstract class Ability
{
    public static List<Ability> Abilities { get; protected set; } = new List<Ability>();

    public abstract string Name { get; }
    public abstract string Description { get;  }

    public abstract ItemType Item { get; }

    public abstract float Cooldown { get; }
    public abstract float Duration { get; }

    protected abstract string PlayerBroadcast { get; }
    protected abstract string VictimBroadcast { get; }
    protected abstract string ErrorBroadcast { get; }
    
    public Ability()
    {
        Exiled.Events.Handlers.Player.UsingItem += OnUse;
        Exiled.Events.Handlers.Player.ChangedItem += OnEquip;

        Abilities.Add(this);
    }

    public virtual void Disable()
    {
        Exiled.Events.Handlers.Player.UsingItem -= OnUse;
        Exiled.Events.Handlers.Player.ChangedItem -= OnEquip;
    }

    public static void DisableAll()
    {
        foreach (Ability ability in Abilities)
        {
            ability.Disable();
        }

        Abilities = null;
    }

    private bool Check(Player player, ItemType item)
    {
        var customRole = CustomRole.Get(typeof(Scp999Role)) as Scp999Role;
        if (player.GetCustomRoles().Contains(customRole) && item == Item)
        {
            return true;
        }
        return false;
    }

    protected virtual void OnEquip(ChangedItemEventArgs ev)
    {
        if (ev.Item != null && Check(ev.Player, ev.Item.Type))
        {
            ev.Player.ShowHint($"Ability {Name} \n {Description}");
        }
    }

    private void OnUse(UsingItemEventArgs ev)
    {
        if (Check(ev.Player, ev.Item.Type))
        {
            if (Invoke(ev))
            {
                ev.Player.RemoveItem(ev.Item);

                Timing.CallDelayed(Cooldown, () =>
                {
                    var customRole = CustomRole.Get(typeof(Scp999Role)) as Scp999Role;
                    if (ev.Player.GetCustomRoles().Contains(customRole))
                    {
                        ev.Player.AddItem(Item);
                        ev.Player.ShowHint($"Ability {Name} reloaded");
                    }
                });
                return;
            }
            ev.Player.Broadcast(5, ErrorBroadcast);
            ev.IsAllowed = false;
        }
    }

    protected abstract bool Invoke(UsingItemEventArgs ev);
}