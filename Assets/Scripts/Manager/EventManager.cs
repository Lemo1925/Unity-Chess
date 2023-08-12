
using System;

public static class EventManager
{
    public static event Action OnGameSwitchedEvent;

    public static void CallOnGameSwitched()
    {
        OnGameSwitchedEvent?.Invoke();
    }
    
    public static event Action OnSelectTurnEvent;

    public static void CallOnSelectTurn()
    {
        OnSelectTurnEvent?.Invoke();
    }
}
