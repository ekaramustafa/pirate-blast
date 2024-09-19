using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using DG.Tweening;

public class FailCommand : IClickCommand
{
    private GridSystem gridSystem;
    private GridPosition gridPosition;

    public FailCommand(GridSystem gridSystem, GridPosition position)
    {
        this.gridSystem = gridSystem;
        this.gridPosition = position;
    }
    public async UniTask<bool> Execute()
    {
        await UniTask.Yield(); 

        GridObject gridObject = gridSystem.GetGridObject(gridPosition);
        Unit unit = gridObject.GetUnit();
        IAnimationService animationService = AnimationServiceLocator.GetAnimationService();
        Tween shake = animationService.TriggerAnimation(unit.transform, unit.transform.localScale, new Vector3(0f, 0f, 10f), 1f, AnimationType.SHAKEROTATION);
        gridObject.SetBeingAnimated(true);
        shake.OnComplete(() =>
        {
            gridObject.SetBeingAnimated(false);
        });

        return true;
    }
}
