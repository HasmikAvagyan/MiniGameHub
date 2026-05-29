using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class ClickHandler : MonoBehaviour
{
    [Header("References")]
    public GameManager gameManager;
    public BoardManager boardManager;

    [Header("Highlight Colors")]
    public Color selectedColor = new Color(1f, 0.85f, 0f, 0.7f);
    public Color validMoveColor = new Color(0f, 0.8f, 0.4f, 0.6f);
    public Color captureColor = new Color(0.9f, 0.2f, 0.2f, 0.6f);

    private ChessPiece selectedPiece = null;
    private int selectedCol = -1;
    private int selectedRow = -1;
    private List<Vector2Int> highlightedSquares = new List<Vector2Int>();

    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
            HandleClick();
    }

    void HandleClick()
    {
        if (gameManager.gameOver) return;

        Vector2 screenPos = Mouse.current.position.ReadValue();
        Vector3 world = Camera.main.ScreenToWorldPoint(screenPos);

        int col = Mathf.RoundToInt(world.x / boardManager.squareSize);
        int row = Mathf.RoundToInt(world.y / boardManager.squareSize);

        if (!GameManager.InBounds(col, row))
        {
            ClearAllHighlights();
            selectedPiece = null;
            selectedCol = -1;
            selectedRow = -1;
            return;
        }

        ChessPiece pieceAtTarget = gameManager.board[col, row];

        // Case 1: clicked own piece — select it
        if (pieceAtTarget != null && pieceAtTarget.isWhite == gameManager.isWhiteTurn)
        {
            ClearAllHighlights();
            selectedPiece = pieceAtTarget;
            selectedCol = col;
            selectedRow = row;
            ShowHighlights(pieceAtTarget);
            return;
        }

        // Case 2: a piece is selected — try to move or capture
        if (selectedPiece != null)
        {
            // Always clear highlights first before attempting move
            ClearAllHighlights();

            bool moved = gameManager.TryMove(selectedPiece, col, row);

            selectedPiece = null;
            selectedCol = -1;
            selectedRow = -1;
            return;
        }

        // Case 3: clicked empty square with nothing selected — do nothing
        ClearAllHighlights();
        selectedPiece = null;
        selectedCol = -1;
        selectedRow = -1;
    }

    void ShowHighlights(ChessPiece piece)
    {
        // Highlight the selected piece's square
        SetSquareColor(piece.col, piece.row, selectedColor);
        highlightedSquares.Add(new Vector2Int(piece.col, piece.row));

        // Highlight valid moves
        List<Vector2Int> moves = MoveValidator.GetValidMoves(gameManager.board, piece);
        foreach (Vector2Int move in moves)
        {
            bool isCapture = gameManager.board[move.x, move.y] != null;
            SetSquareColor(move.x, move.y, isCapture ? captureColor : validMoveColor);
            highlightedSquares.Add(move);
        }
    }

    void ClearAllHighlights()
    {
        foreach (Vector2Int sq in highlightedSquares)
            ClearSquareColor(sq.x, sq.y);
        highlightedSquares.Clear();
    }

    void SetSquareColor(int col, int row, Color color)
    {
        GameObject sq = boardManager.GetSquare(col, row);
        if (sq == null) return;
        BoardSquare bs = sq.GetComponent<BoardSquare>();
        if (bs != null) bs.Highlight(color);
    }

    void ClearSquareColor(int col, int row)
    {
        GameObject sq = boardManager.GetSquare(col, row);
        if (sq == null) return;
        BoardSquare bs = sq.GetComponent<BoardSquare>();
        if (bs != null) bs.ClearHighlight();
    }
}