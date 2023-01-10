using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameStatus : MonoBehaviour
{
    [SerializeField] private bool againstAI;
    [SerializeField] private bool againstTime;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private List<float> clocks;
    [SerializeField] private TeamType playerOneTeam;
    [SerializeField] private float fillerTime;
    [SerializeField] private TextMeshProUGUI consoleTMP;

    private SquareSelector squareSelector;
    private bool moveAllowed = false;
    private int whichPlayerIndex;
    private GameObject currentPlayer;
    private bool showTimer;
    private Team currentGameTurn = new Team();
    private GoTop goTopFeature;

    public GameObject CurrentPlayer { get { return currentPlayer; } }
    public bool MoveAllowed { get { return moveAllowed; } set { value = moveAllowed; } }
    public Team CurrentGameTurn { get { return currentGameTurn; } }

    private void Awake()
    {
        goTopFeature = GameObject.FindGameObjectWithTag("Board").GetComponent<GoTop>();
        squareSelector = GameObject.FindGameObjectWithTag("Board").GetComponent<SquareSelector>();
        if (!againstAI && playerOneTeam == TeamType.Black) { playerOneTeam = TeamType.White; } // Fix
    }

    private void Start()
    {
        StartCoroutine(Starting());
    }
    private IEnumerator Starting()
    {
        yield return Filler(fillerTime + 2f, "Game has started.");
        ProcessTurn();
    }
    private IEnumerator MoveDone(bool continueGame)
    {
        currentGameTurn.SwitchTeam();
        if (continueGame)
        {
            yield return Filler();
            ProcessTurn();
        }
        else 
        {
            yield return GameEnds(currentGameTurn);
        }
    }
    private void Update()
    {

        if (againstTime && moveAllowed)
        {
            ClockTimer();
            if (showTimer) { consoleTMP.text = $"Time Left: {(int)clocks[whichPlayerIndex]} seconds\nIt's {currentGameTurn.teamType}'s turn"; }
        }

        if (moveAllowed)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                if (squareSelector.CurrentSquare.OccupiedPiece)
                {
                    if (squareSelector.CurrentSquare.OccupiedPiece.WithTeam.teamType == currentGameTurn.teamType)
                    {
                        this.GetComponent<MoveIt>().Highlight();
                    }
                }

                if (squareSelector.CurrentSquare.AvailableMove)
                {
                    Scene scene = SceneManager.GetActiveScene();
                    if(scene.buildIndex == 0)
                    {
                        SceneManager.LoadScene(1);
                    }
                    else
                    {
                        bool continueGame = this.GetComponent<MoveIt>().MoveThis(squareSelector.CurrentSquare);
                        moveAllowed = false;
                        StartCoroutine(MoveDone(continueGame));
                    }
                }
            }
        }
    }

    private void ClockTimer()
    {
        if (whichPlayerIndex == 0)
        {
            if (clocks[0] > 0) { clocks[0] -= Time.deltaTime; }
            else { StartCoroutine(GameEnds(currentGameTurn)); }
        }

        if (againstAI) { return; }

        if(whichPlayerIndex == 1)
        {
            if (clocks[1] > 0 && whichPlayerIndex == 1) { clocks[1] -= Time.deltaTime; }
            else { StartCoroutine(GameEnds(currentGameTurn)); }
        }
    }

    private IEnumerator GameEnds(Team loser)
    {
        loser.SwitchTeam(); //Now winner
        yield return Filler(fillerTime + 2f, $"Game has ended.\n{loser.teamType} Wins!", true);
        SceneManager.LoadScene(0);
    }

    private IEnumerator Filler(float waiter = 0, string message = "", bool hideTurn = false)
    {
        if(waiter == 0) { waiter = fillerTime; }
        if (!hideTurn) { message += $"\nIt's {currentGameTurn.teamType}'s turn"; }
        consoleTMP.text = message;
        goTopFeature.TopViewActivate(true, true);
        yield return new WaitForSeconds(waiter);
        goTopFeature.TopViewActivate(false, false);
        consoleTMP.text = "";

    }
    public void ProcessTurn(bool forceShut = false)
    {
        if (forceShut)
        {
            Destroy(currentPlayer);
            return;
        }

        if(currentGameTurn.teamType == playerOneTeam)
        {
            whichPlayerIndex = 0;
            PlayerMove();
        }
        else
        {
            if (againstAI) { whichPlayerIndex = -1; StartCoroutine(EnemyMove()); }
            else { whichPlayerIndex = 1;  PlayerMove(); }
        }
    }

    private void PlayerMove()
    {
        if (currentPlayer) { Destroy(currentPlayer); }
        currentPlayer = Instantiate(playerPrefab);
        CurrentPlayer.name = $"Player {whichPlayerIndex}";
        if(whichPlayerIndex == 0)
        {
            currentPlayer.transform.Rotate(0f, 180, 0f);
        }

        showTimer = true;
        moveAllowed = true;
        //highlighter and checks on
    }
    private IEnumerator EnemyMove()
    {
        showTimer = false;
        moveAllowed = false;
        Destroy(currentPlayer);
        yield return Filler();
        bool continueGame = this.GetComponent<MoveIt>().EnemyRandomSearch(currentGameTurn.teamType);
        yield return Filler();
        StartCoroutine(MoveDone(continueGame));
        //top view on
    }
}
