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

    private void OnTriggerExit(Collider other)
    {
        chessCamp = 2;
    }

    public Vector2Int GetLocation()
    {
        int x = 8, y = 8;
        for (int i = 0; i < 8; i++)
        for (int j = 0; j < 8; j++)
        {
            if (ChessBoard.instance.ChessSelections[i,j].isSelected)
            {
                x = i; y = j;
            }
        }
        return new Vector2Int(x, y);
    }

    public static void SetSelectionLocation(int x, int y)
    {
        var selections = ChessBoard.instance.ChessSelections;
        for (int i = 0; i < 8; i++)
        for (int j = 0; j < 8; j++)
        {
            selections[i, j].isSelected = false;
        }
        
        selections[x,y].isSelected = true;
    }
}
