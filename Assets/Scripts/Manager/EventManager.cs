﻿
using System;

public static class EventManager
{
    public static event Action OnGameSwitchedEvent;
    public static void CallOnGameSwitched() => OnGameSwitchedEvent?.Invoke();
    public static event Action<bool, bool> OnSelectActionEvent;
    public static void CallOnSelectAction(bool selectButton, bool deselectButton) => 
        OnSelectActionEvent?.Invoke(selectButton,deselectButton);
    public static event Action<Pawn, bool> OnPromotionEvent;
    public static void CallOnPromotion(Pawn chess, bool visible) => OnPromotionEvent?.Invoke(chess, visible);
}
