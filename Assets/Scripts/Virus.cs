using System.Collections;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class Virus : MonoBehaviour {
  [SerializeField] Mesh cracked;

  public static void Attach(GameObject go) {
    var obj = go.AddComponent<Virus>();
  }

  Coroutine moveWork = null;

  public void Apply(Ruling.Virus toShow) {
    if (toShow.isCracked) {
      GetComponent<MeshFilter>().mesh = cracked;
    }
    var pos = toShow.VirusPosition;

    if (moveWork != null) StopCoroutine(moveWork);
    moveWork = StartCoroutine(MoveWork(new Vector3(
      pos.X * 1.2f - 3.0f,
      0f, -pos.Y * 1.2f + 15f
    )));
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