using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using DG.Tweening;

public class FailCommand : IClickCommand
{
    private GridSystem gridSystem;

    public FailCommand(GridSystem gridSystem)
    {
        this.gridSystem = gridSystem;
    }
    public bool Execute(GridPosition position)
    {
        
        GridObject gridObject = gridSystem.GetGridObject(position);
        Unit unit = gridObject.GetUnit();
        IAnimationService animationService = AnimationServiceLocator.GetAnimationService();
        Tween shake = animationService.TriggerAnimation(unit.transform, unit.transform.localScale, new Vector3(0f, 0f, 10f), 0.25f, AnimationType.SHAKEROTATION);
        gridObject.SetIsInteractable(false);
        shake.OnComplete(() =>
        {
            gridObject.SetIsInteractable(true);
        });

        return true;
    }
}
