using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridManager : Singleton<GridManager>
{
    public int WIDTH = 9;
    public int HEIGHT = 9;

    private Dictionary<Vector2Int, Tile> _tiles = new Dictionary<Vector2Int, Tile>();

    private void Awake()
    {
        GenerateTiles();
    }

    private void GenerateTiles()
    {
        for(int i = 0; i < WIDTH; i++)
        {
            for(int y = 0; y < HEIGHT; y++)
            {
                Tile tile = ObjectPool.Instance.TakeObject("tile").GetComponent<Tile>();
                tile.SetLocation(i, y);
                tile.gameObject.name = $"tile {i} - {y}";
                tile.gameObject.transform.position = new Vector3(i, y, -0.1f);
                _tiles.Add(new Vector2Int(i, y), tile);
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

    public List<Tile> GetNeighbours(Tile i_tile, int i_depth = 1)
    {
        List<Tile> neighbours = new List<Tile>();

        for (int x = -i_depth; x <= i_depth; x++)
        {
            for (int y = -i_depth; y <= i_depth; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                int checkX = i_tile.GetLocation().x + x;
                int checkY = i_tile.GetLocation().y + y;

                if (checkX >= 0 && checkX < WIDTH && checkY >= 0 && checkY < HEIGHT)
                {
                    neighbours.Add(_tiles[new Vector2Int(checkX, checkY)]);
                }
            }
        }

        return neighbours;
    }
}
