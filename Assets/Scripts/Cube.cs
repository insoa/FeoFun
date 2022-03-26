using UnityEngine;

public sealed class Cube : MonoBehaviour {
	private void OnTriggerEnter(Collider other) {
		if (other.CompareTag("Player")) {
			Destroy(gameObject);
		}
	}
}