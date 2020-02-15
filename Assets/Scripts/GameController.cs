using Ruling;
using UnityEngine;

public class GameController : MonoBehaviour, BoardOperator {
  Board board;
  int playerPos = 3;
  float lastEventTime = 0;

  void Start() {
    board = new Board(this);
    lastEventTime = Time.time;
  }

  void Update() {
    if (Input.GetButtonDown("Left") && 1 < playerPos) {
      Move.Invoke(--playerPos);
      lastEventTime = Time.time;
    }
    if (Input.GetButtonDown("Right") && playerPos < 5) {
      Move.Invoke(++playerPos);
      lastEventTime = Time.time;
    }
    if (Input.GetButtonDown("Drop")) {
      Drop.Invoke();
      lastEventTime = Time.time;
    }
    if (lastEventTime + 1000f <= Time.time) {
      Drop.Invoke();
    }
  }

  public event EventToModel.Move Move;
  public event EventToModel.Manipulate Manipulate;
  public event EventToModel.Drop Drop;

  public void OnSpawn(Ruling.Virus virus) {
    throw new System.NotImplementedException();
  }
  public void OnAbsorb(Ruling.Virus.Id eater, Ruling.Virus.Id eaten) {
    throw new System.NotImplementedException();
  }

  public void OnBreak(Ruling.Virus.Id broken) {
    throw new System.NotImplementedException();
  }
  public void OnChange(Ruling.Virus.Id id, Position from, Position to) {
    throw new System.NotImplementedException();
  }
}