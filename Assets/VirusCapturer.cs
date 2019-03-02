using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirusCapturer : MonoBehaviour {
  Transform captured = null;

  public delegate void Callback();

  public void Capture(Callback callback, Eraser.Callback onPoint) {
    RaycastHit hits;
    if (Physics.Raycast(transform.position, transform.forward, out hits, 20f)) {
      if (captured != null) {
        captured.GetComponent<Collider>().enabled = true;
        StartCoroutine(CaptureWork(hits.transform.position.z - 1f, true, callback, onPoint));
      } else {
        if (!hits.transform.CompareTag("Enemy")) { return; }
        captured = hits.transform;
        hits.collider.enabled = false;
        captured.parent = transform;
        StartCoroutine(CaptureWork(1f, false, callback, onPoint));
      }
    } else {
      callback();
    }
  }

  IEnumerator CaptureWork(float to, bool isRelease, Callback callback, Eraser.Callback onPoint) {
    var start = Time.time;
    var src = captured.transform.position;
    var dst = Align(new Vector3(src.x, src.y, Mathf.Min(to, 13.5f)));
    while (Time.time - start < 0.13f) {
      captured.transform.position = Vector3.Lerp(src, dst, (Time.time - start) / 0.13f);
      yield return null;
    }
    captured.transform.position = dst;
    if (isRelease) {
      var eraser = captured.GetComponent<Eraser>();
      if (eraser) eraser.Activate(onPoint);
      captured.parent = null;
      captured = null;
    }
    callback();
  }

  Vector3 Align(Vector3 v) {
    var aligned = new Vector3();
    aligned.x = Mathf.Round(v.x / 1.2f) * 1.2f;
    aligned.y = 0f;
    aligned.z = Mathf.Round(v.z / 1f) * 1f;
    return aligned;
  }
}