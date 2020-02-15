using System.Collections;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class Virus : MonoBehaviour {
  [SerializeField] Mesh cracked;

  public static void Attach(GameObject go) {
    var obj = go.AddComponent<Virus>();
  }

  Coroutine moveWork = null;

  public void Apply(Vector3 to, bool isCracked) {
    if (isCracked) {
      GetComponent<MeshFilter>().mesh = cracked;
    }

    if (moveWork != null) StopCoroutine(moveWork);
    moveWork = StartCoroutine(MoveWork(to));
  }

  IEnumerator MoveWork(Vector3 to) {
    var start = Time.time;
    var src = transform.position;
    while (Time.time - start < 0.13f) {
      transform.position = Vector3.Lerp(src, to, (Time.time - start) / 0.13f);
      yield return null;
    }
    transform.position = to;
  }
}