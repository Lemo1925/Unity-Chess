
using System.Collections.Generic;

public class Queen : Chess
{
    public override void Move(MoveType moveType)
    {
        MovePiece();
    }

    public override List<Selection> CalculateGrid()
    {
        Selection selection = MatchManager.Instance.currentSelection;
        List<Selection> selections = base.CalculateGrid();

        var selectionCollection = selection.ForwardAndBack(
            ChessBoard.BoardLocationMax.x, ChessBoard.BoardLocationMax.y
        );
        selectionCollection.AddRange(selection.LeftAndRight(
            ChessBoard.BoardLocationMax.x, ChessBoard.BoardLocationMax.y
        ));
        selectionCollection.AddRange(selection.Bevel(
            ChessBoard.BoardLocationMax.x, ChessBoard.BoardLocationMax.y
        ));

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
