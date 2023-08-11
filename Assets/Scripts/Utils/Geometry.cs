
using UnityEngine;

public static class Geometry
{
    public static Vector2 GridPoint(int col, int row)
    {
        //gives you a GridPoint for a given column and row.
        Vector2 gridPoint = new Vector2(col, row);
        return gridPoint;
    }

    public static Vector3 PointFromGrid(Vector2 gridPoint)
    {
        //turns a GridPoint into a Vector3 actual point in the scene.
        Vector3 point = new Vector3(gridPoint.x, 0.001f, gridPoint.y);
        return point;
    }

    public static Vector2 GridFromPoint(Vector3 point)
    {
        //gives the GridPoint for the x and z value of that 3D point, and the y value is ignored.
        Vector2 gridPoint = new Vector2((int)point.x, (int)point.z);
        return gridPoint;
    }
}
