using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPositioner : MonoBehaviour {
  [SerializeField] float movableWidth = 4f;
  int position = 0;

  public delegate void Callback();

  public void MoveLeft(Callback callback) {
    if (-2 < position) {
      --position;
      StartCoroutine(Move(-1, callback));
    } else {
      callback();
    }
  }

  public void MoveRight(Callback callback) {
    if (position < 2) {
      ++position;
      StartCoroutine(Move(1, callback));
    } else {
      callback();
    }
  }

  public int GetPosition() {
    return position;
  }

  IEnumerator Move(int direction, Callback callback) {
    float amount = direction * movableWidth / 5;
    float start = Time.time;
    var src = transform.position;
    var dst = src + new Vector3(amount, 0f, 0f);
    while (Time.time - start < 0.13f) {
      transform.position = Vector3.Slerp(src, dst, (Time.time - start) / 0.13f);
      yield return null;
    }
    transform.position = dst;
    callback();
  }
}