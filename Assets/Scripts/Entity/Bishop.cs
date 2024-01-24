using System.Collections.Generic;
using System.Linq;

public class Bishop:Chess
{
    private void OnEnable() => EventManager.OnTurnEndEvent += Checkmate;
    
    private void OnDisable() => EventManager.OnTurnEndEvent -= Checkmate;
    
    private List<Grid> MoveGrid() => 
        CalculateMove().Where(select => 
            select.occupyType == Grid.OccupyGridType.NoneOccupyGrid).ToList();

    private List<Grid> AttackGrid() => 
        CalculateMove().Where(select => 
            select.occupyType != (Grid.OccupyGridType)camp && 
            select.occupyType != Grid.OccupyGridType.NoneOccupyGrid).ToList();

    private List<Grid> CalculateMove()
    {
        var selection = Grid.GetSelection(location);
        return selection.Bevel(7, 7);
    }

    public override List<Grid> CalculateGrid()
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
