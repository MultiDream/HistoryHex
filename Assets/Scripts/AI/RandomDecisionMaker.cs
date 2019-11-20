using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomDecisionMaker : DecisionMaker {

    public Agent.Actions[] availableDecisions;

    public void Start() {
        // do nothing
    }

    public override Agent.Actions ChooseAction() {
        return availableDecisions[Mathf.RoundToInt(Random.value * availableDecisions.Length)];
    }
}
