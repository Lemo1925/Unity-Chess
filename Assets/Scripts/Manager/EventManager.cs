
using System;

public static class EventManager
{
    public static event Action<string> OnGameOverEvent;

    public static void CallOnGameOver(string text) => OnGameOverEvent?.Invoke(text);
    
    public static event Action OnGameResetEvent;

    public static void CallOnGameReset() => OnGameResetEvent?.Invoke();
    
    public static event Action<bool, bool> OnSelectActionEvent;
    
    public static void CallOnSelectAction(bool selectButton, bool deselectButton) => OnSelectActionEvent?.Invoke(selectButton,deselectButton);
    
    public static event Action<Pawn, bool> OnPromotionEvent;
    
    public static void CallOnPromotion(Pawn chess, bool visible) => OnPromotionEvent?.Invoke(chess, visible);
    
    public static event Action OnTurnEndEvent;
    
    public static void CallOnTurnEnd() => OnTurnEndEvent?.Invoke();

    public static event Action OnCameraChangedEvent;
    
    public static void CallOnCameraChanged() => OnCameraChangedEvent?.Invoke();
    
}
