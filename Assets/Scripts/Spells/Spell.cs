using System.Collections.Generic;
using UnityEngine;

public abstract class Spell {
	
	public int Level { get; set;  }
	public int MaxLevel { get; protected set; }
	public float Damage { get; set; }
	public float FireRate { get; set; }
	public List<GameObject> Prefabs { get; private set; }
	
	
	public float Debounce { get; set; }

	public void Initialize(List<GameObject> prefabs) => Prefabs = prefabs;
	
	public abstract void Invoke(Vector3 position, Quaternion rotation);

	public void Upgrade(UpgradeSO upgradeSO) {
		Level++;
		Damage += upgradeSO.damage;
		FireRate += upgradeSO.fireRate;
	}

}