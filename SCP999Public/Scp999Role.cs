using Exiled.API.Features;
using Exiled.CustomRoles.API.Features;
using PlayerRoles;
using UnityEngine;
using YamlDotNet.Serialization;
using MapEditorReborn.API.Features;
using MapEditorReborn.API.Features.Objects;
using SCPSLAudioApi.AudioCore;
using Exiled.API.Features.Spawn;
using Exiled.API.Enums;
using Exiled.API.Features.Doors;
using System.Collections.Generic;
using System;

namespace SCP999
{
    public class Scp999Role : CustomRole
    {
        public override uint Id { get; set; } = 999;

        // Role and health settings
        public override RoleTypeId VisibleRole { get; set; } = RoleTypeId.Tutorial;
        public override int MaxHealth { get; set; } = 999;
        public override string Name { get; set; } = "SCP-999";
        public override string Description { get; set; } = "The tickle monster.";
        public override string CustomInfo { get; set; } = "SCP-999";

        // Player scaling
        public override Vector3 Scale { get; set; } = new(.5f, .5f, .5f);

        // Spawn properties
        public override SpawnProperties SpawnProperties { get; set; } = new SpawnProperties
        {
            Limit = 1,
            DynamicSpawnPoints = new List<DynamicSpawnPoint>
            {
                new() { Chance = 100, Location = SpawnLocationType.Inside330 }
            }
        };

        // Inventory
        public override List<string> Inventory { get; set; } = new() { "SCP207", "Adrenaline", "Medkit" };

        // Fields for animations and model
        public static Animator Animator { get; private set; }
        public static SchematicObject Scp999Model { get; private set; }
        public static AudioPlayerBase Audio { get; private set; }

        // Yaml ignore for certain properties
        [YamlIgnore]
        public override RoleTypeId Role { get; set; } = RoleTypeId.Tutorial;

        public override void AddRole(Player player)
        {
            if (player.IsNPC || player.Nickname == "SCP-999" || TrackedPlayers.Count >= SpawnProperties.Limit)
                return;

            base.AddRole(player);

            player.Role.Set(Role, RoleSpawnFlags.None);
            player.Health = MaxHealth;
            player.IsGodModeEnabled = Plugin.Singleton.Config.Scp999GodMode;

            try
            {
                player.ReferenceHub.transform.localScale = Vector3.zero;

                foreach (var target in Player.List)
                {
                    if (target != player)
                        Server.SendSpawnMessage?.Invoke(null, new object[] { player.ReferenceHub.networkIdentity, target.Connection });
                }

                player.ReferenceHub.transform.localScale = Scale;

                // Spawn SCP-999 model and set animator
                Scp999Model = ObjectSpawner.SpawnSchematic("SCP_999", player.Position, Quaternion.identity, player.Scale, null, false);
                Animator = Scp999Model.GetComponentInChildren<Animator>(true);

                Scp999Model.transform.SetParent(player.Transform);
                Scp999Model.transform.localPosition = new Vector3(0, -0.25f, 0);
            }
            catch (Exception exception)
            {
                Log.Error($"Error during AddRole: {exception}");
            }

            // Assign items to player
            var inventory = new List<ItemType> { ItemType.SCP207, ItemType.Adrenaline, ItemType.Medkit };
            foreach (var itemType in inventory)
            {
                player.AddItem(itemType);
            }

            // Open doors in LCZ-330 room
            foreach (var room in Room.List)
            {
                if (room.Type == RoomType.Lcz330)
                {
                    foreach (var door in room.Doors)
                    {
                        door.IsOpen = true;
                    }
                }
            }
        }

        public override void RemoveRole(Player player)
        {
            try
            {
                Scp999Model.Destroy();
                player.Scale = Vector3.one;
                TrackedPlayers.Remove(player);
            }
            catch (Exception ex)
            {
                Log.Error($"Error during RemoveRole: {ex}");
            }
        }

        public class AbilityConfig
        {
            public float Range { get; set; }
            public float EffectDuration { get; set; }

            // Constructors for YAML deserialization and manual creation
            public AbilityConfig() { }
            public AbilityConfig(float range, float effectDuration)
            {
                Range = range;
                EffectDuration = effectDuration;
            }
        }
    }
}