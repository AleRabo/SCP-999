using Exiled.API.Enums;
using Exiled.API.Extensions;
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
            public static readonly Dictionary<ItemType, string> ItemToAbilityMap = new()
            {
                { ItemType.Adrenaline, "Invigorate" },
                { ItemType.Medkit, "Heal" },
                { ItemType.SCP207, "SpeedBoost" }
            };

            public static string GetAbilityForItem(ItemType itemType) =>
                ItemToAbilityMap.TryGetValue(itemType, out var abilityName) ? abilityName : null;
        }

        private DateTime lastAbilityUse = DateTime.MinValue;
        private readonly TimeSpan cooldown = TimeSpan.FromSeconds(2);
        private CoroutineHandle cooldownHintCoroutine;

        public void RoleChanged(ChangingRoleEventArgs ev)
        {
            var customRole = CustomRole.Get(typeof(Scp999Role)) as Scp999Role;
            if (customRole != null && ev.Player.GetCustomRoles().Contains(customRole) &&
                !ev.NewRole.GetStartingInventory().Contains(ItemType.SCP207))
            {
                customRole.RemoveRole(ev.Player);
            }
        }

        public void PickingUpItem(SearchingPickupEventArgs ev)
        {
            var customRole = CustomRole.Get(typeof(Scp999Role)) as Scp999Role;
            if (ev.Player.GetCustomRoles().Contains(customRole))
                ev.IsAllowed = false;
        }

        public void Dropping(DroppingItemEventArgs ev)
        {
            var customRole = CustomRole.Get(typeof(Scp999Role)) as Scp999Role;
            if (ev.Player.GetCustomRoles().Contains(customRole))
                ev.IsAllowed = false;
        }

        public void EnterPocket(EnteringPocketDimensionEventArgs ev)
        {
            var customRole = CustomRole.Get(typeof(Scp999Role)) as Scp999Role;
            if (ev.Player.GetCustomRoles().Contains(customRole) && Plugin.Singleton.Config.Scp999GodMode)
                ev.IsAllowed = false;
        }

        public void ExitPocket(FailingEscapePocketDimensionEventArgs ev)
        {
            var customRole = CustomRole.Get(typeof(Scp999Role)) as Scp999Role;
            if (ev.Player.GetCustomRoles().Contains(customRole))
            {
                ev.IsAllowed = false;
                ev.Player.Position = Room.Random().Position + Vector3.up;
                ev.Player.Health -= 100;
            }
        }

        public void Hurting(HurtingEventArgs ev)
        {
            var customRole = CustomRole.Get(typeof(Scp999Role)) as Scp999Role;
            if (ev.Player.GetCustomRoles().Contains(customRole))
            {
                if (ev.IsInstantKill || ev.Amount < customRole.MaxHealth && ev.DamageHandler.Type != DamageType.PocketDimension)
                {
                    ev.IsAllowed = false;
                    ev.Attacker?.ShowHitMarker();
                    ev.Player.Health -= 100;
                }
            }
        }

        public void Spawned(SpawnedEventArgs ev)
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

            if (UnityEngine.Random.Range(0, 100) <= customRole.SpawnChance && !ev.Player.IsNPC && ev.Player.Nickname != "SCP-999" &&
                customRole.TrackedPlayers.Count < customRole.SpawnProperties.Limit)
            {
                customRole.AddRole(ev.Player);
            }
        }

        public void OnUsingItem(UsingItemEventArgs ev)
        {
            if (!ev.Player.GetCustomRoles().Contains(CustomRole.Get(999)))
                return;

            if (DateTime.Now < lastAbilityUse + cooldown)
            {
                ev.IsAllowed = false;
                return;
            }

            ev.IsAllowed = false;

            var abilityName = AbilityMapping.GetAbilityForItem(ev.Item.Type);
            if (abilityName != null && Plugin.Singleton.Config.Abilities.TryGetValue(abilityName, out var ability))
            {
                ExecuteAbility(ev.Player, ev.Item.Type, ability);
                lastAbilityUse = DateTime.Now;

                if (cooldownHintCoroutine.IsRunning)
                    Timing.KillCoroutines(cooldownHintCoroutine);

                cooldownHintCoroutine = Timing.RunCoroutine(ShowCooldownHint(ev.Player, cooldown));
            }
        }

        private void ExecuteAbility(Player player, ItemType itemType, AbilityConfig ability)
        {
            var nearbyPlayers = Player.List.Where(p => p != player && Vector3.Distance(player.Position, p.Position) <= ability.Range);
            foreach (var otherPlayer in nearbyPlayers)
            {
                switch (itemType)
                {
                    case ItemType.Adrenaline:
                        otherPlayer.EnableEffect(EffectType.Invigorated, ability.EffectDuration);
                        break;
                    case ItemType.Medkit:
                        otherPlayer.Heal(ability.EffectDuration);
                        break;
                    case ItemType.SCP207:
                        var effect = player.Role.Team == Team.SCPs && Plugin.Singleton.Config.SpeedSlowsSCPs ? EffectType.Slowness : EffectType.MovementBoost;
                        otherPlayer.EnableEffect(effect, 50, ability.EffectDuration);
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