using System.Collections.Generic;
using UnityEngine;


public class BoardGrid : MonoBehaviour
{
    public OccupyGridType occupyType;
    public GridType gridType;
    public Vector2Int location;
    public List<Material> materials;
    
    public Chess chessPiece;
    public List<Chess> chessList;

    
    private void Awake()
    {
        gridType = GridType.Normal;
        occupyType = OccupyGridType.NoneOccupyGrid;
    }
    
    public Chess GetPiece() => chessList.Count > 0 ? chessList[0] : null;

    public void DestroyPiece()
    {
        chessList.Remove(chessPiece);
        chessPiece.DestroyPiece();
        chessPiece = null;
        occupyType = OccupyGridType.NoneOccupyGrid;
    }
    
    // 越界判断
    private static bool OutOfRangeY(int y) => y is < 0 or > 7;
    private static bool OutOfRangeX(int x) => x is < 0 or > 7;

    #region 方向侦测

       public static BoardGrid GetSelection(int x , int y)
    {
        BoardGrid grid = null;
        if (!OutOfRangeY(y) && !OutOfRangeX(x)) 
            grid = ChessBoard.instance.ChessSelections[x, y];
        return grid;
    }
    public static BoardGrid GetSelection(Vector2Int select)
    {
        int x = select.x, y = select.y;
        BoardGrid grid = null;
        if (!OutOfRangeY(y) && !OutOfRangeX(x)) 
            grid = ChessBoard.instance.ChessSelections[x, y];
        return grid;
    }
    private List<BoardGrid> Forward(int counts)
    {
        // 创建一个List用于获取敌方棋子的位置
        List<BoardGrid> enemies = new List<BoardGrid>();
        // 定义一个侦察器，用于侦察前方
        BoardGrid sensor;
        // 定义一个方向变量，用于判断方向
        int dir = occupyType == OccupyGridType.WhiteOccupyGrid ? -1 : 1,
            x = location.x, 
            y = 0;
        // 遍历前方
        for (var i = 1; i <= counts; i++)
        {
            y = location.y + i * dir;
            sensor = GetSelection(x, y);
            // 如果该位置没有棋子, 向前走一步
            if (ReferenceEquals(sensor, null)) continue;
            // 如果有棋子，停止向前，并判断是否是敌人的棋子
            if (sensor.occupyType != OccupyGridType.NoneOccupyGrid)
            {
                if (sensor.chessPiece.camp != chessPiece.camp)
                    enemies.Add(sensor);
                break;
            }
            enemies.Add(sensor);
        }
        return enemies;
    }
    private List<BoardGrid> Back(int steps)
    {
        BoardGrid sensor;
        var enemies = new List<BoardGrid>();
        int dir = occupyType == OccupyGridType.WhiteOccupyGrid ? -1 : 1, 
            x = location.x,
            y = 0;
        
        for (var i = 1; i <= steps; i++)
        {
            y = location.y - i * dir;
            sensor = GetSelection(x, y);
            if (ReferenceEquals(sensor, null)) continue; 
            if (sensor.occupyType != OccupyGridType.NoneOccupyGrid)
            {
                if (sensor.chessPiece.camp != chessPiece.camp) 
                    enemies.Add(sensor);
                break;
            }
            enemies.Add(sensor);
        }
        return enemies;
    }
    private List<BoardGrid> Left(int steps)
    {
        var enemies = new List<BoardGrid>();
        BoardGrid sensor;
        int dir = occupyType == OccupyGridType.WhiteOccupyGrid ? -1 : 1,
            x = 0,
            y = location.y;
        
        for (var i = 1; i <= steps; i++)
        {
            x = location.x + i * dir;
            sensor = GetSelection(x, y);
            if (ReferenceEquals(sensor, null)) continue;
            if (sensor.occupyType != OccupyGridType.NoneOccupyGrid)
            {
                if (sensor.chessPiece.camp != chessPiece.camp) 
                    enemies.Add(sensor);
                break;
            }
            enemies.Add(sensor);
        }
        return enemies;
    }
    private List<BoardGrid> Right(int steps)
    {
        var selections = new List<BoardGrid>();
        BoardGrid sensor;
        int dir = occupyType == OccupyGridType.WhiteOccupyGrid ? -1 : 1,
            x = 0,
            y = location.y;
        
        for (var i = 1; i <= steps; i++)
        {
            x = location.x - i * dir;
            sensor = GetSelection(x, y);
            if (ReferenceEquals(sensor, null)) continue;
            if (sensor.occupyType != OccupyGridType.NoneOccupyGrid)
            {
                if (sensor.chessPiece.camp != chessPiece.camp) 
                    selections.Add(sensor);
                break;
            }
            selections.Add(sensor);
        }
        return selections;
    }
    private List<BoardGrid> LeftBevel(int forwardLength, int backwardLength)
    {
        var enemies = new List<BoardGrid>();
        BoardGrid sensor;
        int xDir = occupyType == OccupyGridType.WhiteOccupyGrid ? -1 : 1,
            yDir = occupyType == OccupyGridType.WhiteOccupyGrid ? -1 : 1,
            x = 0, y = 0;
        for (var i = 1; i <= forwardLength; i++)
        {
            x = location.x + i * xDir;
            y = location.y + i * yDir;
            sensor = GetSelection(x, y);
            if (ReferenceEquals(sensor, null)) continue;
            if (sensor.occupyType != OccupyGridType.NoneOccupyGrid)
            {
                if (sensor.chessPiece.camp != chessPiece.camp) 
                    enemies.Add(sensor);
                break;
            }
            enemies.Add(sensor);
        }

        for (var i = 1; i <= backwardLength; i++)
        {
            x = location.x - i * xDir;
            y = location.y - i * yDir;
            sensor = GetSelection(x, y);
            if (ReferenceEquals(sensor, null)) continue;
            if (sensor.occupyType != OccupyGridType.NoneOccupyGrid)
            {
                if (sensor.chessPiece.camp != chessPiece.camp) 
                    enemies.Add(sensor);
                break;
            }
            enemies.Add(sensor);
        }
        
        return enemies;
    }
    private List<BoardGrid> RightBevel(int forwardLength, int backwardLength)
    {
        var enemies = new List<BoardGrid>();
        BoardGrid sensor;
        int xDir = occupyType == OccupyGridType.WhiteOccupyGrid ? -1 : 1,
            yDir = occupyType == OccupyGridType.WhiteOccupyGrid ? -1 : 1,
            x = 0, y = 0;
        for (var i = 1; i <= forwardLength; i++)
        {
            x = location.x - i * xDir;
            y = location.y + i * yDir;
            sensor = GetSelection(x, y);
            if (ReferenceEquals(sensor, null)) continue;
            if (sensor.occupyType != OccupyGridType.NoneOccupyGrid)
            {
                if (sensor.chessPiece.camp != chessPiece.camp) 
                    enemies.Add(sensor);
                break;
            }
            enemies.Add(sensor);
        }

        for (var i = 1; i <= backwardLength; i++)
        {
            x = location.x + i * xDir;
            y = location.y - i * yDir;
            sensor = GetSelection(x, y);
            if (ReferenceEquals(sensor, null)) continue;
            if (sensor.occupyType != OccupyGridType.NoneOccupyGrid)
            {
                if (sensor.chessPiece.camp != chessPiece.camp) 
                    enemies.Add(sensor);
                break;
            }
            enemies.Add(sensor);
        }
        return enemies;
    }
    public List<BoardGrid> ForwardAndBack(int forward, int back)
    {
        var selections = new List<BoardGrid>();
        selections.AddRange(Forward(forward));
        selections.AddRange(Back(back));
        return selections;
    }
    public List<BoardGrid> LeftAndRight(int left, int right)
    {
        var selections = new List<BoardGrid>();
        selections.AddRange(Left(left));
        selections.AddRange(Right(right));
        return selections;
    }
    public List<BoardGrid> Bevel(int forwardLength, int backwardLength)
    {
        var selections = new List<BoardGrid>();
        selections.AddRange(LeftBevel(forwardLength, backwardLength));
        selections.AddRange(RightBevel(forwardLength, backwardLength));
        return selections;
    }

    #endregion
 
    #region 选择Grid
    
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
        MatchManager.CurrentGrid = this;
    }
    public void Deselect()
    {
        gridType = GridType.Normal;
        GetComponent<Renderer>().material = materials[0];
        MatchManager.CurrentGrid = null;
    }
    
    #endregion
}
