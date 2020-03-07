using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardAnalyser : MonoBehaviour
{
    public BoardManager bM;

    public int lastPlayer;
    private int previousPlayer;
    public Vector2Int win = new Vector2Int(0,0); //win state, playernum
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

        }
        previousPlayer = lastPlayer;


        if (win.x == 1)
        {
            if (win.y == 1)
            {
                bM.player1Score += 1;
                //HexAgent.AddReward(-1); //Assume ai is 2nd player for now
            }
            else if (win.y == 2)
            {
                bM.player2Score += 1;
                //HexAgent.AddReward(1);
            }
        }

    }



}
