using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BallManager : Singleton<BallManager>
{
    private Queue<Ball> _waitingBall = new Queue<Ball>();
    private Queue<Color> _waitingColor = new Queue<Color>();
    private void OnEnable()
    {
        GameManager.Instance.OnWaitingTurn += DequeueBall;
        GameManager.Instance.OnEndTurn += GrowUpBall;
    }

    private void OnDisable()
    {
        GameManager.Instance.OnWaitingTurn -= DequeueBall;
        GameManager.Instance.OnEndTurn -= GrowUpBall;
    }
    private void Start()
    {
        var tiles = GridManager.Instance.GetTiles();
        SpawnBalls(tiles, true);
        SpawnBalls(tiles, false);
        //Random first queue
        for (int i = 0; i < 3; i++)
        {
            _waitingColor.Enqueue(GameManager.Instance.GetRandomColor());
        }

    }
    private void DequeueBall()
    {
        var tiles = GridManager.Instance.GetTiles();
        for(int i = 0; i < 3; i++)
        {
            SpawnBall(tiles, false, _waitingColor.Dequeue());
        }  
        //Random next color
        for(int j = 0; j < 3; j++)
        {
            Color color = GameManager.Instance.GetRandomColor();
            _waitingColor.Enqueue(color);
            //Todo : Setup color UI

        }
        GameManager.Instance.ChangeGameState(GAMESTATE.PLAYING);
    }
    public void SpawnBall(Dictionary<Vector2Int, Tile> i_tiles, bool i_isShowed, Color i_color)
    {
        List<Vector2Int> availableLocation = i_tiles.Keys.Where(x => !i_tiles[x].isBlocked).ToList();
        if (availableLocation.Count < 1)
        {
            //Todo : Game over
            return;
        }

        int rand = UnityEngine.Random.Range(0, availableLocation.Count);
        Vector2Int location = availableLocation[rand];
        Ball ballSpawned = ObjectPool.Instance.TakeObject("ball").GetComponent<Ball>();
        ballSpawned.gameObject.transform.position = new Vector3(location.x, location.y);
        ballSpawned.SetColor(i_color);
        ballSpawned.SetLocation(location);

        if (i_isShowed)
        {
            ballSpawned.gameObject.transform.localScale = Vector3.one * 0.8f;
            ballSpawned.SetFinished(true);
            i_tiles[location].SetShowed(true);
        }
        else
        {
            ballSpawned.gameObject.transform.localScale = Vector3.one * 0.5f;
            ballSpawned.SetFinished(false);
            _waitingBall.Enqueue(ballSpawned);
        }
        availableLocation.RemoveAt(rand);
        i_tiles[location].SetBall(ballSpawned);
    }
    public void SpawnBalls(Dictionary<Vector2Int, Tile> i_tiles, bool i_isShowed)
    {
        List<Vector2Int> availableLocation = i_tiles.Keys.Where(x => !i_tiles[x].isBlocked).ToList();
        if (availableLocation.Count < 1)
        {
            //Todo : Game over
            return;
        }

        int numSpawn = GameManager.Instance.NumSpawn > availableLocation.Count ? availableLocation.Count : GameManager.Instance.NumSpawn;

        for (int i = 0; i < numSpawn; i++)
        {
            int rand = UnityEngine.Random.Range(0, availableLocation.Count);
            Vector2Int location = availableLocation[rand];
            Ball ballSpawned = ObjectPool.Instance.TakeObject("ball").GetComponent<Ball>();
            ballSpawned.gameObject.transform.position = new Vector3(location.x, location.y);
            ballSpawned.SetColor(GameManager.Instance.GetRandomColor());
            ballSpawned.SetLocation(location);

            if (i_isShowed)
            {
                ballSpawned.gameObject.transform.localScale = Vector3.one * 0.8f;
                ballSpawned.SetFinished(true);
                i_tiles[location].SetShowed(true);
            }
            else
            {
                ballSpawned.gameObject.transform.localScale = Vector3.one * 0.5f;
                ballSpawned.SetFinished(false);
                _waitingBall.Enqueue(ballSpawned);
            }
            availableLocation.RemoveAt(rand);
            i_tiles[location].SetBall(ballSpawned);
        }
        GameManager.Instance.ChangeGameState(GAMESTATE.PLAYING);
    }

    void GrowUpBall()
    {
        while (_waitingBall.Count > 0)
        {
            Ball ball = _waitingBall.Dequeue();
            ball.FinishBall();
        }
        GameManager.Instance.ChangeGameState(GAMESTATE.WAITING);
    }
}
