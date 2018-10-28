using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.AI;

public class DungeonGenerator : MonoBehaviour {

    public string seed;
    public bool useRandomSeed = true;

    public GameObject exitPortal;
    public List<Transform> wayPoints;
    public List<StateController> enemies;


    public void GenerateDungeon() {
        foreach (StateController badGuy in enemies) {
            badGuy.SetupAI(true, wayPoints);
        }
        HideExit();
    }

    void HideExit() {
        if (useRandomSeed) {
            seed = Time.time.ToString();
        }
        System.Random pseudoRandom = new System.Random(seed.GetHashCode());
        exitPortal.transform.position += Vector3.forward * pseudoRandom.Next(0, 5) + Vector3.right * pseudoRandom.Next(0, 5);
    }
}
