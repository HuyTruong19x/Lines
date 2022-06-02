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

    public Transform Grid { get { return _parentGrid; } set { _parentGrid = value; } }
    [SerializeField]
    private UIManager _uiManager;
    [SerializeField]
    private BallManager _ballManager;
    [SerializeField]
    private Transform _parentGrid;
    private Dictionary<Vector2Int, Tile> _tiles = new Dictionary<Vector2Int, Tile>();

    //AR mode
    private ARRaycastManager _arRaycastManager;
    private List<ARRaycastHit> _hits = new List<ARRaycastHit>();
    private bool _isDetecting = false;
    private Vector3 _lastDetectedPosition = Vector3.zero;
    private Quaternion _lastDetectedRotation = Quaternion.identity;
    private float _scaleObjectARMode = 0.15f;
    private void Start()
    {
        _parentGrid.Reset();
        if (GameManager.Instance.GameMode == GAMEMODE.ARMODE)
        {
            _arRaycastManager = GameObject.FindObjectOfType<ARRaycastManager>();
        }
    }

    private void OnEnable()
    {
        EventManager.Instance.RegisterEvent(GAMEEVENT.SETUP, GridInitialize);
        EventManager.Instance.RegisterEvent(GAMEEVENT.CHANGEDGAMEMODE, UpdateGameMode);
    }

    private void OnDisable()
    {
        EventManager.Instance.RemoveEvent(GAMEEVENT.SETUP, GridInitialize);
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
                if(_arRaycastManager.Raycast(touch.position, _hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon))
                {
                    _isDetecting = false;
                    GenerateTiles();

                    _lastDetectedPosition = _hits[0].pose.position;
                    _lastDetectedRotation = Quaternion.Euler(90f, 0f, 0f);
                    _parentGrid.localPosition = _lastDetectedPosition;
                    _parentGrid.localRotation = _lastDetectedRotation;
                    _parentGrid.localScale = Vector3.one * _scaleObjectARMode;

                    _uiManager.SetActiveNotification(false);
                    if(!_ballManager.IsInitialized)
                    {
                        StartInitializeBall();
                    }    
                    Debug.Log("Finish detect plane");  
                }
            }    
        }
    }

    private void UpdateGameMode()
    {
#if !UNITY_EDITOR
        if(GameManager.Instance.GameMode == GAMEMODE.ARMODE)
        {
            //Todo handle switch case
            if (_lastDetectedPosition != Vector3.zero)
            {
                GenerateTiles();
                _parentGrid.localPosition = _lastDetectedPosition;
                _parentGrid.localRotation = _lastDetectedRotation;
                _parentGrid.localScale = Vector3.one * _scaleObjectARMode;
            }
            else
            {
                _uiManager.SetActiveNotification(true);
                if(_ballManager.IsInitialized)
                {
                    HideTiles();
                }    
                _isDetecting = true;
            }
        }    
        else
#endif
        {
            _uiManager.SetActiveNotification(false);
            _parentGrid.Reset();
            GenerateTiles();
            if (!_ballManager.IsInitialized)
            {
                StartInitializeBall();
            }
        }    
    }   
    private void StartInitializeBall()
    {
        _ballManager?.InitializeBall();
    }    
    private void GridInitialize()
    {
        ResetTiles();
        UpdateGameMode();
        if(GameManager.Instance.GameMode == GAMEMODE.NONE && !_ballManager.IsInitialized)
        {
            StartInitializeBall();
        }    
    }
    private void HideTiles()
    {
        for (int i = 0; i < WIDTH; i++)
        {
            for (int y = 0; y < HEIGHT; y++)
            {
                Vector2Int location = new Vector2Int(i, y);
                if (_tiles[location].gameObject.activeSelf)
                {
                    _tiles[location].gameObject.SetActive(false);
                }
                _tiles[location].UpdateBall();
            }
        }
    }    
    private void GenerateTiles()
    {
        int startX = -(WIDTH / 2);
        int startY = -(HEIGHT / 2);
        if (_tiles.Count < 1)
        {
            for (int i = 0; i < WIDTH; i++)
            {
                for (int y = 0; y < HEIGHT; y++)
                {
                    Tile tile = ObjectPool.Instance.TakeObject("tile").GetComponent<Tile>();
                    tile.SetLocation(i, y);
                    tile.gameObject.name = $"tile {i} - {y}";
                    tile.gameObject.transform.localPosition = new Vector3(startX + i, startY +y, -0.1f);
                    tile.transform.parent = _parentGrid;
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
                    if(!_tiles[location].gameObject.activeSelf)
                    {
                        _tiles[location].gameObject.SetActive(true);
                    }
                    _tiles[location].gameObject.transform.localPosition = new Vector3(startX + i, startX + y, -0.1f);
                    _tiles[location].UpdateBall();
                }
            }
        }
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
                if (_tiles[yangNumber].GetBall() != null && _tiles[yangNumber].GetBall().CompareColor(i_tile.GetBall()))
                {
                    listCheck.Add(_tiles[yangNumber]);
                }
                else yang = false;
            }
            else yang = false;

            if (_tiles.ContainsKey(yinNumber) && yin)
            {
                if (_tiles[yinNumber].GetBall() != null && _tiles[yinNumber].GetBall().CompareColor(i_tile.GetBall()))
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

                    if (CheckList(listCheck))
                    {
                        finishList.AddRange(listCheck);
                    }
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
                if (_tiles[yangNumber].GetBall() != null && _tiles[yangNumber].GetBall().CompareColor(i_tile.GetBall()))
                {
                    listCheck.Add(_tiles[yangNumber]);
                }
                else yang = false;
            }
            else yang = false;

            if (_tiles.ContainsKey(yinNumber) && yin)
            {
                if (_tiles[yinNumber].GetBall() != null && _tiles[yinNumber].GetBall().CompareColor(i_tile.GetBall()))
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
                    if(CheckList(listCheck))
                    {
                        finishList.AddRange(listCheck);
                    }
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
                if (_tiles[yangNumber].GetBall() != null && _tiles[yangNumber].GetBall().CompareColor(i_tile.GetBall()))
                {
                    listCheck.Add(_tiles[yangNumber]);
                }
                else yang = false;
            }
            else yang = false;

            if (_tiles.ContainsKey(yinNumber) && yin)
            {
                if (_tiles[yinNumber].GetBall() != null && _tiles[yinNumber].GetBall().CompareColor(i_tile.GetBall()))
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

                    if (CheckList(listCheck))
                    {
                        finishList.AddRange(listCheck);
                    }
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
                if (_tiles[yangNumber].GetBall() != null && _tiles[yangNumber].GetBall().CompareColor(i_tile.GetBall()))
                {
                    listCheck.Add(_tiles[yangNumber]);
                }
                else yang = false;
            }
            else yang = false;

            if (_tiles.ContainsKey(yinNumber) && yin)
            {
                if (_tiles[yinNumber].GetBall() != null && _tiles[yinNumber].GetBall().CompareColor(i_tile.GetBall()))
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

                    if (CheckList(listCheck))
                    {
                        finishList.AddRange(listCheck);
                    }
                    listCheck.Clear();
                }
                else
                    listCheck.Clear();

                break;
            }
        }
        #endregion

        for(int i = 0; i < finishList.Count - 1; i++)
        {
            for(int j = i + 1; j < finishList.Count; j++)
            {
                if(!finishList[i].GetBall().CompareColor(finishList[j].GetBall()))
                {
                    return;
                }    
            }    
        }    

        if(finishList.Count >= 5)
        {
            _ballManager.DestroyBall(finishList);
        }    
    }  
    private bool CheckList(List<Tile> i_listCHeck)
    {
        for (int i = 0; i < i_listCHeck.Count - 1; i++)
        {
            for (int j = i + 1; j < i_listCHeck.Count; j++)
            {
                if (!i_listCHeck[i].GetBall().CompareColor(i_listCHeck[j].GetBall()))
                {
                    return false;
                }
            }
        }
        return true;
    }
    public void RotateGameBoard(float i_angle)
    {
        if(_parentGrid && _lastDetectedPosition != Vector3.zero)
        {
            _parentGrid.transform.Rotate(0, 0, i_angle, Space.Self);
        }    
    }    
}
