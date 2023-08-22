using System.Collections.Generic;
using System.Linq;

public class Knight : Chess
{
    private void OnEnable() => EventManager.OnTurnEndEvent += Checkmate;
    private void OnDisable() => EventManager.OnTurnEndEvent -= Checkmate;
    
    private List<Selection> MoveGrid() => 
        CalculateMove().Where(select => 
            select.occupyType == Selection.OccupyGridType.NoneOccupyGrid).ToList();

    private List<Selection> AttackGrid() => 
        CalculateMove().Where(select => 
            select.occupyType != (Selection.OccupyGridType)camp && 
            select.occupyType != Selection.OccupyGridType.NoneOccupyGrid).ToList();

    private List<Selection> CalculateMove()
    {
        var selection = Selection.GetSelection(Location);
        var collection = new List<Selection>();
        int[] deltaX = {1, 1, -1, -1, 2, 2, -2, -2};
        int[] deltaY = {2, -2, 2, -2, 1, -1, 1, -1};

        for (int i = 0; i < deltaX.Length; i++)
        {
            int X = Location.x + deltaX[i], Y = Location.y + deltaY[i];
            if (selection.GetSelection(X, Y) != null)
                collection.Add(selection.GetSelection(X, Y));
        }

        return collection;
    }

    public override List<Selection> CalculateGrid()
    {
        var selections = base.CalculateGrid();

        MoveGrid().ForEach(s => { s.MoveSelect(); });
        AttackGrid().ForEach(s => { s.AttackSelect(); });
        
        selections.AddRange(MoveGrid());
        selections.AddRange(AttackGrid());
        
        return selections;
    }
    
    private void Checkmate() => CallCheck(AttackGrid());
}
