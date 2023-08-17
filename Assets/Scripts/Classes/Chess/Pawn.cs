using System.Collections.Generic;

public class Pawn : Chess
{
    public int moveTimes;
    
    public void Start() => moveTimes = 0;

    public override void Move(MoveType moveType)
    {
        switch (moveType)
        {
            case MoveType.Move:
                MovePiece();
                moveTimes++;
                break;
            case MoveType.En_Pass:
            
                break;
            case MoveType.Promotion:
            
                break;
        }
        
    }

    public override List<Selection> CalculateGrid()
    {
        List<Selection> selections = new List<Selection>();
        Selection selection = MatchManager.Instance.currentSelection;
        List<Selection> MoveSensors = selection.ForwardAndBack(moveTimes == 0 ? 2 : 1, 0);
        List<Selection> AttackSensors = selection.Bevel(1, 0);
        foreach (var sensor in MoveSensors)
        {
            if (sensor.occupyType != 
                Selection.OccupyGridType.NoneOccupyGrid) continue;
            sensor.MoveSelect();
            selections.Add(sensor);
        }

        foreach (var sensor in AttackSensors)
        {
            if (sensor.occupyType == 
                (Selection.OccupyGridType)camp) continue;
            if (sensor.occupyType == 
                Selection.OccupyGridType.NoneOccupyGrid) continue;
            sensor.AttackSelect();
            selections.Add(sensor);
        }
        return selections;
    }

    public void Promotion(ChessType type) {}

    public void En_Pass(){}
}