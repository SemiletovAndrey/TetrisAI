using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;
using Zenject;

public abstract class Board : MonoBehaviour
{
    public TetrominoData[] tetrominos;
    public Vector3Int spawnPosition;
    public Vector2Int boardSize = new Vector2Int(10, 20);
    [SerializeField] protected ParticleSystem _destroyParticlePrefab;
    [SerializeField] protected Piece activePiece;

    public State currentState;


    protected virtual Tilemap Tilemap { get; set; }
    protected virtual Piece ActivePiece { get { return activePiece; } set { activePiece = value; } }

    public virtual RectInt Bounds
    {
        get
        {
            Vector2Int position = new Vector2Int(-boardSize.x / 2, -boardSize.y / 2);
            return new RectInt(position, boardSize);
        }
    }

    public abstract void SpawnPiece();
    
    public virtual void SetPiece(Piece piece)
    {
        for (int i = 0; i < piece.Cells.Length; i++)
        {
            Vector3Int tilePosition = piece.Cells[i] + piece.Position;
            Tilemap.SetTile(tilePosition, piece.DataTerm.tile);
        }
    }


    public virtual void ClearPiece(Piece piece)
    {
        for (int i = 0; i < piece.Cells.Length; i++)
        {
            Vector3Int tilePosition = piece.Cells[i] + piece.Position;
            Tilemap.SetTile(tilePosition, null);
        }
    }

    public virtual bool IsValidPosition(Piece piece, Vector3Int position)
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

    public abstract void ClearAllLine();


    protected abstract void LineClear(int row);
    
    protected virtual void ToplineInteraction(int row, RectInt bound)
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

    protected bool IsLineHasFull(int row)
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

    public abstract void GameOver();
    
    protected virtual void AnimateLineClear(Vector3 linePosition)
    {
        Vector3 position = linePosition + new Vector3(0, 0, -8);
        ParticleSystem lineDestroyEffect = Instantiate(_destroyParticlePrefab, position, Quaternion.identity);
        lineDestroyEffect.Play();
        Destroy(lineDestroyEffect.gameObject, 2.0f);
    }

    public virtual void UpdateGame() { }
}
