using System.Collections.Generic;
using UnityEngine;

public class Pawn : Chess
{
    public int moveTimes;
    bool isPromotion;
    public void Start() => moveTimes = 0;

    public override void Move(MoveType moveType)
    {
        MovePiece();
        moveTimes++;
    }

    public override List<Selection> CalculateGrid()
    {
        List<Selection> selections = new List<Selection>();
        Selection selection = MatchManager.Instance.currentSelection;
        List<Selection> MoveSensors = selection.ForwardAndBack(moveTimes == 0 ? 2 : 1, 0);
        List<Selection> AttackSensors = selection.Bevel(1, 0);
        foreach (var sensor in MoveSensors)
        {
            if (sensor.occupyType != Selection.OccupyGridType.NoneOccupyGrid) continue;
            if (sensor.Location.y == 0 || selection.Location.y == 7)
                sensor.SpecialSelect();
            else sensor.MoveSelect();
            selections.Add(sensor);
        }

        foreach (var sensor in AttackSensors)
        {
            if (sensor.occupyType == (Selection.OccupyGridType)camp) continue;
            if (sensor.occupyType == Selection.OccupyGridType.NoneOccupyGrid) continue;
            if (sensor.Location.y == 0 || selection.Location.y == 7) sensor.SpecialSelect();
            else sensor.AttackSelect();
            selections.Add(sensor);
        }
        return selections;
    }
    
    public void Promotion()
    {
        isPromotion = true;
        if (isPromotion)
        {
            EventManager.CallOnPromotion(isPromotion);
            var chessIdx = (int)MatchManager.Instance.promotionType;

            if (chessIdx == 0) return;
            GameObject chessInstance = Instantiate(
                ChessBoard.instance.ChessPrefab[Mathf.Abs((int)MatchManager.Instance.promotionType) - 1],
                transform.position, transform.rotation);
                
            chessInstance.GetComponent<Renderer>().material =
                (int)MatchManager.Instance.promotionType > 0
                    ? ChessBoard.instance.materials[0]
                    : ChessBoard.instance.materials[1];
                
            ChessBoard.instance.chessGO[MatchManager.Instance.promotionType].Add(chessInstance);
                
            switch (Mathf.Abs(chessIdx))
            {
                case 1:
                {
                    Rock rock = chessInstance.AddComponent<Rock>();
                    rock.camp = chessIdx > 0 ? Camp.WHITE : Camp.BLACK;
                    rock.Location = Location;
                    break;
                }
                case 2:
                {
                    Knight knight = chessInstance.AddComponent<Knight>();
                    knight.camp = chessIdx > 0 ? Camp.WHITE : Camp.BLACK;
                    knight.Location = Location;
                    break;
                }
                case 3:
                {
                    Bishop bishop = chessInstance.AddComponent<Bishop>();
                    bishop.camp = chessIdx > 0 ? Camp.WHITE : Camp.BLACK;
                    bishop.Location = Location;
                    break;
                }
                case 4:
                {
                    Queen queen = chessInstance.AddComponent<Queen>();
                    queen.camp = chessIdx > 0 ? Camp.WHITE : Camp.BLACK;
                    queen.Location = Location;
                    break;
                }
            }

            isPromotion = false;
            DestroyPiece();
        }
        else EventManager.CallOnPromotion(isPromotion);
    }

    public void En_Pass(){}
}