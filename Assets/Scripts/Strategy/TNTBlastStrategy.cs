using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
public class TNTBlastStrategy : IBlastStrategy
{
    public async UniTask<bool> Blast(GridSystem gridSystem, GridPosition startPosition)
    {
        await UniTask.Yield();

        List<GridPosition> blastablePositions = GetBlastablePositions(gridSystem, startPosition);
        Dictionary<Sprite, int> spriteCountMap = BlastUtils.GetBlastedSpritesCountMap(gridSystem, blastablePositions);
        Dictionary<GridPosition, Sprite> positionSpriteMap = BlastUtils.GetBlastedPositionsSpriteMap(gridSystem, blastablePositions);
        foreach (GridPosition position in blastablePositions)
        {
            BlastUtils.BlastBlockAtPosition(gridSystem, position, BlastType.TNTBlast);
        }
        blastablePositions.Clear();
        BlastUtils.PublishBlastedParts(gridSystem, positionSpriteMap, spriteCountMap);
        return true;
    }

    public List<GridPosition> GetBlastablePositions(GridSystem gridSystem, GridPosition startPosition)
    {
        //For TNT
        List<GridPosition> blastablePositions = new List<GridPosition>();
        Queue<GridPosition> queue = new Queue<GridPosition>();
        HashSet<GridPosition> visited = new HashSet<GridPosition>();
        queue.Enqueue(startPosition);

        while (queue.Count > 0)
        {
            GridPosition currentPosition = queue.Dequeue();

            if (visited.Contains(currentPosition))
            {
                continue;
            }

            visited.Add(currentPosition);
            GridObject currentGridObject = gridSystem.GetGridObject(currentPosition);
            TNTData tntSO = currentGridObject.GetUnit().GetUnitData() as TNTData;
            int range = tntSO.range;
            for (int xOffset = -range; xOffset <= range; xOffset++)
            {
                for (int yOffset = -range; yOffset <= range; yOffset++)
                {
                    GridPosition blastPosition = new GridPosition(currentPosition.x + xOffset, currentPosition.y + yOffset);
                    if (gridSystem.CanPerformOnPosition(blastPosition))
                    {
                        blastablePositions.Add(blastPosition);

                        GridObject gridObject = gridSystem.GetGridObject(blastPosition);
                        UnitType unitType = gridObject.GetUnit().GetUnitType();
                        if (unitType == UnitType.TNT && !visited.Contains(blastPosition))
                        {
                            queue.Enqueue(blastPosition);
                        }
                    }
                }
            }
        }

        return blastablePositions;
    }


}
