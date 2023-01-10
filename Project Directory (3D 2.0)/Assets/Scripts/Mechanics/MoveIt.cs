using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveIt : MonoBehaviour
{
    [SerializeField] private AudioClip[] aClips;
    /*
    private SpecialMove specialMoves;
    private List<PieceBase> deadWhites = new List<PieceBase>();
    private List<PieceBase> deadBlacks = new List<PieceBase>();
    */
    private GameObject board; 
    private AudioSource aSrc;
    private PieceBase what;
    private List<Vector2Int> availableMoves;
    private List<Move> moveList = new List<Move>();
    private List<GameObject> simulationObjects = new List<GameObject>();
    private void Awake()
    {
        aSrc = this.gameObject.AddComponent<AudioSource>();
        board = GameObject.FindGameObjectWithTag("Board");
    }
    public bool MoveThis(SquareBase where)
    {
        //For storing moves later
        Move newMove = new Move(what, where);
        moveList.Add(newMove);

        if (newMove.where.OccupiedPiece)
        {
            aSrc.PlayOneShot(aClips[1]);
            Destroy(where.OccupiedPiece.gameObject);
            board.GetComponent<GenerateBoard>().AllPieces.Remove(where.OccupiedPiece);
        }
        else
        {
            aSrc.PlayOneShot(aClips[0]);
        }

        what.OccupiedSquare.OccupiedPiece = null;
        what.OccupiedSquare = where;
        where.OccupiedPiece = what;
        what.MoveUpdate();
        moveList.Add(newMove);
        AllHighlight(false);

        if (CheckForCheckmate()) { return false; }
        return true;
    }
    public void Highlight()
    {
        AllHighlight(false);
        PieceBase pb = board.GetComponent<SquareSelector>().CurrentSquare.OccupiedPiece;
        what = pb;
        availableMoves = pb.MoveListGenerate();
        PreventCheck();

        foreach (Vector2Int move in availableMoves)
        {
            var moveSquare = board.GetComponent<GenerateBoard>().GetSquareFromPosition(move);
            if (moveSquare)
            {
                GameObject mover = moveSquare.gameObject.transform.GetChild(0).gameObject;
                mover.SetActive(true);
                if (moveSquare.OccupiedPiece) { mover.GetComponent<Renderer>().material.color = Color.red; }
                else { mover.GetComponent<Renderer>().material.color = Color.green; }
                moveSquare.AvailableMove = true;
            }
        }
    }
    private void PreventCheck()
    {
        PieceBase targetKing = null;
        foreach(PieceBase pieceBase in board.GetComponent<GenerateBoard>().AllPieces) 
        { 
            if(pieceBase.WithTeam.teamType == what.WithTeam.teamType && pieceBase.Typing == PieceType.King)
            {
                targetKing = pieceBase;
            }
        }

        SimulateMoveForSinglePiece(what, availableMoves, targetKing);
         
    }
    private void SimulateMoveForSinglePiece(PieceBase pieceBase, List<Vector2Int> movesUsedHere, PieceBase targetKing)                                                                                                                                                                                                  
    {

        //Save the current values to reset after the function call
        Vector2Int actualPos = pieceBase.BoardPosition;
        List<Vector2Int> movesToRemove = new List<Vector2Int>();
        List<SquareBase> actualSquares = board.GetComponent<GenerateBoard>().AllSquares;

        //Going through all the moves, simulate them and check if we are in check
        foreach (Vector2Int move in movesUsedHere)
        {
            Vector2Int simPos = move;
            Vector2Int kingPositionThisSim = new Vector2Int(targetKing.BoardPosition.x, targetKing.BoardPosition.y);

            //Did we simulate the king's move
            if (pieceBase.Typing == PieceType.King) 
            {                                                                                                       
                kingPositionThisSim = new Vector2Int(simPos.x, simPos.y);
            }

            //Deep copies not shallow
            List<SquareBase> simulation = new List<SquareBase>();
            List<PieceBase> simAttackingPieces = new List<PieceBase>();
            foreach(SquareBase sB in board.GetComponent<GenerateBoard>().AllSquares)
            {
                GameObject newSimObj = new GameObject("Simulation (Ignore)");
                newSimObj.transform.parent = this.transform;
                simulationObjects.Add(newSimObj);
                SquareBase newSquareBase = newSimObj.AddComponent<SquareBase>();
                newSquareBase.BoardPosition = new Vector2Int(sB.BoardPosition.x, sB.BoardPosition.y);

                if (!sB.OccupiedPiece)
                {
                    simulation.Add(newSquareBase);
                    continue; 
                }
                else
                {
                    newSquareBase.OccupiedPiece = sB.OccupiedPiece;
                    simulation.Add(newSquareBase);
                }

                if (newSquareBase.OccupiedPiece.WithTeam.teamType != pieceBase.WithTeam.teamType)
                {
                    simAttackingPieces.Add(newSquareBase.OccupiedPiece);
                }
            }


            //Simulate that move
            simulation.Find(s => s.BoardPosition.x == actualPos.x && s.BoardPosition.y == actualPos.y).OccupiedPiece = null;
            pieceBase.OccupiedSquare = board.GetComponent<GenerateBoard>().GetSquareFromPosition(simPos);
            simulation.Find(s => s.BoardPosition.x == simPos.x && s.BoardPosition.y == simPos.y).OccupiedPiece = pieceBase;

            //Did one of the piece got taken during out simulation
            var deadpiece = simAttackingPieces.Find(p => p.BoardPosition.x == simPos.x && p.BoardPosition.y == simPos.y);
            if (deadpiece != null) { simAttackingPieces.Remove(deadpiece); }

            //Get all the simulated attacking piece moves
            board.GetComponent<GenerateBoard>().AllSquares = simulation;
            List<Vector2Int> simMoves = new List<Vector2Int>();
            foreach(PieceBase pB in simAttackingPieces)
            {
               var pieceMoves = pB.MoveListGenerate();
               foreach (var pM in pieceMoves) { simMoves.Add(pM); }
            }

            //If the king is in trouble
            if (ContainsValidMove(simMoves, kingPositionThisSim)) { movesToRemove.Add(move); }

            //Restore
            board.GetComponent<GenerateBoard>().AllSquares = actualSquares;
            pieceBase.OccupiedSquare = board.GetComponent<GenerateBoard>().GetSquareFromPosition(actualPos);
            foreach (GameObject g in simulationObjects) { Destroy(g); }
            simulationObjects.Clear();
        }
        foreach (Vector2Int move in movesToRemove) { movesUsedHere.Remove(move); }
    }
    private bool CheckForCheckmate()
    {
        PieceBase targetKing = null;
        List<PieceBase> attackingPieces = new List<PieceBase>();
        List<PieceBase> defendingPieces = new List<PieceBase>();

        foreach (PieceBase pieceBase in board.GetComponent<GenerateBoard>().AllPieces)
        {
            if (pieceBase.WithTeam.teamType != what.WithTeam.teamType)
            {
                defendingPieces.Add(pieceBase);
                if (pieceBase.Typing == PieceType.King)
                {
                    targetKing = pieceBase;
                }
            }
            if (pieceBase.WithTeam.teamType == what.WithTeam.teamType)
            {
                attackingPieces.Add(pieceBase);
            }
        }

        //Is the king attacked now?
        List<Vector2Int> currentAvailableMoves = new List<Vector2Int>();

        foreach (PieceBase pB in attackingPieces)
        {
            var pieceMoves = pB.MoveListGenerate();
            foreach (var pM in pieceMoves)
            {
                currentAvailableMoves.Add(pM);
            }
        }

        //Are we in check
        if (ContainsValidMove(currentAvailableMoves, targetKing.BoardPosition))
        {
            //King is under attack, can we move something to help him
            foreach (PieceBase pB in defendingPieces)
            {
                List<Vector2Int> defendingMoves = pB.MoveListGenerate();
                int before = defendingMoves.Count;
                SimulateMoveForSinglePiece(pB, defendingMoves, targetKing);
                int after = defendingMoves.Count;

                if (defendingMoves.Count != 0)
                {
                    return false;
                }
            }
            return true;
        }

        return false;
    }
    private bool ContainsValidMove(List<Vector2Int> moves, Vector2 pos)
    {
        foreach(Vector2Int move in moves)
        {
            if (move.x == pos.x && move.y == pos.y) { return true; }
        }
        return false;
       
    }
    private void AllHighlight(bool doIt)
    {
        availableMoves = null;
        foreach (SquareBase squareBase in board.GetComponent<GenerateBoard>().AllSquares)
        {
            squareBase.AvailableMove = false;
            GameObject mover = squareBase.gameObject.transform.GetChild(0).gameObject;
            mover.SetActive(doIt);
            mover.GetComponent<Renderer>().material.color = Color.grey;
        }
    }
    public void EnemyRandomSearch(TeamType currentTeamTurn)
    {
        List<Move> moves = new List<Move>();
        foreach (PieceBase pieceBase in board.GetComponent<GenerateBoard>().AllPieces)
        {
            if(pieceBase.WithTeam.teamType != this.gameObject.GetComponent<GameStatus>().CurrentGameTurn.teamType)
            {
                List<Vector2Int> movesGenerated = pieceBase.MoveListGenerate();
                foreach(Vector2Int move in movesGenerated)
                {
                    var moveSquare = board.GetComponent<GenerateBoard>().GetSquareFromPosition(move);
                    moves.Add(new Move(pieceBase, moveSquare));
                }
            }
        }
        int moveIndex = Random.Range(0, moves.Count);
        Move selectedMove = moves[moveIndex];
        what = selectedMove.what;
        MoveThis(selectedMove.where);
    }

}
