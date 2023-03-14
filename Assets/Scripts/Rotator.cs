using UnityEngine;

public class Rotator : MonoBehaviour {
  void FixedUpdate() {
    transform.Rotate(transform.up, 1.2f);
  }
}
