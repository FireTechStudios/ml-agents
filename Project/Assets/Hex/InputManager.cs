using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public BoardManager bM;

    public bool[] bots = new bool[2] {true, true}; //Both unchecked, both bots
    public Vector2Int[] playerChoice = new Vector2Int[2] {Vector2Int.zero, Vector2Int.zero};

    public Vector2Int aiMove = new Vector2Int(-1,-1);

    public bool player2Turn;

    public Vector2Int latestMove;

    public List<Vector2Int> player1Tiles;
    public List<Vector2Int> player2Tiles;

    public List<Vector2Int>[] playerTiles = new List<Vector2Int>[3] {null, null, null};

    Ray ray;
    RaycastHit hit;

    // Start is called before the first frame update
    void Start()
    {
        // Initialise ray
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetMouseButtonDown(0) && !player2Turn && bots[0] == false)//Player 1 human
        {
            SelectTile(rayCastClick());
        }
        if (Input.GetMouseButtonDown(0) && player2Turn && bots[1] == false)//Player 2 human
        {
            SelectTile(rayCastClick());
        }

        if (!player2Turn && bots[0] == true && aiMove != new Vector2Int(-1, -1))//Player 1 random
        {
            SelectTile(aiMove);
            aiMove = new Vector2Int(-1, -1);
        }
        if (player2Turn && bots[1] == true && aiMove != new Vector2Int(-1, -1))//Player 2 random
        {
            SelectTile(aiMove);
            aiMove = new Vector2Int(-1, -1);
        }
        if(false) //random bots
        {
            if (!player2Turn && bots[0] == true)//Player 1 random
            {
                int x = UnityEngine.Random.Range(0, bM.staticSize.x);
                int y = UnityEngine.Random.Range(0, bM.staticSize.y);

                Vector2Int randomChoice = new Vector2Int(x, y);

                SelectTile(randomChoice);
            }
            if (player2Turn && bots[1] == true)//Player 2 random
            {
                int x = UnityEngine.Random.Range(0, bM.staticSize.x);
                int y = UnityEngine.Random.Range(0, bM.staticSize.y);

                Vector2Int randomChoice = new Vector2Int(x, y);

                SelectTile(randomChoice);
            }
        }
    }

    public Vector2Int rayCastClick()
    {
        //Debug.Log("click");

        Vector2Int clicked = new Vector2Int(-1,-1);

        // Reset ray with new mouse position
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray);
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.gameObject.tag == "Tile")
            {
                clicked = hit.collider.gameObject.GetComponent<TileObject>().tileID;
                //Debug.Log("hit" + clicked);
            }
        }

        //Debug.Log("end" + clicked);

        return clicked;
    }

    void SelectTile(Vector2Int tileId)
    {
        
        bool validChoice = false;

        foreach (GameObject x in bM.gridTiles) //If value chosen is within range
        {
            if (x.GetComponent<TileObject>().tileID == tileId)
            {
                validChoice = true;
            }
        }

        bool[] holding = bM.gridTiles[tileId.x, tileId.y].GetComponent<TileObject>().playerHolding;

        try
        {
            if (holding[0] == true && validChoice && !bM.bA.win) //empty tile, continue
            {
                TileObject tile = bM.gridTiles[tileId.x, tileId.y].GetComponent<TileObject>();
                Array.Clear(tile.playerHolding, 0, tile.playerHolding.Length); //Set affiliation to false

                latestMove = tile.tileID;

                Vector2Int id = new Vector2Int(tileId.x, tileId.y);

                if (player2Turn) //Player 2's turn
                {
                    player2Turn = false;
                    tile.playerHolding[2] = true;
                    player2Tiles.Add(new Vector2Int(id.x, id.y));
                    bM.invalidMoves.Add(new Vector2Int(id.x, id.y));

                }
                else
                {
                    player2Turn = true;
                    tile.playerHolding[1] = true;
                    player1Tiles.Add(new Vector2Int(id.x, id.y));
                    bM.invalidMoves.Add(new Vector2Int(id.x, id.y));

                }

                playerTiles[1] = player1Tiles;
                playerTiles[2] = player2Tiles;
            }
        }
        catch
        {
            //invalid selection
        }

    }
}
