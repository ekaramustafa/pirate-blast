using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class BlockBlastStrategy : IBlastStrategy
{
    public async UniTask<bool> Blast(GridSystem gridSystem, GridPosition startPosition)
    {
        await UniTask.Yield(); // there are no animations requiring blocking, for now...

        List<GridPosition> blastablePositions = GetBlastablePositions(gridSystem, startPosition);
        if (blastablePositions.Count >= GameConstants.BLAST_THRESHOLD)
        {
            AugmentPositionsWithAdjacent(gridSystem, blastablePositions);
            Dictionary<Sprite, int> spriteCountMap = BlastUtils.GetBlastedSpritesCountMap(gridSystem, blastablePositions);
            Dictionary<GridPosition, Sprite> positionSpriteMap = BlastUtils.GetBlastedPositionsSpriteMap(gridSystem, blastablePositions);
            foreach (GridPosition position in blastablePositions)
            {
                BlastUtils.BlastBlockAtPosition(gridSystem, position, BlastType.BlockBlast);
            }
            BlastUtils.PublishBlastedParts(gridSystem, positionSpriteMap, spriteCountMap);
            return true;
        }
        return false;
    }

    public List<GridPosition> GetBlastablePositions(GridSystem gridSystem, GridPosition startPosition)
    {
        List<GridPosition> DFSPositions = GridSearchUtils.GetAdjacentSameColorBlocks(gridSystem, startPosition);
        return DFSPositions;
    }

    /// <summary>
    /// This returns the list of grid positions that can be blasted by adjacent blast. 
    /// </summary>
    /// <param name="gridSystem"></param>
    /// <param name="blastablePositions"></param>
    public void AugmentPositionsWithAdjacent(GridSystem gridSystem, List<GridPosition> blastablePositions)
    {
        HashSet<GridPosition> visitedPositions = new HashSet<GridPosition>(blastablePositions);

        List<GridPosition> AllNeighborPositions = new List<GridPosition>();
        HashSet<UnitType> predicateSet = new HashSet<UnitType>();
        
        predicateSet.Add(UnitType.Chocolate); //Vase cane be hit by blasted blocks
        predicateSet.Add(UnitType.Ice); //Box can be hit by blasted blocks

        foreach (GridPosition gridPosition in blastablePositions)
        {
            List<GridPosition> neighborPositions = GridSearchUtils.GetNeighborPositionsWithUnitTypePredicate(gridSystem, gridPosition, predicateSet);

            foreach (GridPosition neighbor in neighborPositions)
            {
                if (!visitedPositions.Contains(neighbor))
                {
                    AllNeighborPositions.Add(neighbor);
                    visitedPositions.Add(neighbor);
                }
            }
        }

        blastablePositions.AddRange(AllNeighborPositions);
    }






}
