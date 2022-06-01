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
    private BALLTYPE _ballType;

    //Move ball
    private float _smoothMove = 0.1f;
    private List<Tile> _paths;
    private bool _isDoMove = false;
    private Vector3 _targetMove;

    public bool IsGhost { get { return _ballType == BALLTYPE.GHOST; } }
    public Color Color { get{ return _color; } }
    private void Awake()
    {
        _mesh = GetComponent<MeshRenderer>();
        _paths = new List<Tile>();
    }

    private void FixedUpdate()
    {
        if (_isDoMove)
        {
            transform.position = Vector3.MoveTowards(transform.position, _targetMove, _smoothMove);
            if (Vector2.Distance(transform.position, _targetMove) < 0.2f)
            {
                if (_paths.Count < 1)
                {
                    _isDoMove = false;
                    transform.position = _targetMove;
                    transform.DOScale(Vector3.one * BallManager.MAXIMUM, 0.3f);
                    _gridManager.CheckScore(_gridManager.GetTile(_location));
                    GameManager.Instance.EndTurn();
                }
                if (_paths.Count > 0)
                {
                    _location = _paths[0].GetLocation();
                    _targetMove = _paths[0].transform.position;
                    _targetMove.z = 0;
                    _paths.RemoveAt(0);
                }
            }

        }
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
    public void MoveTo(List<Tile> i_paths)
    {
        _paths = i_paths;
        transform.DOScale(Vector3.one * BallManager.MINIMUM * 1.5f, 0.2f).OnComplete(() =>
        {
            _location = _paths[0].GetLocation();
            _targetMove = _paths[0].transform.position;
            _targetMove.z = 0;
            _paths.RemoveAt(0);
            _isDoMove = true;
        }).Play();
    }    
    public void Setup(GridManager i_grid, Vector2Int i_location, Color i_color, BALLTYPE i_ballType)
    {
        _gridManager = i_grid;
        _location = i_location;
        _color = i_color;
        _mesh.material.color = i_color;
        _ghostObject?.SetActive(i_ballType == BALLTYPE.GHOST);
        _ballType = i_ballType;
    }

    public void UpdateBallSize()
    {
        if(_isFinished)
        {
            transform.localScale = Vector3.one * BallManager.MAXIMUM;
        }    
        else
        {
            transform.localScale = Vector3.one * BallManager.MINIMUM;
        }    
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
    public void SetBallMode(BALLTYPE i_ballType)
    {
        _ghostObject?.SetActive(i_ballType == BALLTYPE.GHOST);
        _ballType = i_ballType;
    }
    public Vector2Int GetLocation()
    {
        return _location;
    }    
}


public enum BALLTYPE
{
    NONE,
    GHOST
}