using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyInfo))]
public class Eraser : MonoBehaviour {
  EnemyGrade grade;

  void Start() {
    grade = GetComponent<EnemyInfo>().GetGrade();
  }

  public void Activate() {
    RaycastHit hits;
    if (Physics.Raycast(transform.position, transform.forward, out hits, 20f)) {
      Debug.Log(hits.transform);
      var info = hits.transform.GetComponent<EnemyInfo>();
      if (info != null && info.GetGrade() - grade == 1) {
        Destroy(hits.transform.gameObject);
        Destroy(gameObject);
      }
    }
  }
}