using System.Collections.Generic;
using UnityEngine;
public abstract class Chess : MonoBehaviour
{
    public Player player;

    public Camp camp;
    
    public Vector2Int Location;

    public Material defaultMaterial;
    public abstract void Move(Vector2 tarTile, MoveType moveType);

    public abstract List<Selection> CalculateGrid();
    
    public void SelectPiece(Material selectMaterial)
    {
        MatchManager.Instance.currentChess = this;
        MeshRenderer renderers = GetComponentInChildren<MeshRenderer>();
        defaultMaterial = renderers.material;
        renderers.material = selectMaterial;
    }

    public void DeselectPiece()
    {
        MatchManager.Instance.currentChess = null;
        MeshRenderer renderers = GetComponentInChildren<MeshRenderer>();
        renderers.material = defaultMaterial;
    }

    public void MovePiece(Vector2 gridPoint)
    {
        transform.position = Geometry.PointFromGrid(gridPoint);
    }

}
