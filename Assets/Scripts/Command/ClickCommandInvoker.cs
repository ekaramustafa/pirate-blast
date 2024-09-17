using Cysharp.Threading.Tasks;
using UnityEngine;
public class ClickCommandInvoker
{
    private GridSystem gridSystem;

    public ClickCommandInvoker(GridSystem gridSystem)
    {
        this.gridSystem = gridSystem;
    }

    public async UniTask<bool> HandleClick(Vector3 mousePos)
    {
        GridPosition position = gridSystem.GetGridPosition(mousePos);
        if (!gridSystem.CanPerformOnPosition(position))
        {
            return false;
        }

        bool isEligible = false;

        IClickCommand formNewUnitCommand = new FormNewUnitCommand(gridSystem, position);
        isEligible =  await formNewUnitCommand.Execute();

        if (!isEligible)
        {
            IClickCommand blastCommand = new BlastCommand(gridSystem, position);
            isEligible = await blastCommand.Execute();
        }

        if (!isEligible)
        {
            IClickCommand failCommand = new FailCommand(gridSystem, position);
            await failCommand.Execute();
        }
        return isEligible;
    }
}
