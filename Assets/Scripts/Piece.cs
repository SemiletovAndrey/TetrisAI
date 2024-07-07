
using UnityEngine;
using UnityEngine.Tilemaps;

public class Piece : MonoBehaviour
{
    [SerializeField] private double _pitchDelay = 1f;
    [SerializeField] private float _lockDelay = 0.2f;
    [SerializeField] private bool canAI = false;

    private Vector2Int agentAction;
    private bool actionReady = false;

    public Board Board { get; private set; }
    public TetrominoData DataTerm { get; private set; }
    public Vector3Int Position { get; private set; }
    public Vector3Int[] Cells { get; private set; }
    public int RotateIndex { get; private set; }
    public bool CanAI { get { return canAI; } }

    public Vector2Int[] positionForQTable;
    public double PitchDelay { get { return _pitchDelay; } set { _pitchDelay = value; } }

    private double _stepTime;
    private float _lockTime;


    public float actionTimer = 0.0f;
    public float actionCooldown = 0.5f;

    public void Initialize(Board board, Vector3Int position, TetrominoData data)
    {

        this.Board = board;
        this.DataTerm = data;
        this.Position = position;
        this.RotateIndex = 0;
        this._stepTime = Time.time + _pitchDelay;
        this._lockTime = 0f;
        positionForQTable = new Vector2Int[data.Cells.Length];
        if (Cells == null)
        {
            Cells = new Vector3Int[DataTerm.Cells.Length];
        }
        for (int i = 0; i < DataTerm.Cells.Length; i++)
        {
            Cells[i] = (Vector3Int)DataTerm.Cells[i];
        }

        CalculatePositionForQTable();
    }

    private void Update()
    {
        Board.ClearPiece(this);

        if (canAI)
        {
            actionTimer -= Time.deltaTime;
            if (actionReady)
            {
                PerformAction(agentAction);
                actionReady = false;
            }
        }

        _lockTime += Time.deltaTime;
        if (canAI == false)
        {
            HandlerMovement();
        }

        if (Time.time > _stepTime)
        {
            StepPiece();

        }
        CalculatePositionForQTable();

        Board.SetPiece(this);
    }

    public void SetAgentAction(Vector2Int action)
    {
        agentAction = action;
        actionReady = true;
    }

    private void PerformAction(Vector2Int action)
    {
        if (action == Vector2Int.left)
        {
            MoveLeft();
        }
        else if (action == Vector2Int.right)
        {
            MoveRight();
        }
        else if (action == Vector2Int.down)
        {
            MoveDown();
        }
    }

    private void CalculatePositionForQTable()
    {
        for (int i = 0; i < DataTerm.Cells.Length; i++)
        {
            positionForQTable[i] = new Vector2Int(Position.x + DataTerm.Cells[i].x + 5, -(Position.y + DataTerm.Cells[i].y - 9));
        }
    }

    private bool Move(Vector2Int translation)
    {

        Vector3Int newPositionTetromino = Position;
        newPositionTetromino.x += translation.x;
        newPositionTetromino.y += translation.y;

        bool valid = Board.IsValidPosition(this, newPositionTetromino);
        if (valid)
        {
            Position = newPositionTetromino;
            _lockTime = 0f;
            if (canAI)
            {
                Board.currentState.ClearGrid(positionForQTable);
                CalculatePositionForQTable();
                Board.currentState.UpdateState(positionForQTable, DataTerm);
            }
        }

        return valid;
    }

    private void HandlerMovement()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            CountercLockWise();
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            ClockWise();
        }

        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            MoveLeft();
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            MoveRight();
        }
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            MoveDown();
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            HardDropDown();
        }
    }

    private void ClockWise()
    {
        Rotate(1);
    }

    private void CountercLockWise()
    {
        Rotate(-1);
    }

    public void MoveDown()
    {
        Move(Vector2Int.down);
    }

    public void MoveRight()
    {
        Move(Vector2Int.right);
    }

    public void MoveLeft()
    {
        Move(Vector2Int.left);
    }


    private void HardDropDown()
    {
        while (Move(Vector2Int.down))
        {
            continue;
        }
        LockPiece();
    }

    private void Rotate(int direction)
    {
        int originalRotation = RotateIndex;
        RotateIndex = Wrap(direction + RotateIndex, 0, 4);
        ApplyMatrixRotation(direction);
        if (!TestWallKicks(RotateIndex, direction))
        {
            RotateIndex = originalRotation;
            ApplyMatrixRotation(-direction);
        }
    }

    private void ApplyMatrixRotation(int direction)
    {
        for (int i = 0; i < Cells.Length; i++)
        {
            Vector3 cell = Cells[i];
            int x, y;

            switch (DataTerm.tetromino)
            {
                case Tetromino.I:
                case Tetromino.O:
                    cell.x -= 0.5f;
                    cell.y -= 0.5f;
                    x = Mathf.CeilToInt((cell.x * Data.RotationMatrix[0] * direction) + (cell.y * Data.RotationMatrix[1]) * direction);
                    y = Mathf.CeilToInt((cell.x * Data.RotationMatrix[2] * direction) + (cell.y * Data.RotationMatrix[3]) * direction);
                    break;
                default:
                    x = Mathf.RoundToInt((cell.x * Data.RotationMatrix[0] * direction) + (cell.y * Data.RotationMatrix[1]) * direction);
                    y = Mathf.RoundToInt((cell.x * Data.RotationMatrix[2] * direction) + (cell.y * Data.RotationMatrix[3]) * direction);
                    break;
            }
            Cells[i] = new Vector3Int(x, y, 0);
        }
    }

    private bool TestWallKicks(int rotationIndex, int rotationDirection)
    {
        int wallKickIndex = GetWallKickIndex(rotationIndex, rotationDirection);
        for (int i = 0; i < DataTerm.WallCicksTetromino.GetLength(1); i++)
        {
            Vector2Int translation = DataTerm.WallCicksTetromino[wallKickIndex, i];
            if (Move(translation))
            {
                return true;
            }
        }
        return false;
    }

    private int GetWallKickIndex(int rotationIndex, int rotationDirection)
    {
        int wallKickIndex = rotationIndex * 2;
        if (rotationIndex < 0)
        {
            wallKickIndex--;
        }
        return Wrap(wallKickIndex, 0, DataTerm.WallCicksTetromino.Length);
    }

    private int Wrap(int input, int min, int max)
    {
        if (input < min)
        {
            return max - (min - input) % (max - min);
        }
        else
        {
            return min + (input - min) % (max - min);
        }
    }

    private void StepPiece()
    {
        _stepTime = Time.time + _pitchDelay;

        Move(Vector2Int.down);
        if (_lockTime >= _lockDelay)
        {
            LockPiece();
        }
    }

    private void LockPiece()
    {

        Board.SetPiece(this);
        Board.ClearAllLine();
        if (canAI)
        {
            Board.UpdateGame();
        }
        Board.SpawnPiece();
    }
}
