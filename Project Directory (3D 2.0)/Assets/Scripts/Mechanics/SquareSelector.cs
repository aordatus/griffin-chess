using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SquareSelector : MonoBehaviour
{
    [SerializeField] private GameStatus gameStatus;
    public float threshold;
    private GenerateBoard boardGenerator;
    private Transform player;
    private Transform[] squareTransforms;
    private Transform currentSquareTransform;
    public SquareBase CurrentSquare { get { return currentSquareTransform.gameObject.GetComponent<SquareBase>(); } }

    private void Start()
    {
        boardGenerator = this.GetComponent<GenerateBoard>();
        squareTransforms = boardGenerator.AllSquares.Select(x => x.gameObject.transform).ToArray();
    }
    private void Update()
    {
        if (gameStatus.MoveAllowed)
        {
            var closestSquare = GetClosestSquare(squareTransforms);
            if (closestSquare != currentSquareTransform)
            {
                currentSquareTransform = closestSquare;
            }
        }

    }

    private Transform GetClosestSquare(Transform[] squares)
    {
        Transform tMin = null;
        float minDist = Mathf.Infinity;
        player = gameStatus.CurrentPlayer.transform.GetChild(2);
        Vector3 currentPos = player.position;
        foreach (Transform t in squares)
        {
            float dist = Vector3.Distance(t.position, currentPos);
            if (dist < minDist)
            {
                tMin = t;
                minDist = dist;
            }
        }
        return tMin;
    }

}
