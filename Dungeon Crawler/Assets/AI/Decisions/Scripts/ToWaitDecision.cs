using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "PluggableAI/Decisions/ToWait")]
public class ToWaitDecision : Decision {
    public override bool Decide(StateController controller) {
        bool enemySeenRecently = Scan(controller);
        return enemySeenRecently;
    }

    private bool Scan(StateController controller) {
        controller.navMeshAgent.isStopped = true;
        return controller.CheckIfCountDownElapsed(controller.attribs.searchDuration);
    }

}