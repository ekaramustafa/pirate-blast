using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridColumn
{


    private LinkedList<UserRequest> userRequests;

    public GridColumn()
    {
        userRequests = new LinkedList<UserRequest>();
    }


    public void EnqueueRequest(UserRequest userRequest)
    {
        userRequests.AddLast(userRequest);
    }

    private void ExecuteRequest()
    {
        UserRequest userRequest = userRequests.First.Value;
        if (userRequest.IsReadyForUserRequestCallback)
        {
            userRequest.SetToCallbackExecutingState();
            userRequest.Callback?.Invoke(userRequest);
        }
    }

    public void SignalFinishedUserRequest(UserRequest userRequest)
    {
        //Find the associated userRequest and set the state to callback
        //If it is the first element in the queue execute the callback
        if(userRequest == userRequests.First.Value)
        {
            if (userRequest.IsReadyForUserRequestCallback)
            {
                Debug.Log("User request is already being executed");
            }
            userRequests.First.Value.SetToReadyForCallbackState();
            ExecuteRequest();
        }
        else
        {
            foreach (var request in userRequests)
            {
                if (userRequest == request)
                {
                    request.SetToReadyForCallbackState();
                    break;
                }
            }
        }

    }

    public void SignalFinishedCallback()
    {
        //By the rules of the algorithm, this would happen when the top element finished its callback
        //Dequeue the top element and check whether following request is in the callback state, if yes then execute its callback
        userRequests.RemoveFirst();
        if(userRequests.Count != 0)
        {
            ExecuteRequest();
        }
    }
}

public enum RequestState
{
    PROCESSING_USER_REQUEST,
    READY_FOR_CALLBACK,
    PROCESSING_CALLBACK,

}
