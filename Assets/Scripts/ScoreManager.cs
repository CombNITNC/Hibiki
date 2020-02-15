using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour {
  [SerializeField] Text display;

  int score = 0;

  public void Point(int amount) {
    score += amount;
    display.text = "SCORE " + score;
  }
}