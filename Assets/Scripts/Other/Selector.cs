using System.Collections.Generic;
using UnityEngine;

public class Selector : MonoBehaviour
{
    private Grid _selectGrid;
    private Camera _camera;
    private List<Grid> _grids = new();
    private RaycastHit _hit; 
    private Grid _lastSelect;
    private bool _selectStatus;
    
    private void OnEnable() => EventManager.OnSelectActionEvent += SelectPiece;
    private void OnDisable() => EventManager.OnSelectActionEvent -= SelectPiece;

    private void Awake() => _camera = Camera.main;

    private void SelectPiece(bool selectButtonClick, bool deselectButtonClick)
    {
        if (!ReferenceEquals(_selectGrid, null))
        {
            var chessPiece = _selectGrid.chessPiece;
            // 选择棋子并计算可移动格子
            if (selectButtonClick && !_selectStatus && !GameStatus.IsPromotion &&
                !ReferenceEquals(chessPiece, null) && 
                chessPiece.camp == GameStatus.RoundType)
            {
                chessPiece.SelectPiece();
                _selectStatus = true;
                _grids = chessPiece.CalculateGrid();
            }
            // 移动棋子
            if (_grids.Contains(_selectGrid)) 
                MatchManager.CurrentChess.MovePiece();
            // 取消选择
            if (deselectButtonClick && _selectStatus)
            {
                var chess = MatchManager.CurrentChess; 
                var select = MatchManager.CurrentGrid;
                if (select.gridType is Grid.GridType.Attack or Grid.GridType.Normal)
                    chess.EatPiece(select);
                else if (select.gridType == Grid.GridType.Special)
                {
                    if (chess.GetComponent<Pawn>() != null)
                    {
                        var pawn = chess.GetComponent<Pawn>();
                        if (select.location.y == 0 || select.location.y == 7)
                        {
                            if (select.occupyType != Grid.OccupyGridType.NoneOccupyGrid) 
                                chess.EatPiece(select);
                            if (!GameStatus.IsOver)
                            {
                                GameStatus.IsPromotion = true;
                                pawn.Promotion();
                            }
                        }
                        else
                        {
                            GameStatus.MoveType = "PassBy";
                            select.chessPiece = pawn;
                            select.occupyType = (Grid.OccupyGridType)pawn.camp;
                            pawn.En_Pass(select);
                        }
                    }
                    else
                    {
                        var king = chess.GetComponent<King>();
                        if (select.location.x == 2)
                        {
                            GameStatus.MoveType = "LongCast";
                            king.LongCastling();
                        }
                        else
                        {
                            GameStatus.MoveType = "ShortCast";
                            king.ShortCastling();
                        }
                    }
                }

                chess.DeselectPiece();
                chess.UpdateSelection();
                foreach (var selection in _grids) selection.Deselect();
                _selectStatus = false;
                _lastSelect = null;
                _selectGrid = null;
                _grids.Clear();
            }
        }

        // Selection光标
        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out _hit, 9999f, LayerMask.GetMask("Selection")))
        {
            var hitSelection = _hit.collider.GetComponent<Grid>();
            if (_selectGrid == hitSelection) return;
            if (_selectStatus)
            {
                if (_grids.Contains(hitSelection))
                {
                    RaySelect(hitSelection);
                    _lastSelect = _selectGrid;
                }
                else if (_lastSelect != null) _lastSelect.Select();
            }
            else RaySelect(hitSelection);
        }
        else if (_selectGrid != null && !_selectStatus)
        {
            _selectGrid.Deselect();
            _selectGrid = null;
        }
    }
    private void RaySelect(Grid hitGrid)
    {
        if (_selectGrid != null) _selectGrid.Deselect();
        _selectGrid = hitGrid;
        _selectGrid.Select();
    }
}
