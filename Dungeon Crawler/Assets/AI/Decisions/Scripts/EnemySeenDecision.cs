using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "PluggableAI/Decisions/EnemySeen")]
public class EnemySeenDecision : Decision {

    public override bool Decide(StateController controller) {
        bool targetVisible = Look(controller);
        return targetVisible;
    }

    private bool Look(StateController controller) {
        RaycastHit hit;
        
        Debug.DrawRay(controller.eyes.position, controller.eyes.forward.normalized * controller.attribs.lookRange, Color.green);

        if(Physics.SphereCast (controller.eyes.position, controller.attribs.lookSphereCastRadius, controller.eyes.forward, out hit, controller.attribs.lookRange) && hit.collider.CompareTag("Player")) {
            controller.chaseTarget = hit.transform;
            return true;
        } else {
            return false;
        }


    }
}
