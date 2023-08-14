using System.Collections.Generic;
using UnityEngine;


public class Selection : MonoBehaviour
{
    public GridType gridType;
    public Vector2Int Location;
    
    // todo change name
    public enum GridType
    {
        WhiteGrid = 0,
        BlackGrid = 1,
        NormalGrid = 2,
    }

    private void Awake() => gridType = GridType.NormalGrid;
    private void OnTriggerEnter(Collider other) => gridType = (GridType)other.GetComponent<Chess>().camp;
    private void OnTriggerExit(Collider other) => gridType = GridType.NormalGrid;

    
    // todo 越界判断
    public List<Selection> Forward(int forward, int back)
    {
        List<Selection> selections = new List<Selection>();
        for (var i = 1; i <= forward; i++)
        {
            switch (gridType)
            {
                case GridType.WhiteGrid:
                    selections.Add(ChessBoard.instance.ChessSelections[Location.x, Location.y - i ]);
                    break;
                case GridType.BlackGrid:
                    selections.Add(ChessBoard.instance.ChessSelections[Location.x, Location.y + i ]);
                    break;
            }
        }

        for (var i = 1; i <= back; i++)
        {
            switch (gridType)
            {
                case GridType.WhiteGrid:
                    selections.Add(ChessBoard.instance.ChessSelections[Location.x, Location.y + i]);
                    break;
                case GridType.BlackGrid:
                    selections.Add(ChessBoard.instance.ChessSelections[Location.x, Location.y - i]);
                    break;
            }
        }
        return selections;
    }
    public List<Selection> Left(int left, int right)
    {
        List<Selection> selections = new List<Selection>();
        for (var i = 1; i <= left; i++)
        {
            switch (gridType)
            {
                case GridType.WhiteGrid:
                    selections.Add(ChessBoard.instance.ChessSelections[Location.x - i , Location.y]);
                    break;
                case GridType.BlackGrid:
                    selections.Add(ChessBoard.instance.ChessSelections[Location.x + i , Location.y]);
                    break;
            }
        }
        for (var i = 1; i <= right; i++)
        {
            switch (gridType)
            {
                case GridType.WhiteGrid:
                    selections.Add(ChessBoard.instance.ChessSelections[Location.x + i , Location.y]);
                    break;
                case GridType.BlackGrid:
                    selections.Add(ChessBoard.instance.ChessSelections[Location.x - i , Location.y]);
                    break;
            }
        }
        return selections;
    }
    public List<Selection> Bevel(int forwardLength, int backwardLength)
    {
        List<Selection> selections = new List<Selection>();
        for (var i = 1; i <= forwardLength; i++)
        {
            switch (gridType)
            {
                case GridType.WhiteGrid:
                    selections.Add(ChessBoard.instance.ChessSelections[Location.x - i, Location.y - i]);
                    selections.Add(ChessBoard.instance.ChessSelections[Location.x + i, Location.y - i]);
                    break;
                case GridType.BlackGrid:
                    selections.Add(ChessBoard.instance.ChessSelections[Location.x - i, Location.y + i]);
                    selections.Add(ChessBoard.instance.ChessSelections[Location.x + i, Location.y + i]);
                    break;
            }
        }
        for (var i = 1; i <= backwardLength; i++)
        {
            switch (gridType)
            {
                case GridType.WhiteGrid:
                    selections.Add(ChessBoard.instance.ChessSelections[Location.x - i, Location.y + i]);
                    selections.Add(ChessBoard.instance.ChessSelections[Location.x + i, Location.y + i]);
                    break;
                case GridType.BlackGrid:
                    selections.Add(ChessBoard.instance.ChessSelections[Location.x - i, Location.y - i]);
                    selections.Add(ChessBoard.instance.ChessSelections[Location.x + i, Location.y - i]);
                    break;
            }
        }
        return selections;
    }

    // 判断
    public void Select(Material material)
    {
        MatchManager.Instance.currentSelection = this;
        GetComponent<Renderer>().material = material;
    }
    public void Deselect(Material defaultMaterial)
    {
        MatchManager.Instance.currentSelection = null;
        GetComponent<Renderer>().material = defaultMaterial;
    }
}


public class MatchManager
{
    private static MatchManager instance;
    public static MatchManager Instance => instance ??= new MatchManager();

    public Selection currentSelection;

    public Chess currentChess;
}