using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public class Ball : MonoBehaviour
{
    private MeshRenderer _mesh;
    private bool _isFinished;
    private Vector2Int _location;
    private Color _color;
    private Tween _ballTween;
    private GridManager _gridManager;
    [SerializeField]
    private GameObject _ghostObject;

    public bool isGhost = false;
    public Color Color { get{ return _color; } }
    private void Awake()
    {
        _mesh = GetComponent<MeshRenderer>();
    }
    public void FinishBall()
    {
        if(!_isFinished)
        {
            _isFinished = true;
            transform.DOScale(Vector3.one * BallManager.MAXIMUM, 0.2f);
            _gridManager.GetTile(_location).SetShowed(true);
        }    
    }

    public void Selected(bool i_isSelected)
    {
        if(i_isSelected)
        {
            _ballTween = transform.DOScale(0.6f, 0.5f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
        }
        else
        {
            transform.DOScale(Vector3.one * BallManager.MAXIMUM, 0.2f);
            _ballTween?.SetLoops(0);
            _ballTween?.Complete();
            _ballTween.Kill();
        }
    }
    public void Setup(GridManager i_grid, Vector2Int i_location, Color i_color, bool i_isGhost)
    {
        _gridManager = i_grid;
        _location = i_location;
        _color = i_color;
        _mesh.material.color = i_color;
        _ghostObject?.SetActive(i_isGhost);
        isGhost = i_isGhost;
    }
    public void SetColor(Color i_color)
    {
        _color = i_color;
        _mesh.material.color = i_color;
    }
    public void SetFinished(bool i_isFinished)
    {
        _isFinished = i_isFinished;
    }    
    public void SetLocation(Vector2Int i_location)
    {
        _location = i_location;
    }  
    public void SetGhostMode(bool i_isGhost)
    {
        _ghostObject?.SetActive(i_isGhost);
        isGhost = i_isGhost;
    }
    public Vector2Int GetLocation()
    {
        return _location;
    }    
}
