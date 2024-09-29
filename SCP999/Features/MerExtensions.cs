using System;
using System.IO;
using System.Reflection;
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
        string dirPath = "SCP999.Schematics.SCP999Model";
        string resourcePath = dirPath + ".SCP999Model.json";
        SchematicObjectDataList data = null;
        try
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            Stream stream = assembly.GetManifestResourceStream(resourcePath);
            if (stream == null)
            {
                Log.Error("The schematic was not found in solution");
                return null;
            }
            
            data = JsonSerializer.Deserialize<SchematicObjectDataList>(stream);
            data.Path = dirPath;
        }
        catch (Exception ex)
        {
            Log.Error("OnWaitingRound error in IAbility:" + ex.Message);
        }
        
        return data;
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