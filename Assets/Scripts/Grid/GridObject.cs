using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class GridObject
{

    private GridPosition gridPosition;
    private Unit unit;
    private bool isInteractable;
    private bool willBeOccupied;

    public GridObject(GridPosition gridPosition)
    {
        this.gridPosition = gridPosition;
        isInteractable = true;
    }

    public void SetUnit(Unit unit)
    {
        this.unit = unit;
    }
    public Unit GetUnit()
    {
        return unit;
    }

    public override string ToString()
    {
        return base.ToString() + "\n" + unit;
    }

    public bool IsInteractable() => isInteractable;

    public void SetIsInteractable(bool val)
    {
        isInteractable = val;
    }

    public void HitUnit(BlastType blastType)
    {
        unit.SetHealth(unit.GetHealth() - 1);
        if (unit.GetUnitData() is ChocolateData chocolateData)
        {
            int health = unit.GetHealth();

            int damageLevelIndex = chocolateData.hitSprites.Length - health;

            if (damageLevelIndex >= 0 && damageLevelIndex < chocolateData.hitSprites.Length)
            {
                unit.GetComponent<SpriteRenderer>().sprite = chocolateData.hitSprites[damageLevelIndex];
            }
 
        }

        if (unit.GetHealth() == 0)
        {
            PlayBlastParticleSystem(blastType);
            DOTween.Kill(unit.transform.gameObject);
            UnityEngine.GameObject.Destroy(unit.gameObject);
            unit = null;
        }



    }

    private void PlayBlastParticleSystem(BlastType blastType)
    {
        if (blastType == BlastType.BlockBlastForm || blastType == BlastType.TNTBlastForm) return;
        UnitData unitData = unit.GetUnitData();
        UnitType unitType = unit.GetUnitType();
        
        if(unitType == UnitType.TNT)
        {
            PlayParticleSystem(unitData.tntExplosionParticleSystem, false);
            return;
        }

        else if(blastType == BlastType.TNTBlast)
        {
            PlayParticleSystem(unitData.tntExplosionParticleSystem, false);
            PlayParticleSystem(unitData.blastParticleSystem, true);
        }
        else if(blastType == BlastType.BlockBlast)
        {
            PlayParticleSystem(unitData.blastParticleSystem, true);
        }

    }

    private void PlayParticleSystem(ParticleSystem particleSystemInstance, bool updateSprites)
    {
        ParticleSystem particleSystemClone = CreateParticleSystemInstance(particleSystemInstance);

        if (updateSprites)
        {
            UnitData unitData = unit.GetUnitData();
            ParticleSystemRenderer renderer = particleSystemClone.GetComponent<ParticleSystemRenderer>();
            renderer.material = unitData.particleMaterial;
        }
        particleSystemClone.Play();

    }

    private ParticleSystem CreateParticleSystemInstance(ParticleSystem particleSystemInstance)
    {
        ParticleSystem blastParticleClone = UnityEngine.GameObject.Instantiate(particleSystemInstance, unit.transform.position, Quaternion.identity);
        blastParticleClone.transform.SetParent(null);
        return blastParticleClone;
    }

    public void SetWillBeOccupied(bool val)
    {
        willBeOccupied = val;
    }
    
    public bool WillBeOccupied()
    {
        return willBeOccupied;
    }

}

