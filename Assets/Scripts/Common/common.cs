public enum GameModel { Single = 0, Multiple = 1}
public enum Camp { White = 0, Black = 1, }
public enum GridType { Normal = 0, Select = 1, Move = 2, Attack = 3, Special = 4 }
public enum OccupyGridType { WhiteOccupyGrid = 0, BlackOccupyGrid = 1, NoneOccupyGrid = 2 }
public enum GameState
{
    // 游戏状态
    Init = -1,
    StandBy = 0,
    Action = 1,
    End = 2,
    Over = 3,
    Draw = 4,
    Pause = 5,
}
public enum ChessType
{
    // 棋子类型
    WhiteRook = 1,
    WhiteKnight = 2,
    WhiteBishop = 3,
    WhiteQueen = 4,
    WhiteKing = 5,
    WhitePawn = 6,
    BlackRook = -1,
    BlackKnight = -2,
    BlackBishop = -3,
    BlackQueen = -4,
    BlackKing = -5,
    BlackPawn = -6,
}