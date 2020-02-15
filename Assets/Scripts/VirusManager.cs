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

  public void Take(int rowIndex) {
    StartCoroutine(TakeWork(rowIndex));
  }

  IEnumerator TakeWork(int rowIndex) {
    while (operating) yield return null;
    operating = true;
    held = viruses[rowIndex].Find(e => e != null);
    if (!held) { operating = false; yield break; }
    viruses[rowIndex].RemoveAt(0);
    held.transform.SetParent(transform);
    UpdateBoard();
    StartCoroutine(Move(held, transform.position + new Vector3(0, 0, 1), delegate() {
      operating = false;
    }));
  }

  public void Place(int rowIndex) {
    StartCoroutine(PlaceWork(rowIndex));
  }

  IEnumerator PlaceWork(int rowIndex) {
    while (operating) yield return null;
    operating = true;
    var row = viruses[rowIndex];
    if (0 < row.Count && row[0] && row[0].GetGrade() - held.GetGrade() == 1) {
      var willDestroy = held.gameObject;
      var hp = held.GetHP();
      StartCoroutine(Move(held, row[0].transform.position, delegate() {
        row[0].Hit();
        if (row[0] && 1 <= hp)
          row[0].Hit();
        Destroy(willDestroy);
        UpdateBoard();
        operating = false;
      }));
      yield break;
    }
    row.Insert(0, held);
    held.transform.parent = null;
    held = null;
    UpdateBoard();
    operating = false;
  }

  void Burst(Virus target) {
    operating = true;
    var pos = FindPos(target);
    if (pos.Length < 1) return;
    BurstPos(pos[0], pos[1]);
    operating = false;
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
    scoreManager.Point(10);
  }

  int[] FindPos(Virus target) {
    for (int row = 0; row < 5; ++row) {
      var line = viruses[row].FindIndex(e => e.id == target.id);
      if (line != -1) {
        return new [] { row, line };
      }
    }
    return new int[] { };
  }
}