using Cysharp.Threading.Tasks;

public class FormNewUnitCommand : IClickCommand
{
    private GridSystem gridSystem;
    private GridPosition startPosition;

    public FormNewUnitCommand(GridSystem gridSystem, GridPosition startPosition)
    {
        this.gridSystem = gridSystem;
        this.startPosition = startPosition;
    }

    public async UniTask<bool> Execute()
    {
        GridObject startGridObject = gridSystem.GetGridObject(startPosition);
        UnitAssetsData unitAssetsSO = gridSystem.GetUnitAssetsSO();
        UnitType unitType = startGridObject.GetUnit().GetUnitType();

        IFormNewUnitStrategy formStrategy;
        bool val = false;
        if (unitType == UnitType.Block)
        {
            formStrategy = new BlockFormNewUnitStrategy(gridSystem);
            UnitData unitSO = unitAssetsSO.GetUnitSOByUnitType(UnitType.TNT);
            val = await formStrategy.Form(startPosition, unitSO);

        }
        else if (unitType == UnitType.TNT)
        {
            formStrategy = new TNTFormNewUnitStrategy(gridSystem);
            UnitData unitSO = unitAssetsSO.GetTNTSOByTNTType(TNTType.LARGE);
            val = await formStrategy.Form(startPosition, unitSO);
            BlastCommand blastCommand = new BlastCommand(gridSystem,startPosition); 
            if (val)
                val = await blastCommand.Execute();
        }
        return val;
    }
}
