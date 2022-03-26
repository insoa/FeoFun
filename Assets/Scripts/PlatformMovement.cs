using UnityEngine;

public sealed class PlatformMovement : MonoBehaviour {
   [SerializeField] private float _speed;
   private void Update() {
      transform.Translate(-_speed * Time.deltaTime, 0f, 0f);
   }
}
