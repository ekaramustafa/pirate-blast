using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    private UnitData unitData;
    private int health;

    private void Awake()
    {
    }

    public void SetUnitSO(UnitData unitData)
    {     
        this.unitData = unitData;
        GetComponent<SpriteRenderer>().sprite = unitData.defaultStateSprite;
        this.health = unitData.health;

    }

    public UnitData GetUnitData()
    {
        return unitData;
    }

    public Sprite GetDefaultSprite()
    {
        return unitData.defaultStateSprite;
    }

    public int GetHealth()
    {
        return health;
    }

    public void SetHealth(int value)
    {
        health = value;
    }

    public UnitType GetUnitType()
    {
        return unitData.unitType;
    }
    public bool CanBeAffectedByNeighborBlast()
    {
        return unitData.affectedByNeighborBlast;
    }


    public void SetSortingOrder(int y)
    {
        transform.GetComponent<SpriteRenderer>().sortingOrder = y;
    }



}


