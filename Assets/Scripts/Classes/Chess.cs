using System.Collections.Generic;
using UnityEngine;
public abstract class Chess : MonoBehaviour
{
    public Camp camp;
    public bool isMove;
    public Vector2Int Location, lastLocation;
    public Material defaultMaterial;
    public abstract void Move(MoveType moveType);

    public virtual List<Selection> CalculateGrid()
    {
        List<Selection> selections = new List<Selection>
        {
            Selection.GetSelection(Location)
        };
        selections[0].MoveSelect();

        return selections;
    }
    
    public void SelectPiece()
    {
        MatchManager.Instance.currentChess = this;
        isMove = false;
        lastLocation = Location;

        MeshRenderer renderers = GetComponentInChildren<MeshRenderer>();
        defaultMaterial = renderers.material;
        renderers.material = Resources.Load<Material>("Material/Other/Yellow");
    }

    public virtual void DeselectPiece()
    {
        if (MatchManager.Instance.currentSelection != null)
        {
            Location = MatchManager.Instance.currentSelection.Location;
        }
        isMove = lastLocation != Location;

        GetComponentInChildren<MeshRenderer>().material = defaultMaterial;
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
    
    public virtual void DestroyPiece()
    {
        Destroy(gameObject);
    }
}
