using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    //Score
    [SerializeField]
    TextMeshProUGUI _hightScore;
    [SerializeField]
    TextMeshProUGUI _score;

    //Timer
    [SerializeField]
    TextMeshProUGUI _timer;
    float _currentTime = 0;
    bool _isPlaying = false;

    //Queue ball
    [SerializeField]
    List<RawImage> _balls;
    [SerializeField]
    GameObject _gameOverPopup;

    GameData _gameData;
    private void OnEnable()
    {
        EventManager.Instance.RegisterEvent(GAMEEVENT.SETUP, Initialize);
        EventManager.Instance.RegisterEvent(GAMEEVENT.STARTING, OnStart);
        EventManager.Instance.RegisterEvent(GAMEEVENT.GAMEOVER, ShowGameOver);
    }
    private void OnDisable()
    {
        EventManager.Instance.RemoveEvent(GAMEEVENT.SETUP, Initialize);
        EventManager.Instance.RemoveEvent(GAMEEVENT.STARTING, OnStart);
        EventManager.Instance.RemoveEvent(GAMEEVENT.GAMEOVER, ShowGameOver);
    }
    private void Initialize()
    {
        _currentTime = 0;
    }    
    private void OnStart()
    {
        _isPlaying = true;
        ShowScore(0);

        _gameData = FindObjectOfType<GameData>();
        if(_gameData != null)
        {
            ShowHightScore(_gameData.GetHightScore());
        }    
        else
        {
            Debug.LogWarning("Can't update hight score due to cant find game data");
        }    
    }

    private void Update()
    {
        if( _isPlaying )
        {
            _currentTime += Time.deltaTime;
            _timer.text = GetTimeFormat(_currentTime);
        }    
    }

    private string GetTimeFormat(float i_value)
    {
        TimeSpan time = TimeSpan.FromSeconds((double)i_value);  
        return time.ToString(@"hh\:mm\:ss");
    }    

    public void ShowScore(int i_score)
    {
        _score.text = i_score.ToString("00000");
    }    

    public void ShowHightScore(int i_hightScore)
    {
        _hightScore.text = i_hightScore.ToString("00000");
    }    
    public void ShowGameOver()
    {
        SoundManager.Instance.PlaySFX(SFX.GAMEOVER);
        _gameOverPopup.SetActive(true);
        _gameOverPopup.GetComponent<IPopup>().Open(null);
    }
    public void SetBallQueue(List<Color> i_colors)
    {
        if (i_colors.Count < _balls.Count)
            return;
        //Set aplha = 1 to make sure color always display
        for(int i = 0; i < _balls.Count; i++)
        {
            _balls[i].color = new Color(i_colors[i].r, i_colors[i].g, i_colors[i].b, 1);
        }    
    }    

    public void OnGoToARSceneClick()
    {
        GameObject.FindObjectOfType<GamePermission>().RequestCameraPermission((hasPermission)=>
        {
            if(hasPermission)
            {
                GameManager.Instance.ChangeGameMode(GAMEMODE.ARMODE, false);
                SceneManager.LoadScene(1);
            }    
            else
            {
                Debug.Log("Camera permision denied");
            }    
        });
    }    
}
