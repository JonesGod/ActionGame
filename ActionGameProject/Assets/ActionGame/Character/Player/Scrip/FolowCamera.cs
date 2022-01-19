using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FolowCamera : MonoBehaviour
{
    //156132156
    //000000000
    public float cameraSpeed = 10.0f;
    public Transform lookTarget;
    Vector3 lookTargetPosition;
    public float followDistance;
    public Vector2 minMaxFollowDistance;
    public float cameraHeight;
    private float horizontalAngle;
    private float verticalAngle;
    
    [HideInInspector] public Vector3 horizontalVector;
    [HideInInspector] public Vector3 cameraRight;
    public LayerMask checkHitLayerMask;

    Vector3 cameraPosition;
    void Start()
    {
        horizontalVector = lookTarget.transform.forward;
        lookTargetPosition = lookTarget.position + new Vector3(0.0f, 2.0f, 0.0f);
    }
    
    void Update()
    {
        UpdateCamera();       
    }

    void UpdateCamera()
    {
        float xAngle = Input.GetAxis("Mouse X");
        float yAngle = Input.GetAxis("Mouse Y");
        horizontalAngle = xAngle;
        verticalAngle += yAngle;

       Vector2 moveInput = PlayerInput.Instance.MoveInput/ cameraSpeed;

        if (verticalAngle > 20.0f)
            verticalAngle = 20.0f;
        if (verticalAngle < -60.0f)
            verticalAngle = -60.0f;
        //vector = Quaternion.AngleAxis(角度, 旋轉軸向量) * 欲旋轉向量;
        horizontalVector = Quaternion.AngleAxis(horizontalAngle + moveInput.x, Vector3.up) * horizontalVector;
        horizontalVector.Normalize();

        cameraRight = Vector3.Cross(Vector3.up, horizontalVector);
        Vector3 cameraForward = Quaternion.AngleAxis(verticalAngle, -cameraRight) * horizontalVector;
        cameraForward.Normalize();        
        //牆壁檢測
        lookTargetPosition = lookTarget.position + new Vector3(0.0f, 2.0f, 0.0f);
        if (Physics.Raycast(lookTargetPosition, -cameraForward, out RaycastHit rh, followDistance, checkHitLayerMask))
        {
            Vector3 hitRayDir = rh.point - lookTarget.position;
            float hitRayLength = hitRayDir.magnitude;

            Vector3 newCameraPosition = rh.point + cameraForward * 0.5f + new Vector3(0.0f, 0.2f, 0.0f);//固定住攝影機的位置(不要再後退了)                        

            if (hitRayLength < minMaxFollowDistance.x)
            {
                float upDistance = minMaxFollowDistance.x - hitRayLength;
                cameraPosition = newCameraPosition + Vector3.up * upDistance;
            }
            else
            {
                cameraPosition = newCameraPosition;
            }
            cameraForward = lookTargetPosition - cameraPosition;
        }
        else
        {
            cameraPosition = lookTarget.position + new Vector3(0.0f, cameraHeight, 0.0f) - (cameraForward * followDistance);
        }

        transform.forward = cameraForward;
        transform.position = cameraPosition;       
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(1.0f, 0.0f, 0.0f);
        Gizmos.DrawLine(cameraRight, cameraRight * 3);
        Gizmos.color = new Color(0.0f, 1.0f, 0.0f);
        Gizmos.DrawLine(horizontalVector, horizontalVector * 3);
    }
}
