using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("References")]
    public BoardManager boardManager;
    public PieceSpawner pieceSpawner;
    public Text turnText;

    public ChessPiece[,] board = new ChessPiece[8, 8];

    public bool isWhiteTurn { get; private set; } = true;
    public bool gameOver { get; private set; } = false;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        pieceSpawner.SpawnAll(board);
        UpdateTurnUI();
    }

    public bool TryMove(ChessPiece piece, int toCol, int toRow)
    {
        if (gameOver) return false;
        if (piece.isWhite != isWhiteTurn) return false;
        if (!MoveValidator.IsLegalMove(board, piece, toCol, toRow)) return false;
        ExecuteMove(piece, toCol, toRow);
        return true;
    }

    void ExecuteMove(ChessPiece piece, int toCol, int toRow)
    {
        ChessPiece target = board[toCol, toRow];
        if (target != null) { Destroy(target.gameObject); board[toCol, toRow] = null; }

        board[piece.col, piece.row] = null;
        board[toCol, toRow] = piece;

        Vector3 newPos = boardManager.GetWorldPosition(toCol, toRow);
        newPos.z = -0.1f;
        piece.transform.position = newPos;

        piece.col = toCol;
        piece.row = toRow;

        if (piece.pieceType == PieceType.Pawn)
            if ((piece.isWhite && toRow == 7) || (!piece.isWhite && toRow == 0))
                piece.Init(PieceType.Queen, piece.isWhite, toCol, toRow);

        isWhiteTurn = !isWhiteTurn;
        CheckGameState();
    }

    void CheckGameState()
    {
        bool inCheck = CheckValidator.IsInCheck(board, isWhiteTurn);
        bool hasLegal = CheckValidator.HasAnyLegalMove(board, isWhiteTurn);
        string whose = isWhiteTurn ? "White" : "Black";

        if (!hasLegal)
        {
            if (inCheck)
                EndGame(isWhiteTurn ? "Black Wins!" : "White Wins!");
            else
                EndGame("It's a Draw!");
        }
        else if (inCheck)
        {
            if (turnText != null) turnText.text = $"{whose} is in CHECK!";
        }
        else
        {
            UpdateTurnUI();
        }
    }

    void UpdateTurnUI()
    {
        if (turnText != null)
            turnText.text = isWhiteTurn ? "White's turn" : "Black's turn";
    }

    void EndGame(string message)
    {
        gameOver = true;
        GameResult.winnerText = message;   // store for result scene
        Invoke(nameof(LoadResultScene), 1f); // small delay so last move is visible
    }

    void LoadResultScene()
    {
        SceneManager.LoadScene("ChessResult");
    }

    public static bool InBounds(int c, int r) => c >= 0 && c < 8 && r >= 0 && r < 8;
    public bool IsEmpty(int c, int r) => InBounds(c, r) && board[c, r] == null;
    public bool IsEnemy(int c, int r, bool isWhite) => InBounds(c, r) && board[c, r] != null && board[c, r].isWhite != isWhite;
    public bool CanMoveTo(int c, int r, bool isWhite) => IsEmpty(c, r) || IsEnemy(c, r, isWhite);
}