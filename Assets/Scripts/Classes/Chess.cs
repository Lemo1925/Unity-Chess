using System.Collections.Generic;
using UnityEngine;
public abstract class Chess : MonoBehaviour
{
    public Player player;

    public Camp camp;
    
    public bool isSelected;

    public Vector2 Location;
    
    public virtual void Start() => isSelected = false;

    public abstract void Moveto(Vector2 tarTile, MoveType moveType);

    public abstract List<GameObject> CalculateTarget();

}
