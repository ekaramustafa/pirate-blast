using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public interface IFormNewUnitStrategy
{
    List<GridPosition> GetFormablePositions(GridPosition startPosition);

    UniTask<bool> Form(GridPosition startPosition, UnitData unitSO);

    UniTask AnimateFormation(GridPosition gridPosition, List<GridPosition> formedPositions);
}
