using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public static class TaskScheduler
{
    private static Queue<(List<UniTask>, Action)> taskBatchQueue = new Queue<(List<UniTask>, Action)>();
    private static bool isRunning = false;

    public static void EnqueueTask(UniTask task)
    {
        List<UniTask> taskBatch = new List<UniTask> { task };
        EnqueueTaskBatch(taskBatch, null);
    }

    public static void EnqueueTaskBatch(List<UniTask> taskBatch, Action onCompleteCallback = null)
    {
        taskBatchQueue.Enqueue((taskBatch, onCompleteCallback));

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
            var (currentBatch, callback) = taskBatchQueue.Dequeue();

            await UniTask.WhenAll(currentBatch);

            callback?.Invoke();

            Debug.Log("Batch completed");
        }

        isRunning = false;
    }
}
