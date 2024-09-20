using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;

public static class TaskScheduler
{
    private static Stack<(List<UniTask>, Action, Action)> taskBatchQueue = new Stack<(List<UniTask>, Action, Action)>();
    private static bool isRunning = false;

    public static void EnqueueTask(UniTask task, Action onStartCallback = null, Action onCompleteCallback = null)
    {
        List<UniTask> taskBatch = new List<UniTask> { task };
        EnqueueTaskBatch(taskBatch, onStartCallback, onCompleteCallback);
    }

    public static void EnqueueTaskBatch(List<UniTask> taskBatch,Action onStartCallback = null, Action onCompleteCallback = null)
    {
        taskBatchQueue.Push((taskBatch, onStartCallback, onCompleteCallback));

        if (!isRunning)
        {
            ProcessQueue().Forget();
        }
    }


    private static async UniTaskVoid ProcessQueue()
    {
        isRunning = true;

        while (taskBatchQueue.Count > 0)
        {
            var (currentBatch, onStartCallback, onCompleteCallback) = taskBatchQueue.Pop();
            
            onStartCallback?.Invoke();

            await UniTask.WhenAll(currentBatch);

            onCompleteCallback?.Invoke();
        }

        isRunning = false;
    }

}
