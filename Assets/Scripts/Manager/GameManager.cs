using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField]
    private GameSetting _gameSetting;
    private UIManager _uiManager;
    private bool _isGameOver;
    [SerializeField]
    private GAMESTATE _gameState = GAMESTATE.PLAYING;
    [SerializeField]
    private GAMEMODE _gameMode = GAMEMODE.NONE;

    //Score
    private int _score;
    private const int INSCREASESCORE = 10;

    public int NumSpawn = 3;
    public bool CanPlay { get { return !_isGameOver && _gameState == GAMESTATE.PLAYING; } }
    public GAMEMODE GameMode { get { return _gameMode; } }

    public Color GetRandomColor()
    {
        if (_gameSetting == null)
        {
            Debug.LogWarning("Can't get random color due to _gameSetting is null, return default color : black");
            return Color.black;
        }    
        return _gameSetting.Colors[UnityEngine.Random.Range(0, _gameSetting.Colors.Count)];
    }    
    public List<BallData> GetBallData()
    {
        return _gameSetting.Rates;
    }   
    public void ChangeGameState(GAMESTATE i_gameState)
    {
        _gameState = i_gameState;
        if(i_gameState == GAMESTATE.GAMEOVER)
        {
            _isGameOver = true;
            GameObject.FindObjectOfType<GameData>()?.UpdateHightScore(_score);
        }
        else if(i_gameState == GAMESTATE.SETUP)
        {
            _isGameOver = false;
        }    
        EventManager.Instance.InvokeEvent((GAMEEVENT)i_gameState);
    }    
    public void ChangeGameMode(GAMEMODE i_gameMode, bool i_isTriggerEvent = true)
    {
        _gameMode = i_gameMode;
        if(i_isTriggerEvent)
        {
            EventManager.Instance.InvokeEvent(GAMEEVENT.CHANGEDGAMEMODE);
        }    
    }

    public void IncreaseScore(int i_numBall)
    {
        _score += INSCREASESCORE * i_numBall;
        if(_uiManager == null)
        {
            _uiManager = GameObject.FindObjectOfType<UIManager>();
        }    
        _uiManager?.ShowScore(_score);
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

public enum GAMEMODE
{
    NONE,
    ARMODE
}
