using UnityEngine;

public class BoardManager : MonoBehaviour
{
    [Header("Board Settings")]
    public int boardSize = 8;
    public float squareSize = 1f;

    [Header("Colors")]
    public Color lightColor = new Color(0.9f, 0.85f, 0.75f);
    public Color darkColor = new Color(0.45f, 0.3f, 0.2f);

    private GameObject[,] squares = new GameObject[8, 8];

    void Start()
    {
        CreateBoard();
        CenterCamera();
    }

    void CreateBoard()
    {
        for (int row = 0; row < boardSize; row++)
        {
            for (int col = 0; col < boardSize; col++)
            {
                GameObject square = GameObject.CreatePrimitive(PrimitiveType.Quad);
                square.name = $"Square_{col}_{row}";
                square.transform.parent = transform;
                square.transform.position = new Vector3(col * squareSize, row * squareSize, 0);

                // Remove 3D collider, add 2D
                DestroyImmediate(square.GetComponent<MeshCollider>());
                square.AddComponent<BoxCollider2D>();

                // Set color via BoardSquare so baseColor is stored correctly
                bool isLight = (row + col) % 2 == 0;
                Color color = isLight ? lightColor : darkColor;

                Renderer rend = square.GetComponent<Renderer>();
                rend.material = new Material(Shader.Find("Sprites/Default"));

                BoardSquare bs = square.AddComponent<BoardSquare>();
                bs.col = col;
                bs.row = row;
                bs.SetBaseColor(color);   // ← stores baseColor AND sets the visual

                squares[col, row] = square;
            }
        }
    }

    public Vector3 GetWorldPosition(int col, int row)
    {
        return new Vector3(col * squareSize, row * squareSize, 0);
    }

    public GameObject GetSquare(int col, int row)
    {
        if (col < 0 || col >= boardSize || row < 0 || row >= boardSize) return null;
        return squares[col, row];
    }

    void CenterCamera()
    {
        float center = (boardSize - 1) * squareSize / 2f;
        Camera.main.transform.position = new Vector3(center, center, -10f);
        Camera.main.orthographic = true;
        Camera.main.orthographicSize = (boardSize * squareSize) / 2f + 0.5f;
    }
}