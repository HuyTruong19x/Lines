using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    private Ball _ball;
    private bool _isShowed = false;
    public bool isBlocked { get { return _ball != null && _isShowed; } }
    private Vector2Int _location;

    //Support for PathFinding
    public int G = 0, H;
    public int F { get { return H + G; } }
    private Tile _previousTile;
  
    public void SetBall(Ball i_ball)
    {
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

}
