using System.Collections;
using UnityEngine;

public class PlayerPositioner : MonoBehaviour {
  [SerializeField] float movableWidth = 4f;

  int prevPos = 3;

  void Start() {
    var gc = GameObject.FindWithTag("GameController").GetComponent<GameController>();
    gc.Move += pos => {
      if (moveWork != null) StopCoroutine(moveWork);
      moveWork = StartCoroutine(Move(pos - prevPos));
      prevPos = pos;
    };
  }

  Coroutine moveWork = null;

  IEnumerator Move(int direction) {
    float amount = direction * movableWidth / 5f;
    float start = Time.time;
    var src = transform.position;
    var dst = src + new Vector3(amount, 0f, 0f);
    while (Time.time - start < 0.13f) {
      transform.position = Vector3.Slerp(src, dst, (Time.time - start) / 0.13f);
      yield return null;
    }
    transform.position = dst;
  }
}