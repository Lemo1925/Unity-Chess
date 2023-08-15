using System.Collections.Generic;
using UnityEngine;


public class Selection : MonoBehaviour
{
    public OccupyGridType occupyType;
    public Vector2Int Location;
    public List<Material> materials;

    public Chess chessPiece;
    
    public enum OccupyGridType
    {
        WhiteOccupyGrid = 0,
        BlackOccupyGrid = 1,
        NoneOccupyGrid = 2,
    }
    

    private void Awake()
    {
        occupyType = OccupyGridType.NoneOccupyGrid;
    }

    private void OnTriggerEnter(Collider other)
    {
        chessPiece = other.GetComponent<Chess>();
        occupyType = (OccupyGridType)other.GetComponent<Chess>().camp;
    }

    private void OnTriggerExit(Collider other)
    {
        chessPiece = null;
        occupyType = OccupyGridType.NoneOccupyGrid;
    }


    // 越界判断
    private bool OutOfRangeY(int y) => y <= -1 || y > 7;
    private bool OutOfRangeX(int x) => x <= -1 || x > 7;
    
    // 方向检测
    public List<Selection> Forward(int forward, int back)
    {
        List<Selection> selections = new List<Selection>();
        int Dir = occupyType == OccupyGridType.WhiteOccupyGrid ? -1 : 1;

        for (var i = 1; i <= forward; i++)
        {
            int Y = Location.y + (i * Dir);
            if (OutOfRangeY(Y)) break;
            selections.Add(ChessBoard.instance.ChessSelections[Location.x, Y]);
        }

        for (var i = 1; i <= back; i++)
        {
            int Y = Location.y - (i * Dir);
            if (OutOfRangeY(Y)) break;
            selections.Add(ChessBoard.instance.ChessSelections[Location.x, Y]);
        }

        return selections;
    }
    public List<Selection> Left(int left, int right)
    {
        List<Selection> selections = new List<Selection>();
        int Dir = occupyType == OccupyGridType.WhiteOccupyGrid ? -1 : 1;

        for (var i = 1; i <= left; i++)
        {
            int X = Location.x + (i * Dir);
            if (OutOfRangeX(X)) break;
            selections.Add(ChessBoard.instance.ChessSelections[X, Location.y]);
        }

        for (var i = 1; i <= right; i++)
        {
            int X = Location.x + (i * Dir);
            if (OutOfRangeX(X)) break;
            selections.Add(ChessBoard.instance.ChessSelections[X, Location.y]);
        }

        return selections;
    }
    public List<Selection> Bevel(int forwardLength, int backwardLength)
    {
        List<Selection> selections = new List<Selection>();
        int XDir = occupyType == OccupyGridType.WhiteOccupyGrid ? -1 : 1;
        int YDir = occupyType == OccupyGridType.WhiteOccupyGrid ? -1 : 1;

        for (var i = 1; i <= forwardLength; i++)
        {
            int X = Location.x + (i * XDir);
            int Y = Location.y + (i * YDir);
            int arcX = Location.x - (i * XDir);
            if (!OutOfRangeX(X) && !OutOfRangeY(Y)) 
                selections.Add(ChessBoard.instance.ChessSelections[X, Y]);
            if (!OutOfRangeX(arcX) && !OutOfRangeY(Y)) 
                selections.Add(ChessBoard.instance.ChessSelections[arcX, Y]);
        }

        for (var i = 1; i <= backwardLength; i++)
        {
            int X = Location.x - (i * XDir);
            int Y = Location.y - (i * YDir);            
            int arcX = Location.x - (i * XDir);
            if (!OutOfRangeX(X) && !OutOfRangeY(Y))
                selections.Add(ChessBoard.instance.ChessSelections[X, Y]);
            if (!OutOfRangeX(X) && !OutOfRangeY(Y))
                selections.Add(ChessBoard.instance.ChessSelections[arcX, Y]);
        }

        return selections;
    }

    
    
    public void MoveSelect()
    {
        GetComponent<Renderer>().material = materials[2];
    }
    public void AttackSelect()
    {
        GetComponent<Renderer>().material = materials[3];
    }
    public void Select()
    {
        GetComponent<Renderer>().material = materials[1];
        MatchManager.Instance.currentSelection = this;
    }
    public void Deselect()
    {
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