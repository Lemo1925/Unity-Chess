using System.Collections.Generic;
using UnityEngine;
public abstract class Chess : MonoBehaviour
{
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

        renderers.material = Resources.Load<Material>("Material/Other/Yellow");
    }

    public void DeselectPiece()
    {
        if (MatchManager.Instance.currentSelection != null)
        {
            Location = MatchManager.Instance.currentSelection.Location;
        }

        MeshRenderer renderers = GetComponentInChildren<MeshRenderer>();
        renderers.material = defaultMaterial;
        
        MatchManager.Instance.currentChess = null;
    }

    protected void MovePiece()
    {
        transform.position = MatchManager.Instance.currentSelection.transform.position;
    }

    public void EatPiece(Selection select)
    {
        for (var index = 0; index < select.chessList.Count; index++)
        {
            var chessPiece = select.chessList[index];
            if (chessPiece.camp == camp) continue;
            select.chessList.Remove(chessPiece);
            chessPiece.DestroyPiece();
        }
    }
    
    public void DestroyPiece()
    {
        Destroy(gameObject);
    }
}
