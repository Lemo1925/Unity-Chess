using System.Collections.Generic;
using UnityEngine;
public abstract class Chess : MonoBehaviour
{
    public Camp camp;
    public Vector2Int Location, lastLocation;
    public Material defaultMaterial;
    
    public static bool isMoved { get; set; }
    
    public void Move() => MovePiece();

    public virtual List<Selection> CalculateGrid()
    {
        var selections = new List<Selection> { Selection.GetSelection(Location) };
        selections[0].MoveSelect();
        
        return selections;
    }

    public void SelectPiece()
    {
        MatchManager.Instance.currentChess = this;
        GameStatus.instance.selectChess = this;
        isMoved = false;
        lastLocation = Location;

        MeshRenderer renderers = GetComponentInChildren<MeshRenderer>();
        defaultMaterial = renderers.material;
        renderers.material = Resources.Load<Material>("Material/Other/Yellow");
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

    public virtual void DeselectPiece()
    {
        if (MatchManager.Instance.currentSelection != null) Location = MatchManager.Instance.currentSelection.Location;
        isMoved = lastLocation != Location;
        
        GetComponentInChildren<MeshRenderer>().material = defaultMaterial;
        MatchManager.Instance.currentChess = null;
    }

    private void MovePiece() => 
        transform.position = MatchManager.Instance.currentSelection.transform.position;

    public void MovePiece(int x, int y) => 
        transform.position = ChessBoard.instance.ChessSelections[x, y].transform.position;

    public void MovePiece(Vector2Int location) => transform.position = ChessBoard.instance.ChessSelections[location.x, location.y].transform.position;

    public virtual void DestroyPiece() => Destroy(gameObject);

    protected void CallCheck(List<Selection> selections)
    {
        foreach (var selection in selections)
        {
            if (selection.chessPiece.GetComponent<King>() == null) continue;
            var king = (King)selection.chessPiece;
            if (king != null && king.camp != camp)
            {
                MatchManager.Instance.checkmate = (int)king.camp;
                return;
            }
        }
    }
}
