using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathFinding
{
    public List<Tile> FindPath(Dictionary<Vector2Int, Tile> i_tiles, Tile start, Tile end)
    {
        List<Tile> openList = new List<Tile>();
        List<Tile> closedList = new List<Tile>();

        openList.Add(start);

        if (end.isBlocked)
        {
            return new List<Tile>();
        }

        while (openList.Count > 0)
        {
            Tile currentTile = openList[0];

            openList.Remove(currentTile);
            closedList.Add(currentTile);

            if (currentTile == end)
            {
                return GetFinishedList(start, end);
            }

            if (!start.GetBall().isGhost)
            {
                foreach (var tile in GetNeighbourTile(i_tiles, currentTile))
                {
                    if (tile.isBlocked || closedList.Contains(tile)) continue;

                    tile.G = GetManhattenDistance(currentTile, tile) + currentTile.G;
                    tile.H = GetManhattenDistance(end, tile);

                    tile.SetPreviousTile(currentTile);

                    if (!openList.Contains(tile))
                    {
                        openList.Add(tile);
                    }
                }
            }
            else
            {
                foreach (var tile in GetNeighbourTile(i_tiles, currentTile))
                {
                    if (closedList.Contains(tile)) continue;

                    tile.G = GetManhattenDistance(start, tile);
                    tile.H = GetManhattenDistance(end, tile);

                    tile.SetPreviousTile(currentTile);

                    if (!openList.Contains(tile))
                    {
                        openList.Add(tile);
                    }
                }

            }
        }

        return new List<Tile>();

    }

    private int GetManhattenDistance(Tile start, Tile tile)
    {
        return Mathf.Abs(start.GetLocation().x - tile.GetLocation().x) + Mathf.Abs(start.GetLocation().y - tile.GetLocation().y);
    }

    private List<Tile> GetNeighbourTile(Dictionary<Vector2Int, Tile> i_tiles, Tile currentTile)
    {
        var listTile = i_tiles;

        List<Tile> neighbours = new List<Tile>();

        //right
        Vector2Int locationToCheck = new Vector2Int(currentTile.GetLocation().x + 1, currentTile.GetLocation().y);
        if (listTile.ContainsKey(locationToCheck))
        {
            neighbours.Add(listTile[locationToCheck]);
        }

        //left
        locationToCheck = new Vector2Int(currentTile.GetLocation().x - 1, currentTile.GetLocation().y);
        if (listTile.ContainsKey(locationToCheck))
        {
            neighbours.Add(listTile[locationToCheck]);
        }
        //top
        locationToCheck = new Vector2Int(currentTile.GetLocation().x, currentTile.GetLocation().y + 1);
        if (listTile.ContainsKey(locationToCheck))
        {
            neighbours.Add(listTile[locationToCheck]);
        }
        //bottom
        locationToCheck = new Vector2Int(currentTile.GetLocation().x, currentTile.GetLocation().y - 1);
        if (listTile.ContainsKey(locationToCheck))
        {
            neighbours.Add(listTile[locationToCheck]);
        }

        return neighbours;
    }

    private List<Tile> GetFinishedList(Tile start, Tile end)
    {
        List<Tile> finishedList = new List<Tile>();
        Tile currentTile = end;

        while (currentTile != start)
        {
            finishedList.Add(currentTile);
            currentTile = currentTile.GetPreviousTile();
        }
        finishedList.Add(start);

        finishedList.Reverse();

        return finishedList;
    }
}
