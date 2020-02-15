using System.Collections;
using Ruling;
using UnityEngine;

public class GameController : MonoBehaviour, BoardOperator {

  [SerializeField] float spawnRateMs = 7.5f;

  public Board board { get; private set; }
  int playerPos = 3;
  float dropTimer = 0;

  void Start() {
    board = new Board(this);
    StartCoroutine(GameSetup());
  }

  IEnumerator GameSetup() {
    yield return new WaitForSeconds(1.0f);
    Drop.Invoke();
    Drop.Invoke();
  }

  void Update() {
    if (Input.GetButtonDown("Left") && 1 < playerPos) {
      Move.Invoke(--playerPos);
    }
    if (Input.GetButtonDown("Right") && playerPos < 5) {
      Move.Invoke(++playerPos);
    }
    if (Input.GetButtonDown("Capture")) {
      Manipulate.Invoke();
    }
    if (Input.GetButtonDown("Drop")) {
      Drop.Invoke();
      dropTimer = 0f;
    }
    dropTimer += Time.deltaTime;
    if (spawnRateMs <= dropTimer) {
      Drop.Invoke();
      dropTimer = 0f;
    }
  }

  public event EventToModel.Move Move;
  public event EventToModel.Manipulate Manipulate;
  public event EventToModel.Drop Drop;
}