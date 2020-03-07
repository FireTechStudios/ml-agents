using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;
using MLAgents.Sensors;
using MLAgents.SideChannels;

public class InputManager : MonoBehaviour
{
    public BoardManager bM;

    public bool[] agent = new bool[2] {true, true}; //Both unchecked, both bots
    public bool playerTrueBotFalse = false;
    public Vector2Int[] playerChoice = new Vector2Int[2] {Vector2Int.zero, Vector2Int.zero};

    public Vector2Int aiMove = new Vector2Int(-1,-1);

    public bool player2Turn;

    public Vector2Int latestMove;

    public List<Vector2Int> player1Tiles;
    public List<Vector2Int> player2Tiles;

    public List<Vector2Int>[] playerTiles = new List<Vector2Int>[3] {null, null, null};

    public bool inputAllowed;

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
        if (inputAllowed)
        {
            Academy.Instance.EnvironmentStep();
            if (!player2Turn) //P1 turn
            {
                if (agent[0]) //agent as P1
                {
                    SelectTile(aiMove);
                    aiMove = new Vector2Int(-1, -1);
                }
                else if(playerTrueBotFalse) //player
                {
                    if (Input.GetMouseButtonDown(0))//Player 1 human
                    {
                        SelectTile(rayCastClick()); //player against agent
                    }
                }
                else //random
                {
                    SelectTile(RandomVector());
                }
            }
            else //P2 turn
            {
                if (agent[1]) //agent as P1
                {
                    SelectTile(aiMove);
                    aiMove = new Vector2Int(-1, -1);
                }
                else if (playerTrueBotFalse) //player
                {
                    if (Input.GetMouseButtonDown(0))//Player 1 human
                    {
                        SelectTile(rayCastClick()); //player against agent
                    }
                }
                else //random
                {
                    SelectTile(RandomVector());
                }
            }
        }
    }
    
    public Vector2Int RandomVector()
    {
        int x = bM.validMoves[UnityEngine.Random.Range(0, bM.validMoves.Count)].x;
        int y = bM.validMoves[UnityEngine.Random.Range(0, bM.validMoves.Count)].y;

        Vector2Int randomChoice = new Vector2Int(x, y);

        return(randomChoice);
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
        inputAllowed = false;

        try
        {
            if (bM.validMoves.Contains(tileId) == true) //empty tile, continue
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

                    bM.validMoves.Remove(new Vector2Int(id.x, id.y));

                }
                else
                {
                    player2Turn = true;
                    tile.playerHolding[1] = true;
                    player1Tiles.Add(new Vector2Int(id.x, id.y));

                    bM.validMoves.Remove(new Vector2Int(id.x, id.y));

                }
                playerTiles[1] = player1Tiles;
                playerTiles[2] = player2Tiles;

            }

        }
        catch
        {
            //invalid selection
        }

        StartCoroutine(RenableInput());

    }

    public IEnumerator RenableInput()
    {
        yield return new WaitForSeconds(0);
        inputAllowed = true;
    }
}
