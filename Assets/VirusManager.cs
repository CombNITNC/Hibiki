using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirusManager : MonoBehaviour {
  GameState state;
  List<List<Transform>> enemiesArray = new List<List<Transform>>();

  [SerializeField] GameObject[] spawnableEnemies = new GameObject[3];

  public float spawnRate = 7.5f;

  void Start() {
    state = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameState>();
    for (var i = 0; i < 5; ++i) enemiesArray.Add(new List<Transform>());
    StartCoroutine(SpawnWork());
  }

  IEnumerator SpawnWork() {
    while (true) {
      var pos = new Vector3(-2.4f, 0f, 15f);
      foreach (var row in enemiesArray) {
        var newEnemy = Instantiate(RandomEnemy(), pos, Quaternion.identity);
        row.Add(newEnemy.transform);
        pos += new Vector3(1.2f, 0f, 0f);
        row.RemoveAll(e => e == null);
        if (15 <= row.Count)
          state.Gameover();
      }
      foreach (var row in enemiesArray) {
        foreach (var enemy in row) {
          if (enemy && enemy.parent == null)
            StartCoroutine(MoveFoward(enemy));
        }
      }
      yield return new WaitForSeconds(spawnRate);
    }
  }

  GameObject RandomEnemy() {
    return spawnableEnemies[Random.Range(0, spawnableEnemies.Length)];
  }

  IEnumerator MoveFoward(Transform t) {
    var start = Time.time;
    var src = t.position;
    var dst = src - new Vector3(0f, 0f, 1f);
    while (Time.time - start < 0.13f) {
      t.position = Vector3.Lerp(src, dst, (Time.time - start) / 0.13f);
      yield return null;
      if (!t) yield break;
    }
    t.position = dst;
  }
}