using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public int WIDTH = 9;
    public int HEIGHT = 9;

    private BallManager _ballManager;
    private Dictionary<Vector2Int, Tile> _tiles = new Dictionary<Vector2Int, Tile>();
    private void Start()
    {
        _ballManager = GameObject.FindObjectOfType<BallManager>();
    }

    private void OnEnable()
    {
        EventManager.Instance.RegisterEvent(GAMESTATE.SETUP, GenerateTiles);
    }

    private void OnDisable()
    {
        EventManager.Instance.RemoveEvent(GAMESTATE.SETUP, GenerateTiles);
    }

    private void GenerateTiles()
    {
        if(_tiles.Count < 1)
        {
            for (int i = 0; i < WIDTH; i++)
            {
                for (int y = 0; y < HEIGHT; y++)
                {
                    Tile tile = ObjectPool.Instance.TakeObject("tile").GetComponent<Tile>();
                    tile.SetLocation(i, y);
                    tile.gameObject.name = $"tile {i} - {y}";
                    tile.gameObject.transform.position = new Vector3(i, y, -0.1f);
                    _tiles.Add(new Vector2Int(i, y), tile);
                }
            }
        }
        else
        {
            //Rest tile
            for (int i = 0; i < WIDTH; i++)
            {
                for (int y = 0; y < HEIGHT; y++)
                {
                    Vector2Int location = new Vector2Int(i, y);
                    _tiles[location].ResetTile();
                }
            }
        }
        GameManager.Instance.ChangeGameState(GAMESTATE.STARTING);
    }
    public Tile GetTile(Vector2Int i_location)
    {
        if(_tiles.TryGetValue(i_location, out Tile tile))
        {
            return tile;
        }
        return null;
    }

    public Dictionary<Vector2Int, Tile> GetTiles()
    {
        return _tiles;
    }

    public void CheckScore(Tile i_tile)
    {
        List<Tile> listCheck = new List<Tile>();
        List<Tile> finishList = new List<Tile>();
        bool yang = true;
        bool yin = true;
        int x = i_tile.GetLocation().x;
        int y = i_tile.GetLocation().y;
        listCheck.Add(i_tile);
        int maxPoint = Mathf.Max(WIDTH, HEIGHT);

        #region vertical
        for (int i = 1; i < maxPoint - 1; i++)
        {
            Vector2Int yangNumber = new Vector2Int(x, y - i);
            Vector2Int yinNumber = new Vector2Int(x, y + i);

            if (_tiles.ContainsKey(yangNumber) && yang)
            {
                if (_tiles[yangNumber].GetBall() != null && _tiles[yangNumber].GetBall().Color == i_tile.GetBall().Color)
                {
                    listCheck.Add(_tiles[yangNumber]);
                }
                else yang = false;
            }
            else yang = false;

            if (_tiles.ContainsKey(yinNumber) && yin)
            {
                if (_tiles[yinNumber].GetBall() != null && _tiles[yinNumber].GetBall().Color == i_tile.GetBall().Color)
                {
                    listCheck.Add(_tiles[yinNumber]);
                }
                else yin = false;
            }
            else yin = false;

            if (!yin && !yang)
            {
                if (listCheck.Count >= 5)
                {
                    if (finishList.Contains(i_tile))
                        listCheck.Remove(i_tile);

                    finishList.AddRange(listCheck);
                    listCheck.Clear();
                }
                else
                    listCheck.Clear();

                break;
            }
        }
        #endregion

        #region horizontal
        listCheck.Add(i_tile);
        yang = true; yin = true;
        for (int i = 1; i < maxPoint - 1; i++)
        {
            Vector2Int yangNumber = new Vector2Int(x - i, y);
            Vector2Int yinNumber = new Vector2Int(x + i, y);

            if (_tiles.ContainsKey(yangNumber) && yang)
            {
                if (_tiles[yangNumber].GetBall() != null && _tiles[yangNumber].GetBall().Color == i_tile.GetBall().Color)
                {
                    listCheck.Add(_tiles[yangNumber]);
                }
                else yang = false;
            }
            else yang = false;

            if (_tiles.ContainsKey(yinNumber) && yin)
            {
                if (_tiles[yinNumber].GetBall() != null && _tiles[yinNumber].GetBall().Color == i_tile.GetBall().Color)
                {
                    listCheck.Add(_tiles[yinNumber]);
                }
                else yin = false;
            }
            else yin = false;

            if (!yin && !yang)
            {
                if (listCheck.Count >= 5)
                {
                    if (finishList.Contains(i_tile))
                        listCheck.Remove(i_tile);

                    finishList.AddRange(listCheck);
                    listCheck.Clear();
                }
                else
                    listCheck.Clear();

                break;
            }
        }
        #endregion

        #region diagonal left
        listCheck.Add(i_tile);
        yang = true; yin = true;
        for (int i = 1; i < maxPoint - 1; i++)
        {
            Vector2Int yangNumber = new Vector2Int(x - i, y + i);
            Vector2Int yinNumber = new Vector2Int(x + i, y - i);

            if (_tiles.ContainsKey(yangNumber) && yang)
            {
                if (_tiles[yangNumber].GetBall() != null && _tiles[yangNumber].GetBall().Color == i_tile.GetBall().Color)
                {
                    listCheck.Add(_tiles[yangNumber]);
                }
                else yang = false;
            }
            else yang = false;

            if (_tiles.ContainsKey(yinNumber) && yin)
            {
                if (_tiles[yinNumber].GetBall() != null && _tiles[yinNumber].GetBall().Color == i_tile.GetBall().Color)
                {
                    listCheck.Add(_tiles[yinNumber]);
                }
                else yin = false;
            }
            else yin = false;

            if (!yin && !yang)
            {
                if (listCheck.Count >= 5)
                {
                    if (finishList.Contains(i_tile))
                        listCheck.Remove(i_tile);

                    finishList.AddRange(listCheck);
                    listCheck.Clear();
                }
                else
                    listCheck.Clear();

                break;
            }
        }
        #endregion

        #region diagonal right
        listCheck.Add(i_tile);
        yang = true; yin = true;
        for (int i = 1; i < maxPoint - 1; i++)
        {
            Vector2Int yangNumber = new Vector2Int(x + i, y + i);
            Vector2Int yinNumber = new Vector2Int(x - i, y - i);

            if (_tiles.ContainsKey(yangNumber) && yang)
            {
                if (_tiles[yangNumber].GetBall() != null && _tiles[yangNumber].GetBall().Color == i_tile.GetBall().Color)
                {
                    listCheck.Add(_tiles[yangNumber]);
                }
                else yang = false;
            }
            else yang = false;

            if (_tiles.ContainsKey(yinNumber) && yin)
            {
                if (_tiles[yinNumber].GetBall() != null && _tiles[yinNumber].GetBall().Color == i_tile.GetBall().Color)
                {
                    listCheck.Add(_tiles[yinNumber]);
                }
                else yin = false;
            }
            else yin = false;

            if (!yin && !yang)
            {
                if (listCheck.Count >= 5)
                {
                    if (finishList.Contains(i_tile))
                        listCheck.Remove(i_tile);

                    finishList.AddRange(listCheck);
                    listCheck.Clear();
                }
                else
                    listCheck.Clear();

                break;
            }
        }
        #endregion

        if(finishList.Count >= 5)
        {
            _ballManager.DestroyBall(finishList);
        }    
    }
    private bool isInside(Vector2Int i_location)
    {
        return (i_location.x >= 0 && i_location.x < WIDTH && i_location.y >= 0 && i_location.y < HEIGHT) ;
    }    
}
