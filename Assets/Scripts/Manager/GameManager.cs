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

    #region Game Event
    public UnityAction OnEndTurn;
    public UnityAction OnWaitingTurn;
    #endregion


    public int NumSpawn = 3;
    public bool CanPlay { get { return !_isGameOver && _gameState == GAMESTATE.PLAYING; } }
    public Color GetRandomColor()
    {
        if (_gameSetting == null)
        {
            Debug.LogWarning("Can't get random color due to _gameSetting is null, return default color : black");
            return Color.black;
        }    
        return _gameSetting.Colors[UnityEngine.Random.Range(0, _gameSetting.Colors.Count)];
    }    
    public void EndTurn()
    {
        ChangeGameState(GAMESTATE.ENDTURN);
    }    
    public void ChangeGameState(GAMESTATE i_gameState)
    {
        _gameState = i_gameState;

        switch(_gameState)
        {
            case GAMESTATE.PLAYING:break;
            case GAMESTATE.ENDTURN: OnEndTurn?.Invoke(); break;
            case GAMESTATE.WAITING: OnWaitingTurn?.Invoke(); break;
        }    
    }    
}

public enum GAMESTATE
{
    NONE,
    WAITING,
    PLAYING,
    ENDTURN,
    GAMEOVER
}
