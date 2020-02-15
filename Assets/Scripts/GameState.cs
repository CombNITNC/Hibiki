using System.Collections;
using UnityEngine;

public class GameState : MonoBehaviour {
  [SerializeField] GameObject gameoverDisplay = null;
  [SerializeField] GameObject player = null;
  [SerializeField] GameObject burst = null;
  bool isOver = false;

  public void Gameover() {
    if (isOver) return;
    isOver = true;
    StartCoroutine(GameoverWork());
  }

  IEnumerator GameoverWork() {
    yield return new WaitForSeconds(1f);
    gameoverDisplay.SetActive(true);
    Instantiate(burst, player.transform.position, Quaternion.identity);
    Destroy(player);
  }
}