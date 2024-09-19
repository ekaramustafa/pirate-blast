using System.Collections.Generic;
using UnityEngine;

public abstract class GameEvent { }

public class MoveConsumedEvent : GameEvent
{
    public int MovesConsumed { get; private set; }

    public MoveConsumedEvent(int movesConsumed)
    {
        MovesConsumed = movesConsumed;
    }
}

public class MoveSetupEvent : GameEvent
{
    public int Moves { get; private set; }

    public MoveSetupEvent(int moves)
    {
        Moves = moves;
    }
}

public class GoalsSetupEvent : GameEvent
{
    public Dictionary<Sprite, int> GoalUIParts { get; private set; }

    public GoalsSetupEvent(Dictionary<Sprite, int> goalUIParts)
    {
        GoalUIParts = goalUIParts;
    }
}


public class GoalsUpdateEvent : GameEvent
{
    public Dictionary<Sprite, int> parts { get; private set; }

    public GoalsUpdateEvent(Dictionary<Sprite, int> goalChangedParts)
    {
        parts = goalChangedParts;
    }

}

public class LevelFinishedEvent : GameEvent
{
    public bool success;
    public LevelFinishedEvent(bool success)
    {
        this.success = success;
    }
}
