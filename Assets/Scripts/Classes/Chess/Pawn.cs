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

    private List<BoardGrid> MoveGrid()
    {
        var selections = new List<BoardGrid>();
        var selection = MatchManager.CurrentGrid;

        var collection = selection.ForwardAndBack(isFirstMove ? 2 : 1, 0);

        foreach (var select in collection)
        {
            if (select.occupyType != OccupyGridType.NoneOccupyGrid) continue;
            selections.Add(select);
        }
        
        return selections;
    }

    private List<BoardGrid> AttackGrid()
    {
        var selections = new List<BoardGrid>();
        var selection = BoardGrid.GetSelection(location);

        var collection = selection.Bevel(1, 0);

        foreach (var select in collection)
        {
            if (select.occupyType == (OccupyGridType)camp ||
                select.occupyType == OccupyGridType.NoneOccupyGrid) continue;
            selections.Add(select);
        }
        
        return selections;
    }

    private List<BoardGrid> EnPassGrid()
    {
        var selections = new List<BoardGrid>();
        var selection = MatchManager.CurrentGrid;

        var collection = selection.LeftAndRight(1,1);
        
        foreach (var select in collection)
        {
            if (location.y != 3 && location.y != 4) break;
            
            if (select.occupyType == (OccupyGridType)camp || 
                select.occupyType == OccupyGridType.NoneOccupyGrid) continue;
            
            var pawn = select.chessPiece.GetComponent<Pawn>();
            if (pawn == null || pawn.moveTurn != GameStatus.Count - 1 || pawn.firstMoveStep != 2) continue;
            
            var enPassSelection = BoardGrid.GetSelection(pawn.location.x, pawn.location.y).ForwardAndBack(0,1)[0];
            selections.Add(enPassSelection);
        }

        return selections;
    }

    public override List<BoardGrid> CalculateGrid()
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
            ChessBoard.Instance.chessPrefab[Mathf.Abs((int)chessType) - 1],
            transform.position, transform.rotation);
        
        chessGameObject.GetComponentInChildren<Renderer>().material = (int)chessType > 0 ? 
            ChessBoard.Instance.materials[0] : ChessBoard.Instance.materials[1];

        ChessBoard.Instance.chessGo[chessType].Add(chessGameObject);
        ChessBoard.InitChessComponents(chessGameObject, (int)chessType, location);
        
        var promotionPiece = chessGameObject.GetComponent<Chess>();
        promotionPiece.lastLocation = lastLocation;

        BoardGrid.GetSelection(location).chessList.Remove(this);
        BoardGrid.GetSelection(location).chessPiece = promotionPiece;
        DestroyPiece();

        if (isRemote) return;
        IsMoved = true;
        GameStatus.IsPromotion = false;
    }

    public void En_Pass()
    {
        var enPassSelect = MatchManager.CurrentGrid.ForwardAndBack(0, 1)[0];
        enPassSelect.chessPiece.DestroyPiece();
        enPassSelect.occupyType = OccupyGridType.NoneOccupyGrid;
    }
    
    public void En_Pass(BoardGrid target)
    {
        GameStatus.MoveType = "PassBy";
        target.chessPiece = this;
        target.occupyType = (OccupyGridType)camp;
        // 吃掉对方的兵
        var enemy = target.ForwardAndBack(0, 1)[0];
        enemy.DestroyPiece();
    }

    public void Promotion()
    {
        if (MatchManager.CurrentGrid.occupyType != OccupyGridType.NoneOccupyGrid)
            MatchManager.CurrentChess.EatPiece(MatchManager.CurrentGrid);
        if (GameStatus.IsOver) return;
        GameStatus.IsPromotion = true;
        EventManager.CallOnPromotion(this);
    }

    private void Checkmate() => CallCheck(AttackGrid());
}