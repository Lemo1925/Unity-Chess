
using System.Collections.Generic;

public class Knight : Chess
{

    public override void Move(MoveType moveType)
    {
        MovePiece();
    }

    public override List<Selection> CalculateGrid()
    {
        List<Selection> selections = new List<Selection>();
        Selection selection = MatchManager.Instance.currentSelection;
        List<Selection> selectionCollection = new List<Selection>();
        
        int[] deltaX = {1, 1, -1, -1, 2, 2, -2, -2};
        int[] deltaY = {2, -2, 2, -2, 1, -1, 1, -1};

        for (int i = 0; i < deltaX.Length; i++)
        {
            int newX = Location.x + deltaX[i];
            int newY = Location.y + deltaY[i];
            if (selection.GetSelection(newX, newY) != null)
                selectionCollection.Add(selection.GetSelection(newX, newY));
        }
        
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
