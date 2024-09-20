using Exiled.API.Features;
using Exiled.CustomRoles.API.Features;
using PlayerRoles;
using UnityEngine;
using YamlDotNet.Serialization;
using MapEditorReborn.API.Features;
using MapEditorReborn.API.Features.Objects;
using System.Collections.Generic;
using System;
using Exiled.API.Features.Spawn;
using Exiled.API.Enums;
using Exiled.API.Features.Doors;
using SCPSLAudioApi.AudioCore;

namespace SCP999;
public class Scp999Role : CustomRole
{
    public override uint Id { get; set; } = 999;

    /// <summary>
    /// Gets or sets the role that is visible to players on the server aside from the player playing this role.
    /// </summary>
    public RoleTypeId VisibleRole { get; set; } = RoleTypeId.Tutorial;

    /// <inheritdoc />
    public override int MaxHealth { get; set; } = 999;

    /// <inheritdoc />
    public override string Name { get; set; } = "SCP-999";

    /// <inheritdoc />
    public override string Description { get; set; } =
        "The tickle monster.";

    /// <inheritdoc />
    public override string CustomInfo { get; set; } = "SCP-999";

    /// <summary>
    /// Gets or sets the custom scale factor for players when they are this role.
    /// </summary>
    public override Vector3 Scale { get; set; } = new(.5f, .5f, .5f);

    public override SpawnProperties SpawnProperties { get; set; } = new SpawnProperties()
    {
        Limit = 1,
        DynamicSpawnPoints = new List<DynamicSpawnPoint>()
        {
            new() { Chance = 100, Location = SpawnLocationType.Inside330 }
        }
    };

    // The following properties are only defined so that we can add the YamlIgnore attribute to them so they cannot be changed via configs.
    /// <inheritdoc />
    [YamlIgnore]
    public override RoleTypeId Role { get; set; } = RoleTypeId.Tutorial;

    public override List<string> Inventory { get; set; } = new List<string>() { "SCP207", "Adrenaline", "Medkit" };

    public static Animator animator;
    public static SchematicObject scp999Model;
    public static AudioPlayerBase audio;
    public override void AddRole(Player player)
    {
        Log.Debug($"Player is NPC: {player.IsNPC}");
        Log.Debug($"Player Nickname: {player.Nickname}");
        Log.Debug($"Tracked Players Count: {TrackedPlayers.Count}");
        Log.Debug($"Spawn Limit: {SpawnProperties.Limit}");

        if (!player.IsNPC && player.Nickname != "SCP-999" && TrackedPlayers.Count < SpawnProperties.Limit)
        {
            base.AddRole(player); // Ensure the custom role system applies the role


            player.Role.Set(Role, RoleSpawnFlags.None);
            player.Health = MaxHealth; // Set health
            player.IsGodModeEnabled = Plugin.Singleton.Config.Scp999GodMode;

            try
            {
                player.ReferenceHub.transform.localScale = Vector3.zero;

                // Synchronize the player's state with others
                foreach (Player target in Player.List)
                {
                    if (target != player) // No need to send the message to the player themselves
                    {
                        Server.SendSpawnMessage?.Invoke(null, new object[] { player.ReferenceHub.networkIdentity, target.Connection });
                    }
                }

                player.ReferenceHub.transform.localScale = Scale;

                scp999Model = ObjectSpawner.SpawnSchematic("SCP_999", player.Position, Quaternion.identity, player.Scale, null, false);
                animator = scp999Model.GetComponentInChildren<Animator>(true);

                scp999Model.transform.parent = player.Transform;
                scp999Model.transform.rotation = new Quaternion();
                scp999Model.transform.position = player.Position + new Vector3(0, -.25f, 0);

            }
            catch (Exception exception)
            {
                Log.Error($" error: {exception}");
            }
            List<ItemType> inventory = new()
                {
                    ItemType.SCP207,
                    ItemType.Adrenaline,
                    ItemType.Medkit,
                };

            foreach (ItemType itemType in inventory)
            {
                player.AddItem(itemType);
            }

            foreach (Room room in Room.List)
            {
                if (room.Type == RoomType.Lcz330)
                {
                    foreach (Door door in room.Doors)
                    {
                        door.IsOpen = true;
                    }
                }
            }
        }
    }

    public override void RemoveRole(Player player)
    {
        try
        {
            scp999Model.Destroy();

            // Reset player's scale
            player.Scale = Vector3.one;

            TrackedPlayers.Remove(player);
        }
        catch { Exception ex; }
    }
}