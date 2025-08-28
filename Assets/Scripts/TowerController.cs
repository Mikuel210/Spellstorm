using System;
using UnityEngine;

public class TowerController : MonoBehaviour {

    private void OnDestroy() => UIManager.Instance.Lose();

}
