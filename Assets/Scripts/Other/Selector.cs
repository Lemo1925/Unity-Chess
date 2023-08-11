using System.Collections.Generic;
using Controller;
using UnityEngine;

public class Selector : MonoBehaviour
{
    [Header("选择材质")]
    public List<Material> materials;

    public LayerMask mask;
    private GameObject tileHighlight, chessPieces;

    private bool select;
    public MaterialIndex materialIdx = MaterialIndex.Default , oldIndex = MaterialIndex.Default;

    public enum MaterialIndex
    {
        Selected = 0,
        Moved = 1,
        Attack = 2,
        Default = 3
    }
    
    private void OnEnable()
    {
        EventManager.OnSelectTurnEvent += SelectTile;
        EventManager.OnSelectTurnEvent += SelectPiece;
    }

    private void OnDisable()
    {
        EventManager.OnSelectTurnEvent -= SelectTile;
        EventManager.OnSelectTurnEvent -= SelectPiece;
    }
    
    
    private void SelectTile()
    {
        Ray ray = Camera.main!.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out var hit, 9999f, mask))
        {

            var hitObject = hit.collider.gameObject;
            if (hitObject.CompareTag($"Board") )
            {
                if (materialIdx == MaterialIndex.Selected) return;
                oldIndex = materialIdx;

                tileHighlight = hitObject;
                materialIdx = (int)MaterialIndex.Selected;
                tileHighlight.GetComponent<Renderer>().material = materials[(int)materialIdx];
            }
        }
        else
        {
            if (tileHighlight != null)
            {
                materialIdx = oldIndex;
                tileHighlight.GetComponent<Renderer>().material = materials[(int)materialIdx];
            }
        }
    }

    private void SelectPiece()
    {
        List<GameObject> targets = new List<GameObject>();
        Ray ray = Camera.main!.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out var hit))
        {
            var hitObject = hit.collider.gameObject;

            if (hitObject.CompareTag($"Chess"))
            {
                if (Input.GetMouseButtonDown(0) && !select)
                {
                    chessPieces = hitObject;
                    var chess = chessPieces.GetComponent<Chess>();
                    if (chess.camp == GameController.RoundType)
                    {
                        select = true;
                        ChessBoard.instance.SelectPiece(chessPieces);
                        // 计算可以移动的格子
                        targets = chess.CalculateTarget();
                        // 给格子染色
                        // todo：判断格子上有没有敌方单位
                        foreach (var target in targets)
                        {
                            materialIdx = MaterialIndex.Moved;
                            target.GetComponent<Renderer>().material = materials[(int)materialIdx];
                        }
                    }
                }
            }
            // 移动棋子
            if (hitObject.CompareTag($"Board") && targets.Contains(hitObject))
            {
                Vector2 tile = Geometry.GridFromPoint(hitObject.transform.position);
                chessPieces.GetComponent<Chess>().Moveto(tile, MoveType.Move);
            }
        }
        
        if (Input.GetMouseButtonDown(1) && select)
        {
            ChessBoard.instance.DeselectPiece(chessPieces);
            foreach (var target in targets)
            {
                materialIdx = MaterialIndex.Default;
                target.GetComponent<Renderer>().material = materials[(int)materialIdx];
            }
            select = false;
        }
    }
}
