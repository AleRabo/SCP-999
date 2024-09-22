using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.CreditTags.Features;
using Exiled.Events.EventArgs.Player;
using MEC;
using SCP999.Interfaces;
using UnityEngine;

namespace SCP999.Abilities;
public class Heal : IAbility
{
    public string Name { get; } = "Heal";
    public string Description { get; } = "Restores health to players within a radius";
    public ItemType ItemType { get; } = ItemType.Medkit;
    private float _timing { get; } = 0.25f;

    private float range { get; } = Plugin.Singleton.Config.MaxDistance;

    // Retrieve ability settings from the config
    private AbilityConfig AbilitySettings => Plugin.Singleton.Config.Abilities.TryGetValue(Name, out var config) ? config : null;

    public void Invoke(UsingItemEventArgs ev)
    {
        // Get the ability settings (range, effect duration) from config
        if (AbilitySettings == null)
        {
            Log.Error("Ability settings not found in config.");
            return;
        }

        byte intensity = AbilitySettings.Intensity;
        bool scpFriendly = AbilitySettings.ScpFriendly;

        int count = 0;
        foreach (Player victim in Player.List)
        {
            if (Vector3.Distance(ev.Player.Position, victim.Position) < range && victim != ev.Player)
            {
                count++;

                ev.Player.Broadcast(5, "Speed boost activated!"); // PlayerBroadcast
                victim.ShowHint("You've been speed boosted!", 5); // VictimBroadcast

                Timing.RunCoroutine(HealCoroutine(ev.Player, intensity, scpFriendly));
            }
        }
    }

    public IEnumerator<float> HealCoroutine(Player scp999, byte intensity, bool scpFriendly)
    {
        for (int i = 0; i <= intensity; i++)
        {
            if (scp999 is null)
            {
                break;
            }

            foreach (Player player in Player.List)
            {
                if (Vector3.Distance(scp999.Position, player.Position) > Plugin.Singleton.Config.MaxDistance || player == scp999)
                {
                    continue;
                }
                if (!player.IsHuman && scpFriendly)
                    continue;
                else
                {
                    player.EnableEffect(EffectType.Stained, _timing, true); // Stain the player
                    player.Heal(1); // Heal
                }
            }
            yield return Timing.WaitForSeconds(_timing);
        }
    }
}