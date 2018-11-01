using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InAttackRegion : MonoBehaviour {

    StateController stateControl;

    void Start() {
       stateControl = transform.parent.GetComponent<StateController>();
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Player")) {
            stateControl.SetAttackRegion(true);
        }
    }

    private void OnTriggerExit (Collider other) {
        if (other.gameObject.CompareTag("Player")) {
            stateControl.SetAttackRegion(false);
        }
    }
}
