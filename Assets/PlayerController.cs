using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerPositioner), typeof(VirusCapturer), typeof(ScoreManager))] public class PlayerController : MonoBehaviour {
  PlayerPositioner positioner;
  ScoreManager scoreManager;
  VirusCapturer capturer;
  // Start is called before the first frame update
  void Start() {
    positioner = GetComponent<PlayerPositioner>();
    capturer = GetComponent<VirusCapturer>();
    scoreManager = GetComponent<ScoreManager>();
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
      capturer.Capture(delegate() {
        atomic = false;
      }, delegate() {
        scoreManager.Point(10);
      });
    }
  }
}