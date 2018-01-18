using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour {

  public Transform unit1;
  public float speed;

  //Update is called once per frame
  void Update()
  {
    //Super simple follower
    transform.position = Vector3.Lerp(transform.position, unit1.position, speed * Time.deltaTime);
  }
}
