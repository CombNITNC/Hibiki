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
        if (12 < row.Count)
          state.Gameover();
      }
      UpdateBoard();
      yield return new WaitForSeconds(spawnRate);
    }
  }

  public Virus GetHeld() {
    return held;
  }

  void UpdateBoard() {
    if (held) StartCoroutine(Move(held, transform.position));
    for (int row = 0; row < viruses.Count; ++row) {
      for (int line = 0; line < viruses[row].Count; ++line) {
        var virus = viruses[row][line];
        if (virus) Align(virus, row, line, viruses[row].Count);
      }
      // CompressRow(viruses[row]);
    }
  }

  void Align(Virus v, int row, int line, int depth) {
    var aligned = new Vector3();
    aligned.x = (row - 2) * 1.2f;
    aligned.y = 0f;
    aligned.z = 13.5f - depth + line;
    StartCoroutine(Move(v, aligned));
  }

  IEnumerator Move(Virus v, Vector3 to) {
    var start = Time.time;
    var src = v.transform.position;
    while (Time.time - start < 0.13f) {
      v.transform.position = Vector3.Lerp(src, to, (Time.time - start) / 0.13f);
      yield return null;
    }
    v.transform.position = to;
  }

  void CompressRow(List<Virus> row) {
    int nullCount = 0, index = 0;
    do {
      if (null == row[index]) {
        ++nullCount;
        ++index;
      } else {
        for (int i = 0; i < index - nullCount; ++i) {
          row[i] = row[i + nullCount];
        }
        nullCount = index = 0;
      }
    } while (index <= 12);
    row.RemoveAll(e => e == null);
  }

  public delegate void Callback();

  public void Take(int rowIndex, Callback callback) {
    held = viruses[rowIndex][0];
    viruses[rowIndex].RemoveAt(0);
    UpdateBoard();
    callback();
  }

  public void Place(int rowIndex, Callback callback) {
    var row = viruses[rowIndex];
    row.Insert(0, held);
    held = null;
    UpdateBoard();
    callback();
  }

  void Burst(Virus target) {
    var pos = FindPos(target);
    if (2 <= pos.Length) return;
    BurstPos(pos[0], pos[1]);
  }

  void BurstPos(int row, int line) {
    var looking = viruses[row][line].GetGrade();
    bool hasLeft = 0 < row, hasUp = 0 < line, hasRight = row < 5, hasDown = line < 13;
    if (hasLeft && hasUp && viruses[row - 1][line - 1].GetGrade() == looking) BurstPos(row - 1, line - 1);
    if (hasLeft && viruses[row - 1][line].GetGrade() == looking) BurstPos(row - 1, line);
    if (hasUp && viruses[row][line - 1].GetGrade() == looking) BurstPos(row, line - 1);
    if (hasRight && hasDown && viruses[row + 1][line + 1].GetGrade() == looking) BurstPos(row + 1, line + 1);
    if (hasRight && viruses[row + 1][line].GetGrade() == looking) BurstPos(row + 1, line);
    if (hasDown && viruses[row][line + 1].GetGrade() == looking) BurstPos(row, line + 1);
    if (hasRight && hasUp && viruses[row + 1][line - 1].GetGrade() == looking) BurstPos(row + 1, line - 1);
    if (hasLeft && hasDown && viruses[row - 1][line + 1].GetGrade() == looking) BurstPos(row - 1, line + 1);
    Instantiate(burst, viruses[row][line].transform.position, Quaternion.identity);
    Destroy(viruses[row][line].gameObject);
  }

  int[] FindPos(Virus target) {
    for (int row = 0; row < 5; ++row) {
      for (int line = 0; line < 13; ++line) {
        if (target == viruses[row][line]) {
          return new int[] { row, line };
        }
      }
    }
    return new int[] { };
  }
}