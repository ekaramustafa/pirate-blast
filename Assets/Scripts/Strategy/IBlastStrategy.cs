using System.Collections.Generic;
using Cysharp.Threading.Tasks;
public interface IBlastStrategy
{
    UniTask<bool> Blast(GridSystem gridSystem, GridPosition startPosition);
    
}

public enum BlastType
{
    BlockBlast,
    TNTBlast,
    BlockBlastForm,
    TNTBlastForm,
}
