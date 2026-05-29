using UnityEngine;
using System.Collections.Generic;

public static class CheckValidator
{
    public static bool IsInCheck(ChessPiece[,] board, bool isWhite)
    {
        Vector2Int kingPos = FindKing(board, isWhite);
        if (kingPos.x == -1) return false;
        return IsSquareAttackedBy(board, kingPos.x, kingPos.y, !isWhite);
    }

    // Simulates a move using a simple int[,] grid instead of ChessPiece references
    // so we never touch the real ChessPiece objects at all
    public static bool MoveLeavesKingInCheck(ChessPiece[,] board, ChessPiece piece,
                                              int toCol, int toRow)
    {
        // Build a plain int board:
        //  0 = empty
        //  positive = white piece (value = PieceType + 1)
        //  negative = black piece (value = -(PieceType + 1))
        int[,] sim = new int[8, 8];
        for (int c = 0; c < 8; c++)
            for (int r = 0; r < 8; r++)
                if (board[c, r] != null)
                    sim[c, r] = board[c, r].isWhite
                        ? (int)board[c, r].pieceType + 1
                        : -((int)board[c, r].pieceType + 1);

        // Apply the move on the int board
        sim[toCol, toRow] = sim[piece.col, piece.row];
        sim[piece.col, piece.row] = 0;

        // Find king position on the int board
        int kingVal = piece.isWhite ? (int)PieceType.King + 1 : -((int)PieceType.King + 1);
        int kingCol = -1, kingRow = -1;
        for (int c = 0; c < 8; c++)
            for (int r = 0; r < 8; r++)
                if (sim[c, r] == kingVal) { kingCol = c; kingRow = r; }

        if (kingCol == -1) return false;

        // Check if any enemy piece attacks the king on the int board
        return IsKingAttackedOnIntBoard(sim, kingCol, kingRow, piece.isWhite);
    }

    static bool IsKingAttackedOnIntBoard(int[,] sim, int kingCol, int kingRow, bool kingIsWhite)
    {
        // Check all enemy pieces
        for (int c = 0; c < 8; c++)
        {
            for (int r = 0; r < 8; r++)
            {
                int val = sim[c, r];
                if (val == 0) continue;

                bool isEnemy = kingIsWhite ? val < 0 : val > 0;
                if (!isEnemy) continue;

                PieceType type = (PieceType)(Mathf.Abs(val) - 1);

                if (CanAttackOnIntBoard(sim, type, !kingIsWhite, c, r, kingCol, kingRow))
                    return true;
            }
        }
        return false;
    }

    static bool CanAttackOnIntBoard(int[,] sim, PieceType type, bool isWhite,
                                     int fromCol, int fromRow, int targetCol, int targetRow)
    {
        switch (type)
        {
            case PieceType.Pawn:
                {
                    int dir = isWhite ? 1 : -1;
                    return (targetRow == fromRow + dir) &&
                           (targetCol == fromCol - 1 || targetCol == fromCol + 1);
                }
            case PieceType.Knight:
                {
                    int dc = Mathf.Abs(targetCol - fromCol);
                    int dr = Mathf.Abs(targetRow - fromRow);
                    return (dc == 1 && dr == 2) || (dc == 2 && dr == 1);
                }
            case PieceType.King:
                {
                    int dc = Mathf.Abs(targetCol - fromCol);
                    int dr = Mathf.Abs(targetRow - fromRow);
                    return dc <= 1 && dr <= 1;
                }
            case PieceType.Rook:
                return CanSlideOnIntBoard(sim, fromCol, fromRow, targetCol, targetRow,
                                          new Vector2Int[] { new(0, 1), new(0, -1), new(1, 0), new(-1, 0) });
            case PieceType.Bishop:
                return CanSlideOnIntBoard(sim, fromCol, fromRow, targetCol, targetRow,
                                          new Vector2Int[] { new(1, 1), new(1, -1), new(-1, 1), new(-1, -1) });
            case PieceType.Queen:
                return CanSlideOnIntBoard(sim, fromCol, fromRow, targetCol, targetRow,
                                          new Vector2Int[]{ new(0,1),new(0,-1),new(1,0),new(-1,0),
                                                            new(1,1),new(1,-1),new(-1,1),new(-1,-1) });
        }
        return false;
    }

    static bool CanSlideOnIntBoard(int[,] sim, int fromCol, int fromRow,
                                    int targetCol, int targetRow, Vector2Int[] dirs)
    {
        foreach (var dir in dirs)
        {
            int c = fromCol + dir.x;
            int r = fromRow + dir.y;
            while (c >= 0 && c < 8 && r >= 0 && r < 8)
            {
                if (c == targetCol && r == targetRow) return true;
                if (sim[c, r] != 0) break; // blocked
                c += dir.x;
                r += dir.y;
            }
        }
        return false;
    }

    public static bool IsSquareAttackedBy(ChessPiece[,] board, int col, int row, bool attackerIsWhite)
    {
        for (int c = 0; c < 8; c++)
            for (int r = 0; r < 8; r++)
            {
                ChessPiece p = board[c, r];
                if (p == null || p.isWhite != attackerIsWhite) continue;
                List<Vector2Int> attacks = MoveValidator.GetRawMoves(board, p);
                if (attacks.Contains(new Vector2Int(col, row))) return true;
            }
        return false;
    }

    public static Vector2Int FindKing(ChessPiece[,] board, bool isWhite)
    {
        for (int c = 0; c < 8; c++)
            for (int r = 0; r < 8; r++)
                if (board[c, r] != null &&
                    board[c, r].pieceType == PieceType.King &&
                    board[c, r].isWhite == isWhite)
                    return new Vector2Int(c, r);
        return new Vector2Int(-1, -1);
    }

    public static bool HasAnyLegalMove(ChessPiece[,] board, bool isWhite)
    {
        for (int c = 0; c < 8; c++)
            for (int r = 0; r < 8; r++)
            {
                ChessPiece p = board[c, r];
                if (p == null || p.isWhite != isWhite) continue;
                if (MoveValidator.GetValidMoves(board, p).Count > 0) return true;
            }
        return false;
    }
}