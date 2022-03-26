using UnityEngine;
using UnityEngine.UI;

public sealed class LevelManager : MonoBehaviour {
	[SerializeField] private GameObject _platform;
	[SerializeField] private Button _startButton;

	public void StartGame() {
		_platform.GetComponent<PlatformMovement>().enabled = true;
		_startButton.gameObject.SetActive(false);
	}
}