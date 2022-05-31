using System;
using System.Collections.Generic;
using UnityEngine;

public class PathFinding
{
    public List<Tile> FindPath(Tile i_start, Tile i_end)
    {
		List<Tile> path = new List<Tile>();
		List<Tile> openTile = new List<Tile>();
		List<Tile> closeTile = new List<Tile>();

		bool isSuccess = false;

		openTile.Add(i_start);

		while(openTile.Count > 0)
        {
			Tile currentTile = openTile[0];
			openTile.RemoveAt(0);

			closeTile.Add(currentTile);

			if(currentTile == i_end)
            {
				isSuccess = true;
				break;
            }				

			foreach(Tile tile in GridManager.Instance.GetNeighbours(currentTile))
            {
				if(tile.isBlocked || closeTile.Contains(tile))
                {
					continue;
                }
				int newMovementCostToNeighbour = currentTile.G + GetDistance(currentTile, tile)/* + TurningCost(currentNode, neighbour)*/;
				if (newMovementCostToNeighbour < tile.G || !openTile.Contains(tile))
				{
					tile.G = newMovementCostToNeighbour;
					tile.H = GetDistance(tile, i_end);
					tile.SetPreviousTile(currentTile);

					if (!openTile.Contains(tile))
						openTile.Add(tile);
				}
			}				
        }	
		
		if(isSuccess)
        {
			path = RetracePath(i_start, i_end);
        }			

        return path;
    }
	Tile FindMoveableInRadius(Dictionary<Vector2Int, Tile> i_tiles, int centreX, int centreY, int radius)
	{

		for (int i = -radius; i <= radius; i++)
		{
			int verticalSearchX = i + centreX;
			int horizontalSearchY = i + centreY;

			// top
			if (InBounds(verticalSearchX, centreY + radius))
			{
				var topLocation = new Vector2Int(verticalSearchX, centreY + radius);
				if (!i_tiles[topLocation].isBlocked)
				{
					return i_tiles[topLocation];
				}
			}

			// bottom
			if (InBounds(verticalSearchX, centreY - radius))
			{
				var bottomLocation = new Vector2Int(verticalSearchX, centreY - radius);
				if (!i_tiles[bottomLocation].isBlocked)
				{
					return i_tiles[bottomLocation];
				}
			}
			// right
			if (InBounds(centreY + radius, horizontalSearchY))
			{
				var rightLocation = new Vector2Int(centreX + radius, horizontalSearchY);
				if (!i_tiles[rightLocation].isBlocked)
				{
					return i_tiles[rightLocation];
				}
			}

			// left
			if (InBounds(centreY - radius, horizontalSearchY))
			{
				var leftLocation = new Vector2Int(centreX - radius, horizontalSearchY);
				if (!i_tiles[leftLocation].isBlocked)
				{
					return i_tiles[leftLocation];
				}
			}

		}

		return null;

	}
	bool InBounds(int x, int y)
	{
		return x >= 0 && x < GridManager.Instance.WIDTH && y >= 0 && y < GridManager.Instance.HEIGHT;
	}
	int GetDistance(Tile i_tileA, Tile i_tileB)
	{
		return Mathf.Abs(i_tileA.GetLocation().x - i_tileB.GetLocation().x) + Mathf.Abs(i_tileA.GetLocation().y - i_tileB.GetLocation().y);

		//int dstX = Mathf.Abs(i_tileA.GetLocation().x - i_tileB.GetLocation().x);
		//int dstY = Mathf.Abs(i_tileA.GetLocation().y - i_tileB.GetLocation().y);

		//if (dstX > dstY)
		//	return 14 * dstY + 10 * (dstX - dstY);
		//return 14 * dstX + 10 * (dstY - dstX);
	}
	List<Tile> RetracePath(Tile i_start, Tile i_end)
	{
		List<Tile> path = new List<Tile>();
		Tile currentNode = i_end;

		while (currentNode != i_start)
		{
			path.Add(currentNode);
			currentNode = currentNode.GetPreviousTile();
		}
		path.Add(i_start);
		path.Reverse();
		return path;

	}
}
