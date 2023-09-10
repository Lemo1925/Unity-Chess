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
    
    private List<Selection> AttackGrid() => 
        CalculateMove().Where(select => 
            select.occupyType != (Selection.OccupyGridType)camp && 
            select.occupyType != Selection.OccupyGridType.NoneOccupyGrid).ToList();

    private List<Selection> MoveGrid() => 
        CalculateMove().Where(select => 
            select.occupyType == Selection.OccupyGridType.NoneOccupyGrid).ToList();

    private List<Selection> SpecialGrid()
    {
        var selections = new List<Selection>();
        var selection = MatchManager.currentSelection;

        if (CanLongCastling()) selections.Add(selection.GetSelection(Location.x - 2, Location.y));
        if (CanShortCastling()) selections.Add(selection.GetSelection(Location.x + 2, Location.y));

        return selections;
    }

    private List<Selection> CalculateMove()
    {
        var selection = Selection.GetSelection(Location);

        var collection = selection.Bevel(1, 1);
        collection.AddRange(selection.ForwardAndBack(1, 1));
        collection.AddRange(selection.LeftAndRight(1, 1));

        return collection;
    }

    public override List<Selection> CalculateGrid()
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
        if (isMoved)
        {
            hasMove = true;
            isCheckmate = false;
        }
    }

    public override void DestroyPiece()
    {
        base.DestroyPiece();
        GameStatus.isOver = true;
    }

    #region 王车易位

    public void InitChessList()
    {
        // 初始化存储王车之间棋子的列表
        for (int i = 0; i < 8; i++)
        {
            castChessList.Add(Selection.GetSelection(new Vector2Int(i, Location.y)).chessPiece);
        }
    }
    private bool CanLongCastling()
    {
        isCheckmate = MatchManager.Instance.checkmate == (int)camp;
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
        isCheckmate = MatchManager.Instance.checkmate == (int)camp;
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
        var newLocation = new Vector2Int(3, Location.y);
        castChessList[0].UpdateSelection(castChessList[0].Location, newLocation);
        castChessList[0].Location = newLocation;
        castChessList[0].MovePiece(newLocation);
        
    }
    public void ShortCastling()
    {
        var newLocation = new Vector2Int(5, Location.y);
        castChessList[7].UpdateSelection(castChessList[7].Location, newLocation);
        castChessList[7].Location = newLocation;
        castChessList[7].MovePiece(newLocation);
    }

    #endregion
   
    private void Checkmate() => CallCheck(AttackGrid());
}
