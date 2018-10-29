using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "PluggableAI/Actions/Attack")]
public class AttackAction : Action {

    public override void Act(StateController controller) {
        Attack(controller);
    }

    private void Attack(StateController controller) {
        RaycastHit hit;
        
        Debug.DrawRay(controller.eyes.position, controller.eyes.forward.normalized * controller.attribs.attackRange, Color.red);

        if (Physics.SphereCast(controller.eyes.position, controller.attribs.lookSphereCastRadius, controller.eyes.forward, out hit, controller.attribs.attackRange) && hit.collider.CompareTag("Player")) {
            if (controller.CheckIfCountDownElapsed(controller.attribs.attackRate)) {
                //controller.tankShooting.Fire(controller.enemyStats.attackForce, controller.enemyStats.attackRate);
            }
        }
    }
}
