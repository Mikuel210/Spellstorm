using System.Collections.Generic;
using UnityEngine;

public class FireSpell : Spell {

	public FireSpell() {
		Level = 1;
		MaxLevel = 5;
		Damage = 25;
		FireRate = 1.5f;
	}
	
	public override void Invoke(Vector3 position, Quaternion rotation) {
		switch (Level) {
			case 1: InvokeLevel1(position, rotation); break;
			case 2: InvokeLevel2(position, rotation); break;
			case 3: InvokeLevel3(position, rotation); break;
			case 4: InvokeLevel4(position, rotation); break;
			case 5: InvokeLevel5(position, rotation); break;
		}
	}

	private GameObject MakeFireball(GameObject prefab, Vector3 position, Quaternion rotation, float damage, float angleOffset = 0) {
		Quaternion newRotation = rotation * Quaternion.Euler(angleOffset, 0, 0);
		GameObject fireball = Object.Instantiate(prefab, position, newRotation);
		fireball.transform.right = fireball.transform.forward;
		
		fireball.GetComponent<ProjectileController>().damage = damage;
		
		Object.Destroy(fireball, 5);
		return fireball;
	}

	private void InvokeLevel1(Vector3 position, Quaternion rotation) {
		MakeFireball(
			Prefabs[0],
			position,
			rotation,
			Damage
		);
	}

	private void InvokeLevel2(Vector3 position, Quaternion rotation) {
		MakeFireball(
			Prefabs[1],
			position,
			rotation,
			Damage
		);
	}

	private void InvokeLevel3(Vector3 position, Quaternion rotation) {
		MakeFireball(
			Prefabs[2],
			position,
			rotation,
			Damage
		);
	}

	private void InvokeLevel4(Vector3 position, Quaternion rotation) {
		MakeFireball(
			Prefabs[3],
			position,
			rotation,
			Damage
		);
		
		MakeFireball(
			Prefabs[1],
			position,
			rotation,
			Damage,
			-15
		);
		
		MakeFireball(
			Prefabs[1],
			position,
			rotation,
			Damage,
			15
		);
	}

	private void InvokeLevel5(Vector3 position, Quaternion rotation) {
		MakeFireball(
			Prefabs[4],
			position,
			rotation,
			Damage
		);
		
		MakeFireball(
			Prefabs[2],
			position,
			rotation,
			Damage,
			-10
		);
		
		MakeFireball(
			Prefabs[2],
			position,
			rotation,
			Damage,
			10
		);
	}

}