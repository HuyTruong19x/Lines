using UnityEngine;
public static class VectorExtension
{
    public static Vector3Int CreateFromVector2Int(this Vector3Int vector3Int, Vector2Int vector2Int)
    {
        return new Vector3Int(vector2Int.x, vector2Int.y, 0);
    }
}
