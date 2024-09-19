#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public static class UnitFactory
{
    public static Unit CreateUnit(UnitData unitSO, Transform unitTemplatePrefab, Vector3 position, int sortingOrder)
    {
        Transform unitTransform;

        if (Application.isPlaying)
        {
            // Instantiate normally if in Play Mode
            unitTransform = GameObject.Instantiate(unitTemplatePrefab, position, Quaternion.identity);
        }
        else
        {
            // Instantiate in Edit Mode using PrefabUtility
            unitTransform = (Transform)PrefabUtility.InstantiatePrefab(unitTemplatePrefab);
            unitTransform.position = position;
            unitTransform.rotation = Quaternion.identity;
            EditorUtility.SetDirty(unitTransform.gameObject);

        }

        Unit unit = unitTransform.GetComponent<Unit>();
        unit.SetUnitSO(unitSO);
        unit.GetComponent<SpriteRenderer>().sortingOrder = sortingOrder;

        return unit;
    }
}
