using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "NewUnitAssets", menuName = "UnitAssetsData")]
public class UnitAssetsData : ScriptableObject
{
    public List<UnitAsset> unitAssets;

    public Transform GetPrefabByUnitType(UnitType unitType)
    {
        foreach (UnitAsset unitAsset in unitAssets)
        {
            if (unitAsset.unitData != null && unitAsset.unitData.unitType == unitType)
            {
                return unitAsset.prefab;
            }
        }

        Debug.LogWarning($"Prefab not found for UnitType: {unitType}");
        return null;
    }

    public UnitData GetUnitSOByUnitType(UnitType unitType)
    {
        foreach (UnitAsset unitAsset in unitAssets)
        {
            if (unitAsset.unitData != null && unitAsset.unitData.unitType == unitType)
            {
                return unitAsset.unitData;
            }
        }

        Debug.LogWarning($"Prefab not found for UnitType: {unitType}");
        return null;
    }

    public UnitData GetTNTSOByTNTType(TNTType tntType)
    {
        foreach (UnitAsset unitAsset in unitAssets)
        {
            if (unitAsset.unitData != null && unitAsset.unitData is TNTData tntSO && tntSO.tntType == tntType)
            {
                return tntSO;
            }
        }

        Debug.LogWarning($"TntSO not found for TNTType: {tntType}");
        return null;
    }

    public UnitData GetBlockSOByBlockColor(BlockColor blockColor)
    {
        foreach (UnitAsset unitAsset in unitAssets)
        {
            if (unitAsset.unitData != null && unitAsset.unitData is BlockData blockSO && blockSO.blockColor == blockColor)
            {
                return blockSO;
            }
        }

        Debug.LogWarning($"TntSO not found for BlockColor: {blockColor}");
        return null;
    }

}

[Serializable]
public class UnitAsset
{
    public UnitData unitData;  
    public Transform prefab;

}


