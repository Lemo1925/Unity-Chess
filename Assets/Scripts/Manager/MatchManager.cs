using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchManager
{
    private static MatchManager instance;
    public static MatchManager Instance => instance ??= new MatchManager();

    public static Selection currentSelection;

    public static Chess currentChess;

    public int checkmate = -1;
}
