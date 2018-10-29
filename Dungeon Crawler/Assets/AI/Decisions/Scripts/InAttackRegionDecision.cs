using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Decisions/InAttackRegion")]
public class InAttackRegionDecision : Decision {

    public override bool Decide(StateController controller) {
        return controller.InAttackRegion();
    }
}
