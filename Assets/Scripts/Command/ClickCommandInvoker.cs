using Cysharp.Threading.Tasks;
using UnityEngine;
public class ClickCommandInvoker
{
    private GridSystem gridSystem;

    IClickCommand blastCommand;
    IClickCommand failCommand;

    public ClickCommandInvoker(GridSystem gridSystem)
    {
        this.gridSystem = gridSystem;
        blastCommand = new BlastCommand(gridSystem);
        failCommand = new FailCommand(gridSystem);
    }

    public bool HandleClick(Vector3 mousePos)
    {
        GridPosition position = gridSystem.GetGridPosition(mousePos);
        if (!gridSystem.CanPerformOnPosition(position) || !gridSystem.IsGridPositionInteractable(position))
        {
            return false;
        }

        if(blastCommand.Execute(position))
        {
            return true;
        }

        failCommand.Execute(position);

        return false;
    }
}
