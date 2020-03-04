using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public bool humanPlayer;
    public bool demoRandom;

    public static bool player2Turn;

    public static Vector2Int latestMove;

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
        if (Input.GetMouseButtonDown(0) && humanPlayer)// && !player2Turn)
        {
            // Reset ray with new mouse position
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hits = Physics.RaycastAll(ray);
            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.gameObject.tag == "Tile")
                {
                    bool[] holding = hit.collider.gameObject.GetComponent<TileObject>().playerHolding;

                    if (holding[0] == true) //empty
                    {
                        SelectTile(hit.collider.gameObject.GetComponent<TileObject>().tileID);
                    }
                }
            }

            if (demoRandom && player2Turn)
            {
                StartCoroutine(randomChoice());
            }

        }

        //if (Input.GetKeyDown(KeyCode.Q))
        //{
        //    SelectTile(new Vector2Int(temp, 0));
        //}
    }

    public IEnumerator randomChoice()
    {
        Vector2Int random = new Vector2Int(UnityEngine.Random.Range(0, BoardManager.staticSize.x), UnityEngine.Random.Range(0, BoardManager.staticSize.y));

        if(BoardManager.gridTiles[random.x, random.y].GetComponent<TileObject>().playerHolding[0] == false)
        {
            StartCoroutine(randomChoice());
        }
        else //Empty tile
        {
            yield return new WaitForSeconds(0.25f);
            SelectTile(new Vector2Int(random.x, random.y));
        }

    }

    void SelectTile(Vector2Int tileId)
    {
        TileObject tile = BoardManager.gridTiles[tileId.x, tileId.y].GetComponent<TileObject>();
        Array.Clear(tile.playerHolding, 0, tile.playerHolding.Length); //Set affiliation to false

        latestMove = tile.tileID;

        if (player2Turn) //Player 2's turn
        {
            player2Turn = false;
            tile.playerHolding[2] = true;
        }
        else
        {
            player2Turn = true;
            tile.playerHolding[1] = true;
        }
    }
}
