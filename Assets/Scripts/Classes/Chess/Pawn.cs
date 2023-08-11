using System.Collections.Generic;
using UnityEngine;

public class Pawn : Chess
{
    public bool isFirstMove;

    private void OnEnable()
    {
        EventManager.OnGameInitEvent += Init;
    }

    private void OnDisable()
    {
        EventManager.OnGameInitEvent -= Init;
    }

    protected override void Init()
    {
        isSelected = false;
        isFirstMove = true;
    }
    
    public override void Moveto(Vector2 tarTile, MoveType moveType)
    {
        if (isSelected)
        {
            switch (moveType)
            {
                // TODO 移动策略 FirstMove Move Eat En_Pass Promotion
                case MoveType.Move:
                    ChessBoard.instance.MovePiece(gameObject, tarTile);
                    break;
                case MoveType.Eat:
                
                    break;
                case MoveType.En_Pass:
                
                    break;
                case MoveType.Promotion:
                
                    break;
            }
        }
    }

    public override List<GameObject> CalculateTarget()
    { 
        // todo Caculate the move tiles
        List<GameObject> targets = new List<GameObject>();
        if (isFirstMove)
        {
            isFirstMove = false;
            
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