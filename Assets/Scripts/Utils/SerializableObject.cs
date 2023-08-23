using System;
using System.Collections.Generic;
using UnityEngine;

public class SerializableObject : MonoBehaviour
{
    [Serializable]
    public class Node
    {
        public List<Selection> selections = new List<Selection>();
        public List<ChessType> chessGOKey = new List<ChessType>();
        public List<GameObject> chessGOValue = new List<GameObject>();
    }

    private static Node node = new Node();
    
    public void Initialize()
    {
        foreach (var keyValuePair in ChessBoard.instance.chessGO)
        foreach (var Object in keyValuePair.Value)
        {
            node.chessGOKey.Add(keyValuePair.Key);
            node.chessGOValue.Add(Object);
        }

        for (var i = 0; i < 8; i++)
        for (var j = 0; j < 8; j++)
            node.selections.Add(ChessBoard.instance.ChessSelections[i, j]);
    }

    public static Dictionary<ChessType, List<GameObject>> deserializationChessGO()
    {
        var chessGO = new Dictionary<ChessType, List<GameObject>>();
        for (var i = 0; i < node.chessGOKey.Count; i++) 
            chessGO[node.chessGOKey[i]].Add(node.chessGOValue[i]);
        return chessGO;
    }

    public static Selection[,] deserializationSelections()
    {
        var ChessSelections = new Selection[8, 8];
        for (var i = 0; i < 8; i++)
        for (var j = 0; j < 8; j++)
            ChessSelections[i, j] = node.selections[i * 7 + j];
        return ChessSelections;
    }
}
