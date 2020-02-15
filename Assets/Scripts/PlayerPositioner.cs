using System.Collections;
using UnityEngine;

public class PlayerPositioner : MonoBehaviour {
  [SerializeField] float movableWidth = 4f;
  int position = 0;

  public void MoveLeft() {
    if (-2 < position) {
      --position;
      StartCoroutine(Move(-1));
    }
  }

  public void MoveRight() {
    if (position < 2) {
      ++position;
      StartCoroutine(Move(1));
    }
  }

  public int GetPosition() {
    return position;
  }

  bool atomic = false;

  IEnumerator Move(int direction) {
    while (atomic) yield return null;
    atomic = true;
    float amount = direction * movableWidth / 5;
    float start = Time.time;
    var src = transform.position;
    var dst = src + new Vector3(amount, 0f, 0f);
    while (Time.time - start < 0.13f) {
      transform.position = Vector3.Slerp(src, dst, (Time.time - start) / 0.13f);
      yield return null;
    }
    transform.position = dst;
    atomic = false;
  }
}