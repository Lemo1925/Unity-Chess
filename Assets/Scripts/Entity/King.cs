using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class King : Chess
{
    private bool hasMove;
    public bool isCheckmate;
    
    public List<Chess> castChessList = new List<Chess>();

    private void OnEnable() => EventManager.OnTurnEndEvent += Checkmate;
    private void OnDisable() => EventManager.OnTurnEndEvent -= Checkmate;

    private void Awake()
    {
        hasMove = false; 
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
        var selection = MatchManager.CurrentGrid;

        if (CanLongCastling()) selections.Add(Grid.GetSelection(location.x - 2, location.y));
        if (CanShortCastling()) selections.Add(Grid.GetSelection(location.x + 2, location.y));

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
            hasMove = true;
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
        for (int i = 0; i < 8; i++)
        {
            castChessList.Add(Grid.GetSelection(new Vector2Int(i, location.y)).chessPiece);
        }
    }
    private bool CanLongCastling()
    {
        isCheckmate = MatchManager.Instance.Checkmate == (int)camp;
        if (hasMove || isCheckmate) return false;
        InitChessList();
        if (castChessList[0] != null)
        {
            Rock rock = (Rock)castChessList[0];
            if (rock != null && rock.hasMove == false)
            {
                for (var i = 1; i < 4; i++)
                    if (castChessList[i] != null)
                        return false;
                return true;
            }
        }
        return false;
    }
    private bool CanShortCastling()
    {
        isCheckmate = MatchManager.Instance.Checkmate == (int)camp;
        if (hasMove || isCheckmate) return false;
        InitChessList();
        if (castChessList[7] != null)
        {
            Rock rock = (Rock)castChessList[7];
            if (rock != null && rock.hasMove == false)
            {
                for (var i = 5; i < 7; i++)
                    if (castChessList[i] != null)
                        return false;
                return true;
            }
        }
        return false;
    }
    public void LongCastling()
    {
        var newLocation = new Vector2Int(3, location.y);
        castChessList[0].UpdateSelection(castChessList[0].location, newLocation);
        castChessList[0].location = newLocation;
        castChessList[0].MovePiece(newLocation);
        
    }
    public void ShortCastling()
    {
        var newLocation = new Vector2Int(5, location.y);
        castChessList[7].UpdateSelection(castChessList[7].location, newLocation);
        castChessList[7].location = newLocation;
        castChessList[7].MovePiece(newLocation);
    }

    #endregion
   
    private void Checkmate() => CallCheck(AttackGrid());
}
