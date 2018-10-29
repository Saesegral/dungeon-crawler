using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class StateController : MonoBehaviour {

    public State currentState;
	public Attributes attribs;
	public Transform eyes;
    public State remainState;
    public List<GameObject> wayPoints;


	[HideInInspector] public NavMeshAgent navMeshAgent;
	[HideInInspector] public GameController gameControl;
	[HideInInspector] public List<Transform> wayPointList;
    [HideInInspector] public int nextWayPoint;
    [HideInInspector] public Transform chaseTarget;

    [HideInInspector] public float stateTimeElapsed;
    [HideInInspector] public bool inAttackRegion;

    //Remove aiActive? - Should always be active.
    private bool aiActive;

	void Awake () {
		navMeshAgent = GetComponent<NavMeshAgent> ();
        inAttackRegion = false;
    }

	public void SetupAI(bool aiActivationFromTankManager, List<Transform> wayPointsFromTankManager){
		wayPointList = wayPointsFromTankManager;
		aiActive = aiActivationFromTankManager;
		if (aiActive) 
		{
			navMeshAgent.enabled = true;
		} else 
		{
			navMeshAgent.enabled = false;
		}
	}

    private void Update() {
        if (!aiActive)
            return;
        currentState.UpdateState(this);
    }

    private void OnDrawGizmos() {
        if(currentState !=null && eyes!= null) {
            Gizmos.color = currentState.sceneGizmoColor;
            Gizmos.DrawWireSphere(eyes.position, attribs.lookSphereCastRadius);
        }
    }

    public void TransitionToState(State nextState) {
        if(nextState != remainState) {
            currentState = nextState;
            OnExitState();
        }
    }

    private void OnExitState() {
        stateTimeElapsed = 0;
    }

    public bool CheckIfCountDownElapsed(float duration) {
        stateTimeElapsed += Time.deltaTime;
        return (stateTimeElapsed >= duration);
    }
    
    public bool InAttackRegion() {
        return inAttackRegion;
    }

    public void SetAttackRegion(bool collided) {
        inAttackRegion = collided;
    }
}