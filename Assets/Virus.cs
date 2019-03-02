using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyGrade {
  None,
  Small,
  Medium,
  Large
}

[RequireComponent(typeof(MeshFilter))]
public class Virus : MonoBehaviour {

  [SerializeField] EnemyGrade grade = EnemyGrade.None;
  [SerializeField] Mesh cracked;

  public EnemyGrade GetGrade() {
    return grade;
  }

  int hitPoint = 0;

  public void Activate() {
    ++hitPoint;
    if (2 <= hitPoint) {
      burster(this);
    } else if (1 == hitPoint) {
      GetComponent<MeshFilter>().mesh = cracked;
    }
  }

  public delegate void Burster(Virus target);
  Burster burster;

  public void SetBurster(Burster func) {
    burster = func;
  }
}