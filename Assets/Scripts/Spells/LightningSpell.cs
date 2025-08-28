using System.Collections.Generic;
using UnityEngine;

public class LightningSpell : Spell {

	public LightningSpell() {
		Level = 1;
		MaxLevel = 5;
		Damage = 1;
		FireRate = 0.2f;
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
	
	private GameObject MakeLightning(GameObject prefab, Vector3 position, Quaternion rotation, float damage, float angleOffset = 0) {
		Quaternion newRotation = rotation * Quaternion.Euler(angleOffset, 0, 0);
		GameObject lightning = Object.Instantiate(prefab, position, newRotation);
		lightning.transform.right = lightning.transform.forward;
		
		lightning.GetComponent<ProjectileController>().damage = damage;
		
		Object.Destroy(lightning, 5);
		return lightning;
	}

	private void InvokeLevel1(Vector3 position, Quaternion rotation) {
		MakeLightning(
			Prefabs[0],
			position,
			rotation,
			Damage
		);
	}

	private void InvokeLevel2(Vector3 position, Quaternion rotation) {
		MakeLightning(
			Prefabs[1],
			position,
			rotation,
			Damage
		);
	}

	private void InvokeLevel3(Vector3 position, Quaternion rotation) {
		MakeLightning(
			Prefabs[2],
			position,
			rotation,
			Damage
		);
		
		MakeLightning(
			Prefabs[2],
			position,
			rotation,
			Damage,
			-10
		);
		
		MakeLightning(
			Prefabs[2],
			position,
			rotation,
			Damage,
			10
		);
	}

	private void InvokeLevel4(Vector3 position, Quaternion rotation) {
		MakeLightning(
			Prefabs[3],
			position,
			rotation,
			Damage
		);
		
		MakeLightning(
			Prefabs[3],
			position,
			rotation,
			Damage,
			-15
		);
		
		MakeLightning(
			Prefabs[3],
			position,
			rotation,
			Damage,
			15
		);
	}

	private void InvokeLevel5(Vector3 position, Quaternion rotation) {
		MakeLightning(
			Prefabs[4],
			position,
			rotation,
			Damage
		);
		
		MakeLightning(
			Prefabs[4],
			position,
			rotation,
			Damage,
			15
		);
		
		MakeLightning(
			Prefabs[4],
			position,
			rotation,
			Damage,
			-15
		);
		
		MakeLightning(
			Prefabs[4],
			position,
			rotation,
			Damage,
			30
		);
		
		MakeLightning(
			Prefabs[4],
			position,
			rotation,
			Damage,
			-30
		);
	}

}