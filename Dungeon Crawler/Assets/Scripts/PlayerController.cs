using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityStandardAssets.Characters.ThirdPerson;

public class PlayerController : MonoBehaviour {

    public Camera cam;
    public GameController gameControl;
    public NavMeshAgent agent;
    public ThirdPersonCharacter character;

    private void Start()
    {
        agent.updateRotation = false;
    }


    // Update is called once per frame
    void Update () {
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBGL
        if (Input.GetMouseButton(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
#elif (UNITY_ANDROID || UNIT_IOS) && !UNITY_EDITOR
            //Consider doing something more involved for clicking on enemies and items or UI later.
        if (Input.touchCount > 0)
        {
            Ray ray = cam.ScreenPointToRay(Input.touches[0].position);
#endif
            RaycastHit hit;

            if(Physics.Raycast(ray, out hit))
            {
                agent.SetDestination(hit.point);
            }
        }

        if (agent.remainingDistance > agent.stoppingDistance)
        {
            character.Move(agent.desiredVelocity, false, false);
        }
        else
        {
            character.Move(Vector3.zero, false, false);
        }
	}

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Exit Portal")) {
            gameControl.ExitReached();
        }
    }

}
