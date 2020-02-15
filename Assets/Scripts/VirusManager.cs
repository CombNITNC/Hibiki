using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VirusManager : MonoBehaviour {
  [SerializeField] GameObject burst = null;
  [SerializeField] GameObject[] spawnableEnemies = new GameObject[3];
  [SerializeField] GameObject player;

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
    var pos = v.VirusPosition;
    var to = new Vector3(
      pos.X * 1.2f - 3.0f,
      0f, -pos.Y * 1.2f + 15f
    );
    virus.Apply(to, false);
    viruses.Add(v.VirusId, virus);
  }

  void OnMove(Ruling.Virus.Id id, Ruling.Position from, Ruling.Position to) {
    var v = viruses[id];
    foreach (var found in board.VirusFromId(id)) {
      var dst = new Vector3(
        to.X * 1.2f - 3.0f,
        0f, -to.Y * 1.2f + 15f
      );
      if (to.IsHand) {
        dst = player.transform.position + new Vector3(0, 0, 1);
      }
      v.Apply(dst, found.isCracked);
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