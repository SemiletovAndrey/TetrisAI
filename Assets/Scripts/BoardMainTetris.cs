using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Zenject;

public class BoardMainTetris : Board
{

    private GameDifficultyManager gameDifficultyManager;

    [Inject]
    public void Construct(Tilemap tilemap, Piece piece, GameDifficultyManager gameDifficultyManager)
    {
        Tilemap = tilemap;
        ActivePiece = piece;
        this.gameDifficultyManager = gameDifficultyManager;
        for (int i = 0; i < tetrominos.Length; i++)
        {
            tetrominos[i].Initialize();
        }
    }

    private void Start()
    {
        currentState = new State(boardSize.x, boardSize.y);
        SpawnPiece();
    }

    public override void SpawnPiece()
    {
        int random = Random.Range(0, tetrominos.Length);
        TetrominoData data = tetrominos[random];
        ActivePiece.Initialize(spawnPosition, data);
        if (IsValidPosition(ActivePiece, spawnPosition))
        {
            SetPiece(ActivePiece);
        }
        else
        {
            GameOver();
        }
    }

    public override void ClearAllLine()
    {
        RectInt bound = Bounds;
        int row = bound.yMin;
        while (row < bound.yMax)
        {
            if (IsLineHasFull(row))
            {
                LineClear(row);
            }
            else
            {
                row++;
            }
        }
    }

    protected override void LineClear(int row)
    {
        RectInt bound = Bounds;
        for (int i = bound.xMin; i < bound.xMax; i++)
        {
            Vector3Int position = new Vector3Int(i, row, 0);
            Tilemap.SetTile(position, null);
            AnimateLineClear(position);
        }
        ToplineInteraction(row, bound);
        EventManager.OnScoring();
        gameDifficultyManager?.IncreaseDifficulty();
    }

    public override void GameOver()
    {
        Tilemap.ClearAllTiles();
        EventManager.OnGameOver();
    }
}
