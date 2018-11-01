using UnityEngine;
using UnityEngine.AI;

public class GameController : MonoBehaviour {
    
    public Camera cam;
    public int closestZoom = 1;
    public int farthestZoom = 10;

    public string seed;
    public bool useRandomSeed;
    
    private MapGenerator mapGen;
    private MeshGenerator meshGen;
    private NavMeshSurface surface;
    private DungeonGenerator dunGen;
    
    private bool exitReached;

    private void Start() {
        mapGen = GetComponent<MapGenerator>();
        meshGen = GetComponent<MeshGenerator>();
        surface = GetComponent<NavMeshSurface>();
        dunGen = GetComponent<DungeonGenerator>();
        MakeNewLevel();
    }
    
    private void Update() {
        //Remove right click to generate new level, only for testing purposes
        if (exitReached || Input.GetMouseButtonDown(1) ) {
            MakeNewLevel();
        }
        if (Input.GetAxis("Mouse ScrollWheel") > 0f && cam.orthographicSize > closestZoom) {
            cam.orthographicSize--;
        } else if (Input.GetAxis("Mouse ScrollWheel") < 0f && cam.orthographicSize < farthestZoom) {
            cam.orthographicSize++;
        }
    }

    public void MakeNewLevel() {
        mapGen.GenerateMap(meshGen);
        surface.BuildNavMesh();
        dunGen.GenerateDungeon();
        exitReached = false;
    }

    public void ExitReached() {
        exitReached = true;
    }
}