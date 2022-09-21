using System.Collections;
 using System.Collections.Generic;
 using UnityEngine;
 
 public class AlwaysFaceCamera : MonoBehaviour 
 {
 
 
     public Transform cameraToFace;
 
 
     // Use this for initialization
     void Start () 
     {
 
         cameraToFace = Camera.main.transform;
 
     }
 
     // Update is called once per frame
     void Update()
     {
         // Rotate the camera every frame so it keeps looking at the target
         transform.LookAt(cameraToFace);
     }
 
 }
