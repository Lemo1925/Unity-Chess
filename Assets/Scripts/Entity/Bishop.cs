using System.Collections.Generic;
using System.Linq;

public class Bishop:Chess
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
        return selection.Bevel(7, 7);
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
