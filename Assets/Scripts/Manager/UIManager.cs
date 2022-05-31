using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
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

    GameData _gameData;
    private void OnEnable()
    {
        GameManager.Instance.OnGameStart += OnStart;
    }
    private void OnDisable()
    {
        GameManager.Instance.OnGameStart -= OnStart;
    }
    private void OnStart()
    {
        _isPlaying = true;

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

    public void SetBallQueue(List<Color> i_colors)
    {
        if (i_colors.Count < _balls.Count)
            return;
        for(int i = 0; i < _balls.Count; i++)
        {
            _balls[i].color = new Color(i_colors[i].r, i_colors[i].g, i_colors[i].b, 1);
        }    
    }    
}
