using UnityEngine;

public class PieceSpawner : MonoBehaviour
{
    public BoardManager boardManager;

    // Called by GameManager.Start() — not Unity's Start()
    public void SpawnAll(ChessPiece[,] board)
    {
        SpawnPiece(board, PieceType.Rook, true, 0, 0);
        SpawnPiece(board, PieceType.Knight, true, 1, 0);
        SpawnPiece(board, PieceType.Bishop, true, 2, 0);
        SpawnPiece(board, PieceType.Queen, true, 3, 0);
        SpawnPiece(board, PieceType.King, true, 4, 0);
        SpawnPiece(board, PieceType.Bishop, true, 5, 0);
        SpawnPiece(board, PieceType.Knight, true, 6, 0);
        SpawnPiece(board, PieceType.Rook, true, 7, 0);
        for (int c = 0; c < 8; c++) SpawnPiece(board, PieceType.Pawn, true, c, 1);

        SpawnPiece(board, PieceType.Rook, false, 0, 7);
        SpawnPiece(board, PieceType.Knight, false, 1, 7);
        SpawnPiece(board, PieceType.Bishop, false, 2, 7);
        SpawnPiece(board, PieceType.Queen, false, 3, 7);
        SpawnPiece(board, PieceType.King, false, 4, 7);
        SpawnPiece(board, PieceType.Bishop, false, 5, 7);
        SpawnPiece(board, PieceType.Knight, false, 6, 7);
        SpawnPiece(board, PieceType.Rook, false, 7, 7);
        for (int c = 0; c < 8; c++) SpawnPiece(board, PieceType.Pawn, false, c, 6);
    }

    void SpawnPiece(ChessPiece[,] board, PieceType type, bool isWhite, int col, int row)
    {
        GameObject pieceObj = new GameObject($"{(isWhite ? "W" : "B")}_{type}_{col}_{row}");
        pieceObj.transform.parent = transform;

        Vector3 pos = boardManager.GetWorldPosition(col, row);
        pos.z = -0.1f;
        pieceObj.transform.position = pos;

        SpriteRenderer sr = pieceObj.AddComponent<SpriteRenderer>();
        sr.sprite = CreateRingSprite();
        sr.color = isWhite ? new Color(1f, 1f, 1f, 0.7f) : new Color(0.1f, 0.1f, 0.1f, 0.7f);
        sr.sortingOrder = 1;

        GameObject labelObj = new GameObject("Label");
        labelObj.transform.parent = pieceObj.transform;
        labelObj.transform.localPosition = new Vector3(0, -0.05f, -0.05f);
        labelObj.transform.localScale = new Vector3(0.22f, 0.22f, 1f);

        TextMesh tm = labelObj.AddComponent<TextMesh>();
        tm.anchor = TextAnchor.MiddleCenter;
        tm.alignment = TextAlignment.Center;
        tm.fontSize = 48;

        ChessPiece cp = pieceObj.AddComponent<ChessPiece>();
        cp.Init(type, isWhite, col, row);

        // Write directly into the board array passed from GameManager
        board[col, row] = cp;

        Debug.Log($"Spawned {(isWhite ? "W" : "B")}_{type} at [{col},{row}]");
    }

    Sprite CreateRingSprite()
    {
        int res = 64;
        Texture2D tex = new Texture2D(res, res);
        float center = res / 2f;
        float outerR = res / 2f - 2f;
        float innerR = res / 2f - 10f;

        for (int y = 0; y < res; y++)
            for (int x = 0; x < res; x++)
            {
                float dist = Vector2.Distance(new Vector2(x, y), new Vector2(center, center));
                tex.SetPixel(x, y, (dist <= outerR && dist >= innerR) ? Color.white : Color.clear);
            }

        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, res, res), new Vector2(0.5f, 0.5f), res);
    }
}