using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyCamera : MonoBehaviour
{
    public float MainSpeed = 100;
    public float Shiftspeed = 250;
    public float MaxShiftSpeed = 1000;
    public float CamSen = 0.25f;
    private Vector3 LastMouse = new Vector3(255, 255, 255);
    private float TotalRun = 1;

    private void Update()
    {
        LastMouse = Input.mousePosition - LastMouse;
        LastMouse = new Vector3(-LastMouse.y * CamSen, LastMouse.x * CamSen, 0);
        LastMouse = new Vector3(transform.eulerAngles.x + LastMouse.x, transform.eulerAngles.y + LastMouse.y, 0);
        if (Input.GetMouseButton(0))
        {
            transform.eulerAngles = LastMouse;
        }
        LastMouse = Input.mousePosition;


        float f = 0;
        Vector3 p = GetBaseInput();

        if (Input.GetKey(KeyCode.LeftShift))
        {
            TotalRun += Time.deltaTime;
            p *= TotalRun * Shiftspeed;
            p.x = Mathf.Clamp(p.x, -MaxShiftSpeed, MaxShiftSpeed);
            p.y = Mathf.Clamp(p.y, -MaxShiftSpeed, MaxShiftSpeed);
            p.z = Mathf.Clamp(p.z, -MaxShiftSpeed, MaxShiftSpeed);
        }
        else
        {
            TotalRun = Mathf.Clamp(TotalRun * 0.5f, 1, 1000);
            p *= MainSpeed;
        }

        p *= Time.deltaTime;
        if (Input.GetKey(KeyCode.Space))
        {
            f = transform.position.y;
            transform.Translate(p);
            transform.position = new Vector3(transform.position.x, f, transform.position.z);
        }
        else
        {
            transform.Translate(p);
        }
    }

    private Vector3 GetBaseInput()
    {
        Vector3 Vel = Vector3.zero;
        if (Input.GetKey(KeyCode.W))
        {
            Vel += Vector3.forward;
        }
        if ( Input.GetKey(KeyCode.S) )
        {
            Vel += -Vector3.forward;
        }
        if ( Input.GetKey(KeyCode.A) )
        {
            Vel += -Vector3.right;
        }
        if ( Input.GetKey(KeyCode.D) )
        {
            Vel += Vector3.right;
        }
        return Vel;
    }
}
