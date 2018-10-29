using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "PluggableAI/Attributes")]
public class Attributes : ScriptableObject {

    public float health = 100;
    public float armor = 10;

	public float moveSpeed = 1;
	public float lookRange = 40f;
	public float lookSphereCastRadius = 1f;

    public float fieldOfView = 90f;
    public float attackRange = 1f;
	public float attackRate = 1f;
	public int attackDamage = 50;

	public float searchDuration = 4f;
}