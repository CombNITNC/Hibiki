using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VirusManager : MonoBehaviour {
  [SerializeField] GameObject burst = null;
  [SerializeField] GameObject bigEnemy = null;
  [SerializeField] GameObject midEnemy = null;
  [SerializeField] GameObject tinyEnemy = null;
  [SerializeField] GameObject player = null;
  [SerializeField] Mesh crackedMesh = null;

  Ruling.Board board;
  Dictionary<Ruling.Virus.Id, Virus> viruses = new Dictionary<Ruling.Virus.Id, Virus>();

  void Start() {
    var gcObj = GameObject.FindGameObjectWithTag("GameController");
    var gc = gcObj.GetComponent<GameController>();
    board = gc.board;
    board.Spawn += OnSpawn;
    board.Change += OnMove;
    board.Absorb += OnAbsorb;
    board.Break += OnBreak;
  }

  void OnSpawn(Ruling.Virus v) {
    GameObject enemyBase = null;
    switch (v.VirusGrade) {
      case Ruling.Virus.Grade.Big:
        enemyBase = bigEnemy;
        break;
      case Ruling.Virus.Grade.Mid:
        enemyBase = midEnemy;
        break;
      case Ruling.Virus.Grade.Tiny:
        enemyBase = tinyEnemy;
        break;
    }
    var newVirus = Instantiate(enemyBase, Vector3.zero, Quaternion.identity);
    Virus.Attach(newVirus, crackedMesh);
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

  void OnAbsorb(Ruling.Virus.Id eaterId, Ruling.Virus.Id eatenId) {
    var eater = viruses[eaterId];
    eater.Apply(eater.gameObject.transform.position, true);
    viruses[eatenId].Apply(eater.gameObject.transform.position, false);
    StartCoroutine(AbsorbWork(eatenId));
  }

  IEnumerator AbsorbWork(Ruling.Virus.Id eatenId) {
    yield return new WaitForSeconds(0.2f);
    Destroy(viruses[eatenId].gameObject);
  }

  void OnBreak(Ruling.Virus.Id id) {
    var v = viruses[id];
    var pos = v.transform.position;
    Destroy(v.gameObject);
    StartCoroutine(GenerateBursts(pos));
  }

  IEnumerator GenerateBursts(Vector3 pos) {
    yield return new WaitForSeconds(0.2f);
    Instantiate(burst, pos, Quaternion.identity);
  }
}