using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerPositioner), typeof(VirusCapturer))] public class PlayerController : MonoBehaviour {
  PlayerPositioner positioner;
  VirusCapturer capturer;
  // Start is called before the first frame update
  void Start() {
    positioner = GetComponent<PlayerPositioner>();
    capturer = GetComponent<VirusCapturer>();
  }

  // Update is called once per frame
  void Update() {
    if (Input.GetButtonDown("Left")) {
      positioner.MoveLeft();
    } else if (Input.GetButtonDown("Right")) {
      positioner.MoveRight();
    } else if (Input.GetButtonDown("Capture")) {
      capturer.Capture();
    }
  }
}