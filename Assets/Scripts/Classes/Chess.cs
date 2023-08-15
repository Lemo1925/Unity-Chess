using System.Collections.Generic;
using UnityEngine;
public abstract class Chess : MonoBehaviour
{
    public Player player;

    public Camp camp;
    
    public Vector2Int Location;
    public Material defaultMaterial;
    public abstract void Move(MoveType moveType);

    public abstract List<Selection> CalculateGrid();
    
    public void SelectPiece()
    {
        MatchManager.Instance.currentChess = this;
        MeshRenderer renderers = GetComponentInChildren<MeshRenderer>();
        defaultMaterial = renderers.material;
        renderers.material = MatchManager.Instance.currentSelection.materials[1];
    }

    public void DeselectPiece()
    {
        Location = MatchManager.Instance.currentSelection.Location;
        MatchManager.Instance.currentChess = null;
        MeshRenderer renderers = GetComponentInChildren<MeshRenderer>();
        renderers.material = defaultMaterial;
    }

    protected void MovePiece()
    {
        transform.position = MatchManager.Instance.currentSelection.transform.position;
    }

}
