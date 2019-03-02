using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirusCapturer : MonoBehaviour {
  Transform captured = null;

  public void Capture() {
    RaycastHit hits;
    if (Physics.Raycast(transform.position, transform.forward, out hits, 20f)) {
      if (captured != null) {
        captured.GetComponent<Collider>().enabled = true;
        StartCoroutine(CaptureWork(hits.transform.position.z - 1f, true));
      } else {
        if (!hits.transform.CompareTag("Enemy")) { return; }
        captured = hits.transform;
        hits.collider.enabled = false;
        captured.parent = transform;
        StartCoroutine(CaptureWork(1f, false));
      }
    }
  }

  bool atomic = false;
  IEnumerator CaptureWork(float to, bool isRelease) {
    while (atomic) yield return null;
    atomic = true;
    var start = Time.time;
    var src = captured.transform.position;
    var dst = new Vector3(src.x, src.y, Mathf.Min(to, 13.5f));
    while (Time.time - start < 0.13f) {
      captured.transform.position = Vector3.Lerp(src, dst, (Time.time - start) / 0.13f);
      yield return null;
    }
    captured.transform.position = dst;
    if (isRelease) {
      var eraser = captured.GetComponent<Eraser>();
      if (eraser) eraser.Activate();
      captured.parent = null;
      captured = null;
    }
    atomic = false;
  }
}