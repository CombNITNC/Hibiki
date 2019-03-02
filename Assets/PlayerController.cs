using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerPositioner), typeof(VirusManager))] public class PlayerController : MonoBehaviour {
  PlayerPositioner positioner;
  VirusManager viruses;
  // Start is called before the first frame update
  void Start() {
    positioner = GetComponent<PlayerPositioner>();
    viruses = GetComponent<VirusManager>();
  }

  bool atomic = false;

  // Update is called once per frame
  void Update() {
    if (atomic) return;
    if (Input.GetButtonDown("Left")) {
      atomic = true;
      positioner.MoveLeft(delegate() {
        atomic = false;
      });
    } else if (Input.GetButtonDown("Right")) {
      atomic = true;
      positioner.MoveRight(delegate() {
        atomic = false;
      });
    } else if (Input.GetButtonDown("Capture")) {
      atomic = true;
      var held = viruses.GetHeld();
      Debug.Log(held);
      if (held != null)
        viruses.Take(2 + positioner.GetPosition(), delegate() {
          atomic = false;
        });
      else
        viruses.Place(2 + positioner.GetPosition(), delegate() {
          atomic = false;
        });
    }
  }
}