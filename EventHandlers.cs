using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.CustomRoles.API;
using Exiled.CustomRoles.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;
using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static SCP999.Scp999Role;

namespace SCP999
{
    public class EventHandlers
    {
        private static class AbilityMapping
        {
            public static readonly Dictionary<ItemType, string> ItemToAbilityMap = new Dictionary<ItemType, string>
            {
                { ItemType.Adrenaline, "Invigorate" },
                { ItemType.Medkit, "Heal" },
                { ItemType.SCP207, "SpeedBoost" }
            };

            public static string GetAbilityForItem(ItemType itemType) => ItemToAbilityMap.TryGetValue(itemType, out var abilityName) ? abilityName : null;
        }

        private DateTime lastAbilityUse = DateTime.MinValue;
        private readonly TimeSpan cooldown = TimeSpan.FromSeconds(2);
        private CoroutineHandle cooldownHintCoroutine;

        public void RoleChanged(ChangingRoleEventArgs ev)
        {
            var customRole = CustomRole.Get(typeof(Scp999Role)) as Scp999Role;
            if (customRole != null && ev.Player.GetCustomRoles().Contains(customRole))
            {
                customRole.RemoveRole(ev.Player);
            }
        }
        public void PickingUpItem(SearchingPickupEventArgs ev)
        {
            // Check if the player has the custom role (ID 999)
            if (ev.Player.GetCustomRoles().Contains(CustomRole.Get(999)))
                ev.IsAllowed = false;
        }

        public void Dropping(DroppingItemEventArgs ev)
        {
            if (ev.Player.GetCustomRoles().Contains(CustomRole.Get(999)))
                ev.IsAllowed = false;
        }

        public void Spawned(SpawnedEventArgs ev)
        {
            try
            {
                if (ev.Player == null)
                {
                    Log.Error("Spawned: ev.Player is null.");
                    return;
                }

                var customRole = CustomRole.Get(typeof(Scp999Role)) as Scp999Role;
                if (customRole == null)
                {
                    Log.Error("Spawned: Scp999Role not found.");
                    return;
                }

                if (UnityEngine.Random.Range(0, 100) <= customRole.SpawnChance && !ev.Player.IsNPC && ev.Player.Nickname != "SCP-999" && customRole.TrackedPlayers.Count < customRole.SpawnProperties.Limit)
                {
                    customRole.AddRole(ev.Player);
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Error in Spawned: {ex}");
            }
        }

        public void OnUsingItem(UsingItemEventArgs ev)
        {
            // Check if the player has the custom role (ID 999)
            if (ev.Player.GetCustomRoles().Contains(CustomRole.Get(999)))
            {
                // Check if the ability is on cooldown
                if (DateTime.Now < lastAbilityUse + cooldown)
                {
                    double remainingCooldown = (lastAbilityUse + cooldown - DateTime.Now).TotalSeconds;
                    ev.IsAllowed = false; // Prevent ability usage during cooldown
                    return; // Exit early since the cooldown hasn't expired
                }

                ev.IsAllowed = false;

                // Use the AbilityMapping class to get the ability for the item
                string abilityName = AbilityMapping.GetAbilityForItem(ev.Item.Type);

                // If the ability exists in the mapping, and in the config
                if (abilityName != null && Plugin.Singleton.Config.Abilities.TryGetValue(abilityName, out AbilityConfig ability))
                {
                    float range = ability.Range;
                    float effectDuration = ability.EffectDuration;

                    animator.Play("Pressure");

                    // Perform ability logic based on the item type
                    switch (ev.Item.Type)
                    {
                        case ItemType.Adrenaline:
                            // Apply AOE invigorated effect to players within range
                            foreach (var otherPlayer in Player.List)
                            {
                                if (otherPlayer == ev.Player) continue;

                                if (Vector3.Distance(ev.Player.Position, otherPlayer.Position) <= range)
                                {
                                    otherPlayer.EnableEffect(EffectType.Invigorated, effectDuration);
                                }
                            }
                            break;

                        case ItemType.Medkit:
                            // Apply healing logic
                            foreach (var otherPlayer in Player.List)
                            {
                                if (otherPlayer == ev.Player) continue;

                                if (Vector3.Distance(ev.Player.Position, otherPlayer.Position) <= range)
                                {
                                    otherPlayer.Heal(effectDuration);
                                }
                            }
                            break;

                        case ItemType.SCP207:
                            // Apply speed logic
                            foreach (var otherPlayer in Player.List)
                            {
                                if (otherPlayer == ev.Player) continue;

                                if (Vector3.Distance(ev.Player.Position, otherPlayer.Position) <= range)
                                {
                                    if (ev.Player.Role.Team == Team.SCPs && Plugin.Singleton.Config.SpeedSlowsSCPs)
                                        otherPlayer.EnableEffect(EffectType.Slowness, 50, effectDuration);
                                    else
                                        otherPlayer.EnableEffect(EffectType.MovementBoost, 50, effectDuration);
                                }
                            }
                            break;
                    }

                    // Update the last ability use time
                    lastAbilityUse = DateTime.Now;

                    // Start the cooldown hint coroutine to show the countdown
                    if (cooldownHintCoroutine.IsRunning)
                        Timing.KillCoroutines(cooldownHintCoroutine); // Stop any existing coroutine
                    cooldownHintCoroutine = Timing.RunCoroutine(ShowCooldownHint(ev.Player, cooldown));
                }
            }
        }

        private void ExecuteAbility(Player player, ItemType itemType, AbilityConfig ability)
        {
            var range = ability.Range;
            var effectDuration = ability.EffectDuration;
            var nearbyPlayers = Player.List.Where(p => p != player && Vector3.Distance(player.Position, p.Position) <= range);

            foreach (var otherPlayer in nearbyPlayers)
            {
                switch (itemType)
                {
                    case ItemType.Adrenaline:
                        otherPlayer.EnableEffect(EffectType.Invigorated, effectDuration);
                        break;
                    case ItemType.Medkit:
                        otherPlayer.Heal(effectDuration);
                        break;
                    case ItemType.SCP207:
                        var effect = player.Role.Team == Team.SCPs && Plugin.Singleton.Config.SpeedSlowsSCPs ? EffectType.Slowness : EffectType.MovementBoost;
                        otherPlayer.EnableEffect(effect, 50, effectDuration);
                        break;
                }
            }
        }

        private IEnumerator<float> ShowCooldownHint(Player player, TimeSpan cooldownDuration)
        {
            var cooldownTime = (float)cooldownDuration.TotalSeconds;
            while (cooldownTime > 0)
            {
                player.ShowHint(Plugin.Singleton.Translation.CoolDownText.Replace("%cooldown", cooldownTime.ToString("F1")), 1f);
                cooldownTime -= 0.1f;
                yield return Timing.WaitForSeconds(0.1f);
            }

            player.ShowHint(Plugin.Singleton.Translation.AbilityReadyText, 1f);
        }
    }
}
