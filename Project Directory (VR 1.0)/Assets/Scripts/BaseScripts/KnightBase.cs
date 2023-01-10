using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnightBase : PieceBase
{
    public override List<Vector2Int> MoveListGenerate()
    {
        MoveList = new List<Vector2Int>();
        Vector2Int pos = new Vector2Int();

        pos.Set(BoardPosition.x + 1, BoardPosition.y + 2);
        AddSquare(pos, 0);
        pos.Set(BoardPosition.x + 2, BoardPosition.y + 1);
        AddSquare(pos, 0);
        
        pos.Set(BoardPosition.x - 1, BoardPosition.y + 2);
        AddSquare(pos, 0);
        pos.Set(BoardPosition.x - 2, BoardPosition.y + 1);
        AddSquare(pos, 0);

        pos.Set(BoardPosition.x + 1, BoardPosition.y - 2);
        AddSquare(pos, 0);
        pos.Set(BoardPosition.x + 2, BoardPosition.y - 1);
        AddSquare(pos, 0);

        pos.Set(BoardPosition.x - 1, BoardPosition.y - 2);
        AddSquare(pos, 0);
        pos.Set(BoardPosition.x - 2, BoardPosition.y - 1);
        AddSquare(pos, 0);

        return MoveList;
    }
}
