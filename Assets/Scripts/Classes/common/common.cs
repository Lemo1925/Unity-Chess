﻿// 单人、多人游戏

using UnityEngine;

public enum GameModel
{
    SINGLE = 0,
    MULTIPLE = 1,
}
// 游戏状态
public enum GameState
{
    WHITE_TURN = 0,
    BLACK_TURN = 1,
    GAME_OVER = 2,
}
// 阵营
public enum Camp
{
    WHITE = 0,
    BLACK = 1,
}
// 棋子类型
public enum ChessType
{
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
public enum MoveType
{
    Move = 1,
    Eat = 2,
    Castling = 3,
    En_Pass = 4,
    Promotion = 5
}
public static class Common
{
    public static Vector2 BoardSize = new Vector2(2.125f, 2.125f);
}