using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BallManager : Singleton<BallManager>
{
    public const float MAXIMUM = 0.8f;
    public const float MINIMUM = 0.3f;
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
        GameManager.Instance.OnGameStart += SetupBall;
        GameManager.Instance.OnWaitingTurn += DequeueBall;
        GameManager.Instance.OnEndTurn += GrowUpBall;
    }

    private void OnDisable()
    {
        GameManager.Instance.OnGameStart -= SetupBall;
        GameManager.Instance.OnWaitingTurn -= DequeueBall;
        GameManager.Instance.OnEndTurn -= GrowUpBall;
    }
    private void Start()
    {
        _pathFinding = new PathFinding();
        _paths = new List<Tile>();
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
                    _selectedTileWithBall.GetBall().transform.localScale = Vector3.one * MAXIMUM;
                    _selectedTileWithBall.SetBall(null);
                    _selectedTileWithBall.SetShowed(false);
                    _selectedTileWithBall = null;
                    GridManager.Instance.CheckScore(GridManager.Instance.GetTile(new Vector2Int(_targetMove.x, _targetMove.y)));
                    GameManager.Instance.EndTurn();
                }
                if(_paths.Count > 0)
                {
                    _targetMove = new Vector3Int().CreateFromVector2Int(_paths[0].GetLocation());
                    _paths.RemoveAt(0);
                }    
            }
            
        }    
    }

    private void SetupBall()
    {
        //Random first queue
        List<Color> colors = new List<Color>();
        for (int i = 0; i < 3; i++)
        {
            Color col = GameManager.Instance.GetRandomColor();
            _waitingColor.Enqueue(col);
            colors.Add(col);
        }

        SpawnBall(true);
        SpawnBall(false);
        GameManager.Instance.ChangeGameState(GAMESTATE.PLAYING);
    }    
    private void DequeueBall()
    {
        SpawnBall(false);
        GameManager.Instance.ChangeGameState(GAMESTATE.PLAYING);
    }
    public void SpawnBall(bool i_isShowed)
    {
        var tiles = GridManager.Instance.GetTiles();
        List<Vector2Int> availableLocation = tiles.Keys.Where(x => !tiles[x].hasBall).ToList();
        if (availableLocation.Count < 1)
        {
            GameManager.Instance.ChangeGameState(GAMESTATE.GAMEOVER);
            return;
        }

        int numSpawn = GameManager.Instance.NumSpawn > availableLocation.Count ? availableLocation.Count : GameManager.Instance.NumSpawn;

        for (int i = 0; i < numSpawn; i++)
        {
            GenerateBall(tiles, availableLocation, i_isShowed, _waitingColor.Count > 0 ? _waitingColor.Dequeue() : GameManager.Instance.GetRandomColor());
        }

        List<Color> colors = new List<Color>();
        for (int i = 0; i < GameManager.Instance.NumSpawn; i++)
        {
            Color col = GameManager.Instance.GetRandomColor();
            _waitingColor.Enqueue(col);
            colors.Add(col);
        }
        UIManager.Instance.SetBallQueue(colors);

    }

    private void GenerateBall(Dictionary<Vector2Int, Tile> i_tiles, List<Vector2Int> i_availableLocation, bool i_isShowed, Color i_color)
    {
        //Recheck available Location
        i_availableLocation = i_tiles.Keys.Where(x => !i_tiles[x].hasBall).ToList();

        int rand = UnityEngine.Random.Range(0, i_availableLocation.Count);
        Vector2Int location = i_availableLocation[rand];
        Ball ballSpawned = ObjectPool.Instance.TakeObject("ball").GetComponent<Ball>();
        ballSpawned.gameObject.transform.position = i_tiles[location].transform.position;
        ballSpawned.SetColor(i_color);
        ballSpawned.SetLocation(location);

        if (i_isShowed)
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
            if(!GridManager.Instance.GetTile(ball.GetLocation()).isBlocked)
            {
                ball.FinishBall();
                GridManager.Instance.CheckScore(GridManager.Instance.GetTile(ball.GetLocation()));
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
        if (_selectedTileWithBall == null)
        {
            if (currentTile.GetBall() != null)
            {
                _selectedTileWithBall = currentTile;
                _selectedTileWithBall.GetBall().Selected(true);
                SoundManager.Instance.PlaySFX(SFX.SELECTED);
            }
        }
        else if (!currentTile.isBlocked)
        {
            _paths = _pathFinding.FindPath(_selectedTileWithBall, currentTile);
            if (_paths.Count < 1)
            {
                //Todo cancel select
                Debug.Log("can;t move, zero path");
                SoundManager.Instance.PlaySFX(SFX.CANNOTMOVE);
            }
            else
            {
                Ball ball = _selectedTileWithBall.GetBall();
                currentTile.SetBall(ball);
                currentTile.SetShowed(true);

                _selectedTileWithBall.GetBall().Selected(false);
                _selectedTileWithBall.GetBall().transform.localScale = Vector3.one * MINIMUM;
                _targetMove = new Vector3Int().CreateFromVector2Int(_paths[0].GetLocation());
                _paths.RemoveAt(0);
                _isDoMove = true;
                SoundManager.Instance.PlaySFX(SFX.MOVE);

            }
        }
        else
        {
            _selectedTileWithBall = currentTile;
            _selectedTileWithBall.GetBall().Selected(true);
            SoundManager.Instance.PlaySFX(SFX.SELECTED);
        }
    }      

    public void DestroyBall(List<Tile> i_tiles)
    {
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
