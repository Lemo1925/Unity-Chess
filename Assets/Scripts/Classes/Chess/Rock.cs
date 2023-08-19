
using System.Collections.Generic;

public class Rock : Chess
{

    public override void Move(MoveType moveType)
    {
        if (moveType == MoveType.Move) MovePiece();
    }

    public override List<Selection> CalculateGrid()
    {
        Selection selection = MatchManager.Instance.currentSelection;
        List<Selection> selections = base.CalculateGrid();

        var selectionCollection = selection.LeftAndRight(
            ChessBoard.BoardLocationMax.x, ChessBoard.BoardLocationMax.y);
        selectionCollection.AddRange(selection.ForwardAndBack(
            ChessBoard.BoardLocationMax.x, ChessBoard.BoardLocationMax.y));

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
}
