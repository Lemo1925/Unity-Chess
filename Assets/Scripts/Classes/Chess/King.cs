
using System.Collections.Generic;

public class King : Chess
{

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

    public void Castling()
    {
        
    }

    public override void DestroyPiece()
    {
        base.DestroyPiece();
        print(camp == Camp.BLACK ? "White Winner" : "Black Winner");
    }
}
