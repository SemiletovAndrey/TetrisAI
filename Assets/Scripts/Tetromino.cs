using UnityEngine;
using UnityEngine.Tilemaps;

public enum Tetromino
{
    O,
    I,
    T,
    J,
    L,
    S,
    Z
}
[System.Serializable]
public struct TetrominoData
{
    public Tetromino tetromino;
    public Tile tile;
    public Vector2Int[] Cells { get; private set; }
    public Vector2Int[,] WallCicksTetromino { get; private set; }

    public void Initialize()
    {
        this.Cells = Data.Cells[tetromino];
        this.WallCicksTetromino = Data.WallKicks[tetromino];
    }
}