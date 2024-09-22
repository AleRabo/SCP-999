using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;
using SCP999.Interfaces;
using UnityEngine;

namespace SCP999.Abilities;

public class Heal : IAbility
{
    public override ItemType Item { get; } = ItemType.SCP207;
    public override float Cooldown { get; } = 20;
    public override float Duration { get; } = 10;
    public float Amount { get; } = 40;
    private float corourineCooldown { get; } = 0.25f;
    public override string Name { get; } = "Heal";
    public override string Description { get; } = "Restores health to players within a radius";

    protected override bool Invoke(UsingItemEventArgs ev)
    {
        int count = 0;
        foreach (Player victim in Player.List)
        {
            if (Vector3.Distance(ev.Player.Position, victim.Position) < Plugin.Singleton.Config.MaxDistance && victim != ev.Player)
            {
                count++;

                //ev.Player.Broadcast(5, PlayerBroadcast);
                //victim.ShowHint(VictimBroadcast, 5);

                Timing.RunCoroutine(HealCoroutine(ev.Player));
            }
        }

        if (count == 0)
        {
            return false;
        }

        return true;
    }

    public IEnumerator<float> HealCoroutine(Player scp)
    {
        for (int i = 0; i <= Amount;  i++)
        {
            if (scp == null)
            {
                break;
            }

            foreach (Player victim in Player.List)
            {
                if (Vector3.Distance(scp.Position, victim.Position) > Plugin.Singleton.Config.MaxDistance || victim == scp)
                {
                    continue;
                }

                victim.EnableEffect(EffectType.Stained, corourineCooldown, true);
                victim.Heal(1);
            }
            yield return Timing.WaitForSeconds(corourineCooldown);
        }
    }
}