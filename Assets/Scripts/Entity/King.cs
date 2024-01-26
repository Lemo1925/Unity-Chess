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
            select.occupyType != (Grid.OccupyGridType)camp && 
            select.occupyType != Grid.OccupyGridType.NoneOccupyGrid).ToList();

    private List<Grid> MoveGrid() => 
        CalculateMove().Where(select => 
            select.occupyType == Grid.OccupyGridType.NoneOccupyGrid).ToList();

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

    public void InitChessList()
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
        if (!rock || rock.hasMove) return false;

        int start = castlingType ? 1 : 5, 
            end = castlingType ? 4 : 7;
        for (int i = start; i < end; i++)
            if (castChessList[i]) return false;
        return true;
    }
    
    
    public void LongCastling()
    {
        GameStatus.MoveType = "LongCast";
        var newLocation = new Vector2Int(3, location.y);
        castChessList[0].UpdateSelection(castChessList[0].location, newLocation);
        castChessList[0].location = newLocation;
        castChessList[0].MovePiece(newLocation);
    }
    public void ShortCastling()
    {
        GameStatus.MoveType = "ShortCast";
        var newLocation = new Vector2Int(5, location.y);
        castChessList[7].UpdateSelection(castChessList[7].location, newLocation);
        castChessList[7].location = newLocation;
        castChessList[7].MovePiece(newLocation);
    }

    public void Castling()
    {
        if (MatchManager.CurrentGrid.location.x == 2)
            LongCastling();
        else
            ShortCastling();
    }
    
    #endregion
   
    private void Checkmate() => CallCheck(AttackGrid());
}
