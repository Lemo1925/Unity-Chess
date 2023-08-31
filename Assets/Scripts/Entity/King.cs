using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class King : Chess
{
    // 是否移动过
    private bool hasMove;
    
    // 是否被将军
    public bool isCheckmate;
    
    public List<Chess> chessList = new List<Chess>();

    private void OnEnable() => EventManager.OnTurnEndEvent += Checkmate;
    private void OnDisable() => EventManager.OnTurnEndEvent -= Checkmate;

    private void Start()
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
        var selection = MatchManager.Instance.currentSelection;

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
        EventManager.CallOnGameOver(camp == Camp.BLACK ? "White Win" : "Black Win");
    }

    #region 王车易位

    public void InitChessList()
    {
        // 初始化存储王车之间棋子的列表
        for (int i = 0; i < 8; i++)
        {
            chessList.Add(Selection.GetSelection(new Vector2Int(i, Location.y)).chessPiece);
        }
    }
    private bool CanLongCastling()
    {
        isCheckmate = MatchManager.Instance.checkmate == (int)camp;
        if (hasMove || isCheckmate) return false;
        InitChessList();
        if (chessList[0] != null)
        {
            Rock rock = (Rock)chessList[0];
            if (rock != null && rock.hasMove == false)
            {
                for (var i = 1; i < 4; i++)
                    if (chessList[i] != null)
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
        if (chessList[7] != null)
        {
            Rock rock = (Rock)chessList[7];
            if (rock != null && rock.hasMove == false)
            {
                for (var i = 5; i < 7; i++)
                    if (chessList[i] != null)
                        return false;
                return true;
            }
        }
        return false;
    }
    public void LongCastling()
    {
        chessList[0].Location = new Vector2Int(3, Location.y);
        chessList[0].MovePiece(3, Location.y);
    }
    public void ShortCastling()
    {
        chessList[7].Location = new Vector2Int(5, Location.y);
        chessList[7].MovePiece(5, Location.y);
    }

    #endregion
   
    private void Checkmate() => CallCheck(AttackGrid());
}
