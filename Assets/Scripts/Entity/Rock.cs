using System.Collections.Generic;
using System.Linq;

public class Rock : Chess
{
    public bool hasMove{ get; private set; }

    private void Awake()
    {
        hasMove = false;
    }

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

        var collection = selection.ForwardAndBack(7, 7);
        collection.AddRange(selection.LeftAndRight(7, 7));

        return collection;
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

    public override void DeselectPiece()
    {
        base.DeselectPiece();
        if (IsMoved) hasMove = true;
    }
    
    private void Checkmate() => CallCheck(AttackGrid());
}
