using System.Collections.Generic;
using UnityEngine;

public class Pawn : Chess
{
    public bool isFirstMove;
    public int firstMoveStep;
    public int moveTurn = -1;
    
    public void Awake() => isFirstMove = true;

    private void OnEnable() => EventManager.OnTurnEndEvent += Checkmate;
    private void OnDisable() => EventManager.OnTurnEndEvent -= Checkmate;

    private List<Grid> MoveGrid()
    {
        var selections = new List<Grid>();
        var selection = MatchManager.CurrentGrid;

        var collection = selection.ForwardAndBack(isFirstMove ? 2 : 1, 0);

        foreach (var select in collection)
        {
            if (select.occupyType != Grid.OccupyGridType.NoneOccupyGrid) continue;
            selections.Add(select);
        }
        
        return selections;
    }

    private List<Grid> AttackGrid()
    {
        var selections = new List<Grid>();
        var selection = Grid.GetSelection(location);

        var collection = selection.Bevel(1, 0);

        foreach (var select in collection)
        {
            if (select.occupyType == (Grid.OccupyGridType)camp ||
                select.occupyType == Grid.OccupyGridType.NoneOccupyGrid) continue;
            selections.Add(select);
        }
        
        return selections;
    }

    private List<Grid> EnPassGrid()
    {
        var selections = new List<Grid>();
        var selection = MatchManager.CurrentGrid;

        var collection = selection.LeftAndRight(1,1);
        
        foreach (var select in collection)
        {
            if (location.y != 3 && location.y != 4) break;
            
            if (select.occupyType == (Grid.OccupyGridType)camp || 
                select.occupyType == Grid.OccupyGridType.NoneOccupyGrid) continue;
            
            var pawn = select.chessPiece.GetComponent<Pawn>();
            if (pawn == null || pawn.moveTurn != GameStatus.Count - 1 || pawn.firstMoveStep != 2) continue;
            
            var EnPassSelection = Grid.GetSelection(pawn.location.x, pawn.location.y).ForwardAndBack(0,1)[0];
            selections.Add(EnPassSelection);
        }

        return selections;
    }

    public override List<Grid> CalculateGrid()
    {
        var selections = base.CalculateGrid();

        MoveGrid().ForEach(s => { s.MoveSelect(); });
        AttackGrid().ForEach(s => { s.AttackSelect(); });
        EnPassGrid().ForEach(s => { s.SpecialSelect(); });
        
        selections.AddRange(MoveGrid());
        selections.AddRange(AttackGrid());
        selections.AddRange(EnPassGrid());
        
        foreach (var select in selections)
            if (select.location.y == 0 || select.location.y == 7)
                select.SpecialSelect();

        return selections;
    }
    
    public override void DeselectPiece()
    {
        base.DeselectPiece();
        if (IsMoved)
        {           
            firstMoveStep = Mathf.Abs(lastLocation.y - location.y);
            isFirstMove = false;
            moveTurn = GameStatus.Count;
        }
    }

    public void PromotionLogic(ChessType chessType, bool isRemote = false)
    {
        var chessGameObject = Instantiate(
            ChessBoard.instance.ChessPrefab[Mathf.Abs((int)chessType) - 1],
            transform.position, transform.rotation);
        
        chessGameObject.GetComponentInChildren<Renderer>().material = (int)chessType > 0 ? 
            ChessBoard.instance.materials[0] : ChessBoard.instance.materials[1];

        ChessBoard.instance.chessGO[chessType].Add(chessGameObject);
        ChessBoard.InitChessComponents(chessGameObject, (int)chessType, location);
        
        var PromotionPiece = chessGameObject.GetComponent<Chess>();
        PromotionPiece.lastLocation = lastLocation;

        Grid.GetSelection(location).chessList.Remove(this);
        Grid.GetSelection(location).chessPiece = PromotionPiece;
        DestroyPiece();

        if (isRemote) return;
        IsMoved = true;
        GameStatus.IsPromotion = false;
    }

    public void En_Pass()
    {
        var EnPassSelect = MatchManager.CurrentGrid.ForwardAndBack(0, 1)[0];
        print(EnPassSelect.location);
        EnPassSelect.chessPiece.DestroyPiece();
        EnPassSelect.occupyType = Grid.OccupyGridType.NoneOccupyGrid;
    }
    
    public void En_Pass(Grid target)
    {
        var EnPassSelect = target.ForwardAndBack(0, 1)[0];
        EnPassSelect.chessList.Remove(EnPassSelect.chessPiece);
        EnPassSelect.chessPiece.DestroyPiece();
        EnPassSelect.chessPiece = null;
        EnPassSelect.occupyType = Grid.OccupyGridType.NoneOccupyGrid;
    }

    public void Promotion() => EventManager.CallOnPromotion(this,true);

    private void Checkmate() => CallCheck(AttackGrid());
}