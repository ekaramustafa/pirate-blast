using System.Collections.Generic;
using Cysharp.Threading.Tasks;
public interface IBlastStrategy
{
    List<GridPosition> GetBlastablePositions(GridSystem gridSystem, GridPosition startPosition);

    UniTask<bool> Blast(GridSystem gridSystem, GridPosition startPosition);
    
}

public enum BlastType
{
    BlockBlast,
    TNTBlast,
    None
}
