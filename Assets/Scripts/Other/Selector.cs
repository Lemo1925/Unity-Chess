using System.Collections.Generic;
using Controller;
using UnityEngine;

public class Selector : MonoBehaviour
{
    public List<Selection> selections = new List<Selection>();
    private GameObject chessPieces;
    public Selection gridSelection;
    public LayerMask mask;

    private bool selectStatus;
    private RaycastHit raycastHit;
    private void OnEnable()
    {
        EventManager.OnSelectTurnEvent += SelectTile;
        EventManager.OnSelectActionEvent += SelectPiece;
    }

    private void OnDisable()
    {
        EventManager.OnSelectTurnEvent -= SelectTile;
        EventManager.OnSelectActionEvent -= SelectPiece;
    }
    
    private void SelectTile()
    {
        Ray ray = Camera.main!.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out raycastHit, 9999f, mask))
        {
            if (gridSelection == raycastHit.collider.GetComponent<Selection>()) return;
            if (gridSelection != null) gridSelection.Deselect();
            gridSelection = raycastHit.collider.GetComponent<Selection>();
            gridSelection.Select();
        }
    }

    // todo: Refactor this
    private void SelectPiece(bool SelectButtonClick, bool DeselectButtonClick)
    {
        Ray ray = Camera.main!.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out var hit))
        {
            var hitObject = hit.collider.gameObject;
            if (hitObject.CompareTag($"Chess"))
            {
                if (SelectButtonClick && !selectStatus)
                {
                    chessPieces = hitObject;
                    MatchManager.Instance.currentChess = chessPieces.GetComponent<Chess>();
                    var chess = MatchManager.Instance.currentChess;
                    if (chess.camp == GameController.RoundType)
                    {
                        selectStatus = true;
                        chess.SelectPiece();
                        selections = chess.CalculateGrid();
                    }
                }
            }
            // 移动棋子
            if (hitObject.CompareTag($"Board"))
                if (selections.Contains(hitObject.GetComponent<Selection>()))
                    MatchManager.Instance.currentChess.Move(MoveType.Move);
        }   
        
        if (DeselectButtonClick && selectStatus)
        {
            chessPieces.GetComponent<Chess>().DeselectPiece();
            foreach (var selection in selections) selection.Deselect();
            selectStatus = false;
            selections.Clear();
        }
    }
}
