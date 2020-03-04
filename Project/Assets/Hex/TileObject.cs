using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileObject : MonoBehaviour
{
    public Vector2Int tileID;
    public bool[] playerHolding; //[emptyTile, player1Held, player2Held]
    public List<Vector2Int> adjacentTiles = new List<Vector2Int>();
    public List<Vector2Int> sameAdjacent = new List<Vector2Int>();
    public List<GameObject> adjacentGO = new List<GameObject>();
    public int lastPlayer;
    private int previousPlayer;
    public List<bool> winCondition = new List<bool>();

    // Start is called before the first frame update
    void Start()
    {
        winCondition = new List<bool>() { false, false, false, false }; //Top, Bottom, Left, Right

        playerHolding = new bool[3];
        playerHolding[0] = true; //Set emptyTile to true

        if (tileID.y == BoardManager.staticSize.y - 1)
        {
            winCondition[0] = true; //Top
        }
        if (tileID.y == 0)
        {
            winCondition[1] = true; //Bottom
        }
        if (tileID.x == 0)
        {
            winCondition[2] = true; //Left
        }
        if (tileID.x == BoardManager.staticSize.x - 1) //10 = 10
        {
            winCondition[3] = true; //Right
        }
    }

    // Update is called once per frame
    void Update()
    {
        CheckForPlayerChange(); //And call for update if nesseccary

        if (playerHolding[0] == true) //Empty
        {
            gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1);
            gameObject.tag = "Tile";
            //Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
        }
        if (playerHolding[1] == true) //Player1
        {
            gameObject.GetComponent<SpriteRenderer>().color = new Color(1,0,0);
            gameObject.layer = 10;
        }
        if (playerHolding[2] == true) //Player2
        {
            gameObject.GetComponent<SpriteRenderer>().color = new Color(0, 0, 1);
            gameObject.layer = 11;
        }

    }

    void CheckForPlayerChange()
    {
        //Determine who went last
        if (BoardManager.gridTiles[InputManager.latestMove.x, InputManager.latestMove.y].GetComponent<TileObject>().playerHolding[1] == true) //Player1
        {
            lastPlayer = 1;
        }
        if (BoardManager.gridTiles[InputManager.latestMove.x, InputManager.latestMove.y].GetComponent<TileObject>().playerHolding[2] == true) //Player2
        {
            lastPlayer = 2;
        }

        if (previousPlayer != lastPlayer) //Check when new player
        {
            GetAdjacent(tileID.x, tileID.y);
        }
        previousPlayer = lastPlayer;
    }


    void GetAdjacent(int x, int y)
    {
        adjacentTiles = new List<Vector2Int>
        { new Vector2Int(x, y + 1),
            new Vector2Int(x + 1, y + 1),
            new Vector2Int(x - 1, y),
            new Vector2Int(x + 1, y),
            new Vector2Int(x - 1, y - 1),
            new Vector2Int(x, y - 1) };

        if(playerHolding[0] == false)
        {
            GetSameAdjacent();
        }
    }

    public void GetSameAdjacent()
    {
        for (int i = 0; i < 6; i++)
        {
            if(adjacentTiles[i].x < 0 || adjacentTiles[i].y < 0 || adjacentTiles[i].x > BoardManager.staticSize.x -1 || adjacentTiles[i].y > BoardManager.staticSize.y - 1) //Invalid if negative
            {
                
            }
            else
            {
                if (BoardManager.gridTiles[adjacentTiles[i].x, adjacentTiles[i].y].GetComponent<TileObject>().playerHolding[1] == playerHolding[1] &&
                    BoardManager.gridTiles[adjacentTiles[i].x, adjacentTiles[i].y].GetComponent<TileObject>().playerHolding[2] == playerHolding[2]
                    ) //adjacent tile is of same player
                {
                    sameAdjacent.Add(new Vector2Int(adjacentTiles[i].x, adjacentTiles[i].y));
                }
            }

        }

        GameObjectAdjacent();
    }

    public void GameObjectAdjacent()
    {
        foreach(Vector2Int id in sameAdjacent)
        {
            if(!adjacentGO.Contains(BoardManager.gridTiles[id.x, id.y].gameObject))
            {
                adjacentGO.Add(BoardManager.gridTiles[id.x, id.y].gameObject);
            }
            //Debug.Log(id);
        }
    }

}
