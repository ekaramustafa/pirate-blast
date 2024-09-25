using System;
using System.Collections.Generic;
using System.Linq;

public class UserRequest
{
    public GridPosition[] AffectedPositions;
    public Action<UserRequest> Callback;
    private RequestState state;


    public UserRequest(GridPosition[] positions, Action<UserRequest> callback)
    {
        AffectedPositions = positions;
        Callback = callback;
        state = RequestState.PROCESSING_USER_REQUEST;
    }

    public UserRequest(GridPosition[] positions, Action<UserRequest> callback, RequestState state)
    {
        AffectedPositions = positions;
        Callback = callback;
        this.state = state;
    }

    public int GetMinCol()
    {
        int minVal = AffectedPositions[0].x;
        for (int i = 1; i < AffectedPositions.Length; i++)
        {
            if (AffectedPositions[i].x < minVal)
            {
                minVal = AffectedPositions[i].x;
            }
        }
        return minVal;
    }


    public int GetMaxCol()
    {
        int maxVal = AffectedPositions[0].x;
        for (int i = 1; i < AffectedPositions.Length; i++)
        {
            if (AffectedPositions[i].x > maxVal)
            {
                maxVal = AffectedPositions[i].x;
            }
        }
        return maxVal;
    }

    public int GetMinRow()
    {
        int minVal = AffectedPositions[0].y;
        for (int i = 1; i < AffectedPositions.Length; i++)
        {
            if (AffectedPositions[i].y < minVal)
            {
                minVal = AffectedPositions[i].y;
            }
        }
        return minVal;
    }

    public int GetMaxRow()
    {
        int maxVal = AffectedPositions[0].y;
        for (int i = 1; i < AffectedPositions.Length; i++)
        {
            if (AffectedPositions[i].y > maxVal)
            {
                maxVal = AffectedPositions[i].y;
            }
        }
        return maxVal;
    }


    public void SetToCallbackExecutingState()
    {
        state = RequestState.PROCESSING_CALLBACK;
    }

    public void SetToReadyForCallbackState()
    {
        state = RequestState.READY_FOR_CALLBACK;
    }

    public bool IsProcessingUserRequest => state == RequestState.PROCESSING_USER_REQUEST;

    public bool IsReadyForUserRequestCallback => state == RequestState.READY_FOR_CALLBACK;


    public override bool Equals(object obj)
    {
        if (!(obj is UserRequest))
            return false;

        UserRequest other = (UserRequest)obj;

        if (AffectedPositions.Length != other.AffectedPositions.Length)
            return false;

        bool allMatch = AffectedPositions.All(pos => other.AffectedPositions.Contains(pos)) &&
                other.AffectedPositions.All(pos => AffectedPositions.Contains(pos));

        return allMatch;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public static bool operator ==(UserRequest req1, UserRequest req2)
    {
        return req1.Equals(req2);
    }

    public static bool operator !=(UserRequest req1, UserRequest req2)
    {
        return !req1.Equals(req2);
    }


    public override string ToString()
    {
        String result = "Positions: ";
        foreach(GridPosition position in AffectedPositions)
        {
            result += position.ToString() + " ";
        }
        result += "State: " + state.ToString();
        return result;
    }

    public UserRequest Clone()
    {
        return new UserRequest((GridPosition[])AffectedPositions.Clone(), Callback, state);
    }

    public static UserRequest GetFilteredRequestByColumn(UserRequest req1, int col)
    {
        List<GridPosition> gridPositions = new List<GridPosition>();
        foreach(GridPosition pos in req1.AffectedPositions)
        {
            if(pos.x == col)
            {
                gridPositions.Add(pos);
            }
        }
        return new UserRequest(gridPositions.ToArray(), req1.Callback);
    }


    


}

