using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class LookAtCamera : MonoBehaviour
{
    Camera m_camera;
    Vector3 m_cameraPosition;
    Quaternion m_angleToLookAt;

    void Start()
    {
        m_camera = Camera.main;
    }
// Update is called once per frame
    void Update()
    {
        Vector3 target = Camera.main.transform.position;
        transform.LookAt(target);// = angle + new Vector3(-90,0,-90);
        transform.eulerAngles += new Vector3(-90, 0, 0);
    }
}
