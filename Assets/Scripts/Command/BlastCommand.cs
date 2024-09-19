using Cysharp.Threading.Tasks;
public class BlastCommand : IClickCommand
{
    private GridSystem gridSystem;
    private GridPosition gridPosition;

    public BlastCommand(GridSystem gridSystem, GridPosition gridPosition)
    {
        this.gridSystem = gridSystem;
        this.gridPosition = gridPosition;
    }

    public async UniTask<bool> Execute()
    {
        GridObject startGridObject = gridSystem.GetGridObject(gridPosition);
        UnitType unitType = startGridObject.GetUnit().GetUnitType();

        IBlastStrategy blastStrategy;
        if (unitType == UnitType.Block)
        {
            blastStrategy = new BlockBlastStrategy();
        }
        else if (unitType == UnitType.TNT)
        {
            blastStrategy = new TNTBlastStrategy();
        }
        else
        {
            return false;
        }
        bool val = await blastStrategy.Blast(gridSystem, gridPosition);

        return val;        
    }
}
