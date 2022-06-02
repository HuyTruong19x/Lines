using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public class Ball : MonoBehaviour
{
    [SerializeField]
    private Renderer _renderer;
    private bool _isFinished;
    private Vector2Int _location;
    private Color _color;
    private Tween _ballTween;
    private GridManager _gridManager;
    [SerializeField]
    private GameObject _ghostObject;
    private BALLTYPE _ballType;
    private float _smoothRotate = 5f;

    //Move ball
    private float _smoothMove = 0.1f;
    private List<Tile> _paths;
    private bool _isDoMove = false;
    private Vector3 _targetMove;
    [SerializeField]
    private Material _defaultMaterial;

    public bool IsGhost { get { return _ballType == BALLTYPE.GHOST; } }
    private void Awake()
    {
        _paths = new List<Tile>();
    }

    private void FixedUpdate()
    {
        if(_ballType == BALLTYPE.RAINBOW)
        {
            transform.Rotate(0, _smoothRotate, 0, Space.Self);
        }    
        if (_isDoMove)
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, _targetMove, _smoothMove);
            if (Vector2.Distance(transform.localPosition, _targetMove) < 0.2f)
            {
                if (_paths.Count < 1)
                {
                    _isDoMove = false;
                    transform.localPosition = _targetMove;
                    transform.DOScale(Vector3.one * BallManager.MAXIMUM, 0.3f);
                    _gridManager.CheckScore(_gridManager.GetTile(_location));

                    if(_ballType == BALLTYPE.HORIZONTAL)
                    {
                        PainOtherBall();
                    }    

                    GameManager.Instance.EndTurn();
                }
                if (_paths.Count > 0)
                {
                    _location = _paths[0].GetLocation();
                    _targetMove = _paths[0].transform.localPosition;
                    _targetMove.z = 0;
                    _paths.RemoveAt(0);
                }
            }

        }
    }
    public bool CompareColor(Ball i_ball)
    {
        if (_color == i_ball._color || _ballType == BALLTYPE.RAINBOW || i_ball._ballType == BALLTYPE.RAINBOW)
        {
            return true;
        }    
        return false;
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

    public void PainOtherBall()
    {
        var leftLocation = new Vector2Int(_location.x - 1, _location.y);
        if(leftLocation.x > 0)
        {
            _gridManager.GetTile(leftLocation).GetBall()?.SetColor(_color);
        }    
        var rightLocation = new Vector2Int(_location.x + 1, _location.y);
        if(rightLocation.x < _gridManager.WIDTH)
        {
            _gridManager.GetTile(rightLocation).GetBall()?.SetColor(_color);
        }
        _ballType = BALLTYPE.NONE;
        _renderer.material = _defaultMaterial;
        _renderer.material.color = _color;
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
        transform.DOScale(Vector3.one * BallManager.MINIMUM * 1.6f, 0.2f).OnComplete(() =>
        {
            _location = _paths[0].GetLocation();
            _targetMove = _paths[0].transform.localPosition;
            _targetMove.z = 0;
            _paths.RemoveAt(0);
            _isDoMove = true;
        }).Play();
    }    
    public void Setup(GridManager i_grid, Vector2Int i_location, Color i_color, BALLTYPE i_ballType, Material i_material)
    {
        _gridManager = i_grid;
        _location = i_location;
        _ghostObject?.SetActive(i_ballType == BALLTYPE.GHOST);
        _ballType = i_ballType;
        
        if(i_material != null)
        {
            _renderer.material = i_material;
        }    

        if (i_ballType != BALLTYPE.RAINBOW)
        {
            _color = i_color;
            _renderer.material.color = _color;
        }    
    }

    public void UpdateBallSize()
    {
        if(_isFinished)
        {
            transform.localScale = Vector3.one * BallManager.MAXIMUM;
        }    
        else
        {
            transform.localScale = Vector3.one * BallManager.MINIMUM * 1.5f;
        }    
    }    
    public void SetColor(Color i_color)
    {
        if(_ballType == BALLTYPE.NONE)
        {
            _color = i_color;
            _renderer.material.color = _color;
        }
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
[System.Serializable]
public class BallMaterial
{
    public BALLTYPE Type;
    public Material Material;
}

public enum BALLTYPE
{
    NONE,
    GHOST,
    RAINBOW,
    HORIZONTAL
}