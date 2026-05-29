using UnityEngine;

public enum PieceType { King, Queen, Rook, Bishop, Knight, Pawn }

public class ChessPiece : MonoBehaviour
{
    public PieceType pieceType;
    public bool isWhite;

    public int col;
    public int row;

    private TextMesh label;

    void Awake()
    {
        label = GetComponentInChildren<TextMesh>();
    }

    public void Init(PieceType type, bool white, int c, int r)
    {
        pieceType = type;
        isWhite = white;
        col = c;
        row = r;

        UpdateVisual();
    }

    void UpdateVisual()
    {
        if (label == null) return;
        label.text = GetSymbol();
        label.color = isWhite ? Color.white : Color.black;
    }

    string GetSymbol()
    {
        if (isWhite)
        {
            return pieceType switch
            {
                PieceType.King => "♔",
                PieceType.Queen => "♕",
                PieceType.Rook => "♖",
                PieceType.Bishop => "♗",
                PieceType.Knight => "♘",
                PieceType.Pawn => "♙",
                _ => "?"
            };
        }
        else
        {
            return pieceType switch
            {
                PieceType.King => "♚",
                PieceType.Queen => "♛",
                PieceType.Rook => "♜",
                PieceType.Bishop => "♝",
                PieceType.Knight => "♞",
                PieceType.Pawn => "♟",
                _ => "?"
            };
        }
    }
}