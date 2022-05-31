using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    private Tile _selectedTileWithBall;
    private Ray _ray;
    private RaycastHit _hitInfo;
    private PathFinding _pathFinding;
    private List<Tile> _paths;
    private void Start()
    {
        _pathFinding = new PathFinding();
        _paths = new List<Tile>();
    }
    private void Update()
    {
        if(GameManager.Instance.CanPlay)
        {
            InputInteract();
        }    
    }
#if UNITY_STANDALONE_WIN
    private void InputInteract()
    {
        if(Input.GetMouseButtonDown(0))
        {
            _ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(_ray, out _hitInfo))
            {
                HandleInput(_hitInfo);   
            }    
        }    
    }
#else
    private void InputInteract()
    {
        if(Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if(touch.phase == TouchPhase.Ended)
            {
                _ray = Camera.main.ScreenPointToRay(touch.position);
                if (Physics.Raycast(_ray, out _hitInfo))
                {
                    HandleInput(_hitInfo);
                }
            }    
        }    
    }
#endif
    private void HandleInput(RaycastHit i_hitInfo)
    {
        Tile currentTile = i_hitInfo.collider.GetComponent<Tile>();
        Debug.Log($"Has ball {currentTile.GetBall() != null}");
        if (_selectedTileWithBall == null)
        {
            if(currentTile.GetBall() != null)
            { _selectedTileWithBall = currentTile; }
        }
        else if (!currentTile.isBlocked)
        {
            Vector2Int location = currentTile.GetLocation();

            _paths = _pathFinding.FindPath(_selectedTileWithBall, currentTile);
            if(_paths.Count < 1)
            {
                //Todo cancel select
            }    
            else
            {
                Ball ball = _selectedTileWithBall.GetBall();
                ball.transform.position = new Vector3(location.x, location.y, 0);
                Debug.Log($"Move ball to location : {location.x} - {location.y}");
                currentTile.SetBall(ball);
                currentTile.SetShowed(true);
                _selectedTileWithBall.SetBall(null);
                _selectedTileWithBall = null;
                GameManager.Instance.EndTurn();
            }    
            //Todo use path finding to move object
        }
        else
        {
            _selectedTileWithBall = currentTile;
        }    
    }    
}
