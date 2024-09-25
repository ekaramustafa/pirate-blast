using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewUnit", menuName = "Unit")]
public class UnitData : ScriptableObject
{
    public Sprite defaultStateSprite;
    public UnitType unitType;
    public int health;
    public bool affectedByNeighborBlast;
    public bool isStationary;
    public ParticleSystem blastParticleSystem;
    public ParticleSystem tntExplosionParticleSystem;
    public Material particleMaterial;
}

public enum UnitType
{
    Block,
    TNT,
    Ice,
    Stone,
    Chocolate,
    Random,

}

public static class UnitSOExtenstions{

    public static string GetUnitName(UnitData unitData)
    {
        UnitType unitType = unitData.unitType;
        if (unitType == UnitType.Block)
        {
            BlockData blockSO = unitData as BlockData;
            string str = blockSO.blockColor.ToString() + " " + unitType.ToString();
            return str.ToLower();
        }
        if(unitData.unitType == UnitType.TNT)
        {
            TNTData tntSO = unitData as TNTData;
            string str = tntSO.tntType.ToString() + " " + unitType.ToString();
            return str.ToLower();

        }
        return unitType.ToString().ToLower();
    }

}






