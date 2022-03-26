using System.Collections;
using System.Collections.Generic;
using ObjectPool.ObjectPool;
using UnityEngine;
using UnityEngine.SceneManagement;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public sealed class PlayerController : MonoBehaviour {
	[SerializeField] private Cube _playerCube;
	[SerializeField] private Material _defaultMaterial;
	[SerializeField] private Material _towerMaterial;
	[SerializeField] private Rigidbody _rigidbody;

	private List<Cube> _items = new List<Cube>();
	private GameObject _tower;
	private bool _onGround;
	private float _jumpForce;
	private float _jumpForceCounter;
	private float _jumpDuration = 20;
	private bool _isPressed;

	private void Start() {
		_items.Add(_playerCube);
	}

	private void Update() {
#if UNITY_EDITOR
		_jumpForce = 9;
		_isPressed = Input.GetKey(KeyCode.Space);
		if (Input.GetKey(KeyCode.Space)) {
			Jump();
		}
#elif UNITY_ANDROID
      _jumpForce = 23;
			Touch touch = Input.GetTouch(0);
			if (touch.phase == TouchPhase.Began) {
				_rigidbody.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
			}

#endif
	}

	private void OnCollisionEnter(Collision collision) {
		if (collision.gameObject.CompareTag("Ground")) {
			_onGround = true;
		}
	}

	private void OnCollisionExit(Collision other) {
		if (other.gameObject.CompareTag("Ground")) {
			_onGround = false;
		}
	}

	private void OnTriggerEnter(Collider other) {
		if (other.CompareTag("CubeItem")) {
			
			_playerCube.transform.localPosition += Vector3.up;
			_tower = ObjectPooler.Instance.SpawnFromPool("CubeItem", transform.position, Quaternion.identity);
			_tower.transform.SetParent(_playerCube.transform);
			Destroy(other.gameObject);
			_items.Add(_tower.GetComponent<Cube>());

			foreach (var item in _items) {
				item.GetComponent<MeshRenderer>().material = _towerMaterial;
			}
			
		} else if (other.CompareTag("Obstacle")) {
			
			var index = _items.Count - 1;
			_playerCube.transform.localPosition -= Vector3.up;
			for (var i = 0; i < _items.Count; i++) {
				if (i != index)
					continue;
				_items[i].transform.SetParent(other.transform);
				_items[i].transform.position = other.transform.position + Vector3.left;
				_items[i].GetComponent<MeshRenderer>().material = _defaultMaterial;
				StartCoroutine(ReturnToPool(_items[i]));
				
				if (_items.Count == 1) {
					SceneManager.LoadScene(0);
				}
				
				_items.RemoveAt(i);
			}
		} else if (other.CompareTag("Finish")) {
			SceneManager.LoadScene(0);
		}
	}

	private IEnumerator ReturnToPool(Cube cube) {
		yield return new WaitForSeconds(3f);
		cube.gameObject.SetActive(false);
		cube.transform.SetParent(null);
	}

	private void Jump() {
		if (_isPressed && _jumpForceCounter < _jumpDuration && _onGround) {
			_jumpForceCounter++;
			_rigidbody.AddForce(Vector3.up * _jumpForce / _jumpForceCounter, ForceMode.Impulse);
		} else {
			_jumpForceCounter = 0;
		}
	}
}