using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class GenerateBoard : MonoBehaviour
{
    [SerializeField] private float squareGridSize = 1;
    [SerializeField] private float squareLength = 1;
    [SerializeField] private GameObject squarePrefab;
    [SerializeField] private float wallHeight;
    [SerializeField] private Material wallMaterial;
    [SerializeField] private Transform piecesRoot;
    private string spawnCode = "0-1-4-15, 1-4-2-4 means White Rook at Y4 & X15 and Black Queen at Y2 & X4";
    private DatabookBase dBB;
    private float gridLength;
    private List<SquareBase> allSquares = new List<SquareBase>();
    private List<PieceBase> allPieces = new List<PieceBase>();
    public float GridLength { get { return gridLength; } }
    public List<SquareBase> AllSquares { get { return allSquares; } set { allSquares = value; } } //Set for simulation
    public List<PieceBase> AllPieces { get { return allPieces; } }


    private void Awake()
    {
        gridLength = squareGridSize * squareLength;
        dBB = this.transform.GetChild(0).GetComponent<DatabookBase>();
        spawnCode = dBB.StandardSpawnCode;
        GenerateArena();
        GenerateAllPieces();
    }
    private void GenerateAllPieces() 
    {
        spawnCode = Regex.Replace(spawnCode, @"\s+", string.Empty);
        string[] spawns = spawnCode.Split(',');
        foreach(string spawn in spawns)
        {
            string[] codeStrings = spawn.Split(new char[] { '-', ' '}, StringSplitOptions.RemoveEmptyEntries);
            int[] codeInts = Array.ConvertAll(codeStrings, x => int.Parse(x));
            GenerateSinglePiece(codeInts[0], codeInts[1], codeInts[2], codeInts[3]);
        }
    }
    private void GenerateSinglePiece(int teamIndex, int pieceIndex, int y, int x)
    {
        Team t = new Team();
        Vector2Int pos = new Vector2Int(x, y);
        if (teamIndex == 1) { t.teamType = TeamType.Black; }

        #region Checking Spawn Code
        if (teamIndex > 2)
        { print($"Spawn Code Invalid\n Check Team Index {teamIndex}"); return; }
        if (pieceIndex > 5) 
        { print($"Spawn Code Invalid\nCheck Piece Index {pieceIndex}"); return; }
        if (x > squareGridSize || y > squareGridSize )
        { print($"Spawn Code Invalid\nCheck Grid Size"); return; }
        #endregion

        GameObject newPiece = Instantiate(dBB.PiecePrefab[pieceIndex], piecesRoot);
        PieceBase newPieceBase = newPiece.GetComponent<PieceBase>();
        SquareBase occupiedSquareBase = GetSquareFromPosition(pos);
        if(occupiedSquareBase == null)
        {
            print($"Spawn Code Invalid\nCheck Square X:{x} & Y:{y}");
            return;
        }
        newPieceBase.Init(occupiedSquareBase, t.teamType, GetMaterial(t));
        occupiedSquareBase.OccupiedPiece = newPieceBase;
        AllPieces.Add(newPieceBase);
    }
    private void GenerateArena()
    {
        this.transform.localScale = new Vector3(squareLength, 1, squareLength);
        List<GameObject> walls = new List<GameObject>(); ;
        GameObject wallRoot = new GameObject("Wall Root");
        for (int i = 0; i < 4; i++)
        {
            GameObject newWall = Instantiate(squarePrefab, wallRoot.transform);
            newWall.name = $"Wall:{i}";
            newWall.transform.localScale = new Vector3(1f, wallHeight, GridLength);
            newWall.GetComponent<Renderer>().material = wallMaterial;
            walls.Add(newWall);
        }
        float sharp = (GridLength / 2) + 0.5f;
        walls[0].transform.localPosition = new Vector3(sharp, wallHeight / 2, 0);
        walls[1].transform.localPosition = new Vector3(-sharp, wallHeight / 2, 0);
        walls[2].transform.localPosition = new Vector3(0, wallHeight / 2, sharp);
        walls[3].transform.localPosition = new Vector3(0, wallHeight / 2, -sharp);
        walls[2].transform.Rotate(0, 90f, 0);
        walls[3].transform.Rotate(0, 90f, 0);

        GenerateAllSquares();
    }
    private void GenerateAllSquares()
    {
        Team currentSquareTeam = new Team();
        currentSquareTeam.teamType = TeamType.Black; //0,0 of chess board is black
        float offset = -(squareGridSize-1)/2f;
        for (int x = 0; x < squareGridSize; x++)
        {
            for (int y = 0; y < squareGridSize; y++)
            {
                GameObject newSquare = Instantiate(squarePrefab, this.transform);
                GameObject mover = newSquare.transform.GetChild(0).gameObject;
                mover.GetComponent<Renderer>().material.color = Color.grey;

                SquareBase newSquareBase = newSquare.AddComponent<SquareBase>();
                newSquareBase.Init(x, y, offset, currentSquareTeam, GetMaterial(currentSquareTeam));
                
                currentSquareTeam.SwitchTeam();
                allSquares.Add(newSquareBase);
            }
            if (squareGridSize % 2 == 0) { currentSquareTeam.SwitchTeam(); }
        }
    }
    private Material GetMaterial(Team t)
    {
        if (t.teamType == TeamType.White)
        {
            return dBB.TeamMaterial[0];
        }
        else
        {
            return dBB.TeamMaterial[1];
        }
    }
    public SquareBase GetSquareFromPosition(Vector2Int pos)
    {
        return allSquares.Find(square => square.BoardPosition.x == pos.x && square.BoardPosition.y == pos.y);
    }
}
