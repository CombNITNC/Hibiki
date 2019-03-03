using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameState : MonoBehaviour {
  [SerializeField] GameObject gameoverDisplay;
  [SerializeField] Transform player;
  [SerializeField] GameObject burst;
  bool isOver = false;

  public void Gameover() {
    if (isOver) return;
    isOver = true;
    StartCoroutine(GameoverWork());
  }

  IEnumerator GameoverWork() {
    yield return new WaitForSeconds(1f);
    gameoverDisplay.SetActive(true);
    Instantiate(burst, player.position, Quaternion.identity);
    Destroy(player.gameObject);
  }
}