using System.Collections.Generic;
using Controller;
using UnityEngine;

public class Selector : MonoBehaviour
{
    private List<Selection> selections = new List<Selection>();
    private Selection gridSelection;
    private RaycastHit raycastHit;
    
    private bool selectStatus;
    
    private void OnEnable() => EventManager.OnSelectActionEvent += SelectPiece;
    private void OnDisable() => EventManager.OnSelectActionEvent -= SelectPiece;

    private void SelectPiece(bool SelectButtonClick, bool DeselectButtonClick)
    {
        // 选择棋子并计算可移动格子
        if (SelectButtonClick && !selectStatus && gridSelection.chessPiece != null)
        {
            var chess = gridSelection.chessPiece;
            chess.SelectPiece();
            selectStatus = true;
            selections = chess.CalculateGrid();
        }
        // 移动棋子
        if (selections.Contains(gridSelection)) 
            MatchManager.Instance.currentChess.Move(MoveType.Move);
        // 取消选择
        if (DeselectButtonClick && selectStatus)
        {
            var chess = gridSelection.chessPiece;
            chess.DeselectPiece();
            foreach (var selection in selections) selection.Deselect();
            selectStatus = false;
            selections.Clear();
        }
        // Selection光标
        Ray ray = Camera.main!.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out raycastHit, 9999f, LayerMask.GetMask("Selection")))
        {
            if (gridSelection == raycastHit.collider.GetComponent<Selection>()) return;
            if (gridSelection != null) gridSelection.Deselect();
            gridSelection = raycastHit.collider.GetComponent<Selection>();
            if (selectStatus)
            {
                if (selections.Contains(gridSelection))
                    gridSelection.Select();
            }
            else gridSelection.Select();
        }
    }
}
