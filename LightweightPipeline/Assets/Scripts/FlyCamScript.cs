using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyCamScript : MonoBehaviour
{
    struct CameraState
    {
        public float yaw;
        public float pitch;
        public float roll;
        public float x;
        public float y;
        public float z;

        public void setFromTransform(Transform t)
        {
            pitch = t.eulerAngles.x;
            yaw = t.eulerAngles.y;
            roll = t.eulerAngles.z;
            x = t.position.x;
            y = t.position.y;
            z = t.position.z;
        }

        public void Translate(Vector3 delta)
        {
            Quaternion q = new Quaternion();
            q.eulerAngles = new Vector3(pitch, yaw, roll);

            Vector3 rotated= q * delta;

            x += rotated.x;
            y += rotated.y;
            z += rotated.z;
        }

        public void LerpTowards(CameraState target, float lerpSpeed)
        {
            float rspeed = 0.25f;
            yaw =       Mathf.Lerp(yaw,     target.yaw,     rspeed);
            pitch =     Mathf.Lerp(pitch,   target.pitch,   rspeed);
            roll =      Mathf.Lerp(roll,    target.roll,    rspeed);

            float speed = lerpSpeed;
            x =         Mathf.Lerp(x,       target.x,       speed);
            y =         Mathf.Lerp(y,       target.y,       speed);
            z =         Mathf.Lerp(z,       target.z,       speed);
        }

        public void updateTransform(Transform t)
        {
            t.eulerAngles = new Vector3(pitch, yaw, roll);
            t.position = new Vector3(x, y, z);
        }
    }

//    public float mainSpeed = 100.0f;        // regular speed
//    public float shiftAdd = 250.0f;         // multiplied by how long shift is held.  Basically running
//    public float maxShift = 1000.0f;        // Maximum speed when holdin gshift

    public bool rotateOnlyIfMousedown = true;
    public bool movementStaysFlat = true;


//    private Vector3 lastMouse = new Vector3(255, 255, 255);
//    private Vector3 cacheMouse = new Vector3(255, 255, 255);

    private CameraState camState = new CameraState();
    private CameraState curState = new CameraState();

    public float boost;
    public float lerpSpeed;

    // Use this for initialization
    void Start ()
    {
		
	}

    void Awake()
    {
        // save off existing camera transform
        camState.setFromTransform(transform);
        curState.setFromTransform(transform);

        //boost =  0.0f;
    }

    private Vector3 GetKeyboardDelta()
    {
        Vector3 delta = new Vector3();
        if (Input.GetKey(KeyCode.W))
        {
            delta += new Vector3(0, 0, 1);
        }
        if (Input.GetKey(KeyCode.S))
        {
            delta += new Vector3(0, 0, -1);
        }
        if (Input.GetKey(KeyCode.A))
        {
            delta += new Vector3(-1, 0, 0);
        }
        if (Input.GetKey(KeyCode.D))
        {
            delta += new Vector3(1, 0, 0);
        }
        return delta;
    }

    // Update is called once per frame
    void Update ()
    {
        float camSens = 1.50f;           // How sensitive it with mouse
        float camPow = 2.00f;

        if (Input.GetMouseButtonDown(1))
        {
//            lastMouse = Input.mousePosition;
//            cacheMouse = Input.mousePosition;
//            Cursor.visible= false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        if (Input.GetMouseButtonUp(1))
        {
            Cursor.visible = true;

            // reset position ??
//            Cursor.lockState = CursorLockMode.Locked;
            Cursor.lockState = CursorLockMode.None;
        }


        //            Vector3 delta = Input.mousePosition - lastMouse;
        if (!rotateOnlyIfMousedown ||
            (rotateOnlyIfMousedown && Input.GetMouseButton(1)))
        {
            float mx = Input.GetAxis("Mouse X");
            float my = Input.GetAxis("Mouse Y");

            float len = Mathf.Sqrt(mx * mx + my * my);
            if (len > 0.0f)
            {
                float powlen = Mathf.Pow(len, camPow);
                float scale = (camSens * powlen / len);
                mx = mx * scale;
                my = -my * scale;       // non-inverted Y
            }

            camState.yaw += mx;
            camState.pitch += my;
        }
            // save for next Update()
//            lastMouse = Input.mousePosition;

            //Mouse  camera angle done.  

            Vector3 delta = GetKeyboardDelta() * Time.deltaTime;

            boost += Input.mouseScrollDelta.y * 0.2f;

            if (Input.GetKey(KeyCode.LeftShift))
            {
                delta *= 10.0f;
            }

            delta *= Mathf.Pow(2.0f, boost);

            camState.Translate(delta);
        

        curState.LerpTowards(camState, lerpSpeed);

        curState.updateTransform(transform);
    }
}
