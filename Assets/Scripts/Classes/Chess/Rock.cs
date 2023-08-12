
using System.Collections.Generic;
using UnityEngine;

public class Rock : Chess
{

    public override void Moveto(Vector2 tarTile, MoveType moveType)
    {
        switch (moveType)
        {
            case MoveType.Move:
                
                break;
            case MoveType.Eat:
                
                break;
        }
    }

    public override List<GameObject> CalculateTarget()
    {
        throw new System.NotImplementedException();
    }
}
