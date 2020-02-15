using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VirusManager : MonoBehaviour {
  [SerializeField] GameObject burst;
  [SerializeField] GameObject[] spawnableEnemies = new GameObject[3];

  Ruling.Board board;
  Dictionary<Ruling.Virus.Id, Virus> viruses = new Dictionary<Ruling.Virus.Id, Virus>();

  void Start() {
    var gcObj = GameObject.FindGameObjectWithTag("GameController");
    var gc = gcObj.GetComponent<GameController>();
    board = gc.board;
    board.Spawn += OnSpawn;
    board.Change += OnMove;
    board.Break += OnBreak;
  }

  void OnSpawn(Ruling.Virus v) {
    var enemyBase = spawnableEnemies[Random.Range(0, spawnableEnemies.Length)];
    var newVirus = Instantiate(enemyBase, Vector3.zero, Quaternion.identity);
    Virus.Attach(newVirus);
    var virus = newVirus.GetComponent<Virus>();
    virus.Apply(v);
    viruses.Add(v.VirusId, virus);
  }

  void OnMove(Ruling.Virus.Id id, Ruling.Position from, Ruling.Position to) {
    var v = viruses[id];
    var pos = v.gameObject.transform.position;
    foreach (var found in board.VirusFromId(id)) {
      v.Apply(found);
    }
  }

  Coroutine burstWork = null;

  void OnBreak(Ruling.Virus.Id id) {
    var v = viruses[id];
    if (burstWork != null) StopCoroutine(burstWork);
    burstWork = StartCoroutine(GenerateBursts(v));
  }

  IEnumerator GenerateBursts(Virus v) {
    yield return new WaitForSeconds(0.2f);
    Instantiate(burst, v.transform.position, Quaternion.identity);
    Destroy(v.gameObject);
  }
}