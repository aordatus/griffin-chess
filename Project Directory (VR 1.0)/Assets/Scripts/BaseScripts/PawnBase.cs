using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PawnBase : PieceBase
{
    public override List<Vector2Int> MoveListGenerate()
    {
        MoveList = new List<Vector2Int>();

        int direction = (WithTeam.teamType == TeamType.White) ? 1 : -1;
        Vector2Int pos = new Vector2Int();

        //OneFront
        pos.Set(BoardPosition.x, BoardPosition.y + direction);
        AddSquare(pos, 1);

        //TwoFront
        if (Virginity)
        {
            pos.Set(BoardPosition.x, BoardPosition.y + direction * 2);
            AddSquare(pos, 1);
        }

        //KillMove Both Directions
        pos.Set(BoardPosition.x+1, BoardPosition.y + direction);
        AddSquare(pos, 2);
        pos.Set(BoardPosition.x - 1, BoardPosition.y + direction);
        AddSquare(pos, 2);

        return MoveList;
    }
}
    