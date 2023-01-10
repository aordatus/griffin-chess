using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoTop : MonoBehaviour
{
    private bool topViewActive;
    private bool restriction;
    [SerializeField] private GameObject secondaryCam;
    [SerializeReference] private bool sizeFeature;
    [SerializeField] private float normal = 1;
    [SerializeField] private float enlarged = 2;
    private GameStatus gameStatus;
    private void Awake()
    {
        gameStatus = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameStatus>();
        secondaryCam.SetActive(false);
        PiecesSize(normal);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C) && !restriction)
        {
            if (topViewActive)
            {
                TopViewActivate(false);
            }
            else
            {
                TopViewActivate(true);
            }
        }
    }

    public void TopViewActivate(bool doIt, bool restricted = false)
    {
        gameStatus.MoveAllowed = !doIt;
        float offset = this.GetComponent<GenerateBoard>().GridLength;
        secondaryCam.transform.position = new Vector3(0, offset*0.9f, 0);
        restriction = restricted;
        secondaryCam.SetActive(doIt);
        topViewActive = doIt;
        if (gameStatus.CurrentPlayer)
        {
            gameStatus.CurrentPlayer.SetActive(!doIt);
        }
        if (doIt)
        {
            PiecesSize(enlarged); 
            return;
        }
        PiecesSize(normal);
    }

    private void PiecesSize(float k)
    {
        if (!sizeFeature) { return; }
        
        foreach(PieceBase pb in this.GetComponent<GenerateBoard>().AllPieces)
        {
            pb.gameObject.transform.localScale = new Vector3(k, k, k);
        }
        
    }
}
