using System.Collections.Generic;
using System.Linq;

public class Bishop:Chess
{
    private void OnEnable() => EventManager.OnTurnEndEvent += Checkmate;
    
    private void OnDisable() => EventManager.OnTurnEndEvent -= Checkmate;
    
    private List<BoardGrid> MoveGrid() => 
        CalculateMove().Where(select => 
            select.occupyType == OccupyGridType.NoneOccupyGrid).ToList();

    private List<BoardGrid> AttackGrid() => 
        CalculateMove().Where(select => 
            select.occupyType != (OccupyGridType)camp && 
            select.occupyType != OccupyGridType.NoneOccupyGrid).ToList();

    private List<BoardGrid> CalculateMove()
    {
        var selection = BoardGrid.GetSelection(location);
        return selection.Bevel(7, 7);
    }

    public override List<BoardGrid> CalculateGrid()
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
