using System;
using UnityEngine;


public class Selection : MonoBehaviour
{
    public int chessCamp = 2;
    public bool isSelected;
    public static Selection instance;

    private void Awake()
    {
        if (instance == null) instance = this;
    }

    private void OnTriggerEnter(Collider other)
    {
        chessCamp = (int)other.GetComponent<Chess>().camp;
    }

    private void OnTriggerStay(Collider collider)
    {
        if (collider.CompareTag($"Chess"))
        {
            isSelected = collider.gameObject.GetComponent<Chess>().isSelected;
            if (isSelected)
            {
                print("Selected"+collider.transform.position);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        chessCamp = 2;
    }

    public Vector2 GetLocation()
    {
        if (isSelected)
        {
            int x = 8, y = 8;
            for (int i = 0; i < 7; i++)
            for (int j = 0; j < 7; j++)
            {
                if (ChessBoard.instance.ChessBoardTiles[i, j] == gameObject)
                {
                    x = i;
                    y = j;
                }
            }

            return new Vector2(x, y);
        }

        return default;
    }
    
}
