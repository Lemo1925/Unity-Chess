using System.Collections.Generic;
using Controller;
using UnityEngine;

public class Selector : MonoBehaviour
{
    [Header("选择材质")]
    public List<Material> materials;
    public List<Selection> selections = new List<Selection>();
    private GameObject gridObject, chessPieces;

    public LayerMask mask;

    private bool selectStatus;
    public MaterialList material = MaterialList.Default , oldMaterial = MaterialList.Default;

    public enum MaterialList
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
            if (material == MaterialList.Selected || material == MaterialList.Moved) return;
            gridObject = hit.collider.gameObject;
            oldMaterial = material;
            material = MaterialList.Selected;
            gridObject.GetComponent<Selection>().Select(materials[(int)material]);
        }
        else
        {
            if (gridObject == null) return;
            material = oldMaterial;
            gridObject.GetComponent<Selection>().Deselect(materials[(int)material]);
        }
    }

    private void SelectPiece()
    {
        Ray ray = Camera.main!.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out var hit))
        {
            var hitObject = hit.collider.gameObject;
            if (hitObject.CompareTag($"Chess"))
            {
                if (Input.GetMouseButtonDown(0) && !selectStatus)
                {
                    chessPieces = hitObject;
                    var chess = chessPieces.GetComponent<Chess>();
                    if (chess.camp == GameController.RoundType)
                    {
                        selectStatus = true;
                        chess.SelectPiece(materials[(int)MaterialList.Selected]);
                        // 计算可以移动的格子
                        selections = chess.CalculateGrid();
                        // 给格子染色
                        foreach (var selection in selections)
                        {
                            if (selection.gridType == Selection.GridType.NormalGrid)
                            {
                                material = MaterialList.Moved;
                                selection.Select(materials[(int)material]);
                            }
                            else if ((int)selection.gridType != (int)GameController.RoundType)
                            {
                                material = MaterialList.Attack;
                                selection.Select(materials[(int)material]);
                            }
                        }
                    }
                }
            }
            // 移动棋子
            if (hitObject.CompareTag($"Board"))
            {
                var selection = hitObject.GetComponent<Selection>();
                if (selections.Contains(selection))
                {
                    Vector2 tile = Geometry.GridFromPoint(hitObject.transform.position);
                    chessPieces.GetComponent<Chess>().Move(tile, MoveType.Move);
                }
            }
        }
        
        if (Input.GetMouseButtonDown(1) && selectStatus)
        {
            chessPieces.GetComponent<Chess>().DeselectPiece();
            foreach (var selection in selections)
            {
                material = MaterialList.Default;
                selection.Select(materials[(int)material]);
            }
            selectStatus = false;
            selections.Clear();
        }
    }
}
