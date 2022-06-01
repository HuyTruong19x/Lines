using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventManager : Singleton<EventManager>
{
    Dictionary<GAMEEVENT, UnityAction> _actions = new Dictionary<GAMEEVENT, UnityAction>();
    public void RegisterEvent(GAMEEVENT i_key, UnityAction i_action)
    {
        if (_actions.ContainsKey(i_key))
        {
            _actions[i_key] += i_action;
        }
        else
        {
            _actions.Add(i_key, i_action);
        }
    }
    public void RemoveEvent(GAMEEVENT i_key, UnityAction i_action)
    {
        if (_actions.ContainsKey(i_key))
        {
            _actions[i_key] -= i_action;
        }
    }

    public void InvokeEvent(GAMEEVENT i_key)
    {
        if(_actions.ContainsKey(i_key))
        {
            _actions[i_key]?.Invoke();
        }
    }
}

public enum GAMEEVENT
{
    NONE,
    SETUP,
    STARTING,
    WAITING,
    MOVINGBALL,
    PLAYING,
    ENDTURN,
    GAMEOVER,
    CHANGEDGAMEMODE,
}
