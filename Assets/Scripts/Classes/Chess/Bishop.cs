
using System.Collections.Generic;

public class Bishop:Chess
{

    public override void Move()
    {
        MovePiece();
    }

    public override List<Selection> CalculateGrid()
    {
        Selection selection = MatchManager.Instance.currentSelection;
        List<Selection> selections = base.CalculateGrid();

        var selectCollection = 
            selection.Bevel(ChessBoard.BoardLocationMax.x, 
                ChessBoard.BoardLocationMax.y);
        foreach (var sensor in selectCollection)
        {
            if (sensor == null || sensor.occupyType == (Selection.OccupyGridType)camp) continue;
            if (sensor.occupyType == Selection.OccupyGridType.NoneOccupyGrid )
            {
                sensor.MoveSelect();
                selections.Add(sensor);
            }
            else
            {
                sensor.AttackSelect();
                var king = sensor.chessPiece.GetComponent<King>();
                if (king != null) king.isCheckmate = true;
                selections.Add(sensor);
            }
        }
        return selections;
    }
}
