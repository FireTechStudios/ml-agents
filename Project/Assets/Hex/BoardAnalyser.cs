using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardAnalyser : MonoBehaviour
{
    public BoardManager bM;

    public int lastPlayer;
    private int previousPlayer;
    public bool win;
    public Vector2Int winMet = new Vector2Int(0, 0);
    private List<Vector2Int> visited;
    public List<Vector2Int> toCheck;

    private List<Vector2Int> playerHeld;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
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

        if (lastPlayer == 1)
        {
            playerHeld = bM.iM.playerTiles[1];
        }
        if (lastPlayer == 2)
        {
            playerHeld = bM.iM.playerTiles[2];
        }

        if (previousPlayer != lastPlayer) //Clear if new player
        {

            //if (BoardManager.gridTiles[InputManager.latestMove.x, InputManager.latestMove.y].GetComponent<TileObject>().winCondition[0] == true && lastPlayer == 1 ||
            //    BoardManager.gridTiles[InputManager.latestMove.x, InputManager.latestMove.y].GetComponent<TileObject>().winCondition[1] == true && lastPlayer == 1 ||
            //    BoardManager.gridTiles[InputManager.latestMove.x, InputManager.latestMove.y].GetComponent<TileObject>().winCondition[2] == true && lastPlayer == 2 ||
            //    BoardManager.gridTiles[InputManager.latestMove.x, InputManager.latestMove.y].GetComponent<TileObject>().winCondition[3] == true && lastPlayer == 2
            //    )
            //{
            //    Debug.Log("check");
            //    visited.Clear();
            //    CheckForWin(new Vector2Int(InputManager.latestMove.x, InputManager.latestMove.y), -1); //Latest played tile is a win condition,
            //
            //}

            visited = new List<Vector2Int>();
            toCheck = new List<Vector2Int>();


            BuildToCheckList(new Vector2Int(bM.iM.latestMove.x, bM.iM.latestMove.y));
            CheckForWin(); //check based on the current to check list vein
        }
        previousPlayer = lastPlayer;

        if(win)
        {
            visited = new List<Vector2Int>();
            toCheck = new List<Vector2Int>();

            if(lastPlayer == 1)
            {
                bM.player1Score += 1;
                //HexAgent.AddReward(-1); //Assume ai is 2nd player for now
            }
            else
            {
                bM.player2Score += 1;
                //HexAgent.AddReward(1);
            }
        }

    }

    void CheckForWin()
    {
        //Debug.Log("Called," + fromTo);
        //From tile, get adjacent tiles, fill until reach other end then WIN, else, lose

        //TileObject AdjacentGO

        Vector2Int winCond = new Vector2Int(0, 0);

        foreach(Vector3Int currentTile in toCheck)
        {
            TileObject tileData = bM.gridTiles[currentTile.x, currentTile.y].GetComponent<TileObject>();

            if(lastPlayer == 1) //Player 1 wants top[0] and bottom[1]
            {
                if (tileData.winCondition[0] == true)
                {
                    winCond.x = 1;
                }
                if (tileData.winCondition[1] == true)
                {
                    winCond.y = 1;
                }
            }
            if(lastPlayer == 2)//Player 2 wants left[2] and right[3]
            {
                if (tileData.winCondition[2] == true)
                {
                    winCond.x = 1;
                }
                if (tileData.winCondition[3] == true)
                {
                    winCond.y = 1;
                }
            }
        }

        if (winCond == new Vector2Int(1, 1))
        {
            win = true;
        }

    }

    private void BuildToCheckList(Vector2Int fromTo)
    {

        TileObject tile = bM.gridTiles[fromTo.x, fromTo.y].GetComponent<TileObject>();

        //Debug.Log("else");
        foreach (GameObject adjacentTile in tile.adjacentGO)
        {
            //Get ID of adjacent tile
            Vector2Int id = adjacentTile.GetComponent<TileObject>().tileID; //New tile

            if (!toCheck.Contains(id)) //Continue if not already visited
            {
                toCheck.Add(new Vector2Int(id.x, id.y)); //Latest played tile is a win condition,
                BuildToCheckList(new Vector2Int(id.x, id.y));
            }
            else
            {
                //Debug.Log("already checked" + id);
            }
        }
        visited.Add(new Vector2Int(fromTo.x, fromTo.y));
    }
}
