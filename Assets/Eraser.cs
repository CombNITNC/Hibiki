using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyInfo))]
public class Eraser : MonoBehaviour {
  EnemyGrade grade;
  bool willErase = false;

  [SerializeField] GameObject burst;

  void Start() {
    grade = GetComponent<EnemyInfo>().GetGrade();
  }

  public delegate void Callback();

  public void Activate(Callback callback, EnemyGrade lookGrade = EnemyGrade.None) {
    if (willErase) return;
    if (lookGrade == EnemyGrade.None) {
      lookGrade = grade;
    }
    var rightHits = Physics.SphereCastAll(transform.position, 0.5f, Vector3.right, 0f);
    var leftHits = Physics.SphereCastAll(transform.position, 0.5f, Vector3.left, 0f);
    var forwardHits = Physics.SphereCastAll(transform.position, 0.5f, Vector3.forward, 0f);
    var backHits = Physics.SphereCastAll(transform.position, 0.5f, Vector3.back, 0f);

    RaycastHit[] aroundEnemy = new RaycastHit[rightHits.Length + leftHits.Length + forwardHits.Length + backHits.Length];
    rightHits.CopyTo(aroundEnemy, 0);
    leftHits.CopyTo(aroundEnemy, rightHits.Length);
    forwardHits.CopyTo(aroundEnemy, rightHits.Length + leftHits.Length);
    backHits.CopyTo(aroundEnemy, rightHits.Length + leftHits.Length + forwardHits.Length);
    foreach (var hit in aroundEnemy) {
      var info = hit.transform.GetComponent<EnemyInfo>();
      if (info != null && info.GetGrade() - lookGrade == 1) {
        var eraser = hit.transform.GetComponent<Eraser>();
        willErase = true;
        if (eraser) eraser.Activate(callback, lookGrade);
        Destroy(hit.transform.gameObject);
        Destroy(gameObject);
        callback();
      }
    }
    if (willErase) {
      Instantiate(burst, transform.position, Quaternion.identity);
    }
  }
}