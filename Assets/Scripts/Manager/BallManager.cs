using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
public class BallManager : MonoBehaviour
{
    public const float MAXIMUM = 0.8f;
    public const float MINIMUM = 0.2f;
    private GridManager _gridManager;
    private bool _isInitialized = false;
    private Queue<Ball> _waitingBall = new Queue<Ball>();
    private Queue<Color> _waitingColor = new Queue<Color>();

    [SerializeField]
    private float _smoothMove = 0.1f;
    private PathFinding _pathFinding;
    private List<Tile> _paths;
    private Tile _selectedTileWithBall;
    private bool _isDoMove = false;
    private Vector3Int _targetMove;

    private void OnEnable()
    {
        EventManager.Instance.RegisterEvent(GAMEEVENT.STARTING, SetupBall);
        EventManager.Instance.RegisterEvent(GAMEEVENT.WAITING, DequeueBall);
        EventManager.Instance.RegisterEvent(GAMEEVENT.ENDTURN, GrowUpBall);
    }

    private void OnDisable()
    {
        EventManager.Instance.RemoveEvent(GAMEEVENT.STARTING, SetupBall);
        EventManager.Instance.RemoveEvent(GAMEEVENT.WAITING, DequeueBall);
        EventManager.Instance.RemoveEvent(GAMEEVENT.ENDTURN, GrowUpBall);
    }
    private void Awake()
    {
        _pathFinding = new PathFinding();
        _paths = new List<Tile>();
        _gridManager = GameObject.FindObjectOfType<GridManager>();
    }
    private void FixedUpdate()
    {
        if(_isDoMove)
        {
            _selectedTileWithBall.GetBall().transform.position = Vector3.MoveTowards(_selectedTileWithBall.GetBall().transform.position, _targetMove, _smoothMove);
            if (Vector3.Distance(_selectedTileWithBall.GetBall().transform.position, _targetMove) < 0.1f)
            {
                if (_paths.Count < 1)
                {
                    _isDoMove = false;
                    _selectedTileWithBall.GetBall().transform.position = _targetMove;
                    _selectedTileWithBall.GetBall().transform.DOScale(Vector3.one * MAXIMUM, 0.3f).OnComplete(() =>
                    {
                        _selectedTileWithBall.SetBall(null);
                        _selectedTileWithBall.SetShowed(false);
                        _selectedTileWithBall = null;
                        _gridManager.CheckScore(_gridManager.GetTile(new Vector2Int(_targetMove.x, _targetMove.y)));
                        GameManager.Instance.EndTurn();
                    }).Play();
                    
                }
                if(_paths.Count > 0)
                {
                    _targetMove = new Vector3Int().CreateFromVector2Int(_paths[0].GetLocation());
                    _paths.RemoveAt(0);
                }    
            }
            
        }    
    }

    private void ChangedGameMode()
    {
        if(!_isInitialized)
        {
            GameManager.Instance.ChangeGameState(GAMESTATE.STARTING);
        }    
    }    
    private void SetupBall()
    {
        if(_isInitialized)
        {
            return;
        }    
        //Random first queue
        List<Color> colors = new List<Color>();
        for (int i = 0; i < 3; i++)
        {
            Color col = GameManager.Instance.GetRandomColor();
            _waitingColor.Enqueue(col);
            colors.Add(col);
        }

        SpawnBall(true);
        DequeueBall();
        _isInitialized = true;
    }    
    private void DequeueBall()
    {
        SpawnBall(false);
    }
    public void SpawnBall(bool i_isShowed)
    {
        var tiles = _gridManager.GetTiles();
        List<Vector2Int> availableLocation = tiles.Keys.Where(x => !tiles[x].hasBall).ToList();
        if (availableLocation.Count < 1)
        {
            GameManager.Instance.ChangeGameState(GAMESTATE.GAMEOVER);
            return;
        }

        int numSpawn = GameManager.Instance.NumSpawn > availableLocation.Count ? availableLocation.Count : GameManager.Instance.NumSpawn;
        if(numSpawn <= 1)
        {
            GenerateBall(tiles, availableLocation, true, false, _waitingColor.Count > 0 ? _waitingColor.Dequeue() : GameManager.Instance.GetRandomColor());
            GameManager.Instance.ChangeGameState(GAMESTATE.GAMEOVER);
            return;
        }
        for (int i = 0; i < numSpawn; i++)
        {
            int rand = UnityEngine.Random.Range(0, 100);
            GenerateBall(tiles, availableLocation, i_isShowed, rand <= GameManager.Instance.GetRateSpawnGhostBall(), _waitingColor.Count > 0 ? _waitingColor.Dequeue() : GameManager.Instance.GetRandomColor());
        }

        List<Color> colors = new List<Color>();
        for (int i = 0; i < GameManager.Instance.NumSpawn; i++)
        {
            Color col = GameManager.Instance.GetRandomColor();
            _waitingColor.Enqueue(col);
            colors.Add(col);
        }
        UIManager.Instance.SetBallQueue(colors);
        GameManager.Instance.ChangeGameState(GAMESTATE.PLAYING);
    }

