using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BoardManager : MonoBehaviour
{
    public InputManager iM;
    public BoardAnalyser bA;

    public List<Vector2Int> validMoves;
    public List<Vector2Int> invalidMoves;

    public Vector2Int size; //Determines the size of the board
    public Vector2Int staticSize;

    public bool[,][] playerHoldingTiles; //2D array with each array point being [x,x,x]


    public GameObject tilePrefab;
    public GameObject deadTile;


    private Transform pos;
    public GameObject[,] gridTiles;
    public Camera cam;

    public static bool lastGameFirstPlayer;

    public int player1Score;
    public int player2Score;
    public TextMeshPro counter;

    //activate ml-agents
    //cd C:\...\ml-agents
    //mlagents-learn config/trainer_config.yaml --run-id hex_01 --train


    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 10f;

        iM = GetComponent<InputManager>();
        bA = GetComponent<BoardAnalyser>();

        staticSize = size;

        playerHoldingTiles = new bool[size.x, size.y][];

        SpawnGrid(size.x, size.y);

        //Camera FOV & POS
        cam.fieldOfView = Mathf.Clamp(Mathf.Max(size.x, size.y) * 10f, 90, 120);
        cam.gameObject.transform.position = new Vector3(gridTiles[size.x / 2, size.y / 2].gameObject.transform.position.x, gridTiles[size.x / 2, size.y / 2].gameObject.transform.position.y, -10 - (1 + Mathf.Max(size.x, size.y) / 10f)); //Move camera to centre of board

    }

    void SpawnGrid(int x, int y)
    {
        gridTiles = new GameObject[x, y];

        for (int i = 0; i < x; i++) //For each X row
        {
            for (int j = 0; j < y; j++) //Create Y column
            {
                gridTiles[i, j] = Instantiate(tilePrefab, transform.position + new Vector3(2 * i * 0.4f, j * 0.7f, transform.position.z), Quaternion.identity); //Instantiate Tile

                gridTiles[i, j].gameObject.transform.position += new Vector3(j * -0.4f, 0, 0); //Shift Tile to form connected Hexagons

                gridTiles[i, j].GetComponent<TileObject>().tileID = new Vector2Int(i, j); //Assign GridID

                gridTiles[i, j].transform.parent = transform;

                gridTiles[i, j].GetComponent<TileObject>().bM = GetComponent<BoardManager>();

                validMoves.Add(new Vector2Int(i, j));
            }
        }

        //Spawn Colored Markers each side of grid

        float topX = (gridTiles[size.x - 1, size.y - 1].transform.position.x + gridTiles[0, size.y - 1].transform.position.x) / 2f;
        float bottomX = (gridTiles[size.x - 1, 0].transform.position.x + gridTiles[0, 0].transform.position.x) / 2f;

        float averageY = (gridTiles[0, size.y - 1].transform.position.y + gridTiles[0, 0].transform.position.y) / 2f;


        GameObject topIndicator = Instantiate(deadTile, new Vector3(topX, gridTiles[0, size.y - 1].transform.position.y + 1.5f, transform.position.z), Quaternion.identity); //Top
        GameObject bottomIndicator = Instantiate(deadTile, new Vector3(bottomX, gridTiles[0, 0].transform.position.y - 1.5f, transform.position.z), Quaternion.identity); //Bottom

        GameObject leftIndicator = Instantiate(deadTile, new Vector3(gridTiles[0, size.y - 1].transform.position.x - 0.5f, averageY - 1f, transform.position.z), Quaternion.identity); //Left
        GameObject rightIndicator = Instantiate(deadTile, new Vector3(gridTiles[size.x - 1, 0].transform.position.x + 0.5f, averageY + 1f, transform.position.z), Quaternion.identity); //Right

        topIndicator.GetComponent<SpriteRenderer>().color = new Color(1, 0, 0);
        bottomIndicator.GetComponent<SpriteRenderer>().color = new Color(1, 0, 0);

        leftIndicator.GetComponent<SpriteRenderer>().color = new Color(0, 0, 1);
        rightIndicator.GetComponent<SpriteRenderer>().color = new Color(0, 0, 1);

        topIndicator.transform.parent = transform;
        bottomIndicator.transform.parent = transform;

        leftIndicator.transform.parent = transform;
        rightIndicator.transform.parent = transform;
    }

    // Update is called once per frame
    void Update()
    {

        for (int i = 0; i < size.x; i++) //For each X row
        {
            for (int j = 0; j < size.y; j++) //Create Y column
            {
                playerHoldingTiles[i,j] = gridTiles[i, j].GetComponent<TileObject>().playerHolding;
            }
        }

        if (bA.win.x == 1)
        {
            foreach(Transform child in transform)
            {
                Destroy(child.gameObject);
            }

            if(lastGameFirstPlayer)
            {
                lastGameFirstPlayer = false;
            }
            else
            {
                lastGameFirstPlayer = true;
            }

            iM.player2Turn = lastGameFirstPlayer;

            validMoves.Clear();
            invalidMoves.Clear();

            bA.win = Vector2Int.zero;

            SpawnGrid(size.x, size.y);
        }

        counter.text = "<color=red>Player 1</color> Wins:\n<b>"+ player1Score.ToString() + "</b>\n\n<color=blue>Player 2</color> Wins:\n<b>"+ player2Score.ToString() + "</b>\n";

    }

}
