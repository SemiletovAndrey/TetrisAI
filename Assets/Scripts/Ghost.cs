using UnityEngine;
using UnityEngine.Tilemaps;
using Zenject;

public class Ghost : MonoBehaviour
{
    [SerializeField] private Piece _trakingPiece;
    [SerializeField] private Tile _tile;
    [SerializeField] private Board _board;
    public Tilemap TilemapPiece { get;private set; }
    public Vector3Int[] Cells { get; private set; }
    public Vector3Int Position { get; private set; }

    [Inject]

    public void Construct(Piece piece, Board board)
    {
        _trakingPiece = piece;
        _board = board;
    }

    private void Awake()
    {
        this.TilemapPiece = GetComponentInChildren<Tilemap>();
        this.Cells = new Vector3Int[4];
    }

    private void LateUpdate()
    {
        ClearPiece();
        Copy();
        Drop();
        Set();
    }

    private void ClearPiece()
    {
        for (int i = 0; i < Cells.Length; i++)
        {
            Vector3Int tilePosition = Cells[i] + Position;
            TilemapPiece.SetTile(tilePosition, null);
        }
    }

    private void Copy()
    {
        for (int i = 0; i < Cells.Length; i++)
        {
            Cells[i] = _trakingPiece.Cells[i];
        }
    }

    private void Drop()
    {
        Vector3Int position = _trakingPiece.Position;
        int current = position.y;
        int bottom = -_board.boardSize.y / 2 - 1;
        _board.ClearPiece(_trakingPiece);
        for(int row = current; row >= bottom; row--)
        {
            position.y = row;
            if (_board.IsValidPosition(_trakingPiece, position))
            {
                Position = position;
            }
            else
            {
                break;
            }
        }
        _board.SetPiece(_trakingPiece);
    }

    private void Set()
    {
        for (int i = 0; i < Cells.Length; i++)
        {
            Vector3Int tilePosition = Cells[i] + Position;
            TilemapPiece.SetTile(tilePosition, _tile);
        }
    }
}
