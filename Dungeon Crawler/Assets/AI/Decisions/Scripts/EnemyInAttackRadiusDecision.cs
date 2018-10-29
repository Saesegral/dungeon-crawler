using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "PluggableAI/Decisions/EnemyInAttackRadius")]
public class EnemyInAttackRadiusDecision : Decision {

    public override bool Decide(StateController controller) {
        RaycastHit hit;

        Debug.DrawRay(controller.eyes.position, controller.eyes.forward.normalized * controller.attribs.lookRange, Color.green);

        //Change this to attack RADIUS not line of sight!
        if (Physics.SphereCast (controller.eyes.position, controller.attribs.lookSphereCastRadius, controller.eyes.forward, out hit, controller.attribs.lookRange) && hit.collider.CompareTag("Player")) {
            controller.chaseTarget = hit.transform;
            return true;
        } else {
            return false;
        }
    }
}
