using System.Collections.Generic;
using UnityEngine;

public class Selector : MonoBehaviour
{
    private List<Selection> selections = new List<Selection>();
    public Selection gridSelection;
    private RaycastHit raycastHit; 
    private Selection lastSelect;

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
        {
            MatchManager.Instance.currentChess.Move(MoveType.Move);
        }
        // 取消选择
        if (DeselectButtonClick && selectStatus)
        {
            var chess = MatchManager.Instance.currentChess;
            var select = MatchManager.Instance.currentSelection;
            if (select.gridType == Selection.GridType.Attack || 
                select.gridType == Selection.GridType.Normal)
            {
                for (var index = 0; index < select.chessList.Count; index++)
                {
                    var chessPiece = select.chessList[index];
                    if (chessPiece.camp != chess.camp)
                    {
                        select.chessList.Remove(chessPiece);
                        chessPiece.DestroyPiece();
                    }
                }
            }

            chess.DeselectPiece();
            foreach (var selection in selections) selection.Deselect();
            selectStatus = false;
            selections.Clear();
        }
        // Selection光标
        Ray ray = Camera.main!.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out raycastHit, 9999f, LayerMask.GetMask("Selection")))
        {
            var hitSelection = raycastHit.collider.GetComponent<Selection>();
            if (gridSelection == hitSelection) return;
            if (gridSelection != null) gridSelection.Deselect();
            gridSelection = hitSelection;
            if (selectStatus)
            {
                if (selections.Contains(gridSelection))
                {
                    gridSelection.Select();
                    lastSelect = gridSelection;
                }
                else lastSelect.Select();
            }
            else gridSelection.Select();
        }
    }
}
