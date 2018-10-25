using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
    public float speed;
    Rigidbody rigidbody;
    Vector3 velocity;

    void Start() {
        rigidbody = GetComponent<Rigidbody>();
    }

    void Update() {
        velocity = speed*new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized * 10;
    }

    void FixedUpdate() {
        rigidbody.MovePosition(rigidbody.position + velocity * Time.fixedDeltaTime);
    }
}