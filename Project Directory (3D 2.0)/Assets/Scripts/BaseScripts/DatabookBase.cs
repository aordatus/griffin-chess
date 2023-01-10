using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

#region Operational Classes
public enum PieceType
{
    Pawn,
    Rook,
    Knight,
    Bishop,
    King,
    Queen
}
public enum TeamType
{
    White,
    Black
}
public class Team
{
    public TeamType teamType { get; set; }
    public void SwitchTeam()
    {
        if (teamType == TeamType.White)
        {
            teamType = TeamType.Black; return;
        }
        teamType = TeamType.White;
    }
}
public class Move
{
    public PieceBase what { get; set; }
    public SquareBase where { get; set; }
    public Move(PieceBase piece, SquareBase where)
    {
        this.what = piece;
        this.where = where;
    }
}
public enum SpecialMove
{
    None = 0,
    EnPassant,
    Castling,
    Promotion
}

#endregion

public class DatabookBase : MonoBehaviour
{
    [SerializeField] private GameObject[] piecePrefab;
    [SerializeField] private Material[] teamMaterial;
    [SerializeField] private TextAsset standardSpawnCode;

    public string StandardSpawnCode { get { return standardSpawnCode.text; } }
    public GameObject[] PiecePrefab { get { return piecePrefab; } }
    public Material[] TeamMaterial { get { return teamMaterial; } }
} 
