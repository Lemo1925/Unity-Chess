using System;
using System.Collections.Generic;
using UnityEngine;
public abstract class Chess : MonoBehaviour
{
    public Player player;

    public Camp camp;
    
    public bool isSelected;

    protected abstract void Init();
    public abstract void Moveto(Vector2 tarTile, MoveType moveType);

    public abstract List<GameObject> CalculateTarget();

}
