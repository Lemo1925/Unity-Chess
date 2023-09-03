using System.Collections.Generic;
using UnityEngine;
public abstract class Chess : MonoBehaviour
{
    public Camp camp;
    public Vector2Int Location, lastLocation;
    public Material defaultMaterial;

    public static bool isMoved;

    public void Start()
    {
        var selection = Selection.GetSelection(Location);
        selection.chessPiece = this;
        selection.occupyType = (Selection.OccupyGridType)camp;
        selection.chessList.Add(this);
    }

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

    public void UpdateSelection()
    {
        // remove Chess Piece in Selection
        var lastSelection = Selection.GetSelection(lastLocation);
        if (lastSelection.chessList.Contains(this))
            lastSelection.chessList.Remove(this);
        lastSelection.chessPiece = null;
        lastSelection.occupyType = Selection.OccupyGridType.NoneOccupyGrid;

        // update Chess Piece in Selection
        var newSelection = Selection.GetSelection(Location);
        if (!newSelection.chessList.Contains(this))
            newSelection.chessList.Add(this);
        newSelection.chessPiece = this;
        newSelection.occupyType = (Selection.OccupyGridType)camp;
    }
    
    public void UpdateSelection(Vector2Int lastLocation, Vector2Int Location)
    {
        // remove Chess Piece in Selection
        var lastSelection = Selection.GetSelection(lastLocation);
        if (lastSelection.chessList.Contains(this)) 
            lastSelection.chessList.Remove(this);
        lastSelection.chessPiece = null;
        lastSelection.occupyType = Selection.OccupyGridType.NoneOccupyGrid;

        // update Chess Piece in Selection
        var newSelection = Selection.GetSelection(Location);
        if (!newSelection.chessList.Contains(this))
            newSelection.chessList.Add(this);
        newSelection.chessPiece = this;
        newSelection.occupyType = (Selection.OccupyGridType)camp;
    }
    
    public virtual void DeselectPiece()
    {
        if (MatchManager.Instance.currentSelection != null) Location = MatchManager.Instance.currentSelection.Location;
        
        isMoved = lastLocation != Location && !GameStatus.instance.isPromotion;

        GetComponentInChildren<MeshRenderer>().material = defaultMaterial;
        MatchManager.Instance.currentChess = null;
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

    private void MovePiece() => 
        transform.position = MatchManager.Instance.currentSelection.transform.position;

    public void MovePiece(int x, int y) => 
        transform.position = ChessBoard.instance.ChessSelections[x, y].transform.position;

    public void MovePiece(Vector2Int location) => 
        transform.position = ChessBoard.instance.ChessSelections[location.x, location.y].transform.position;

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
