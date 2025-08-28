using System.Collections.Generic;
using UnityEngine;

public class IceSpell : Spell {

	public IceSpell() {
		Level = 1;
		MaxLevel = 5;
		Damage = 2;
		FireRate = 0.3f;
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
	
	private GameObject MakeSnowball(GameObject prefab, Vector3 position, Quaternion rotation, float damage, float angleOffset = 0) {
		Quaternion newRotation = rotation * Quaternion.Euler(angleOffset, 0, 0);
		GameObject snowball = Object.Instantiate(prefab, position, newRotation);
		snowball.transform.right = snowball.transform.forward;
		
		snowball.GetComponent<ProjectileController>().damage = damage;
		
		Object.Destroy(snowball, 5);
		return snowball;
	}

	private void InvokeLevel1(Vector3 position, Quaternion rotation) {
		MakeSnowball(
			Prefabs[0],
			position,
			rotation,
			Damage
		);
	}

	private void InvokeLevel2(Vector3 position, Quaternion rotation) {
		MakeSnowball(
			Prefabs[1],
			position,
			rotation,
			Damage
		);
	}

	private void InvokeLevel3(Vector3 position, Quaternion rotation) {
		MakeSnowball(
			Prefabs[2],
			position,
			rotation,
			Damage
		);
		
		MakeSnowball(
			Prefabs[1],
			position,
			rotation,
			Damage,
			-15
		);
		
		MakeSnowball(
			Prefabs[1],
			position,
			rotation,
			Damage,
			15
		);
	}

	private void InvokeLevel4(Vector3 position, Quaternion rotation) {
		MakeSnowball(
			Prefabs[3],
			position,
			rotation,
			Damage
		);
		
		MakeSnowball(
			Prefabs[2],
			position,
			rotation,
			Damage,
			-10
		);
		
		MakeSnowball(
			Prefabs[2],
			position,
			rotation,
			Damage,
			10
		);
		
		MakeSnowball(
			Prefabs[1],
			position,
			rotation,
			Damage,
			-20
		);
		
		MakeSnowball(
			Prefabs[1],
			position,
			rotation,
			Damage,
			20
		);
	}

	private void InvokeLevel5(Vector3 position, Quaternion rotation) {
		MakeSnowball(
			Prefabs[4],
			position,
			rotation,
			Damage
		);
		
		MakeSnowball(
			Prefabs[3],
			position,
			rotation,
			Damage,
			-10
		);
		
		MakeSnowball(
			Prefabs[3],
			position,
			rotation,
			Damage,
			10
		);
		
		MakeSnowball(
			Prefabs[2],
			position,
			rotation,
			Damage,
			-20
		);
		
		MakeSnowball(
			Prefabs[2],
			position,
			rotation,
			Damage,
			20
		);
	}

}