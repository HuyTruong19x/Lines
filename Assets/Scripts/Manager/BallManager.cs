using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
public class BallManager : MonoBehaviour
{
    public const float MAXIMUM = 0.8f;
    public const float MINIMUM = 0.2f;

    public bool IsInitialized { get { return _isInitialized; } }
    [SerializeField]
    private GridManager _gridManager;
    private UIManager _uiManager;
    private bool _isInitialized = false;
    private bool _isSkipRound = false;
    private Queue<Ball> _waitingBall = new Queue<Ball>();
    private Queue<Color> _waitingColor = new Queue<Color>();

    [SerializeField]
    private PathFinding _pathFinding;
    private Tile _selectedTileWithBall;

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
        _uiManager = GameObject.FindObjectOfType<UIManager>();
    }    

    private BallData GetBallData()
    {
        int rand = UnityEngine.Random.Range(0, 100);
        var Rate = GameManager.Instance.GetBallData();

        for(int i = 0; i < Rate.Count; i++)
        {
            if(Rate[i].Rate >  rand)
            {
                return Rate[i];
            }    
        }    
        BallData ballData = new BallData();
        ballData.Type = BALLTYPE.NONE;
        return ballData;
    }    
    public void InitializeBall()
    {
        _isInitialized = false;
        //Random first queue
        _waitingColor.Clear();
        List<Color> colors = new List<Color>();
        for (int i = 0; i < 3; i++)
        {
            Color col = GameManager.Instance.GetRandomColor();
            _waitingColor.Enqueue(col);
            colors.Add(col);
        }
        GameManager.Instance.ChangeGameState(GAMESTATE.STARTING);
    }    
    private void SetupBall()
    {
        if (_isInitialized)
        {
            Debug.Log("Ball was finished, update ball size");
            UpdateBallSize();
            GameManager.Instance.ChangeGameState(GAMESTATE.PLAYING);
            return;
        }    
        SpawnBall(true);
        DequeueBall();
        _isInitialized = true;
        Debug.Log("Setup finish");
    }    
    private void DequeueBall()
    {
        if(_isSkipRound)
        {
            _isSkipRound = false;
            GameManager.Instance.ChangeGameState(GAMESTATE.PLAYING);
            return;
        }
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
            GenerateBall(tiles, availableLocation, true, BALLTYPE.NONE, _waitingColor.Count > 0 ? _waitingColor.Dequeue() : GameManager.Instance.GetRandomColor(), null);
            GameManager.Instance.ChangeGameState(GAMESTATE.GAMEOVER);
            return;
        }
        for (int i = 0; i < numSpawn; i++)
        {
            var ballData = GetBallData();
            GenerateBall(tiles, availableLocation, i_isShowed, ballData.Type, _waitingColor.Count > 0 ? _waitingColor.Dequeue() : GameManager.Instance.GetRandomColor(), ballData.Material);
        }

        List<Color> colors = new List<Color>();
        for (int i = 0; i < GameManager.Instance.NumSpawn; i++)
        {
            Color col = GameManager.Instance.GetRandomColor();
            _waitingColor.Enqueue(col);
            colors.Add(col);
        }
        _uiManager.SetBallQueue(colors);
        GameManager.Instance.ChangeGameState(GAMESTATE.PLAYING);
    }

    private void GenerateBall(Dictionary<Vector2Int, Tile> i_tiles, List<Vector2Int> i_availableLocation,bool i_isShowed, BALLTYPE i_ballType, Color i_color, Material i_material)
    {
        //Recheck available Location
        i_availableLocation = i_tiles.Keys.Where(x => !i_tiles[x].hasBall).ToList();

        int rand = UnityEngine.Random.Range(0, i_availableLocation.Count);
        Vector2Int location = i_availableLocation[rand];
        Ball ballSpawned = ObjectPool.Instance.TakeObject("ball").GetComponent<Ball>();

        ballSpawned.gameObject.transform.parent = i_tiles[location].gameObject.transform.parent;
        ballSpawned.gameObject.transform.localPosition = new Vector3(i_tiles[location].transform.localPosition.x, i_tiles[location].transform.localPosition.y, 0);
        ballSpawned.Setup(_gridManager, location, i_color, i_ballType, i_material);

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

    private void GrowUpBall()
    {
        if(_isSkipRound)
        {
            GameManager.Instance.ChangeGameState(GAMESTATE.WAITING);
            return;
        }
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

    private void UpdateBallSize()
    {
        var tiles = _gridManager.GetTiles();
        foreach (var tile in tiles)
        {
            if(tile.Value.hasBall)
            {
                tile.Value.GetBall().UpdateBallSize();
            }    
        }    
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
            List<Tile> paths = _pathFinding.FindPath(_gridManager.GetTiles(), _selectedTileWithBall, currentTile);
            if (paths.Count < 1)
            {
                Debug.Log($"Can't move, zero path from {_selectedTileWithBall.GetLocation()} to {currentTile.GetLocation()} in {_gridManager.GetTiles().Count} objects");
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
                _selectedTileWithBall.GetBall().MoveTo(paths);
                _selectedTileWithBall.SetBall(null);
                _selectedTileWithBall.SetShowed(false);
                _selectedTileWithBall = null;
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
        _isSkipRound = true;
        SoundManager.Instance.PlaySFX(SFX.CONFETTI);
        foreach (Tile tile in i_tiles)
        {
            tile.GetBall().gameObject.SetActive(false);
            tile.SetShowed(false);
            tile.SetBall(null);
            VFXManager.Instance.TriggerVFX(VFXMode.CONFETTI, tile.gameObject.transform.position);
        }
        GameManager.Instance.IncreaseScore(i_tiles.Count);
    }      
}
