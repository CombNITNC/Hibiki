using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour {
  [SerializeField] Text display = null;

  int score = 0;

  void Start() {
    var gc = GameObject.FindWithTag("GameController").GetComponent<GameController>();
    gc.board.Break += Point;
  }

  void Point(Ruling.Virus.Id _id) {
    score += 10;
    display.text = "SCORE " + score;
  }
}