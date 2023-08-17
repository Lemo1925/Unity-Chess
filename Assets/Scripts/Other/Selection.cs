﻿using System.Collections.Generic;
using UnityEngine;


public class Selection : MonoBehaviour
{
    public OccupyGridType occupyType;
    public GridType gridType;
    public Vector2Int Location;
    public List<Material> materials;

    public Chess chessPiece;
    public List<Chess> chessList;
    public enum OccupyGridType
    {
        WhiteOccupyGrid = 0,
        BlackOccupyGrid = 1,
        NoneOccupyGrid = 2,
    }
    
    public enum GridType
    {
        Normal = 0,
        Move = 1,
        Attack = 2,
        Special = 3
    }

    private void Awake()
    {
        gridType = GridType.Normal;
        occupyType = OccupyGridType.NoneOccupyGrid;
    }

    private void OnTriggerEnter(Collider other)
    {
        chessPiece = other.GetComponent<Chess>();
        occupyType = (OccupyGridType)other.GetComponent<Chess>().camp;
        chessList.Add(chessPiece);
    }

    private void OnTriggerExit(Collider other)
    {
        chessList.Remove(chessPiece);
        chessPiece = null;
        occupyType = OccupyGridType.NoneOccupyGrid;
    }


    // 越界判断
    private bool OutOfRangeY(int y) => y < ChessBoard.BoardLocationMin.y || y > ChessBoard.BoardLocationMax.y;
    private bool OutOfRangeX(int x) => x < ChessBoard.BoardLocationMin.x || x > ChessBoard.BoardLocationMax.y;
    
