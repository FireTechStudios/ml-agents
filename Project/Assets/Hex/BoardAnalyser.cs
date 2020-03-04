using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardAnalyser : MonoBehaviour
{
    public int playerLongest1;
    public int playerLongest2;
    public List<Vector2Int> visited = new List<Vector2Int>();
    public int lastPlayer;
    private int previousPlayer;
    public static bool win;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
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

        if (lastPlayer == 1)
        {
            playerLongest1 = visited.Count;
        }
        if (lastPlayer == 2)
        {
            playerLongest2 = visited.Count;
        }

        if (previousPlayer != lastPlayer) //Clear if new player
        {
            visited.Clear();
            GetLongestChain(InputManager.latestMove.x, InputManager.latestMove.y);
        }
        previousPlayer = lastPlayer;
    }

    void GetLongestChain(int x, int y)
    {
        if(!visited.Contains(new Vector2Int(x,y)))
        {
            visited.Add(new Vector2Int(x, y));

            List<Vector2Int> sameAdjacent = BoardManager.gridTiles[x, y].GetComponent<TileObject>().sameAdjacent;

            foreach(Vector2Int adjacent in sameAdjacent) //Remove same adjacents if visited before
            {
                if(visited.Contains(adjacent))
                {
                    sameAdjacent.Remove(adjacent);
                }
            }

            foreach (Vector2Int tile in sameAdjacent)
            {
                GetLongestChain(tile.x, tile.y);
                CheckForWin();
            }
            
        }
    }

    void CheckForWin()
    {
        //lastPlayer == 1, need [0] and [1]
        //lastPlayer == 1, need [2] and [3]

        Vector2Int winMet = new Vector2Int(0,0);

        List< List<bool>> winConditions = new List<List<bool>>();
        foreach(Vector2Int tile in visited)
        {
            winConditions.Add(BoardManager.gridTiles[tile.x, tile.y].GetComponent<TileObject>().winCondition);
        }
        foreach(List<bool> winConditionList in winConditions)
        {
            if(lastPlayer == 1) //[0],[1]
            {
                if(winConditionList[0] == true)
                {
                    winMet.x = 1;
                }
                if (winConditionList[1] == true)
                {
                    winMet.y = 1;
                }
            }

            if (lastPlayer == 2) //[2],[3]
            {
                if (winConditionList[2] == true)
                {
                    winMet.x = 1;
                }
                if (winConditionList[3] == true)
                {
                    winMet.y = 1;
                }
            }
        }

        if(winMet == new Vector2Int(1,1))
        {
            win = true;
            playerLongest1 = 0;
            playerLongest2 = 0;
            visited.Clear();
        }
    }

}
