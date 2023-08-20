using System.Collections.Generic;
using UnityEngine;

public class Pawn : Chess
{
    public bool isFirstMove;
    public int firstMoveStep;
    public int moveTurn = -1;
    public void Start() => isFirstMove = true;
    public override void Move() => MovePiece();
    public override List<Selection> CalculateGrid()
    {
        Selection selection = MatchManager.Instance.currentSelection;
        List<Selection> selections = base.CalculateGrid();
        List<Selection> MoveSensors = selection.ForwardAndBack(isFirstMove ? 2 : 1, 0);
        List<Selection> AttackSensors = selection.Bevel(1, 0);
        List<Selection> EnPassSensors = selection.LeftAndRight(1, 1);
        foreach (var sensor in MoveSensors)
        {
            if (sensor.occupyType != Selection.OccupyGridType.NoneOccupyGrid) continue;
            if (sensor.Location.y == 0 || sensor.Location.y == 7) sensor.SpecialSelect();
            else sensor.MoveSelect();
            selections.Add(sensor);
        }
        foreach (var sensor in AttackSensors)
        {
            if (sensor.occupyType == (Selection.OccupyGridType)camp ||
                sensor.occupyType == Selection.OccupyGridType.NoneOccupyGrid) continue;
            if (sensor.Location.y == 0 || sensor.Location.y == 7) sensor.SpecialSelect();
            else sensor.AttackSelect();
            var king = sensor.chessPiece.GetComponent<King>();
            if (king != null) king.isCheckmate = true;
            selections.Add(sensor);
        }
        foreach (var sensor in EnPassSensors)
        {
            if (Location.y != 3 && Location.y != 4) break;
            if (sensor.occupyType == Selection.OccupyGridType.NoneOccupyGrid || 
                sensor.occupyType == (Selection.OccupyGridType)camp) continue;
            var pawn = sensor.chessPiece.GetComponent<Pawn>();
            if (pawn == null || pawn.moveTurn != GameController.count - 1 || pawn.firstMoveStep != 2) continue;
            var EnPassSelection = sensor.GetSelection(pawn.Location.x, pawn.Location.y).ForwardAndBack(0,1)[0];
            EnPassSelection.SpecialSelect();
            selections.Add(EnPassSelection);
        }
        
        return selections;
    }
    public override void DeselectPiece()
    {
        base.DeselectPiece();
        if (isMoved)
        {           
            firstMoveStep = Mathf.Abs(lastLocation.y - Location.y);
            isFirstMove = false;
            moveTurn = GameController.count;
        }
    }
    public void Promotion() => EventManager.CallOnPromotion(this,true);
    public void PromotionLogic(ChessType chessType)
    {
        var chessGameObject = Instantiate(
            ChessBoard.instance.ChessPrefab[Mathf.Abs((int)chessType) - 1],
            transform.position, transform.rotation);
        
        chessGameObject.GetComponentInChildren<Renderer>().material = (int)chessType > 0 ? 
            ChessBoard.instance.materials[0] : ChessBoard.instance.materials[1];
        
        ChessBoard.instance.chessGO[chessType].Add(chessGameObject);
        ChessBoard.InitChessComponents(chessGameObject, (int)chessType, Location);
        DestroyPiece();
    }
    public void En_Pass()
    {
        Selection EnPassSelect = MatchManager.Instance.currentSelection.ForwardAndBack(0, 1)[0];
        EnPassSelect.chessPiece.DestroyPiece();
        EnPassSelect.occupyType = Selection.OccupyGridType.NoneOccupyGrid;
    }
}