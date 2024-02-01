using System.Collections.Generic;
using System.Linq;

public class Knight : Chess
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
        var collection = new List<BoardGrid>();
        int[] deltaX = {1, 1, -1, -1, 2, 2, -2, -2};
        int[] deltaY = {2, -2, 2, -2, 1, -1, 1, -1};

        for (int i = 0; i < deltaX.Length; i++)
        {
            int X = location.x + deltaX[i], Y = location.y + deltaY[i];
            if (BoardGrid.GetSelection(X, Y) != null)
                collection.Add(BoardGrid.GetSelection(X, Y));
        }

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
    
    private void Checkmate() => CallCheck(AttackGrid());
}
