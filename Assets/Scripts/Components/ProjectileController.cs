using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProjectileController : MonoBehaviour {

	public float speed;
	public float damage;

	[Space] public bool isFire;
	public bool isIce;
	public bool isLightning;

	[Space] public bool useModifier;
	public bool cancelModifiers;
	public SpeedModifier modifier;

	[Space] public int bounces;

	void FixedUpdate() {
		transform.Translate(transform.right * speed, Space.World);
	}

	public void Bounce() {
		GameObject nearestEnemy = GetNearestEnemy();

		if (!nearestEnemy) {
			Destroy(gameObject);
			return;
		}
		
		Vector2 fromPosition = transform.position;
		Vector2 toPosition = nearestEnemy.transform.position;
		Vector2 direction = toPosition - fromPosition;
		Quaternion rotation = Quaternion.LookRotation(direction);

		transform.rotation = rotation;
		transform.right = transform.forward;
		bounces--;
	}

	private GameObject GetNearestEnemy() {
		List<GameObject> enemies = GameObject.FindGameObjectsWithTag("Enemy")
			.OrderBy(e => (transform.position - e.transform.position).sqrMagnitude)
			.ToList();

		enemies.RemoveAt(0);
		return enemies.FirstOrDefault();
	}

}