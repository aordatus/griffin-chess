using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceBase : MonoBehaviour
{
    [SerializeField] private string console;
    [SerializeField] PieceType pieceType;
    private GenerateBoard generateBoard;
    public PieceType Typing { get { return pieceType; } }
    public Team WithTeam { get; set; } = new Team();
    public bool Virginity { get; set; }
    public SquareBase OccupiedSquare { get; set; } = null;
    public GenerateBoard GenBoard { get { return generateBoard; } }
    public Vector2Int BoardPosition { get { return OccupiedSquare.BoardPosition; } }
    public List<Vector2Int> MoveList { get; set; }

    public void Init(SquareBase sB, TeamType t, Material m)
    {
        generateBoard = GameObject.FindGameObjectWithTag("Board").GetComponent<GenerateBoard>();
        this.Virginity = true;
        this.OccupiedSquare = sB;
        this.WithTeam.teamType = t;
        this.transform.position = OccupiedSquare.transform.position;
        this.transform.position = new Vector3(this.transform.position.x, 0.5f, this.transform.position.z);
        this.name = $"{t} {pieceType}";
        this.GetComponent<Renderer>().material = m;
        console = $"X:{BoardPosition.x} & Y:{BoardPosition.y}";
    }
    public virtual List<Vector2Int> MoveListGenerate()
    {
        List<Vector2Int> moveList = new List<Vector2Int>();

        moveList.Add(new Vector2Int(3, 3));
        moveList.Add(new Vector2Int(3, 4));
        moveList.Add(new Vector2Int(4, 3));
        moveList.Add(new Vector2Int(4, 4));

        return moveList;
    }
   
    public void MoveUpdate()
    {
        this.transform.position = OccupiedSquare.transform.position;
        this.transform.position = new Vector3(this.transform.position.x, 0.5f, this.transform.position.z);
        if (Virginity) { Virginity = false; }
    }

    //Returns true if hits block
    //0 - can kill, 1 - can't kill, 2 - only kill
    public bool AddSquare(Vector2Int pos, int typeSearch) 
    {
        var sq = GenBoard.GetSquareFromPosition(pos);
        if (sq)
        {
            if (typeSearch == 2 && sq.OccupiedPiece)
            {
                if (sq.OccupiedPiece.WithTeam.teamType != WithTeam.teamType)
                {
                    MoveList.Add(pos);
                    return true;
                }
            }
            if (typeSearch == 1)
            {
                if (!sq.OccupiedPiece)
                {
                    MoveList.Add(pos);
                    return false;
                }
            }

            if (typeSearch == 0)
            {
                if (!sq.OccupiedPiece)
                {
                    MoveList.Add(pos);
                    return false;
                }
               
                if (sq.OccupiedPiece.WithTeam.teamType != WithTeam.teamType)
                {
                    MoveList.Add(pos);
                    return true;
                }
            }

        }
        return true;
       
    }
}
