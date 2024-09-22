using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;
using SCP999.Interfaces;
using UnityEngine;

namespace SCP999.Abilities;
public class SpeedBoost : IAbility
{
    public string Name { get; } = "SpeedBoost";
    public string Description { get; } = "Boost the speed of the players within a radius";
    public ItemType ItemType { get; } = ItemType.SCP207;

    float range = Plugin.Singleton.Config.MaxDistance;

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
        float effectDuration = AbilitySettings.EffectDuration;
        bool scpFriendly = AbilitySettings.ScpFriendly;

        int count = 0;
        foreach (Player victim in Player.List)
        {
            if (Vector3.Distance(ev.Player.Position, victim.Position) < range && victim != ev.Player)
            {
                count++;

                ev.Player.Broadcast(5, "Speed boost activated!"); // PlayerBroadcast
                victim.ShowHint("You've been speed boosted!", 5); // VictimBroadcast

                Timing.RunCoroutine(BoostCoroutine(ev.Player, effectDuration, intensity, scpFriendly));
            }
        }
    }

    public IEnumerator<float> BoostCoroutine(Player scp999, float effectDuration, byte intensity, bool scpFriendly)
    {
            if (scp999 is null)
                yield break;
            
            foreach (Player player in Player.List)
            {
                if (Vector3.Distance(scp999.Position, player.Position) > range || player == scp999)
                    continue;

                if (!player.IsHuman && scpFriendly)
                {
                    player.EnableEffect(EffectType.Slowness, intensity, effectDuration); // Slows SCPs if configured
                }
                else
                {
                    player.EnableEffect(EffectType.MovementBoost, intensity, effectDuration); // Speed boost
                    player.EnableEffect(EffectType.Invigorated, intensity, effectDuration); // Additional boost
                }
            }

            yield return Timing.WaitForSeconds(effectDuration);
    }
}
