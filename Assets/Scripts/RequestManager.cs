using System.Collections.Generic;

public class RequestManager
{
    public GridColumn[] columns;
    
    public RequestManager(int width)
    {
        columns = new GridColumn[width];
        for (int i = 0; i < columns.Length; i++)
        {
            columns[i] = new GridColumn();
        }
    }

    public void PostRequest(UserRequest userRequest)
    {
        int minCol = userRequest.GetMinCol();
        int maxCol = userRequest.GetMaxCol();

        for(int i = minCol; i < maxCol + 1; i++)
        {
            UserRequest filteredRequest = UserRequest.GetFilteredRequestByColumn(userRequest, i);
            columns[i].EnqueueRequest(filteredRequest);
            
        }

    }

    public void FinishRequest(UserRequest userRequest)
    {
        int minCol = userRequest.GetMinCol();
        int maxCol = userRequest.GetMaxCol();

        for(int i = minCol;i<maxCol + 1; i++)
        {
            UserRequest filteredRequest = UserRequest.GetFilteredRequestByColumn(userRequest, i);
            columns[i].SignalFinishedUserRequest(filteredRequest);
        }
    }

    public void FinishCallback(UserRequest userRequest)
    {
        int minCol = userRequest.GetMinCol();
        int maxCol = userRequest.GetMaxCol();

        for (int i = minCol; i < maxCol + 1; i++)
        {
            columns[i].SignalFinishedCallback();
        }
    }

}
