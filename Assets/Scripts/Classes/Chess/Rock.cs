
using System.Collections.Generic;
using UnityEngine;

public class Rock : Chess
{

    public override void Move(MoveType moveType)
    {
        switch (moveType)
        {
            case MoveType.Move:
                break;
            case MoveType.Eat:
                break;
        }
    }

    public override List<Selection> CalculateGrid()
    {
        throw new System.NotImplementedException();
    }
}
