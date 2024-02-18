using System.Collections.Generic;
using UnityEngine;

public abstract class Chess : MonoBehaviour
{
    public Camp camp;
    public Vector2Int location, lastLocation;
    public Material defaultMaterial;

    public static bool IsMoved;

    public void Start()
    {
        var selection = BoardGrid.GetSelection(location);
        selection.chessPiece = this;
        selection.occupyType = (OccupyGridType)camp;
        selection.chessList.Add(this);
    }

    public virtual List<BoardGrid> CalculateGrid()
    {
        var selections = new List<BoardGrid> { BoardGrid.GetSelection(location) };
        selections[0].MoveSelect();
        
        return selections;
    }

    public void SelectPiece()
    {
        MatchManager.CurrentChess = this;
        GameStatus.Instance.selectChess = this;
        IsMoved = false;
        lastLocation = location;
        
        MeshRenderer renderers = GetComponentInChildren<MeshRenderer>();
        defaultMaterial = renderers.material;
        renderers.material = Resources.Load<Material>("Material/Other/Yellow");
    }

    public void UpdateSelection() => UpdateSelection(lastLocation, location);

    public void UpdateSelection(Vector2Int lastSelect, Vector2Int select)
    {
        // remove Chess Piece in Selection
        var lastSelection = BoardGrid.GetSelection(lastSelect);
        if (lastSelection.chessList.Contains(this)) 
            lastSelection.chessList.Remove(this);
        lastSelection.chessPiece = null;
        lastSelection.occupyType = OccupyGridType.NoneOccupyGrid;

        // update Chess Piece in Selection
        var newSelection = BoardGrid.GetSelection(select);
        if (!newSelection.chessList.Contains(this))
            newSelection.chessList.Add(this);
        newSelection.chessPiece = this;
        newSelection.occupyType = (OccupyGridType)camp;
    }
    
    public virtual void DeselectPiece()
    {
        if (MatchManager.CurrentGrid != null) 
            location = MatchManager.CurrentGrid.location;
        
        IsMoved = lastLocation != location && !GameStatus.IsPromotion;

        GetComponentInChildren<MeshRenderer>().material = defaultMaterial;
        MatchManager.CurrentChess = null;
    }

    public void EatPiece(BoardGrid select)
    {
        for (var index = 0; index < select.chessList.Count; index++)
        {
            var chessPiece = select.chessList[index];
            if (chessPiece.camp == camp) continue;
            select.chessList.Remove(chessPiece);
            chessPiece.DestroyPiece();
        }
    }

    public void MovePiece() => 
        transform.position = MatchManager.CurrentGrid.transform.position;

    public void MovePiece(Vector2Int loc) => 
        transform.position = ChessBoard.Instance.ChessSelections[loc.x, loc.y].transform.position;

    public virtual void DestroyPiece() => Destroy(gameObject);

    protected void CallCheck(List<BoardGrid> selections)
    {
        foreach (var selection in selections)
        {
            if (selection.chessPiece.GetComponent<King>() == null) continue;
            var king = (King)selection.chessPiece;
            if (king != null && king.camp != camp)
            {
                MatchManager.Instance.Checkmate = (int)king.camp;
                return;
            }
        }
    }
}
