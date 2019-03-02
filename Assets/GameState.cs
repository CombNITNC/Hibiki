using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameState : MonoBehaviour {
  [SerializeField] GameObject gameoverDisplay;
  [SerializeField] Transform player;
  bool isOver = false;

  void Start() {

  }

  public void Gameover() {
    if (isOver) return;
    isOver = true;
    gameoverDisplay.SetActive(true);

    Destroy(player.gameObject);
  }
}