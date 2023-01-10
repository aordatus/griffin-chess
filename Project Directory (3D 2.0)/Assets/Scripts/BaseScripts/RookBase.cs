using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RookBase : PieceBase
{
    public override List<Vector2Int> MoveListGenerate()
    {
        MoveList = new List<Vector2Int>();
        Vector2Int pos = new Vector2Int();
        for (int i = BoardPosition.y - 1; i >= 0; i--)
        {
            pos.Set(BoardPosition.x, i);
            bool breakPoint = AddSquare(pos, 0);
            if (breakPoint) { break; }
        } 
        for (int i = BoardPosition.y + 1; i  < GenBoard.GridLength; i++)
        {
            pos.Set(BoardPosition.x, i);
            bool breakPoint = AddSquare(pos, 0);
            if (breakPoint) { break; }
        }
        for (int i = BoardPosition.x - 1; i >= 0; i--)
        {
            pos.Set(i, BoardPosition.y);
            bool breakPoint = AddSquare(pos, 0);
            if (breakPoint) { break; }
        }
        for (int i = BoardPosition.x + 1; i < GenBoard.GridLength; i++)
        {
            pos.Set(i, BoardPosition.y);
            bool breakPoint = AddSquare(pos, 0);
            if (breakPoint) { break; }
        }
        return MoveList;
    }
}
