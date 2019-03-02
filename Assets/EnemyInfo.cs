using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyGrade {
  Small,
  Medium,
  Large
}

public class EnemyInfo : MonoBehaviour {

  [SerializeField] EnemyGrade grade;

  static void Make(Transform toAttach, EnemyGrade grade) {
    var self = toAttach.gameObject.AddComponent<EnemyInfo>();
    self.grade = grade;
  }

  public EnemyGrade GetGrade() {
    return grade;
  }
}