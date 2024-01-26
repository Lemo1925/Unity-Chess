using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class King : Chess
{
    private bool _hasMove;
    public bool isCheckmate;
    
    public List<Chess> castChessList = new();

    private void OnEnable() => EventManager.OnTurnEndEvent += Checkmate;
    private void OnDisable() => EventManager.OnTurnEndEvent -= Checkmate;

    private void Awake()
    {
        _hasMove = false; 
        isCheckmate = false;
    }
    
    private List<Grid> AttackGrid() => 
        CalculateMove().Where(select => 
            select.occupyType != (OccupyGridType)camp && 
            select.occupyType != OccupyGridType.NoneOccupyGrid).ToList();

    private List<Grid> MoveGrid() => 
        CalculateMove().Where(select => 
            select.occupyType == OccupyGridType.NoneOccupyGrid).ToList();

    private List<Grid> SpecialGrid()
    {
        var selections = new List<Grid>();
        if (CanCastling(true)) selections.Add(Grid.GetSelection(location.x - 2, location.y));
        if (CanCastling(false)) selections.Add(Grid.GetSelection(location.x + 2, location.y));

        return selections;
    }

    private List<Grid> CalculateMove()
    {
        var selection = Grid.GetSelection(location);

        var collection = selection.Bevel(1, 1);
        collection.AddRange(selection.ForwardAndBack(1, 1));
        collection.AddRange(selection.LeftAndRight(1, 1));

        return collection;
    }

    public override List<Grid> CalculateGrid()
    {
        var selections = base.CalculateGrid();
        
        AttackGrid().ForEach(s => { s.AttackSelect(); });
        MoveGrid().ForEach(s => { s.MoveSelect(); });
        SpecialGrid().ForEach(s => { s.SpecialSelect(); });

        selections.AddRange(MoveGrid());
        selections.AddRange(AttackGrid());
        selections.AddRange(SpecialGrid());

        return selections;
    }

    public override void DeselectPiece()
    {
        base.DeselectPiece();
        if (IsMoved)
        {
            _hasMove = true;
            isCheckmate = false;
        }
    }

    public override void DestroyPiece()
    {
        base.DestroyPiece();
        GameStatus.IsOver = true;
    }

    #region 王车易位

    private void InitChessList()
    {
        // 初始化存储王车之间棋子的列表
        castChessList.Clear();
        for (int i = 0; i < 8; i++) 
            castChessList.Add(Grid.GetSelection(i, location.y).chessPiece);
    }

    private bool CanCastling(bool castlingType)
    {
        isCheckmate = MatchManager.Instance.Checkmate == (int)camp;
        if (_hasMove || isCheckmate) return false;
        InitChessList();

        int index = castlingType ? 0 : 7;
        Rock rock = (Rock)castChessList[index];
        if (!rock || rock.HasMove) return false;

        int start = castlingType ? 1 : 5, 
            end = castlingType ? 4 : 7;
        for (int i = start; i < end; i++)
            if (castChessList[i]) return false;
        return true;
    }
    
    public void Castling(bool castlingType) // true is long cast, false is short cast 
    {
        InitChessList();
        GameStatus.MoveType = castlingType ? "LongCast" : "ShortCast";
        int index = castlingType ? 0 : 7, loc = castlingType ? 3 : 5;
        Vector2Int newLoc = new Vector2Int(loc, location.y);
        castChessList[index].UpdateSelection(castChessList[index].location, newLoc);
        castChessList[index].location = newLoc;
        castChessList[index].MovePiece(newLoc);
    }
    
    #endregion
   
    private void Checkmate() => CallCheck(AttackGrid());
}
