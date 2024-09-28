using System;
using System.IO;
using Exiled.API.Features;
using MapEditorReborn.API.Features;
using MapEditorReborn.API.Features.Objects;
using MapEditorReborn.API.Features.Serializable;
using Utf8Json;
using UnityEngine;

namespace SCP999;
public class MerExtensions
{
    /// <summary>
    /// Spawn schematics and binding to the player by transform
    /// </summary>
    /// <param name="player"></param>
    /// <param name="scale"></param>
    /// <returns></returns>
    public static SchematicObject SpawnSchematicForPlayer(string schematicName)
    {
        // If the schematics don't exist, then don't do anything
        SchematicObject schematicObject = SpawnSchematicByName(schematicName);
        if (schematicObject == null)
        {
            return null;
        }
        
        return schematicObject;
    }

    /// <summary>
    /// Spawn schematics by name in the solution or MER folder
    /// </summary>
    /// <param name="schematicName"></param>
    /// <returns></returns>
    public static SchematicObject SpawnSchematicByName(string schematicName)
    {
        SchematicObjectDataList data;
        
        if (schematicName == "SCP999Model")
        {
            // The schematics are in the solution
            data = GetSchematicDataByProject(schematicName);
        }
        else
        {
            // The schematic is located in the mer folder
            data = MapUtils.GetSchematicDataByName(schematicName);
        }
        
        if (data == null)
        {
            Log.Error("The schematic was not found");
            return null;
        }
        
        GameObject gameObject = new GameObject("CustomSchematic-" + schematicName);
        SchematicObject schematicObject = gameObject.AddComponent<SchematicObject>().Init(new SchematicSerializable(schematicName), data, false);
        return schematicObject;
    }

    /// <summary>
    /// Get data for schematic from json in the solution
    /// </summary>
    /// <param name="schematicName"></param>
    /// <returns></returns>
    public static SchematicObjectDataList GetSchematicDataByProject(string schematicName)
    {
        // SCP999/Schematics/SCP999Model
        string dirPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Schematics", schematicName);
        if (!Directory.Exists(dirPath))
        {
            Log.Error($"The directory {dirPath} was not found");
            return null;
        }

        // SCP999/Schematics/SCP999Model/SCP999Model.json
        string schematicPath = Path.Combine(dirPath, $"{schematicName}.json");
        if (!File.Exists(schematicPath))
            return null;
        
        SchematicObjectDataList data = JsonSerializer.Deserialize<SchematicObjectDataList>(dirPath);
        data.Path = dirPath;

        return data;
    }
    
    /// <summary>
    /// Make the player invisible to all players
    /// </summary>
    /// <param name="player"></param>
    /// <param name="scale"></param>
    public static void SendFakeSpawnMessage(Player player, Vector3 scale)
    {
        player.ReferenceHub.transform.localScale = Vector3.zero;

        foreach (Player target in Player.List)
        {
            if (target == player)
                continue;
            
            Server.SendSpawnMessage?.Invoke(null, new object[] { player.ReferenceHub.networkIdentity, target.Connection });
        }
        
        player.ReferenceHub.transform.localScale = scale;
    }
    
    /// <summary>
    /// Get animator from SchematicObject
    /// </summary>
    /// <param name="schematicObject"></param>
    /// <returns></returns>
    public static Animator GetAnimatorFromSchematic(SchematicObject schematicObject)
    {
        Animator animator = schematicObject.GetComponentInChildren<Animator>(true);
        if (animator == null)
        {
            Log.Error("The animator was not found");
        }
        
        return animator;
    }
}