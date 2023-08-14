
using System.Collections.Generic;
using UnityEngine;

public class Bishop:Chess
{

    public override void Move(MoveType moveType)
    {
        switch (moveType)
        {
            case MoveType.Move:
                break;
            case MoveType.Eat:
                break;
        }
    }

    public override List<Selection> CalculateGrid()
    {
        List<Selection> grids = new List<Selection>();
        Vector2Int curGrid = MatchManager.Instance.currentSelection.Location;
        
        int x = curGrid.x, y = curGrid.y;

        return grids;
    }
}
