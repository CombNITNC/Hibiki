﻿using UnityEngine;

public class Rotator : MonoBehaviour {
  void Update() {
    transform.Rotate(transform.up, 1.2f);
  }
}