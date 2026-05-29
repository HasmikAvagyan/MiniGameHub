using UnityEngine;
using System.Collections.Generic;

public static class MoveValidator
{
    public static bool IsLegalMove(ChessPiece[,] board, ChessPiece piece, int toCol, int toRow)
    {
        if (!GameManager.InBounds(toCol, toRow)) return false;

        ChessPiece target = board[toCol, toRow];
        if (target != null && target.isWhite == piece.isWhite) return false;

        List<Vector2Int> valid = GetValidMoves(board, piece);
        return valid.Contains(new Vector2Int(toCol, toRow));
    }

    public static List<Vector2Int> GetValidMoves(ChessPiece[,] board, ChessPiece piece)
    {
        List<Vector2Int> raw = GetRawMoves(board, piece);
        List<Vector2Int> legal = new List<Vector2Int>();

        foreach (Vector2Int move in raw)
            if (!CheckValidator.MoveLeavesKingInCheck(board, piece, move.x, move.y))
                legal.Add(move);

        return legal;
    }

    public static List<Vector2Int> GetRawMoves(ChessPiece[,] board, ChessPiece piece)
    {
        return piece.pieceType switch
        {
            PieceType.Pawn => PawnMoves(board, piece),
            PieceType.Rook => SlidingMoves(board, piece, RookDirections()),
            PieceType.Bishop => SlidingMoves(board, piece, BishopDirections()),
            PieceType.Queen => SlidingMoves(board, piece, QueenDirections()),
            PieceType.Knight => KnightMoves(board, piece),
            PieceType.King => KingMoves(board, piece),
            _ => new List<Vector2Int>()
        };
    }

    static List<Vector2Int> PawnMoves(ChessPiece[,] board, ChessPiece piece)
    {
        var moves = new List<Vector2Int>();
        int dir = piece.isWhite ? 1 : -1;
        int startRow = piece.isWhite ? 1 : 6;
        int c = piece.col, r = piece.row;

        if (GameManager.InBounds(c, r + dir) && board[c, r + dir] == null)
        {
            moves.Add(new Vector2Int(c, r + dir));
            if (r == startRow && board[c, r + 2 * dir] == null)
                moves.Add(new Vector2Int(c, r + 2 * dir));
        }

        foreach (int dc in new[] { -1, 1 })
        {
            int nc = c + dc, nr = r + dir;
            if (GameManager.InBounds(nc, nr) &&
                board[nc, nr] != null &&
                board[nc, nr].isWhite != piece.isWhite)
                moves.Add(new Vector2Int(nc, nr));
        }

        return moves;
    }

    static List<Vector2Int> KnightMoves(ChessPiece[,] board, ChessPiece piece)
    {
        var moves = new List<Vector2Int>();
        var offsets = new Vector2Int[]
        {
            new(1,2), new(2,1), new(2,-1), new(1,-2),
            new(-1,-2), new(-2,-1), new(-2,1), new(-1,2)
        };

        foreach (var o in offsets)
        {
            int nc = piece.col + o.x, nr = piece.row + o.y;
            if (GameManager.InBounds(nc, nr))
            {
                ChessPiece target = board[nc, nr];
                if (target == null || target.isWhite != piece.isWhite)
                    moves.Add(new Vector2Int(nc, nr));
            }
        }
        return moves;
    }

    static List<Vector2Int> KingMoves(ChessPiece[,] board, ChessPiece piece)
    {
        var moves = new List<Vector2Int>();
        for (int dc = -1; dc <= 1; dc++)
        {
            for (int dr = -1; dr <= 1; dr++)
            {
                if (dc == 0 && dr == 0) continue;
                int nc = piece.col + dc, nr = piece.row + dr;
                if (GameManager.InBounds(nc, nr))
                {
                    ChessPiece target = board[nc, nr];
                    if (target == null || target.isWhite != piece.isWhite)
                        moves.Add(new Vector2Int(nc, nr));
                }
            }
        }
        return moves;
    }

    static List<Vector2Int> SlidingMoves(ChessPiece[,] board, ChessPiece piece,
                                          Vector2Int[] directions)
    {
        var moves = new List<Vector2Int>();
        foreach (var dir in directions)
        {
            int nc = piece.col + dir.x;
            int nr = piece.row + dir.y;
            while (GameManager.InBounds(nc, nr))
            {
                ChessPiece target = board[nc, nr];
                if (target == null)
                    moves.Add(new Vector2Int(nc, nr));
                else
                {
                    if (target.isWhite != piece.isWhite)
                        moves.Add(new Vector2Int(nc, nr));
                    break;
                }
                nc += dir.x;
                nr += dir.y;
            }
        }
        return moves;
    }

    static Vector2Int[] RookDirections() => new[] { new Vector2Int(0, 1), new(0, -1), new(1, 0), new(-1, 0) };
    static Vector2Int[] BishopDirections() => new[] { new Vector2Int(1, 1), new(1, -1), new(-1, 1), new(-1, -1) };
    static Vector2Int[] QueenDirections()
    {
        var d = new List<Vector2Int>();
        d.AddRange(RookDirections());
        d.AddRange(BishopDirections());
        return d.ToArray();
    }
}