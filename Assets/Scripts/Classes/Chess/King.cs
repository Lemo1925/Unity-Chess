using System.Collections.Generic;
using UnityEngine;

public class King : Chess
{
    // 是否移动过
    private bool hasMove;
    
    // 是否被将军
    public bool isCheckmate;
    
    public List<Chess> chessList = new List<Chess>();
    private void Start()
    {
        hasMove = false; 
        isCheckmate = false;
    }

    public override void Move()
    {
        MovePiece();
    }

    public override List<Selection> CalculateGrid()
    {
        Selection selection = MatchManager.Instance.currentSelection;
        List<Selection> selections = base.CalculateGrid();

        var selectionCollection = selection.Bevel(1, 1);
        selectionCollection.AddRange(selection.ForwardAndBack(1,1));
        selectionCollection.AddRange(selection.LeftAndRight(1, 1));

        foreach (var sensor in selectionCollection)
        {
            if (sensor == null || sensor.occupyType == (Selection.OccupyGridType)camp) continue;
            if (sensor.occupyType == Selection.OccupyGridType.NoneOccupyGrid)
            {
                sensor.MoveSelect();
                selections.Add(sensor);
            }
            else
            {
                sensor.AttackSelect();
                var king = sensor.chessPiece.GetComponent<King>();
                if (king != null) king.isCheckmate = true;
                selections.Add(sensor);
            }
        }

        if (CanLongCastling())
        {
            var sensor = selection.GetSelection(Location.x - 2, Location.y);
            sensor.SpecialSelect();
            selections.Add(sensor);
        }

        if (CanShortCastling())
        {
            var sensor = selection.GetSelection(Location.x + 2, Location.y);
            sensor.SpecialSelect();
            selections.Add(sensor);
        }

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
        print(camp == Camp.BLACK ? "White Winner" : "Black Winner");
    }

    private List<Chess> InitChessList()
    {
        List<Chess> list = new List<Chess>();

        for (int i = 0; i < 8; i++) 
            list.Add(Selection.GetSelection(new Vector2Int(i, Location.y)).chessPiece);

        return list;
    }

    private bool CanLongCastling()
    {
        if (hasMove || isCheckmate) return false;
        chessList = InitChessList();
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
        if (hasMove || isCheckmate) return false;
        chessList = InitChessList();
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
}
