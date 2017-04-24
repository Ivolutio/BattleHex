using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour {

    public Vector2 coords;
    public List<Tile> neighbours;
    public TileType tileType;
    public bool occupied;
    public Pawn pawn;
    public Chest chest;

    public void SetColor(Color color)
    {
        transform.GetChild(0).GetComponent<Renderer>().materials[1].color = color;
    }
}