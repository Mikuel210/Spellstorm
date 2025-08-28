using UnityEngine;

[CreateAssetMenu(fileName = "New Upgrade", menuName = "Scriptable Objects/Upgrade")]
public class UpgradeSO : ScriptableObject {

	public float damage;
	public float fireRate;
	public int cost;

}