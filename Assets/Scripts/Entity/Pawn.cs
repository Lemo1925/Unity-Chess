﻿using System.Collections.Generic;
using UnityEngine;

public class Pawn : Chess
{
    public bool isFirstMove;
    public int firstMoveStep;
    public int moveTurn = -1;
    
    public void Awake()
    {
        isFirstMove = true;
    }

    private void OnEnable() => EventManager.OnTurnEndEvent += Checkmate;
    private void OnDisable() => EventManager.OnTurnEndEvent -= Checkmate;

    private List<Selection> MoveGrid()
    {
        var selections = new List<Selection>();
        var selection = MatchManager.currentSelection;

        var collection = selection.ForwardAndBack(isFirstMove ? 2 : 1, 0);

        foreach (var select in collection)
        {
            if (select.occupyType != Selection.OccupyGridType.NoneOccupyGrid) continue;
            selections.Add(select);
        }
        
        return selections;
    }

    private List<Selection> AttackGrid()
    {
        var selections = new List<Selection>();
        var selection = Selection.GetSelection(Location);

        var collection = selection.Bevel(1, 0);

        foreach (var select in collection)
        {
            if (select.occupyType == (Selection.OccupyGridType)camp ||
                select.occupyType == Selection.OccupyGridType.NoneOccupyGrid) continue;
            selections.Add(select);
        }
        
        return selections;
    }

    private List<Selection> EnPassGrid()
    {
        var selections = new List<Selection>();
        var selection = MatchManager.currentSelection;

        var collection = selection.LeftAndRight(1,1);
        
        foreach (var select in collection)
        {
            if (Location.y != 3 && Location.y != 4) break;
            
            if (select.occupyType == (Selection.OccupyGridType)camp || 
                select.occupyType == Selection.OccupyGridType.NoneOccupyGrid) continue;
            
            var pawn = select.chessPiece.GetComponent<Pawn>();
            if (pawn == null || pawn.moveTurn != GameStatus.count - 1 || pawn.firstMoveStep != 2) continue;
            
            var EnPassSelection = select.GetSelection(pawn.Location.x, pawn.Location.y).ForwardAndBack(0,1)[0];
            selections.Add(EnPassSelection);
        }

        return selections;
    }

    public override List<Selection> CalculateGrid()
    {
        var selections = base.CalculateGrid();

        MoveGrid().ForEach(s => { s.MoveSelect(); });
        AttackGrid().ForEach(s => { s.AttackSelect(); });
        EnPassGrid().ForEach(s => { s.SpecialSelect(); });
        
        selections.AddRange(MoveGrid());
        selections.AddRange(AttackGrid());
        selections.AddRange(EnPassGrid());
        
        foreach (var select in selections)
            if (select.Location.y == 0 || select.Location.y == 7)
                select.SpecialSelect();

        return selections;
    }
    
    public override void DeselectPiece()
    {
        base.DeselectPiece();
        if (isMoved)
        {           
            firstMoveStep = Mathf.Abs(lastLocation.y - Location.y);
            isFirstMove = false;
            moveTurn = GameStatus.count;
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
        ChessBoard.InitChessComponents(chessGameObject, (int)chessType, Location);
        
        var PromotionPiece = chessGameObject.GetComponent<Chess>();
        PromotionPiece.lastLocation = lastLocation;

        Selection.GetSelection(Location).chessList.Remove(this);
        Selection.GetSelection(Location).chessPiece = PromotionPiece;
        DestroyPiece();

        if (isRemote) return;
        isMoved = true;
        GameStatus.isPromotion = false;
    }

    public void En_Pass()
    {
        var EnPassSelect = MatchManager.currentSelection.ForwardAndBack(0, 1)[0];
        print(EnPassSelect.Location);
        EnPassSelect.chessPiece.DestroyPiece();
        EnPassSelect.occupyType = Selection.OccupyGridType.NoneOccupyGrid;
    }
    
    public void En_Pass(Selection target)
    {
        var EnPassSelect = target.ForwardAndBack(0, 1)[0];
        EnPassSelect.chessList.Remove(EnPassSelect.chessPiece);
        EnPassSelect.chessPiece.DestroyPiece();
        EnPassSelect.chessPiece = null;
        EnPassSelect.occupyType = Selection.OccupyGridType.NoneOccupyGrid;
    }

    public void Promotion() => EventManager.CallOnPromotion(this,true);

    private void Checkmate() => CallCheck(AttackGrid());
}