    private void GenerateBall(Dictionary<Vector2Int, Tile> i_tiles, List<Vector2Int> i_availableLocation, bool i_isShowed, bool i_isGhost, Color i_color)
    {
        //Recheck available Location
        i_availableLocation = i_tiles.Keys.Where(x => !i_tiles[x].hasBall).ToList();

        int rand = UnityEngine.Random.Range(0, i_availableLocation.Count);
        Vector2Int location = i_availableLocation[rand];
        Ball ballSpawned = ObjectPool.Instance.TakeObject("ball").GetComponent<Ball>();
        ballSpawned.gameObject.transform.position = new Vector3(i_tiles[location].transform.position.x, i_tiles[location].transform.position.y, 0);
        ballSpawned.Setup(_gridManager, location, i_color, i_isGhost);
        //ballSpawned.SetColor(i_color);
        //ballSpawned.SetLocation(location);
        //ballSpawned.SetGhostMode(i_isGhost);

        if (i_isShowed || i_availableLocation.Count == 1)
        {
            ballSpawned.gameObject.transform.localScale = Vector3.one * MAXIMUM;
            ballSpawned.SetFinished(true);
            i_tiles[location].SetShowed(true);
        }
        else
        {
            ballSpawned.gameObject.transform.localScale = Vector3.one * MINIMUM * 1.5f;
            ballSpawned.SetFinished(false);
            _waitingBall.Enqueue(ballSpawned);
        }

        i_tiles[location].SetBall(ballSpawned);
    }    

    void GrowUpBall()
    {
        while (_waitingBall.Count > 0)
        {
            Ball ball = _waitingBall.Dequeue();
            if(!_gridManager.GetTile(ball.GetLocation()).isBlocked)
            {
                ball.FinishBall();
                _gridManager.CheckScore(_gridManager.GetTile(ball.GetLocation()));
            }
            else
            {
                ball.gameObject.SetActive(false);
            }    
        }
        GameManager.Instance.ChangeGameState(GAMESTATE.WAITING);
    }

    //Handle ball movement
    public void HandleBallMoveMent(RaycastHit i_hitInfo)
    {
        Tile currentTile = i_hitInfo.collider.GetComponent<Tile>();
        if(currentTile == null)
        {
            return;
        }    
        if (_selectedTileWithBall == null)
        {
            if (currentTile.isBlocked)
            {
                _selectedTileWithBall = currentTile;
                _selectedTileWithBall.GetBall().Selected(true);
                SoundManager.Instance.PlaySFX(SFX.SELECTED);
            }
        }
        else if (!currentTile.isBlocked)
        {
            _paths = _pathFinding.FindPath(_gridManager.GetTiles(), _selectedTileWithBall, currentTile);
            if (_paths.Count < 1)
            {
                Debug.Log("can;t move, zero path");
                SoundManager.Instance.PlaySFX(SFX.CANNOTMOVE);
            }
            else
            {
                GameManager.Instance.ChangeGameState(GAMESTATE.MOVINGBALL);
                
                //Try to remove queue
                if(currentTile.hasBall)
                {
                    while(true)
                    {
                        Ball tmpBall = _waitingBall.Dequeue();
                        if(tmpBall != currentTile.GetBall())
                        {
                            _waitingBall.Enqueue(tmpBall);
                        }
                        else
                        {
                            tmpBall.gameObject.SetActive(false);
                            break;
                        }
                    }
                }

                Ball ball = _selectedTileWithBall.GetBall();
                currentTile.SetBall(ball);
                currentTile.SetShowed(true);

                SoundManager.Instance.PlaySFX(SFX.MOVE);

                _selectedTileWithBall.GetBall().Selected(false);
                _selectedTileWithBall.GetBall().transform.DOScale(Vector3.one * MINIMUM * 1.5f, 0.2f).OnComplete(() =>
                {
                    _targetMove = new Vector3Int().CreateFromVector2Int(_paths[0].GetLocation());
                    _paths.RemoveAt(0);
                    _isDoMove = true;
                }).Play();
                

            }
        }
        else
        {
            _selectedTileWithBall.GetBall().Selected(false);
            _selectedTileWithBall = currentTile;
            _selectedTileWithBall.GetBall().Selected(true);
            SoundManager.Instance.PlaySFX(SFX.SELECTED);
        }
    }      

    public void DestroyBall(List<Tile> i_tiles)
    {
        SoundManager.Instance.PlaySFX(SFX.CONFETTI);
        foreach (Tile tile in i_tiles)
        {
            tile.GetBall().gameObject.SetActive(false);
            tile.SetShowed(false);
            tile.SetBall(null);
            VFXManager.Instance.TriggerVFX(VFXMode.CONFETTI, tile.gameObject.transform.position);
        }
        GameManager.Instance.IncreaseScore();
        //Todo spawn VFX
    }      
}
