using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public bool player1;
    public World World;
    public GameObject PawnPrefab;
    List<Tile> startingTiles;
    public int placedPawns;
    public bool placingPawns;
    public List<Pawn> pawns;
    List<Tile> possibleSteps;

    public void Setup(List<Tile> startingTiles)
    {
        this.startingTiles = startingTiles;
        pawns = new List<Pawn>();
        placedPawns = 0;
        possibleSteps = new List<Tile>();
        selectedPawn = null;
    }

    void Update()
    {
        if (World.player != player1 || Camera.main == null)
            return;

        if (placingPawns)
        {
            PlacingPawns();
            return;
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo))
        {
            Tile hit = hitInfo.collider.transform.parent.GetComponent<Tile>();
            if (Input.GetMouseButtonDown(0))
            {
                if (hit.pawn != null)
                {
                    if (hit.pawn != selectedPawn && hit.pawn.player1 == player1)
                    {
                        PawnSelect(selectedPawn, false);
                        PawnSelect(hit.pawn, true);
                        //Debug.Log("Selected pawn: " + selectedPawn);
                    }
                    else if (hit.pawn == selectedPawn)
                        PawnSelect(hit.pawn, false);
                    

                }
                if (possibleSteps.Contains(hit))
                {
                    if (hit.pawn != null)
                    {
                        if (hit.pawn != selectedPawn && hit.pawn.player1 != player1)
                        {
                            foreach (Tile tile in possibleSteps)
                                tile.SetColor(tile.tileType.defaultColor);
                            World.RemovePawn(hit.pawn, player1);
                            World.MovePawn(selectedPawn, hit, true);
                            PawnSelect(selectedPawn, false);
                        }
                    }
                    else if (hit.chest != null)
                    {
                        foreach (Tile tile in possibleSteps)
                            tile.SetColor(tile.tileType.defaultColor);
                        World.OpenChest(hit.chest, selectedPawn, player1);
                        PawnSelect(selectedPawn, false);
                    }
                    else
                    {
                        World.MovePawn(selectedPawn, hit, player1);
                        PawnSelect(selectedPawn, false);
                    }
                }
            }
            else if (Input.GetMouseButtonDown(1))
            {
                if (hit.pawn != null)
                {
                    if (hit.pawn == selectedPawn)
                        PawnSelect(hit.pawn, false);
                }
            }
        }
    }

    Pawn selectedPawn;
    void PawnSelect(Pawn pawn, bool selectedState)
    {
        if (pawn == null)
            return;
        if (selectedState) //select
        {
            possibleSteps = new List<Tile>();
            if (pawn.currentTile.tileType.steps == 1)
            {
                possibleSteps = PossibleTiles(pawn.currentTile, true);
                if (player1)
                {
                    if (World.map.ContainsKey(new Vector2(pawn.currentTile.coords.x + 1, pawn.currentTile.coords.y - 2)))
                    {
                        Tile backTile = World.map[new Vector2(pawn.currentTile.coords.x + 1, pawn.currentTile.coords.y - 2)];
                        if (backTile != null)
                            if (backTile.tileType.walkable && !backTile.occupied)
                                possibleSteps.Add(backTile);
                    }
                }
                else
                {
                    if (World.map.ContainsKey(new Vector2(pawn.currentTile.coords.x - 1, pawn.currentTile.coords.y + 2)))
                    {
                        Tile backTile = World.map[new Vector2(pawn.currentTile.coords.x - 1, pawn.currentTile.coords.y + 2)];
                        if (backTile != null)
                            if (backTile.tileType.walkable && !backTile.occupied)
                                possibleSteps.Add(backTile);
                    }
                }
            }
            else if (pawn.currentTile.tileType.steps == 2)
                possibleSteps = PossibleTilesDouble(pawn.currentTile);
            foreach (Tile tile in possibleSteps)
                tile.SetColor(tile.tileType.hoverColor);

            selectedPawn = pawn;
        }
        else //unselect
        {
            foreach (Tile tile in possibleSteps)
                tile.SetColor(tile.tileType.defaultColor);
            selectedPawn = null;
            possibleSteps = new List<Tile>();
        }
    }

    Tile prevHover;
    void PlacingPawns()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo))
        {
            Tile hit = hitInfo.collider.transform.parent.GetComponent<Tile>();
            if (startingTiles.Contains(hit) && hit.occupied != true && placedPawns < World.maxPawns)
            {
                if (hit != prevHover)
                {
                    if (prevHover != null)
                        prevHover.SetColor(prevHover.tileType.defaultColor);
                    hit.SetColor(hit.tileType.hoverColor);
                    prevHover = hit;
                }
                if (Input.GetMouseButtonDown(0))
                {
                    placedPawns += 1;
                    Pawn pawn = World.PlacePawn(hit.coords, PawnPrefab);
                    pawn.player1 = player1;
                    pawns.Add(pawn);
                }
            }
            else if (prevHover != null)
            {
                prevHover.SetColor(prevHover.tileType.defaultColor);
                prevHover = null;
            }
            if (Input.GetMouseButtonDown(1))
            {
                if (hit.occupied && hit.pawn != null)
                {
                    Destroy(hit.pawn.gameObject);
                    hit.pawn = null;
                    hit.occupied = false;
                    placedPawns -= 1;
                }
            }
        }
    }

    List<Tile> PossibleTiles(Tile start, bool specialAllowed)
    {
        List<Tile> tiles = new List<Tile>();

        foreach (Tile tile in start.neighbours)
        {
            if (player1)
            {
                if (tile.coords.y >= start.coords.y && tile.tileType.walkable && !tile.occupied)
                    tiles.Add(tile);
                else if (tile.occupied && tile.coords.y < start.coords.y && specialAllowed)
                {
                    if (tile.pawn != null)
                        if (tile.pawn.player1 != player1)
                            tiles.Add(tile);
                    if (tile.chest != null)
                        tiles.Add(tile);
                }
            }
            else
            {
                if (tile.coords.y <= start.coords.y && tile.tileType.walkable && !tile.occupied)
                    tiles.Add(tile);
                else if (tile.occupied && tile.coords.y > start.coords.y && specialAllowed)
                {
                    if (tile.pawn != null)
                        if (tile.pawn.player1 != player1)
                            tiles.Add(tile);
                    if (tile.chest != null)
                        tiles.Add(tile);
                }
            }

        }
        return tiles;
    }

    List<Tile> PossibleTilesDouble(Tile start)
    {
        List<Tile> tiles = new List<Tile>();
        foreach (Tile tile in PossibleTiles(start, true))
        {
            if(!tiles.Contains(tile))
                tiles.Add(tile);
            foreach (Tile t in PossibleTiles(tile, false))
                if (!tiles.Contains(t))
                    tiles.Add(t);
        }
        if (player1)
        {
            if (World.map.ContainsKey(new Vector2(start.coords.x + 2, start.coords.y - 4)))
            {
                Tile backTile = World.map[new Vector2(start.coords.x + 2, start.coords.y - 4)];
                if (backTile != null)
                    if (backTile.tileType.walkable && !backTile.occupied)
                        tiles.Add(backTile);
            }
        }
        else
        {
            if (World.map.ContainsKey(new Vector2(start.coords.x - 2, start.coords.y + 4)))
            {
                Tile backTile = World.map[new Vector2(start.coords.x - 2, start.coords.y + 4)];
                if (backTile != null)
                    if (backTile.tileType.walkable && !backTile.occupied)
                        tiles.Add(backTile);
            }
        }
        return tiles;
    }
}
