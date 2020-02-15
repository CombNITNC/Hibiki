using Ruling;
using UnityEngine;

public class GameController : MonoBehaviour, BoardOperator {
  public Board board { get; private set; }
  int playerPos = 3;
  float lastEventTime = 0;

  [SerializeField] float spawnRateMs = 7500f;

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
    if (lastEventTime + spawnRateMs <= Time.time) {
      Drop.Invoke();
    }
  }

  public event EventToModel.Move Move;
  public event EventToModel.Manipulate Manipulate;
  public event EventToModel.Drop Drop;
}