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
        if (_selectGrid&& !GameStatus.IsPromotion )
        {
            var chessPiece = _selectGrid.chessPiece;
            // 选择棋子并计算可移动格子
            if (selectButtonClick && !_selectStatus&& chessPiece && chessPiece.camp == GameStatus.RoundType)
            {
                chessPiece.SelectPiece();
                _selectStatus = true;
                _grids = chessPiece.CalculateGrid();
            }
            // 移动棋子
            if (_grids.Contains(_selectGrid)) MatchManager.CurrentChess.MovePiece();
            // 取消选择
            if (deselectButtonClick && _selectStatus)
            {
                var chess = MatchManager.CurrentChess; 
                var select = MatchManager.CurrentGrid;
                // 判断移动类型
                switch (select.gridType)
                {
                    case GridType.Attack:
                        chess.EatPiece(select);
                        break;
                    case GridType.Normal:
                        chess.EatPiece(select);
                        break;
                    case GridType.Special:
                    {
                        var pawn = chess.GetComponent<Pawn>();
                        var king = chess.GetComponent<King>();
                        if (pawn)
                            if (select.location.y is 0 or 7) pawn.Promotion();
                            else pawn.En_Pass(select);
                        if (king) 
                            king.Castling(select.location.x == 2);
                        break;
                    }
                }

                chess.DeselectPiece();
                chess.UpdateSelection();
                
                foreach (var selection in _grids) 
                    selection.Deselect();
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
        if (_selectGrid) _selectGrid.Deselect();
        _selectGrid = hitGrid;
        _selectGrid.Select();
    }
}
