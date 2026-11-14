using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D;
using LitJson;
using System;
using System.Globalization;

public class ScriptableEditor
{
    [MenuItem("Tools/GenerateWeaponData")]
    public static void GenerateWeaponData()
    {
        JsonData datas = JsonManager.Instance.Load<JsonData>("WeaponData.json");

        // 确保父目录存在
        if (!AssetDatabase.IsValidFolder("Assets/ScriptableObjects"))
            AssetDatabase.CreateFolder("Assets", "ScriptableObjects");

        if (!AssetDatabase.IsValidFolder("Assets/ScriptableObjects/WeaponDatas"))
            AssetDatabase.CreateFolder("Assets/ScriptableObjects", "WeaponDatas");

        foreach (JsonData row in datas)
        {
            string weaponID = (string)row["weaponID"];
            int level = int.Parse(row["level"].ToString(), CultureInfo.InvariantCulture);

            // 为每种武器创建独立文件夹
            string folder = $"Assets/ScriptableObjects/WeaponDatas/{weaponID}";

            if (!AssetDatabase.IsValidFolder(folder))
                AssetDatabase.CreateFolder("Assets/ScriptableObjects/WeaponDatas", weaponID);

            string path = $"{folder}/{weaponID}_{level}.asset";

            WeaponData data = AssetDatabase.LoadAssetAtPath<WeaponData>(path);

            bool isNew = data == null;

            if (isNew)
                data = ScriptableObject.CreateInstance<WeaponData>();

            data.weaponID = weaponID;
            data.Name = (string)row["Name"];
            data.Tip = (string)row["Tip"];
            data.damageType = Enum.Parse<DamageType>(
                row["damageType"].ToString(), true);
            data.Mode = (string)row["Mode"];
            data.resEff = (string)row["resEff"];
            data.resName = (string)row["resName"];
            data.bullet = row["bullet"]?.ToString();
            data.level = level;
            data.internalTime = ReadFloat(row["internalTime"]);
            data.range = int.Parse(row["range"].ToString(), CultureInfo.InvariantCulture);
            data.atk = int.Parse(row["atk"].ToString(), CultureInfo.InvariantCulture);
            data.repel = int.Parse(row["repel"].ToString(), CultureInfo.InvariantCulture);

            if (isNew)
                AssetDatabase.CreateAsset(data, path);
            else
                EditorUtility.SetDirty(data);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    [MenuItem("Tools/GenerateRoleData")]

    public static void GenerateRoleData()
    {
        JsonData datas = JsonManager.Instance.Load<JsonData>("RoleData.json");

        // 确保父目录存在
        if (!AssetDatabase.IsValidFolder("Assets/ScriptableObjects"))
            AssetDatabase.CreateFolder("Assets", "ScriptableObjects");

        if (!AssetDatabase.IsValidFolder("Assets/ScriptableObjects/RoleDatas"))
            AssetDatabase.CreateFolder("Assets/ScriptableObjects", "RoleDatas");

        foreach (JsonData row in datas)
        {
            string roleID = (string)row["roleID"];

            string folder = $"Assets/ScriptableObjects/RoleDatas";


            string path = $"{folder}/{roleID}.asset";

            RoleData data = AssetDatabase.LoadAssetAtPath<RoleData>(path);

            bool isNew = data == null;

            if (isNew)
                data = ScriptableObject.CreateInstance<RoleData>();

            data.roleID = roleID;
            data.armor = int.Parse(row["armor"].ToString(), CultureInfo.InvariantCulture);
            data.hp = int.Parse(row["hp"].ToString(), CultureInfo.InvariantCulture);
            data.moveSpeed = ReadFloat(row["moveSpeed"]);
            data.range = ReadFloat(row["range"]);
            data.lucky = int.Parse(row["lucky"].ToString(), CultureInfo.InvariantCulture);
            data.STR = int.Parse(row["STR"].ToString(), CultureInfo.InvariantCulture);
            data.AST = int.Parse(row["AST"].ToString(), CultureInfo.InvariantCulture);
            data.drain = ReadFloat(row["drain"]);
            data.recovery = int.Parse(row["recovery"].ToString(), CultureInfo.InvariantCulture);
            data.atkSpeed = ReadFloat(row["atkSpeed"]);
            data.money = int.Parse(row["money"].ToString(), CultureInfo.InvariantCulture);
            data.remark = ((IDictionary)row).Contains("remark") ? row["remark"]?.ToString() : "";
            data.damage= ReadFloat(row["damage"]);
            data.passive = ((IDictionary)row).Contains("passive") ? row["passive"]?.ToString() : "";
            if (isNew)
                AssetDatabase.CreateAsset(data, path);
            else
                EditorUtility.SetDirty(data);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    [MenuItem("Tools/GenerateAccessoryData")]

    public static void GenerateAccessoryData()
    {
        JsonData datas = JsonManager.Instance.Load<JsonData>("AccessoryData.json");

        // 确保父目录存在
        if (!AssetDatabase.IsValidFolder("Assets/ScriptableObjects"))
            AssetDatabase.CreateFolder("Assets", "ScriptableObjects");

        if (!AssetDatabase.IsValidFolder("Assets/ScriptableObjects/AccessoryData"))
            AssetDatabase.CreateFolder("Assets/ScriptableObjects", "AccessoryData");

        foreach (JsonData row in datas)
        {
            string accessoryID = (string)row["accessoryID"];

            string folder = $"Assets/ScriptableObjects/AccessoryData";


            string path = $"{folder}/{accessoryID}.asset";

            AccessoryData data = AssetDatabase.LoadAssetAtPath<AccessoryData>(path);

            bool isNew = data == null;

            if (isNew)
                data = ScriptableObject.CreateInstance<AccessoryData>();

            data.accessoryID= accessoryID;
            data.description = (string)row["description"];
            data.level = (string)row["level"];
            data.resIcon = (string)row["resIcon"];
            if (isNew)
                AssetDatabase.CreateAsset(data, path);
            else
                EditorUtility.SetDirty(data);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private static float ReadFloat(JsonData value)
    {
        if (value.IsString)
            return float.Parse((string)value, CultureInfo.InvariantCulture);
        return value.IsDouble ? (float)(double)value : (int)value;
    }
}
