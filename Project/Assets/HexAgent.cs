using UnityEngine;
using MLAgents;
using MLAgents.Sensors;
using MLAgents.SideChannels;

public class HexAgent : Agent
{
    [Header("Specific to HexAgent")]
    public BoardManager bM;
    Vector2Int m_tileChoice;
    IFloatProperties m_ResetParams;

    public override void InitializeAgent()
    {
        m_ResetParams = Academy.Instance.FloatProperties;
        SetResetParameters();
    }

    public override void CollectDiscreteActionMasks(DiscreteActionMasker actionMasker)
    {
        //actionMasker.SetMask(0, invalidx);
        //actionMasker.SetMask(0, invalidy);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        Debug.Log("CollectObservations");
        foreach (GameObject tile in bM.gridTiles)
        {
            Vector2Int tileID = tile.GetComponent<TileObject>().tileID;
            int playerHolding = tile.GetComponent<TileObject>().playerHoldingInt;

            Vector3Int boardScenario = new Vector3Int(tileID.x, tileID.y, playerHolding); //(CoordsX),(CoordsY),(PLayer:0,1,2)

            sensor.AddObservation(boardScenario);
            //Debug.Log(boardScenario);

        }

        base.CollectObservations(sensor);
    }

    private void Update()
    {
        if(bM.bA.win == new Vector2Int(1,1))
        {
            SetReward(1);
            Done();
            AgentReset();
        }
        if(bM.bA.win == new Vector2Int(1, 2))
        {
            SetReward(-1);
            Done();
            AgentReset();
        }

        //Debug.Log(GetCumulativeReward());
    }


    public override void AgentAction(float[] vectorAction)
    {
        int x = Mathf.RoundToInt(vectorAction[0]);
        int y = Mathf.RoundToInt(vectorAction[1]);

        Vector2Int choice = new Vector2Int(x, y);

        //Debug.Log(choice);
        if (bM.validMoves.Contains(choice)) //valid move, continue
        {
            if (TeamId == 0 && !bM.iM.player2Turn || TeamId == 1 && bM.iM.player2Turn)
            {
                //Debug.Log("Part1");

                //Debug.Log("Successful Move at " + choice);
                if (!bM.iM.player2Turn && TeamId == 0 && bM.iM.inputAllowed)
                {
                    bM.iM.aiMove = choice;
                }
                if (bM.iM.player2Turn && TeamId == 1 && bM.iM.inputAllowed)
                {
                    bM.iM.aiMove = choice;
                }

                TileRewards(choice);
            }

        }
        else //picked a spot already taken/invalid
        {
            AddReward(-1f);
            Done();
            AgentReset();
        }
    }

    void TileRewards(Vector2Int tile)
    {
        TileObject data = bM.gridTiles[tile.x, tile.y].GetComponent<TileObject>();
        foreach (TileObject vein in data.vein)
        {
            //Debug.Log(vein);
        }
    }

    public override void AgentReset()
    {
        m_tileChoice = new Vector2Int(-1,-1);

        //Reset the parameters when the Agent is reset.
        SetResetParameters();
    }
    //mlagents-learn config/trainer_config.yaml --env=Project\Builds\Unity Environment.exe --run-id=cob_1 --train
    public override float[] Heuristic()
    {

        var action = new float[2];

        int x = bM.validMoves[Random.Range(0, bM.validMoves.Count)].x;
        int y = bM.validMoves[Random.Range(0, bM.validMoves.Count)].y;

        action = new float[2] { x, y };

        //Debug.Log("end" + clicked);

        bM.iM.aiMove = new Vector2Int((int)action[0], (int)action[1]);

        return action;
    }

    public void SetResetParameters()
    {
        bM.invalidMoves.Clear();
        bM.iM.player1Tiles.Clear();
        bM.iM.player2Tiles.Clear();
    }
}
