using System.Collections.Generic;

public class Pawn : Chess
{
    public bool isFirstMove;
    
    public void Start() => isFirstMove = true;

    public override void Move(MoveType moveType)
    {
        switch (moveType)
        {
            // TODO 移动策略 Move Eat En_Pass Promotion
            case MoveType.Move:
                MovePiece();
                isFirstMove = false;
                break;
            case MoveType.Eat:
            
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
        List<Selection> MoveSensors = selection.Forward(isFirstMove ? 2 : 1, 0);
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

    public void Promotion(ChessType type)
    {
        
    }
}