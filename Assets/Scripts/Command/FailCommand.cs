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
        ShakeAnimation shakeAnimation = unit.GetComponent<ShakeAnimation>();
        Tween shake = shakeAnimation.TriggerAnimation(new Vector3(0f, 0f, 10f), AnimationType.SHAKEROTATION);
        return true;
    }
}
