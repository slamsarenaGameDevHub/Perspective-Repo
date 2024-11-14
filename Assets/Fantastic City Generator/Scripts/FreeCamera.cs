using UnityEngine;

public class FreeCamera : MonoBehaviour
{
    public bool enableInputCapture = true;
    public bool lockAndHideCursor = false;
    public bool holdRightMouseCapture = true;

    public float lookSpeed = 5f;
    public float moveSpeed = 5f;
    public float sprintSpeed = 50f;

    float m_yaw;
    float m_pitch;
    bool fly = false; 

    void CaptureInput(bool f)
    {
        if (f)
        {
            m_yaw = transform.eulerAngles.y;
            m_pitch = transform.eulerAngles.x;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        } 
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        fly = f;
    }

       

    void Update()
    {

      
        if (Input.GetMouseButton(1))
        {
            if (!fly)
            CaptureInput(true);
        }
        else
        {
            if (fly)
            CaptureInput(false);
            return;
        }
       

        CaptureInput(true);

        var rotStrafe = Input.GetAxis("Mouse X");
        var rotFwd = Input.GetAxis("Mouse Y");

        m_yaw = (m_yaw + lookSpeed * rotStrafe) % 360f;
        m_pitch = (m_pitch - lookSpeed * rotFwd) % 360f;
        transform.rotation = Quaternion.AngleAxis(m_yaw, Vector3.up) * Quaternion.AngleAxis(m_pitch, Vector3.right);

        var speed = Time.deltaTime * (Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : moveSpeed);

        var forward = speed * Input.GetAxis("Vertical");
        var right = speed * Input.GetAxis("Horizontal");

        var up = speed * ((Input.GetKey(KeyCode.E) ? 1f : 0f) - (Input.GetKey(KeyCode.Q) ? 1f : 0f));

        transform.position += transform.forward * forward + transform.right * right + Vector3.up * up;

    }

}
