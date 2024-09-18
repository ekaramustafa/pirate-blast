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

        IClickCommand blastCommand = new BlastCommand(gridSystem, position);
        if(await blastCommand.Execute())
        {
            return true;
        }

        IClickCommand failCommand = new FailCommand(gridSystem, position);
        await failCommand.Execute();

        return false;
    }
}
