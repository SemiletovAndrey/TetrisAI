using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;


public class State
{
    public int[,] grid;
    public Vector2Int[] position;
    public int rotation;
    public TetrominoData currentTetromino;
    

    private int width;
    private int height;
    public State(int width, int height)
    {
        this.width = width;
        this.height = height;
        grid = new int[width, height];
    }

    public void UpdateState(Vector2Int[] newPosition, TetrominoData newTetromino)
    {
        position = newPosition;
        currentTetromino = newTetromino;
        for (int i = 0; i < newPosition.Length; i++)
        {
            grid[newPosition[i].x, newPosition[i].y] = 1;
        }
    }

    public void ClearGrid(Vector2Int[] position)
    {
        for (int i = 0; i < position.Length; i++)
        {
            grid[position[i].x, position[i].y] = 0;
        }
    }

    public void ClearAllLineInGrid()
    {
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                grid[i, j] = 0;

            }
        }
    }

    public void UpdateAllLineInGrid(RectInt bounds, Tilemap tilemap)
    {
        for (int x = bounds.x; x < bounds.x + bounds.size.x; x++)
        {
            for (int y = bounds.y; y < bounds.y + bounds.size.y; y++)
            {
                Vector3Int cellPosition = new Vector3Int(x, y, 0);
                TileBase tile = tilemap.GetTile(cellPosition);
                int xPos = cellPosition.x + (width / 2);
                int yPos = cellPosition.y;
                if (yPos <= 0)
                {
                    yPos = -(yPos - (width - 1));
                }
                else
                {
                    yPos = -(yPos - (width - 1));
                }

                Vector2Int positionV2 = new Vector2Int(xPos, yPos);
                if (tile != null)
                {

                    grid[positionV2.x, positionV2.y] = 1;
                }
            }
        }
    }
}
