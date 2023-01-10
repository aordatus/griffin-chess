using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SquareBase : MonoBehaviour
{
    public Vector2Int BoardPosition { get; set; }
    public Team WithTeam { get; set; }
    public PieceBase OccupiedPiece { get; set; } = null;
    public bool PossibleMove { get; set; }
    public bool AvailableMove { get; set; }
    public void Init(int x, int y, float offset, Team t, Material m)
    {
        WithTeam = new Team();
        this.transform.localPosition = new Vector3(offset + x, 0, offset + y);
        this.BoardPosition = new Vector2Int(x, y);
        this.WithTeam = t;
        this.name = $"X:{x} & Y:{y}";
        this.GetComponent<Renderer>().material = m;
    }
}