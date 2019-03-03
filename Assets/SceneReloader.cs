using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneReloader : MonoBehaviour {
  void Update() {
    if (Input.GetButtonDown("Submit")) {
      SceneManager.LoadSceneAsync(0, LoadSceneMode.Single);
    }
  }
}