using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ScoreManager))]
public class VirusManager : MonoBehaviour {
  [SerializeField] GameObject burst;

  GameState state;
  ScoreManager scoreManager;

  List<List<Virus>> viruses = new List<List<Virus>>();
  Virus held = null;

  [SerializeField] GameObject[] spawnableEnemies = new GameObject[3];

  public float spawnRate = 7.5f;

  void Start() {
    state = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameState>();
    scoreManager = GetComponent<ScoreManager>();
    for (var i = 0; i < 5; ++i) viruses.Add(new List<Virus>());
    StartCoroutine(SpawnWork());
  }

  bool operating = false;

  IEnumerator SpawnWork() {
    while (true) {
      var pos = new Vector3(-2.4f, 0f, 15f);
      foreach (var row in viruses) {
        var enemyBase = spawnableEnemies[Random.Range(0, spawnableEnemies.Length)];
        var newEnemy = Instantiate(enemyBase, pos, Quaternion.identity);
        var virus = newEnemy.GetComponent<Virus>();
        virus.SetBurster(Burst);
        row.Add(virus);
        pos += new Vector3(1.2f, 0f, 0f);
      }
      UpdateBoard();
      yield return new WaitForSeconds(spawnRate);
      yield return new WaitUntil(() => !operating);
    }
  }

  public Virus GetHeld() {
    return held;
  }

  void UpdateBoard() {
    for (int row = 0; row < viruses.Count; ++row) {
      viruses[row].RemoveAll(e => e == null);
      for (int line = 0; line < viruses[row].Count; ++line) {
        var virus = viruses[row][line];
        if (virus) Align(virus, row, line, viruses[row].Count);
      }
      if (11 < viruses[row].Count)
        state.Gameover();
    }
  }

  void Align(Virus v, int row, int line, int depth) {
    var aligned = new Vector3();
    aligned.x = (row - 2) * 1.2f;
    aligned.y = 0f;
    aligned.z = 13.5f - depth + line;
    StartCoroutine(Move(v, aligned));
  }

  IEnumerator Move(Virus v, Vector3 to, Callback callback = null) {
    var start = Time.time;
    var src = v.transform.position;
    while (Time.time - start < 0.13f) {
      v.transform.position = Vector3.Lerp(src, to, (Time.time - start) / 0.13f);
      yield return null;
    }
    v.transform.position = to;
    if (callback != null) callback();
  }

  public delegate void Callback();

  public void Take(int rowIndex, Callback callback) {
    operating = true;
    held = viruses[rowIndex][0];
    viruses[rowIndex].RemoveAt(0);
    held.transform.parent = transform;
    StartCoroutine(Move(held, transform.position + new Vector3(0, 0, 1), delegate() {
      operating = false;
    }));
    UpdateBoard();
    callback();
  }

  public void Place(int rowIndex, Callback callback) {
    operating = true;
    var row = viruses[rowIndex];
    if (0 < row.Count && row[0].GetGrade() - held.GetGrade() == 1) {
      if (1 < row.Count && row[1].GetGrade() - row[0].GetGrade() == 1) {
        StartCoroutine(Move(held, row[0].transform.position, delegate() {
          row[0].Hit();
          StartCoroutine(Move(row[0], row[1].transform.position, delegate() {
            row[1].Hit();
            row[1].Hit();
            operating = false;
          }));
        }));
      } else {
        var willDestroy = held;
        StartCoroutine(Move(held, row[0].transform.position, delegate() {
          row[0].Hit();
          Destroy(willDestroy.gameObject);
          operating = false;
        }));
      }
    } else {
      row.Insert(0, held);
    }
    held.transform.parent = null;
    held = null;
    UpdateBoard();
    callback();
  }

  void Burst(Virus target) {
    var pos = FindPos(target);
    if (pos.Length < 1) return;
    BurstPos(pos[0], pos[1]);
  }

  void BurstPos(int row, int line) {
    if (viruses[row][line] == null) return;
    var looking = viruses[row][line].GetGrade();
    StartCoroutine(GenerateBursts(viruses[row][line]));
    viruses[row][line] = null;
    if (0 < row &&
      line < viruses[row - 1].Count &&
      viruses[row - 1][line] &&
      viruses[row - 1][line].GetGrade() == looking) BurstPos(row - 1, line);
    if (0 < line &&
      line - 1 < viruses[row].Count &&
      viruses[row][line - 1] &&
      viruses[row][line - 1].GetGrade() == looking) BurstPos(row, line - 1);
    if (row < viruses.Count - 1 &&
      line < viruses[row + 1].Count &&
      viruses[row + 1][line] &&
      viruses[row + 1][line].GetGrade() == looking) BurstPos(row + 1, line);
    if (line < viruses[row].Count - 1 &&
      viruses[row][line + 1] &&
      viruses[row][line + 1].GetGrade() == looking) BurstPos(row, line + 1);
  }

  IEnumerator GenerateBursts(Virus v) {
    yield return new WaitForSeconds(0.2f);
    Instantiate(burst, v.transform.position, Quaternion.identity);
    Destroy(v.gameObject);
  }

  int[] FindPos(Virus target) {
    for (int row = 0; row < 5; ++row) {
      var line = viruses[row].FindIndex(e => e.transform.position == target.transform.position);
      if (line != -1) {
        return new [] { row, line };
      }
    }
    return new int[] { };
  }
}