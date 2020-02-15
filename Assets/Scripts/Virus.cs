using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class Virus : MonoBehaviour {

  [SerializeField] Ruling.Virus.Grade grade;
  [SerializeField] Mesh cracked;

  int hitPoint = 0;

  static int globalId = 0;

  public readonly int id = ++globalId;

  public Ruling.Virus.Grade GetGrade() {
    return grade;
  }

  public int GetHP() {
    return hitPoint;
  }

  public void Hit() {
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