using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class GridManager : MonoBehaviour
{
    public int WIDTH = 9;
    public int HEIGHT = 9;

    private BallManager _ballManager;
    private Dictionary<Vector2Int, Tile> _tiles = new Dictionary<Vector2Int, Tile>();

    //AR mode
    private ARRaycastManager _arRaycastManager;
    private List<ARRaycastHit> _hits = new List<ARRaycastHit>();
    private RaycastHit _hitInfo;
    private bool _isDetecting = false;
    private Vector2Int _firstTileLocation = Vector2Int.zero;
    private void Start()
    {
        _ballManager = GameObject.FindObjectOfType<BallManager>();
        if (GameManager.Instance.GameMode == GAMEMODE.ARMODE)
        {
            _arRaycastManager = GameObject.FindObjectOfType<ARRaycastManager>();
        }
    }

    private void OnEnable()
    {
        EventManager.Instance.RegisterEvent(GAMEEVENT.SETUP, GenerateTiles);
        EventManager.Instance.RegisterEvent(GAMEEVENT.CHANGEDGAMEMODE, UpdateGameMode);
    }

    private void OnDisable()
    {
        EventManager.Instance.RemoveEvent(GAMEEVENT.SETUP, GenerateTiles);
        EventManager.Instance.RemoveEvent(GAMEEVENT.CHANGEDGAMEMODE, UpdateGameMode);
    }

    private void Update()
    {
        if (GameManager.Instance.GameMode == GAMEMODE.ARMODE)
        {
            if(_isDetecting)
            {
                if (Input.touchCount == 0)
                {
                    return;
                }    
                Touch touch = Input.GetTouch(0);
                if(_arRaycastManager.Raycast(touch.position, _hits))
                {
                    if(touch.phase == TouchPhase.Ended)
                    {
                        Ray ray = Camera.main.ScreenPointToRay(touch.position);
                        if (Physics.Raycast(ray, out _hitInfo))
                        {
                            _firstTileLocation = new Vector2Int((int)_hitInfo.collider.transform.position.x, (int)_hitInfo.collider.transform.position.y);
                            _isDetecting = false;
                            GenerateTiles(_firstTileLocation.x, _firstTileLocation.y);
                            GameManager.Instance.ChangeGameState(GAMESTATE.STARTING);
                            Debug.Log("Finish detect plane");
                        }    
                    }    
                }
            }    
        }
    }

    private void UpdateGameMode()
    {
        if(GameManager.Instance.GameMode == GAMEMODE.ARMODE)
        {
            if (_firstTileLocation != Vector2Int.zero)
            {
                GenerateTiles(_firstTileLocation.x, _firstTileLocation.y);
            }
            else
            {
                _isDetecting = true;
            }
        }    
        else
        {
            GenerateTiles(0, 0);
        }    
    }    
    private void GenerateTiles()
    {
        ResetTiles();
        UpdateGameMode();  
    }

    private void GenerateTiles(int i_StartX, int i_StartY)
    {
        if (_tiles.Count < 1)
        {
            for (int i = 0; i < WIDTH; i++)
            {
                for (int y = 0; y < HEIGHT; y++)
                {
                    Tile tile = ObjectPool.Instance.TakeObject("tile").GetComponent<Tile>();
                    tile.SetLocation(i, y);
                    tile.gameObject.name = $"tile {i} - {y}";
                    tile.gameObject.transform.position = new Vector3(i_StartX + i, i_StartY + y, -0.1f);
                    _tiles.Add(new Vector2Int(i, y), tile);
                }
            }
        }
        else
        {
            //Reset position if already available
            for (int i = 0; i < WIDTH; i++)
            {
                for (int y = 0; y < HEIGHT; y++)
                {
                    Vector2Int location = new Vector2Int(i, y);
                    _tiles[location].gameObject.transform.position = new Vector3(i_StartX + i, i_StartY + y, -0.1f);
                    _tiles[location].UpdateBallPosition();
                }
            }
        }
        GameManager.Instance.ChangeGameState(GAMESTATE.STARTING);
    }

    private void ResetTiles()
    {
        if (_tiles.Count < 1)
            return;
        for (int i = 0; i < WIDTH; i++)
        {
            for (int y = 0; y < HEIGHT; y++)
            {
                Vector2Int location = new Vector2Int(i, y);
                _tiles[location].ResetTile();
            }
        }
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