    // 方向检测
    private List<Selection> Forward(int steps)
    {
        List<Selection> selections = new List<Selection>();
        int Dir = occupyType == OccupyGridType.WhiteOccupyGrid ? -1 : 1;
        int X = Location.x;
        for (var i = 1; i <= steps; i++)
        {
            int Y = Location.y + i * Dir;
            var selection = GetSelection(X, Y);
            if (selection == null) continue;
            if (selection.occupyType != OccupyGridType.NoneOccupyGrid)
            {
                if (selection.chessPiece.camp != chessPiece.camp)
                { 
                    selections.Add(selection);
                    break;
                }
                if (selection.chessPiece.camp == chessPiece.camp) break;
            }
            selections.Add(selection);
        }

        return selections;
    }
    private List<Selection> Back(int steps)
    {
        List<Selection> selections = new List<Selection>();
        int Dir = occupyType == OccupyGridType.WhiteOccupyGrid ? -1 : 1;
        int X = Location.x;
        for (var i = 1; i <= steps; i++)
        {
            int Y = Location.y - i * Dir;
            var selection = GetSelection(X, Y);
            if (selection == null) continue; 
            if (selection.occupyType != OccupyGridType.NoneOccupyGrid)
            {
                if (selection.chessPiece.camp != chessPiece.camp)
                { 
                    selections.Add(selection);
                    break;
                }
                if (selection.chessPiece.camp == chessPiece.camp) break;
            }
            selections.Add(selection);
        }

        return selections;
    }
    public List<Selection> ForwardAndBack(int forward, int back)
    {
        List<Selection> selections = new List<Selection>();
        selections.AddRange(Forward(forward));
        selections.AddRange(Back(back));
        return selections;
    }
    private List<Selection> Left(int steps)
    {
        List<Selection> selections = new List<Selection>();
        int Dir = occupyType == OccupyGridType.WhiteOccupyGrid ? -1 : 1;
        int Y = Location.y;
        for (var i = 1; i <= steps; i++)
        {
            int X = Location.x + i * Dir;
            var selection = GetSelection(X, Y);
            if (selection == null) continue;
            if (selection.occupyType != OccupyGridType.NoneOccupyGrid)
            {
                if (selection.chessPiece.camp != chessPiece.camp)
                {
                    selections.Add(selection);
                    break;
                }
                if (selection.chessPiece.camp == chessPiece.camp) break;
            }
            selections.Add(selection);
        }

        return selections;
    }
    private List<Selection> Right(int steps)
    {
        List<Selection> selections = new List<Selection>();
        int Dir = occupyType == OccupyGridType.WhiteOccupyGrid ? -1 : 1;
        int Y = Location.y;
        for (var i = 1; i <= steps; i++)
        {
            int X = Location.x - i * Dir;
            var selection = GetSelection(X, Y);
            if (selection == null) continue;
            if (selection.occupyType != OccupyGridType.NoneOccupyGrid)
            {
                if (selection.chessPiece.camp != chessPiece.camp)
                {
                    selections.Add(selection);
                    break;
                }
                if (selection.chessPiece.camp == chessPiece.camp) break;
            }
            selections.Add(selection);
        }

        return selections;
    }
    public List<Selection> LeftAndRight(int left, int right)
    {
        List<Selection> selections = new List<Selection>();
        selections.AddRange(Left(left));
        selections.AddRange(Right(right));
        return selections;
    }
    private List<Selection> LeftBevel(int forwardLength, int backwardLength)
    {
        List<Selection> selections = new List<Selection>();
        int XDir = occupyType == OccupyGridType.WhiteOccupyGrid ? -1 : 1;
        int YDir = occupyType == OccupyGridType.WhiteOccupyGrid ? -1 : 1;

        for (int i = 1; i <= forwardLength; i++)
        {
            int X = Location.x + i * XDir;
            int Y = Location.y + i * YDir;
            var selection = GetSelection(X, Y);
            if (selection == null) continue;
            if (selection.occupyType != OccupyGridType.NoneOccupyGrid)
            {
                if (selection.chessPiece.camp != chessPiece.camp)
                {
                    selections.Add(selection);
                    break;
                }
                if (selection.chessPiece.camp == chessPiece.camp) break;
            }
            selections.Add(selection);
        }

        for (int i = 1; i < backwardLength; i++)
        {
            int X = Location.x - i * XDir;
            int Y = Location.y - i * YDir;
            var selection = GetSelection(X, Y);
            if (selection == null) continue;
            if (selection.occupyType != OccupyGridType.NoneOccupyGrid)
            {
                if (selection.chessPiece.camp != chessPiece.camp)
                {
                    selections.Add(selection);
                    break;
                }
                if (selection.chessPiece.camp == chessPiece.camp) break;
            }
            selections.Add(selection);
        }
        
        return selections;
    }
    private List<Selection> RightBevel(int forwardLength, int backwardLength)
    {
        List<Selection> selections = new List<Selection>();
        int XDir = occupyType == OccupyGridType.WhiteOccupyGrid ? -1 : 1;
        int YDir = occupyType == OccupyGridType.WhiteOccupyGrid ? -1 : 1;

        for (var i = 1; i <= forwardLength; i++)
        {
            int X = Location.x - i * XDir;
            int Y = Location.y + i * YDir;
            var selection = GetSelection(X, Y);
            if (selection == null) continue;
            if (selection.occupyType != OccupyGridType.NoneOccupyGrid)
            {
                if (selection.chessPiece.camp != chessPiece.camp)
                {
                    selections.Add(selection);
                    break;
                }
                if (selection.chessPiece.camp == chessPiece.camp) break;
            }
            selections.Add(selection);
        }

        for (var i = 1; i <= backwardLength; i++)
        {
            int X = Location.x + i * XDir;
            int Y = Location.y - i * YDir;
            var selection = GetSelection(X, Y);
            if (selection == null) continue;
            if (selection.occupyType != OccupyGridType.NoneOccupyGrid)
            {
                if (selection.chessPiece.camp != chessPiece.camp)
                {
                    selections.Add(selection);
                    break;
                }
                if (selection.chessPiece.camp == chessPiece.camp) break;
            }
            selections.Add(selection);
        }
        
        return selections;
    }
    public List<Selection> Bevel(int forwardLength, int backwardLength)
    {
        var selections = new List<Selection>();
        selections.AddRange(LeftBevel(forwardLength, backwardLength));
        selections.AddRange(RightBevel(forwardLength, backwardLength));
        return selections;
    }
    public Selection GetSelection(int x , int y)
    {
        Selection selection = null;
        if (!OutOfRangeY(y) && !OutOfRangeX(x)) 
            selection = ChessBoard.instance.ChessSelections[x, y];
        return selection;
    }

    public void MoveSelect()
    {
        gridType = GridType.Move;
        GetComponent<Renderer>().material = materials[2];
    }
    public void AttackSelect()
    {
        gridType = GridType.Attack;
        GetComponent<Renderer>().material = materials[3];
    }
    public void SpecialSelect()
    {
        gridType = GridType.Special;
        GetComponent<Renderer>().material = materials[4];
    }
    public void Select()
    {
        GetComponent<Renderer>().material = materials[1];
        MatchManager.Instance.currentSelection = this;
    }
    public void Deselect()
    {
        gridType = GridType.Normal;
        GetComponent<Renderer>().material = materials[0];
        MatchManager.Instance.currentSelection = null;
    }
}


public class MatchManager
{
    private static MatchManager instance;
    public static MatchManager Instance => instance ??= new MatchManager();

    public Selection currentSelection;

    public Chess currentChess;
}