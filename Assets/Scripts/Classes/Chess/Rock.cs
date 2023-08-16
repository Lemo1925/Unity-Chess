﻿
using System.Collections.Generic;

public class Rock : Chess
{

    public override void Move(MoveType moveType)
    {
        if (moveType == MoveType.Move) MovePiece();
    }

    public override List<Selection> CalculateGrid()
    {
        List<Selection> selections = new List<Selection>();
        Selection selection = MatchManager.Instance.currentSelection;

        var selectionCollection = selection.Left(
            ChessBoard.BoardLocationMax.x, ChessBoard.BoardLocationMax.y);
        selectionCollection.AddRange(selection.Forward(
            ChessBoard.BoardLocationMax.x, ChessBoard.BoardLocationMax.y));

        foreach (var sensor in selectionCollection)
        {
            if (sensor == null) continue;
            if (sensor.occupyType == Selection.OccupyGridType.NoneOccupyGrid)
            {
                sensor.MoveSelect();
                selections.Add(sensor);
            }else if (sensor.occupyType == (Selection.OccupyGridType)camp)
            {
                
            }
            else
            {
                sensor.AttackSelect();
                selections.Add(sensor);
            }
        }

        return selections;
    }
}
