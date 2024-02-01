using System.Collections.Generic;
using System.Linq;

public class Rock : Chess
{
    public bool HasMove{ get; private set; }

    private void Awake() => HasMove = false;

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

        var collection = selection.ForwardAndBack(7, 7);
        collection.AddRange(selection.LeftAndRight(7, 7));

        return collection;
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

    public override void DeselectPiece()
    {
        base.DeselectPiece();
        if (IsMoved) HasMove = true;
    }
    
    private void Checkmate() => CallCheck(AttackGrid());
}
