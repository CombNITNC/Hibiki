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

  void Update() {
    if (Input.GetButtonDown("Left")) {
      positioner.MoveLeft();
    } else if (Input.GetButtonDown("Right")) {
      positioner.MoveRight();
    } else if (Input.GetButtonDown("Capture")) {
      var held = viruses.GetHeld();
      if (held == null) {
        viruses.Take(2 + positioner.GetPosition());
      } else {
        viruses.Place(2 + positioner.GetPosition());
      }
    }
  }
}