using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Zenject;

public class BoardAI : Board
{
    private TetrisAgentML _tetrisAgentML;

    [Inject]
    public void Construct(Tilemap tilemap, Piece piece, TetrisAgentML tetrisAgentML)
    {
        Tilemap = tilemap;
        ActivePiece = piece;
        _tetrisAgentML = tetrisAgentML;
        for (int i = 0; i < tetrominos.Length; i++)
        {
            tetrominos[i].Initialize();
        }
    }

    private void Start()
    {
        currentState = new State(boardSize.x, boardSize.y);
    }

    public override void SpawnPiece()
    {
        int random = Random.Range(0, tetrominos.Length);
        TetrominoData data = tetrominos[random];
        ActivePiece.Initialize(spawnPosition, data);

        currentState.UpdateState(ActivePiece.positionForQTable, data);

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
        int countLine = 0;
        bool isLine = false;
        while (row < bound.yMax)
        {
            if (IsLineHasFull(row))
            {
                LineClear(row);
                currentState.ClearAllLineInGrid();
                currentState.UpdateAllLineInGrid(bound, Tilemap);
                isLine = true;
                countLine++;

            }
            else
            {
                row++;
            }
        }
        if (isLine)
        {
            _tetrisAgentML.CalculateReward(countLine * 25);
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
    }

    public override void GameOver()
    {
        Tilemap.ClearAllTiles();
        currentState.ClearAllLineInGrid();
        _tetrisAgentML.CalculateReward(-50f);
        _tetrisAgentML.EndEpisodeTetris();
        EventManager.OnClearScore();
    }

    public override void UpdateGame()
    {
        if (currentState.position[0].y < (boardSize.y - 2))
        {
            int fine = (currentState.position[0].y - (boardSize.y - 2)) / 3;
            _tetrisAgentML.CalculateReward(fine);
        }
        else
        {
            _tetrisAgentML.CalculateReward(10f);
        }
        CalculateHoles();
    }

    public virtual void CalculateHoles()
    {
        int xPlus1 = currentState.position[3].x + 1;
        int xPlus2 = currentState.position[3].x + 2;
        int xMinus1 = currentState.position[0].x - 1;
        int xMinus2 = currentState.position[0].x - 2;

        if (xPlus1 < boardSize.x && currentState.grid[xPlus1, currentState.position[0].y] == 0)
        {
            if (xPlus2 < boardSize.x)
            {
                if (currentState.position[0].x % 2 != 0)
                {

                    _tetrisAgentML.CalculateReward(-30f);
                    return;
                }
            }
            else
            {
                _tetrisAgentML.CalculateReward(-30f);
                return;
            }
        }
        if (xMinus1 >= 0 && currentState.grid[xMinus1, currentState.position[0].y] == 0)
        {
            if (xMinus2 >= 0)
            {
                if (currentState.position[0].x % 2 != 0)
                {
                    _tetrisAgentML.CalculateReward(-30f);
                    return;
                }
            }
            else
            {
                _tetrisAgentML.CalculateReward(-30f);
                return;
            }
        }
    }
}
