using System.Collections.Generic;
using System.Linq;

public class Queen : Chess
{
    private void OnEnable() => EventManager.OnTurnEndEvent += Checkmate;
    
    private void OnDisable() => EventManager.OnTurnEndEvent -= Checkmate;
    
    public override void Move() => MovePiece();

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

        var collection = selection.ForwardAndBack(7, 7);
        collection.AddRange(selection.LeftAndRight(7, 7));
        collection.AddRange(selection.Bevel(7, 7));
        
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
