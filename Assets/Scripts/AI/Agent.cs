
using System;
using UnityEngine;

public class Agent : MonoBehaviour
{
    public enum Actions : int {
        CREATE_ARMY,
        MOVE_ARMY,
        ATTACK_ARMY,
    }

    public DecisionMaker decisionMaker;

    private Action endTurn;
    private bool continueMakingDecisions;

    // Start is called before the first frame update
    void Start() {
        endTurn = new Action(EndTurn);
    }

    public void InitTurn() {
        continueMakingDecisions = true;
    }

    public void FinishTurn() {

    }

    private void EndTurn() {
        continueMakingDecisions = false;
    }
}
