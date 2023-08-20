using System.Collections.Generic;
using UnityEngine;

public class King : Chess
{
    // 是否被将军
    private bool isCheckmate;
    // 是否移动过
    private bool firstMove;
    // 是否能易位
    private bool castling;

    public override void Move(MoveType moveType)
    {
        MovePiece();
    }

    public override List<Selection> CalculateGrid()
    {
        Selection selection = MatchManager.Instance.currentSelection;
        List<Selection> selections = base.CalculateGrid();

        var selectionCollection = selection.Bevel(1, 1);
        selectionCollection.AddRange(selection.ForwardAndBack(1,1));
        selectionCollection.AddRange(selection.LeftAndRight(1, 1));

        foreach (var sensor in selectionCollection)
        {
            if (sensor == null || sensor.occupyType == (Selection.OccupyGridType)camp) continue;
            if (sensor.occupyType == Selection.OccupyGridType.NoneOccupyGrid)
            {
                sensor.MoveSelect();
                selections.Add(sensor);
            }
            else
            {
                sensor.AttackSelect();
                selections.Add(sensor);
            }
        }

        return selections;
    }

    public override void DeselectPiece()
    {
        base.DeselectPiece();
        if (isMoved)
        {
            firstMove = false;
        }    
    }

    public void Castling()
    {
        for (int i = 0; i < 7; i++)
        {
            var selection = Selection.GetSelection(new Vector2Int(i, 7));
            var chess = selection.chessPiece;
            
        }
    }

    public override void DestroyPiece()
    {
        base.DestroyPiece();
        print(camp == Camp.BLACK ? "White Winner" : "Black Winner");
    }
}
