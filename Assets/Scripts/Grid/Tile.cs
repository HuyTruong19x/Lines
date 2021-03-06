using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    private Ball _ball;
    private bool _isShowed = false;
    public bool isBlocked { get { return _ball != null && _isShowed; } }
    public bool hasBall { get { return _ball != null; } }
    private Vector2Int _location;

    //Support for PathFinding
    public int G = 0, H;
    public int F { get { return H + G; } }
    private Tile _previousTile;
  
    public void SetBall(Ball i_ball)
    {
        if(_ball != null)
        {
            _ball = null;
        }
        _ball = i_ball;
    }    
    public void SetShowed(bool i_isShowed)
    {
        _isShowed = i_isShowed;
    }    
    public void SetLocation(int X, int Y)
    {
        _location = new Vector2Int(X, Y);
    }
    public void SetPreviousTile(Tile i_tile)
    {
        _previousTile = i_tile;
    }
    public Ball GetBall()
    {
        return _ball;
    }
    public Vector2Int GetLocation()
    {
        return _location;
    }

    public Tile GetPreviousTile()
    {
        return _previousTile;
    }

    public void UpdateBall()
    {
        if(_ball != null)
        {
            _ball.gameObject.transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 0);
            _ball.gameObject.SetActive(gameObject.activeSelf);
        }
    }

    public void ResetTile()
    {
        _ball?.gameObject.SetActive(false);
        _ball = null;
        _isShowed = false;
        this.gameObject.SetActive(false);
    }

}
