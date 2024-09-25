#if UNITY_EDITOR
using System;
using UnityEditor;
#endif
using UnityEngine;

public static class UnitFactory
{
    public static Unit CreateUnit(UnitData unitData, Transform unitTemplatePrefab, Vector3 position, int sortingOrder)
    {
        Transform unitTransform = null;

        if (Application.isPlaying)
        {
            // Instantiate normally if in Play Mode
            unitTransform = GameObject.Instantiate(unitTemplatePrefab, position, Quaternion.identity);
        }
    #if UNITY_EDITOR
        else
        {
            // Instantiate in Edit Mode using PrefabUtility
            unitTransform = (Transform)PrefabUtility.InstantiatePrefab(unitTemplatePrefab);
            unitTransform.position = position;
            unitTransform.rotation = Quaternion.identity;
            EditorUtility.SetDirty(unitTransform.gameObject);

        }
    #endif

        Unit unit = unitTransform.GetComponent<Unit>();
        unit.SetUnitData(unitData);
        unit.GetComponent<SpriteRenderer>().sortingOrder = sortingOrder;

        return unit;
    }

    public static Unit CreateRandomBlockUnit(UnitAssetsData unitAssetsData, Vector3 worldPosition, int sortingOrder)
    {
        BlockColor blockColor = BlockColorExtensions.GetRandomBlockColor();
        UnitData unitData = unitAssetsData.GetBlockDataByBlockColor(blockColor);
        Transform unitTemplatePrefab = unitAssetsData.GetPrefabByUnitType(unitData.unitType);
        return CreateUnit(unitData, unitTemplatePrefab, worldPosition, sortingOrder);
    }
}
