using Cysharp.Threading.Tasks;
using System;
using System.Runtime.InteropServices;

public class BlastCommand : IClickCommand
{
    private GridSystem gridSystem;

    IBlastStrategy blockBlastStrategy;
    IBlastStrategy tntblastStrategy;
    public BlastCommand(GridSystem gridSystem)
    {
        this.gridSystem = gridSystem;
        blockBlastStrategy = new BlockBlastStrategy();
        tntblastStrategy = new TNTBlastStrategy();

    }

    public bool Execute(GridPosition position)
    {
        GridObject startGridObject = gridSystem.GetGridObject(position);
        UnitType unitType = startGridObject.GetUnit().GetUnitType();

        if (unitType == UnitType.Block)
        {
            bool val = blockBlastStrategy.Blast(gridSystem, position);
            return val;
        }
        else if (unitType == UnitType.TNT)
        {
            bool val = tntblastStrategy.Blast(gridSystem, position);
            return val;
        }
        else
        {
            return false;
        }  
    }
}
