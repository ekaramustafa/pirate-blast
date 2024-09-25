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

    public void SetUnitData(UnitData unitData)
    {     
        this.unitData = unitData;
        GetComponent<SpriteRenderer>().sprite = unitData.defaultStateSprite;
        this.health = unitData.health;

    }

    public UnitData GetUnitData()
    {
        return unitData;
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

    public bool IsStationary()
    {
        return unitData.isStationary;
    }

    public void SetSortingOrder(int y)
    {
        transform.GetComponent<SpriteRenderer>().sortingOrder = y;
    }

    public Sprite GetDefaultSprite()
    {
        return unitData.defaultStateSprite;
    }

    public void SetSpriteToTNTState()
    {
        BlockData blockSO = GetUnitData() as BlockData;
        GetComponent<SpriteRenderer>().sprite = blockSO.tntStateSprite;
    }


    public void SetSpriteToDefault()
    {
        GetComponent<SpriteRenderer>().sprite = GetDefaultSprite();
    }




}


