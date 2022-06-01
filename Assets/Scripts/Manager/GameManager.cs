using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : Singleton<GameManager>
{
    [SerializeField]
    private GameSetting _gameSetting;
    private bool _isGameOver;
    private GAMESTATE _gameState = GAMESTATE.PLAYING;

    //Score
    private int _score;
    private const int INSCREASESCORE = 10;

    public int NumSpawn = 3;
    public bool CanPlay { get { return !_isGameOver && _gameState == GAMESTATE.PLAYING; } }

    private void Start()
    {
        EventManager.Instance.InvokeEvent(GAMESTATE.SETUP);
    }


    public Color GetRandomColor()
    {
        if (_gameSetting == null)
        {
            Debug.LogWarning("Can't get random color due to _gameSetting is null, return default color : black");
            return Color.black;
        }    
        return _gameSetting.Colors[UnityEngine.Random.Range(0, _gameSetting.Colors.Count)];
    }    
    public int GetRateSpawnGhostBall()
    {
        return _gameSetting.RateSpawnGhostBall;
    }
    public void EndTurn()
    {
        ChangeGameState(GAMESTATE.ENDTURN);
    }    
    public void ChangeGameState(GAMESTATE i_gameState)
    {
        _gameState = i_gameState;
        if(i_gameState == GAMESTATE.GAMEOVER)
        {
            _isGameOver = true;
            GameObject.FindObjectOfType<GameData>()?.UpdateHightScore(_score);
        }
        EventManager.Instance.InvokeEvent(i_gameState);
    }    

    public void IncreaseScore()
    {
        _score += INSCREASESCORE;
        UIManager.Instance.ShowScore(_score);
    }    
    public int GetScore()
    {
        return _score;
    }
}

public enum GAMESTATE
{
    NONE,
    SETUP,
    STARTING,
    WAITING,
    MOVINGBALL,
    PLAYING,
    ENDTURN,
    GAMEOVER
}
