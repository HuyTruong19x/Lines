using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventManager : Singleton<EventManager>
{
    Dictionary<GAMESTATE, UnityAction> _actions = new Dictionary<GAMESTATE, UnityAction>();
    public void RegisterEvent(GAMESTATE i_key, UnityAction i_action)
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
    public void RemoveEvent(GAMESTATE i_key, UnityAction i_action)
    {
        if (_actions.ContainsKey(i_key))
        {
            _actions[i_key] -= i_action;
        }
    }

    public void InvokeEvent(GAMESTATE i_key)
    {
        if(_actions.ContainsKey(i_key))
        {
            _actions[i_key]?.Invoke();
        }
    }
}
