using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ParticleDisposer : MonoBehaviour {
  ParticleSystem ps;
  void Start() {
    ps = GetComponent<ParticleSystem>();
  }

  void Update() {
    if (ps.isStopped) {
      Destroy(gameObject);
    }
  }
}