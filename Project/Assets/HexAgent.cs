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

    public static int team;

    public override void InitializeAgent()
    {
        m_ResetParams = Academy.Instance.FloatProperties;
        SetResetParameters();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        foreach(GameObject tile in bM.gridTiles)
        {
            sensor.AddObservation(tile.GetComponent<TileObject>().tileID);
            sensor.AddObservation(tile.GetComponent<TileObject>().playerHoldingInt);
        }
    }

    public override void AgentAction(float[] vectorAction)
    {
        Debug.Log("actionS");

        int x = Mathf.RoundToInt(vectorAction[0]);
        int y = Mathf.RoundToInt(vectorAction[1]);

        Vector2Int choice = new Vector2Int(x, y);

        if(!bM.invalidMoves.Contains(choice)) //valid move, continue
        {

        }
        else //picked a spot already taken
        {
            SetReward(-1f);
            Done();
        }

        if(x > bM.staticSize.x || x < 0) //OOB X
        {
            SetReward(-1f);
            Done();
        }
        else
        {
            AddReward(0.125f);
        }

        if (y > bM.staticSize.y || y < 0) //OOB Y
        {
            SetReward(-1f);
            Done();
        }
        else
        {
            AddReward(0.125f);
        }

        if (bM.gridTiles[x,y].GetComponent<TileObject>().playerHoldingInt != 0)
        {
            SetReward(-1f);
            Done();
        }
        else //valid
        {
            AddReward(0.25f);
        }

        Debug.Log("actionE");

        bM.iM.aiMove = choice;
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

        int x = UnityEngine.Random.Range(0, bM.staticSize.x);
        int y = UnityEngine.Random.Range(0, bM.staticSize.y);

        action = new float[2] { x, y };

        //Debug.Log("end" + clicked);

        bM.iM.aiMove = new Vector2Int(Mathf.RoundToInt(action[0]), Mathf.RoundToInt(action[1]));

        return action;
    }

    public void SetResetParameters()
    {
        bM.invalidMoves.Clear();
        bM.iM.player1Tiles.Clear();
        bM.iM.player2Tiles.Clear();
    }
}
