using System.Collections.Generic;
using Controller;
using UnityEngine;

public class Pawn : Chess
{
    public int moveTurn = -1;
    public bool isFirstMove;
    public void Start() => isFirstMove = true;

    public override void Move(MoveType moveType) => MovePiece();

    // todo: is first move and moveturn
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
            selections.Add(sensor);
        }
        foreach (var sensor in EnPassSensors)
        {
            if (sensor.occupyType == Selection.OccupyGridType.NoneOccupyGrid || 
                sensor.occupyType == (Selection.OccupyGridType)camp) continue;
            var pawn = sensor.chessPiece.GetComponent<Pawn>();
            if (pawn != null && pawn.moveTurn == GameController.count)
            {
                var EnPassSelection = sensor.GetSelection(pawn.Location.x, pawn.Location.y);
                var targetGrid = EnPassSelection.ForwardAndBack(0,1)[0];
                targetGrid.SpecialSelect();
                selections.Add(targetGrid);
            }
        }
        
        return selections;
    }

    public override void DeselectPiece()
    {
        base.DeselectPiece();
        if (isMove)
        {
            isFirstMove = false;
            moveTurn = GameController.count;
        }
    }

    // 显示升变选择面板
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
        MatchManager.Instance.currentSelection.ForwardAndBack(0, 1)[0].chessPiece.DestroyPiece();
    }
}