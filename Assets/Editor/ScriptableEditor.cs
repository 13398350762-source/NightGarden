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
        if (!AssetDatabase.IsValidFolder("Assets/WeaponDatas"))
            AssetDatabase.CreateFolder("Assets", "WeaponDatas");
        foreach (JsonData row in datas)
        {
            string weaponID = (string)row["weaponID"];
            int level = int.Parse(row["level"].ToString(), CultureInfo.InvariantCulture);
            string folder = $"Assets/WeaponDatas/{weaponID}";
            if (!AssetDatabase.IsValidFolder(folder))
                AssetDatabase.CreateFolder("Assets/WeaponDatas", weaponID);
            string path =
                $"{folder}/{weaponID}_{level}.asset";

            WeaponData data = AssetDatabase.LoadAssetAtPath<WeaponData>(path);
            bool isNew = data == null;
            if (isNew)
                data = ScriptableObject.CreateInstance<WeaponData>();

            data.weaponID = weaponID;
            data.Name = (string)row["Name"];
            data.Tip = (string)row["Tip"];
            data.damageType = (DamageType)Enum.Parse(typeof(DamageType), row["damageType"].ToString());
            data.Mode = (string)row["Mode"];
            data.resEff = (string)row["resEff"];
            data.resName = (string)row["resName"];
            data.bullet = row["bullet"]?.ToString();
            data.level = level;
            data.internalTime = ReadFloat(row["internalTime"]);
            data.range = int.Parse(row["range"].ToString(), CultureInfo.InvariantCulture);
            data.atk = int.Parse(row["atk"].ToString(), CultureInfo.InvariantCulture);
            data.repel = ReadFloat(row["repel"]);

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
