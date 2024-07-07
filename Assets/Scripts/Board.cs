using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class Board : MonoBehaviour
{
    public Tilemap Tilemap { get; private set; }
    public Piece ActivePiece { get { return activePiece; } private set { activePiece = value; } }

    public TetrominoData[] tetrominos;
    public Vector3Int spawnPosition;
    public Vector2Int boardSize = new Vector2Int(10, 20);
    [SerializeField] private ParticleSystem _destroyParticlePrefab;
    [SerializeField] private Piece activePiece;


    private bool canAI;

    public State currentState;

    public TetrisAgentML tetrisAgentML;

    public RectInt Bounds
    {
        get
        {
            Vector2Int position = new Vector2Int(-boardSize.x / 2, -boardSize.y / 2);
            return new RectInt(position, boardSize);
        }
    }

    private void Awake()
    {
        Tilemap = GetComponentInChildren<Tilemap>();
        ActivePiece = GetComponentInChildren<Piece>();
        for (int i = 0; i < tetrominos.Length; i++)
        {
            tetrominos[i].Initialize();
        }
    }

    private void Start()
    {
        canAI = ActivePiece.CanAI;
        currentState = new State(boardSize.x, boardSize.y, 2, 2);
        if (canAI == false)
        {
            SpawnPiece();
        }
    }

    public void SpawnPiece()
    {
        int random = Random.Range(0, tetrominos.Length);
        TetrominoData data = tetrominos[random];
        ActivePiece.Initialize(this, spawnPosition, data);

        if (canAI)
        {
            currentState.UpdateState(ActivePiece.positionForQTable, data);
        }

        if (IsValidPosition(ActivePiece, spawnPosition))
        {
            SetPiece(ActivePiece);
        }
        else
        {
            GameOver();
        }
    }
    public void SetPiece(Piece piece)
    {
        for (int i = 0; i < piece.Cells.Length; i++)
        {
            Vector3Int tilePosition = piece.Cells[i] + piece.Position;
            Tilemap.SetTile(tilePosition, piece.DataTerm.tile);
        }
    }


    public void ClearPiece(Piece piece)
    {
        for (int i = 0; i < piece.Cells.Length; i++)
        {
            Vector3Int tilePosition = piece.Cells[i] + piece.Position;
            Tilemap.SetTile(tilePosition, null);
        }
    }

    public bool IsValidPosition(Piece piece, Vector3Int position)
    {
        RectInt bounds = Bounds;
        for (int i = 0; i < piece.Cells.Length; i++)
        {
            Vector3Int tilePosition = piece.Cells[i] + position;
            if (!bounds.Contains((Vector2Int)tilePosition))
            {
                return false;
            }
            if (Tilemap.HasTile(tilePosition))
            {
                return false;
            }
        }
        return true;
    }

    public void ClearAllLine()
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
                if (canAI)
                {
                    currentState.ClearAllLineInGrid();
                    currentState.UpdateAllLineInGrid(bound, Tilemap);
                    isLine = true;
                    countLine++;
                }
            }
            else
            {
                row++;
            }
        }
        if (isLine)
        {
            if (canAI)
            {
                tetrisAgentML.CalculateReward(countLine * 25);

            }
        }


    }

    private void LineClear(int row)
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
        if (canAI == false)
        {
            Managers.GameDifficultyManager?.IncreaseDifficulty();
        }

    }

    private void ToplineInteraction(int row, RectInt bound)
    {

        while (row < bound.yMax)
        {
            for (int i = bound.xMin; i < bound.xMax; i++)
            {
                Vector3Int position = new Vector3Int(i, row + 1, 0);
                TileBase upTile = Tilemap.GetTile(position);
                position = new Vector3Int(i, row, 0);
                Tilemap.SetTile(position, upTile);

            }
            row++;
        }
    }



    private bool IsLineHasFull(int row)
    {
        RectInt bound = Bounds;
        for (int i = bound.xMin; i < bound.xMax; i++)
        {
            Vector3Int position = new Vector3Int(i, row, 0);
            if (!Tilemap.HasTile(position))
            {
                return false;
            }
        }
        return true;
    }
    public void GameOver()
    {
        Tilemap.ClearAllTiles();

        if (canAI)
        {
            currentState.ClearAllLineInGrid();
            tetrisAgentML.CalculateReward(-50f);
            tetrisAgentML.EndEpisodeTetris();
            EventManager.OnClearScore();
        }
        if (canAI == false)
        {
            EventManager.OnGameOver();
        }
    }

    private void AnimateLineClear(Vector3 linePosition)
    {
        Vector3 position = linePosition + new Vector3(0, 0, -8);
        ParticleSystem lineDestroyEffect = Instantiate(_destroyParticlePrefab, position, Quaternion.identity);
        lineDestroyEffect.Play();
        Destroy(lineDestroyEffect.gameObject, 2.0f);
    }

    public void UpdateGame()
    {
        if (currentState.position[0].y < (boardSize.y - 2))
        {
            int fine = (currentState.position[0].y - (boardSize.y - 2)) / 3;
            tetrisAgentML.CalculateReward(fine);
        }
        else
        {
            tetrisAgentML.CalculateReward(10f);
        }
        CalculateHoles();
    }

    public void CalculateHoles()
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

                    tetrisAgentML.CalculateReward(-30f);
                    return;

                }
            }
            else
            {

                tetrisAgentML.CalculateReward(-30f);
                return;
            }
        }

        if (xMinus1 >= 0 && currentState.grid[xMinus1, currentState.position[0].y] == 0)
        {
            if (xMinus2 >= 0)
            {
                if (currentState.position[0].x % 2 != 0)
                {
                    tetrisAgentML.CalculateReward(-30f);
                    return;
                }
            }
            else
            {

                tetrisAgentML.CalculateReward(-30f);
                return;
            }
        }
    }

    [ContextMenu("PrintArrayGrid")]
    public void PrintArrayGrid()
    {
        for (int i = 0; i < currentState.grid.GetLength(0); i++)
        {
            for (int j = 0; j < currentState.grid.GetLength(1); j++)
            {
                Debug.Log($"myArray[{i},{j}] = {currentState.grid[i, j]}");
            }
        }
        Debug.Log($"Tetromino index {(int)currentState.currentTetromino.tetromino}");
    }
}
