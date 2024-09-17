using System;
using System.Collections.Generic;

public class EventAggregator
{
    private static EventAggregator instance;
    private Dictionary<Type, List<Action<GameEvent>>> eventListeners = new Dictionary<Type, List<Action<GameEvent>>>();

    private EventAggregator() { }

    public static EventAggregator GetInstance()
    {
        if (instance == null)
        {
            instance = new EventAggregator();
        }
        return instance;
    }


    public void Subscribe<T>(Action<T> listener) where T : GameEvent
    {
        Type eventType = typeof(T);
        if (!eventListeners.ContainsKey(eventType))
        {
            eventListeners[eventType] = new List<Action<GameEvent>>();
        }
        eventListeners[eventType].Add(e => listener((T)e));
    }

    public void Unsubscribe<T>(Action<T> listener) where T : GameEvent
    {
        Type eventType = typeof(T);
        if (eventListeners.ContainsKey(eventType))
        {
            eventListeners[eventType].Remove(e => listener((T)e));
        }
    }

    public void Publish(GameEvent gameEvent)
    {
        Type eventType = gameEvent.GetType();
        if (eventListeners.ContainsKey(eventType))
        {
            foreach (var listener in eventListeners[eventType])
            {
                listener(gameEvent);
            }
        }
    }
    public void ResetEventListeners()
    {
        eventListeners = new Dictionary<Type, List<Action<GameEvent>>>();
    }
}
