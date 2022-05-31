using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour
{
    private int _hightScore = 0;
    private string _hightScoreKey = "HightScore";
    private void Awake()
    {
        _hightScore = PlayerPrefs.GetInt(_hightScoreKey, 0);
    }

    public void UpdateHightScore(int i_value)
    {
        if(i_value > _hightScore)
        {
            _hightScore = i_value;
            PlayerPrefs.SetInt(_hightScoreKey, i_value);
        }    
    }
    public int GetHightScore()
    {
        return _hightScore;
    }    
}
