using System.Collections.Generic;
using System.Linq;

public class Rock : Chess
{
    public bool hasMove{ get; private set; }

    private void Start() => hasMove = false;
    public override void Move() => MovePiece();

    private void OnEnable() => EventManager.OnTurnEndEvent += Checkmate;
    private void OnDisable() => EventManager.OnTurnEndEvent -= Checkmate;
    
    private List<Selection> CalculateMove()
    {
        var selection = Selection.GetSelection(Location);

        var collection = selection.ForwardAndBack(7, 7);
        collection.AddRange(selection.LeftAndRight(7, 7));

        return collection;
    }

    private List<Selection> MoveGrid() => 
        CalculateMove().Where(select => 
            select.occupyType == Selection.OccupyGridType.NoneOccupyGrid).ToList();

    private List<Selection> AttackGrid() => 
        CalculateMove().Where(select => 
            select.occupyType != (Selection.OccupyGridType)camp && 
            select.occupyType != Selection.OccupyGridType.NoneOccupyGrid).ToList();

    public override List<Selection> CalculateGrid()
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
        if (isMoved) hasMove = true;
    }
    
    private void Checkmate() => CallCheck(AttackGrid());
}
