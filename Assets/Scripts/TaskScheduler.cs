using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public static class TaskScheduler
{
    private static Queue<(List<UniTask>, Action, Action)> taskBatchQueue = new Queue<(List<UniTask>, Action, Action)>();
    private static bool isRunning = false;

    public static void EnqueueTask(UniTask task)
    {
        List<UniTask> taskBatch = new List<UniTask> { task };
        EnqueueTaskBatch(taskBatch, null);
    }

    public static void EnqueueTaskBatch(List<UniTask> taskBatch,Action onStartCallback = null, Action onCompleteCallback = null)
    {
        taskBatchQueue.Enqueue((taskBatch, onStartCallback, onCompleteCallback));

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
            var (currentBatch, onStartCallback, onCompleteCallback) = taskBatchQueue.Dequeue();
            
            onStartCallback?.Invoke();

            await UniTask.WhenAll(currentBatch);

            onCompleteCallback?.Invoke();
        }

        isRunning = false;
    }

}
