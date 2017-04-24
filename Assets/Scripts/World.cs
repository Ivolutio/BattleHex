using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour {
    [Header("References")]
    public GameObject TilePrefab;
    public GameObject ChestPrefabTNT;
    public GameObject ChestPrefabGOLD;
    public Animation cam;
    public AudioPlayer audio;
    public GameUI gameUI;
    Player Player1;
    Player Player2;
    [Space]
    [Header("Game Options")]
    public int mapRadius = 10;
    public int maxPawns = 3;
    public int chestAmount = 3;
    public float playerSpeed = .3f;

    [Space]
    [Header("Hex stuff")]
    public float hexHeight = 2.00f;
    private float hexWidth;

    [Space]
    [Header("Do not touch")]
    public bool player;
    [Header("Maps etc")]
    public Dictionary<Vector2, Tile> map;
    Dictionary<string, TileType> tileTypes;
    List<Tile> startingTiles1;
    List<Tile> startingTiles2;

    void Start()
    {
        hexWidth = Mathf.Sqrt(3) / 2 * hexHeight;
        CreateTileTypes();
        Generate();
    }

    public void StartGame ()
    {        Player1 = GameObject.Find("Player1").GetComponent<Player>();
        Player2 = GameObject.Find("Player2").GetComponent<Player>();        
        Player1.Setup(startingTiles1);
        Player2.Setup(startingTiles2);

        Player1.placingPawns = true;
        player = true;
    }
    void CreateTileTypes()
    {
        tileTypes = new Dictionary<string, TileType>();
        tileTypes.Add("Grass", new TileType("Grass", true, 2, new Color(0.010096f, 0.538228f, 0.000000f, 1.000000f), new Color(0.007798f, 0.367237f, 0.000000f, 1.000000f)));
        tileTypes.Add("Desert", new TileType("Desert", true, 1, new Color(0.800006f, 0.718946f, 0.331958f, 1.000000f), new Color(0.517333f, 0.465529f, 0.217431f, 1.000000f)));
        tileTypes.Add("Ocean", new TileType("Ocean", false, 0, new Color(0.051384f, 0.167926f, 0.476990f, 1.000000f), new Color(0.028944f, 0.089447f, 0.246194f, 1.000000f)));
    }

    void Generate()
    {
        float seed = Random.Range(0, 500);
        map = new Dictionary<Vector2, Tile>();
        startingTiles1 = new List<Tile>();
        startingTiles2 = new List<Tile>();
        for (int q = -mapRadius; q <= mapRadius; q++)
        {
            int r1 = Mathf.Max(-mapRadius, -q - mapRadius);
            int r2 = Mathf.Min(mapRadius, -q + mapRadius);
            for (int r = r1; r <= r2; r++)
            {
                Vector3 realPos = new Vector3();
                realPos.x = q * hexWidth + 0.866026f * r;
                realPos.z = r * (hexHeight - (.25f * hexHeight));
                GameObject tileObj = Instantiate(TilePrefab, realPos, Quaternion.identity, transform);
                tileObj.name = "Tile (" + q + ", " + r + ")";
                Tile tile = tileObj.GetComponent<Tile>();
                tile.coords = new Vector2(q, r);
                float noise = Noise(q, r, seed);
                if (noise >= 90)
                    tile.tileType = tileTypes["Desert"];
                else if (noise >= 50)
                    tile.tileType = tileTypes["Grass"];
                else
                    tile.tileType = tileTypes["Ocean"];
                tile.SetColor(tile.tileType.defaultColor);
                map.Add(new Vector2(q, r), tile);
                if ((q >= 0 && q <= mapRadius) && r == -mapRadius)
                    if (tile.tileType.walkable)
                        startingTiles1.Add(tile);
                if ((q <= 0 && q >= -mapRadius) && r == mapRadius)
                    if (tile.tileType.walkable)
                        startingTiles2.Add(tile);
            }
        }
        // Find neighbours
        foreach (Tile tile in map.Values)
        {
            List<Tile> neighbours = new List<Tile>();
            //  0, -1
            if (map.ContainsKey(new Vector2(tile.coords.x, tile.coords.y - 1)))
                neighbours.Add(map[new Vector2(tile.coords.x, tile.coords.y - 1)]);
            // +1, -1
            if (map.ContainsKey(new Vector2(tile.coords.x + 1, tile.coords.y - 1)))
                neighbours.Add(map[new Vector2(tile.coords.x + 1, tile.coords.y - 1)]);
            // -1,  0
            if (map.ContainsKey(new Vector2(tile.coords.x - 1, tile.coords.y)))
                neighbours.Add(map[new Vector2(tile.coords.x - 1, tile.coords.y)]);
            // +1,  0
            if (map.ContainsKey(new Vector2(tile.coords.x + 1, tile.coords.y)))
                neighbours.Add(map[new Vector2(tile.coords.x + 1, tile.coords.y)]);
            // -1, +1
            if (map.ContainsKey(new Vector2(tile.coords.x - 1, tile.coords.y + 1)))
                neighbours.Add(map[new Vector2(tile.coords.x - 1, tile.coords.y + 1)]);
            //  0, +1
            if (map.ContainsKey(new Vector2(tile.coords.x, tile.coords.y + 1)))
                neighbours.Add(map[new Vector2(tile.coords.x, tile.coords.y + 1)]);
            tile.neighbours = neighbours;
        }
        for (int i = 0; i < chestAmount; i++)
        {
            Tile chestTile = map[new Vector2(Random.Range(-9, 9), Random.Range(-1, 1))];
            while (chestTile.occupied || !chestTile.tileType.walkable)
                chestTile = map[new Vector2(Random.Range(-9, 9), Random.Range(-1, 1))];
            if (i == 0)
                PlaceChest(chestTile.coords, ChestPrefabGOLD, true);
            else
                PlaceChest(chestTile.coords, ChestPrefabTNT, false);
        }
    }
    float Noise(float x, float y, float seed)
    {
        x += seed;
        y += seed;
        float nx = x / mapRadius - 0.5f;
        float ny = y / mapRadius - 0.5f;
        float noise = 1 * Mathf.PerlinNoise(1 * nx, 1 * ny) + 0.5f * Mathf.PerlinNoise(2 * nx, 2 * ny) + 0.25f * Mathf.PerlinNoise(2 * nx, 2 * ny);
        return noise * 100;
    }

    public Pawn PlacePawn(Vector2 tileCoords, GameObject prefab)
    {
        Tile tile = map[tileCoords];
        if (tile.occupied || tile.pawn != null || !tile.tileType.walkable)
            return null;
        GameObject pawn = Instantiate(prefab, tile.transform.position, Quaternion.identity, transform);
        pawn.name = pawn.name.Replace("(Clone)", "");
        tile.pawn = pawn.GetComponent<Pawn>();
        tile.occupied = true;
        pawn.GetComponent<Pawn>().currentTile = tile;
        return pawn.GetComponent<Pawn>();
    }
    public Chest PlaceChest(Vector2 tileCoords, GameObject prefab, bool good)
    {
        Tile tile = map[tileCoords];
        if (tile.occupied || tile.pawn != null || !tile.tileType.walkable)
            return null;
        GameObject chest = Instantiate(prefab, tile.transform.position, Quaternion.identity, transform);
        if(good)
            chest.name = chest.name.Replace("Clone", "Good");
        else
            chest.name = chest.name.Replace("Clone", "Bad");
        chest.GetComponent<Chest>().good = good;
        tile.chest = chest.GetComponent<Chest>();
        tile.occupied = true;
        chest.GetComponent<Chest>().currentTile = tile;
        return chest.GetComponent<Chest>();
    }
    public void RemovePawn(Pawn pawn, bool player1)
    {
        pawn.currentTile.pawn = null;
        pawn.currentTile.occupied = false;
        
        if (player1)
        {
            Player2.placedPawns -= 1;
            Player2.pawns.Remove(pawn);
            if (Player2.placedPawns == 0)
            {
                audio.Win();
                gameUI.WinP1();
            }
        }
        else
        {
            Player1.placedPawns -= 1;
            Player1.pawns.Remove(pawn);
            if(Player1.placedPawns == 0)
            {
                audio.Win();
                gameUI.WinP2();
            }
        }
        Destroy(pawn.gameObject);
        audio.Pawn();
    }
    public void OpenChest(Chest chest, Pawn pawn, bool player1)
    {
        audio.ChestOpen();
        StartCoroutine(chest.StartCam(pawn, player1));
    }

    bool moving;
    Pawn movingPawn;
    Tile moveDest;
    public void MovePawn(Pawn pawn, Tile destination, bool player)
    {
        this.movingPawn = pawn;
        this.moveDest = destination;
        this.moving = true;
        pawn.currentTile.pawn = null;
        pawn.currentTile.occupied = false;
        pawn.currentTile = destination;
        destination.pawn = pawn;
        destination.occupied = true;
    }    
    void Update()
    {
        if (moving && movingPawn != null && moveDest != null)
        {
            movingPawn.transform.position = Vector3.MoveTowards(movingPawn.transform.position, moveDest.transform.position, playerSpeed);
            if (Vector3.Distance(movingPawn.transform.position, moveDest.transform.position) == 0)
            {
                StartCoroutine(DoneMoving());
            }
        }
    }

    void SwitchPlayer()
    {        if (player)        {            cam["PlayerSwap"].time = 0;            cam["PlayerSwap"].speed = 1;            cam.Play("PlayerSwap");        }        else        {            cam["PlayerSwap"].time = cam["PlayerSwap"].length;            cam["PlayerSwap"].speed = -1;            cam.Play("PlayerSwap");        }        player = !player;    }

    public void RerollBoardButton()    {        for(int i = 0; i < transform.childCount; i++)        {            Destroy(transform.GetChild(i).gameObject);        }        Start();    }

    public void DonePlacingButton()    {        if (player)        {            if (Player1.placedPawns < maxPawns == false)            {                Player1.placingPawns = false;                Player2.placingPawns = true;                player = false;                cam["PlayerSwap"].time = 0;                cam["PlayerSwap"].speed = 1;                cam.Play("PlayerSwap");            }            else                Debug.LogError("You haven't placed three pawns yet!");        }        else        {            if (Player2.placedPawns < maxPawns == false)            {                Destroy(GameObject.Find("DonePlacingButton"));                Player1.placingPawns = false;                Player2.placingPawns = false;                player = true;                cam["PlayerSwap"].time = cam["PlayerSwap"].length;                cam["PlayerSwap"].speed = -1;                cam.Play("PlayerSwap");            }            else                Debug.LogError("You haven't placed three pawns yet!");        }    }

    IEnumerator DoneMoving()    {        moving = false;        yield return new WaitForSeconds(.1f);        SwitchPlayer();    }

    public void Win(bool player1)    {        audio.Win();        if (player1)            gameUI.WinP1();        else            gameUI.WinP2();    }
}
