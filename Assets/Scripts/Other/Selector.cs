using System.Collections.Generic;
using UnityEngine;

public class Selector : MonoBehaviour
{
    private Selection gridSelection;
    
    private List<Selection> selections = new List<Selection>();
    private RaycastHit raycastHit; 
    private Selection lastSelect;
    private bool selectStatus;
    
    private void OnEnable() => EventManager.OnSelectActionEvent += SelectPiece;
    private void OnDisable() => EventManager.OnSelectActionEvent -= SelectPiece;
    private void SelectPiece(bool SelectButtonClick, bool DeselectButtonClick)
    {
        if (gridSelection != null)
        {
            // 选择棋子并计算可移动格子
            if (SelectButtonClick && !selectStatus && gridSelection.chessPiece != null 
                && gridSelection.chessPiece.camp == GameStatus.RoundType)
            {
                var chess = gridSelection.chessPiece;
                chess.SelectPiece();
                selectStatus = true;
                selections = chess.CalculateGrid();
            }
            // 移动棋子
            if (selections.Contains(gridSelection))
            {
                MatchManager.currentChess.MovePiece();
            }
            // 取消选择
            if (DeselectButtonClick && selectStatus)
            {
                var chess = MatchManager.currentChess;
                var select = MatchManager.currentSelection;
                if (select.gridType == Selection.GridType.Attack || select.gridType == Selection.GridType.Normal)
                {
                    chess.EatPiece(select);
                }
                else if (select.gridType == Selection.GridType.Special)
                {
                    if (chess.GetComponent<Pawn>() != null)
                    {
                        var pawn = chess.GetComponent<Pawn>();
                        if (select.Location.y == 0 || select.Location.y == 7)
                        {
                            if (select.occupyType != Selection.OccupyGridType.NoneOccupyGrid) 
                                chess.EatPiece(select);
                            if (!GameStatus.isOver)
                            {
                                GameStatus.isPromotion = true;
                                pawn.Promotion();
                            }
                        }
                        else
                        {
                            GameStatus.moveType = "PassBy";
                            select.chessPiece = pawn;
                            select.occupyType = (Selection.OccupyGridType)pawn.camp;
                            pawn.En_Pass(select);
                        }
                    }
                    else
                    {
                        var king = chess.GetComponent<King>();
                        if (select.Location.x == 2)
                        {
                            GameStatus.moveType = "LongCast";
                            king.LongCastling();
                        }
                        else
                        {
                            GameStatus.moveType = "ShortCast";
                            king.ShortCastling();
                        }
                    }
                }

                chess.DeselectPiece();
                chess.UpdateSelection();
                foreach (var selection in selections) selection.Deselect();
                selectStatus = false;
                lastSelect = null;
                gridSelection = null;
                selections.Clear();
            }
        }

        // Selection光标
        Ray ray = Camera.main!.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out raycastHit, 9999f, LayerMask.GetMask("Selection")))
        {
            var hitSelection = raycastHit.collider.GetComponent<Selection>();
            if (gridSelection == hitSelection) return;
            if (selectStatus)
            {
                if (selections.Contains(hitSelection))
                {
                    RaySelect(hitSelection);
                    lastSelect = gridSelection;
                }
                else if (lastSelect != null) lastSelect.Select();
            }
            else RaySelect(hitSelection);
        }
        else if (gridSelection != null && !selectStatus)
        {
            gridSelection.Deselect();
            gridSelection = null;
        }
    }
    private void RaySelect(Selection hitSelection)
    {
        if (gridSelection != null) gridSelection.Deselect();
        gridSelection = hitSelection;
        gridSelection.Select();
    }
}
