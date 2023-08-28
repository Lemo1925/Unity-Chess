public enum GameModel
{
    SINGLE = 0,
    MULTIPLE = 1,
}

public enum GameState
{
    // 游戏状态
    Init = -1,
    StandBy = 0,
    Action = 1,
    End = 2
}
public enum Camp
{
    // 阵营
    WHITE = 0,
    BLACK = 1,
}
public enum ChessType
{
    // 棋子类型
    WhiteRock = 1,
    WhiteKnight = 2,
    WhiteBishop = 3,
    WhiteQueen = 4,
    WhiteKing = 5,
    WhitePawn = 6,
    BlackRock = -1,
    BlackKnight = -2,
    BlackBishop = -3,
    BlackQueen = -4,
    BlackKing = -5,
    BlackPawn = -6,
}