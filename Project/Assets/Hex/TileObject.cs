using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileObject : MonoBehaviour
{
    public BoardManager bM;

    public Vector2Int tileID;
    public bool[] playerHolding; //[emptyTile, player1Held, player2Held]
    public int playerHoldingInt;
    public List<Vector2Int> adjacentTiles = new List<Vector2Int>();
    public List<Vector2Int> sameAdjacent = new List<Vector2Int>();
    public List<TileObject> adjacentGO = new List<TileObject>();


    public List<TileObject> vein = new List<TileObject>();
    public List<int> veinConditions = new List<int>();

    public int lastPlayer;
    private int previousPlayer;
    public List<bool> winCondition = new List<bool>();

    // Start is called before the first frame update
    void Start()
    {
        winCondition = new List<bool>() { false, false, false, false }; //Top, Bottom, Left, Right

        playerHolding = new bool[3];
        playerHolding[0] = true; //Set emptyTile to true

        if (tileID.y == bM.staticSize.y - 1)
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
        if (tileID.x == bM.staticSize.x - 1) //10 = 10
        {
            winCondition[3] = true; //Right
        }

        vein.Add(this);

    }

    // Update is called once per frame
    void Update()
    {
        //CheckForPlayerChange(); //And call for update if nesseccary

        GetAdjacent(tileID.x, tileID.y);

        if (playerHolding[0] == true) //Empty
        {
            playerHoldingInt = 0;
            gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1);
            gameObject.tag = "Tile";
            //gameObject.GetComponent<SpriteRenderer>().color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
        }
        if (playerHolding[1] == true) //Player1
        {
            playerHoldingInt = 1;
            gameObject.layer = 10;

            if(veinConditions.Contains(0) || veinConditions.Contains(1))
            {
                gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 0, 0);
            }
            else
            {
                gameObject.GetComponent<SpriteRenderer>().color = new Color(0.5f, 0, 0);
            }

            bM.invalidMoves.Add(tileID);

        }
        if (playerHolding[2] == true) //Player2
        {
            playerHoldingInt = 2;
            gameObject.layer = 11;

            if (veinConditions.Contains(2) || veinConditions.Contains(3))
            {
                gameObject.GetComponent<SpriteRenderer>().color = new Color(0, 0, 1);
            }
            else
            {
                gameObject.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0.5f);
            }

            bM.invalidMoves.Add(tileID);
        }
        CheckForWin();
        CheckForPlayerChange();
    }

    void CheckForPlayerChange()
    {
        //Determine who went last
        if (bM.gridTiles[bM.iM.latestMove.x, bM.iM.latestMove.y].GetComponent<TileObject>().playerHolding[1] == true) //Player1
        {
            lastPlayer = 1;
        }
        if (bM.gridTiles[bM.iM.latestMove.x, bM.iM.latestMove.y].GetComponent<TileObject>().playerHolding[2] == true) //Player2
        {
            lastPlayer = 2;
        }

        if (previousPlayer != lastPlayer) //Check when new player
        {
            GetAdjacent(tileID.x, tileID.y);
        }
        previousPlayer = lastPlayer;
    }

    void CheckForWin()
    {
        //Debug.Log("Called," + fromTo);
        //From tile, get adjacent tiles, fill until reach other end then WIN, else, lose

        //TileObject AdjacentGO

        if (playerHolding[1]) //Player 1 wants top[0] and bottom[1]
        {
            if (veinConditions.Contains(0) && veinConditions.Contains(1))
            {
                bM.bA.win = new Vector2Int(1, 1);
            }
        }
        if (playerHolding[2])//Player 2 wants left[2] and right[3]
        {
            if (veinConditions.Contains(2) && veinConditions.Contains(3))
            {
                bM.bA.win = new Vector2Int(1, 2);
            }
        }

    }

    void UpdateVeinConditions()
    {
        foreach(TileObject tile in vein)
        {
            if (tile.winCondition[0]) //check for win condition
            {
                if (!veinConditions.Contains(0)) //if not already in vein list
                {
                    veinConditions.Add(0);
                }
            }
            if (tile.winCondition[1]) //check for win condition
            {
                if (!veinConditions.Contains(1)) //if not already in vein list
                {
                    veinConditions.Add(1);
                }
            }
            if (tile.winCondition[2]) //check for win condition
            {
                if (!veinConditions.Contains(2)) //if not already in vein list
                {
                    veinConditions.Add(2);
                }
            }
            if (tile.winCondition[3]) //check for win condition
            {
                if (!veinConditions.Contains(3)) //if not already in vein list
                {
                    veinConditions.Add(3);
                }
            }
        }
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
            if(adjacentTiles[i].x < 0 || adjacentTiles[i].y < 0 || adjacentTiles[i].x > bM.staticSize.x -1 || adjacentTiles[i].y > bM.staticSize.y - 1) //Invalid if negative
            {
                
            }
            else
            {
                if (bM.gridTiles[adjacentTiles[i].x, adjacentTiles[i].y].GetComponent<TileObject>().playerHolding[1] == playerHolding[1] &&
                    bM.gridTiles[adjacentTiles[i].x, adjacentTiles[i].y].GetComponent<TileObject>().playerHolding[2] == playerHolding[2]
                    ) //adjacent tile is of same player
                {
                    if(!sameAdjacent.Contains(new Vector2Int(adjacentTiles[i].x, adjacentTiles[i].y)))
                    {
                        sameAdjacent.Add(new Vector2Int(adjacentTiles[i].x, adjacentTiles[i].y));

                    }
                }
            }

        }

        GameObjectAdjacent();
    }

    public void GameObjectAdjacent()
    {
        foreach(Vector2Int id in sameAdjacent)
        {
            if(!adjacentGO.Contains(bM.gridTiles[id.x, id.y].GetComponent<TileObject>()))
            {
                adjacentGO.Add(bM.gridTiles[id.x, id.y].GetComponent<TileObject>());
            }
            //Debug.Log(id);
        }

        BuildVein(tileID);

    }


    private void BuildVein(Vector2Int fromTo)
    {
        TileObject tile = bM.gridTiles[fromTo.x, fromTo.y].GetComponent<TileObject>();

        //Debug.Log("else");
        foreach (TileObject adjacentTile in tile.adjacentGO)
        {
            if (!vein.Contains(adjacentTile)) //If not already in list
            {
                adjacentTile.vein.Add(this);
                vein.Add(adjacentTile); //Add to vein
                BuildVein(adjacentTile.tileID); //loop for new adjacent tiles
            }

            foreach (TileObject veinTile in adjacentTile.vein) //get adjacent tile's vein and add to your own
            {
                if (!vein.Contains(veinTile))
                {
                    vein.Add(veinTile);
                }
            }
        }

        UpdateVeinConditions();


    }

}
