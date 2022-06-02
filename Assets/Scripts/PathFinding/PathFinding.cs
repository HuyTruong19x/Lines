using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathFinding
{
    public List<Tile> FindPath(Dictionary<Vector2Int, Tile> i_tiles, Tile i_tileStart, Tile i_tileEnd)
    {
        List<Tile> openList = new List<Tile>();
        List<Tile> closedList = new List<Tile>();

        openList.Add(i_tileStart);

        if (i_tileEnd.isBlocked)
        {
            Debug.Log($"Tile at {i_tileEnd.GetLocation()} is blocked");
            return new List<Tile>();
        }

        while (openList.Count > 0)
        {
            Tile currentTile = openList[0];

            openList.Remove(currentTile);
            closedList.Add(currentTile);
            if (currentTile.GetLocation().x == i_tileEnd.GetLocation().x && currentTile.GetLocation().y == i_tileEnd.GetLocation().y)
            {
                return GetFinishedList(i_tileStart, i_tileEnd);
            }

            if (!i_tileStart.GetBall().IsGhost)
            {
                var neighboers = GetNeighbourTile(i_tiles, currentTile);
                for (int i = 0; i < neighboers.Count; i++)
                {
                    if (neighboers[i].isBlocked || closedList.Contains(neighboers[i])) continue;

                    neighboers[i].G = GetManhattenDistance(currentTile, neighboers[i]) + currentTile.G;
                    neighboers[i].H = GetManhattenDistance(i_tileEnd, neighboers[i]);

                    neighboers[i].SetPreviousTile(currentTile);

                    if (!openList.Contains(neighboers[i]))
                    {
                        openList.Add(neighboers[i]);
                    }
                }    
            }
            else
            {
                var neighboers = GetNeighbourTile(i_tiles, currentTile);
                for (int i = 0; i < neighboers.Count; i++)
                {
                    if (closedList.Contains(neighboers[i])) continue;

                    neighboers[i].G = GetManhattenDistance(currentTile, neighboers[i]) + currentTile.G;
                    neighboers[i].H = GetManhattenDistance(i_tileEnd, neighboers[i]);

                    neighboers[i].SetPreviousTile(currentTile);

                    if (!openList.Contains(neighboers[i]))
                    {
                        openList.Add(neighboers[i]);
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
