using System.Collections.Generic;
using UnityEngine;

public class Pawn : Chess
{
    public bool isFirstMove;
    
    public void Start()
    {
        isFirstMove = true;
    }
    
    public override void Move(Vector2 tarTile, MoveType moveType)
    {
        switch (moveType)
        {
            // TODO 移动策略 FirstMove Move Eat En_Pass Promotion
            case MoveType.Move:
                MovePiece(tarTile);
                if (isFirstMove) isFirstMove = false;
                break;
            case MoveType.Eat:
            
                break;
            case MoveType.En_Pass:
            
                break;
            case MoveType.Promotion:
            
                break;
        }
        
    }

    public override List<Selection> CalculateGrid()
    { 
        List<Selection> targets = new List<Selection>();
        if (isFirstMove)
        {
            Vector2Int curTile = MatchManager.Instance.currentSelection.Location;
            if (camp == Camp.WHITE)
            {
                int x = curTile.x, y = curTile.y;
                targets.Add(ChessBoard.instance.ChessSelections[x, y - 1].gameObject.GetComponent<Selection>());
                targets.Add(ChessBoard.instance.ChessSelections[x, y - 2].gameObject.GetComponent<Selection>());
            }
            else
            {
                int x = curTile.x, y = curTile.y;
                targets.Add(ChessBoard.instance.ChessSelections[x, y + 1].gameObject.GetComponent<Selection>());
                targets.Add(ChessBoard.instance.ChessSelections[x, y + 2].gameObject.GetComponent<Selection>());
            }
        }
        else
        {
            
        }

        return targets;
    }

    public void Promotion(ChessType type)
    {
        
    }
}