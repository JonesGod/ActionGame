using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FolowCamera : MonoBehaviour
{
    public Transform lookTarget;
    public float followDistance;
    public Vector2 minMaxFollowDistance;
    private float horizontalAngle;
    private float verticalAngle;    
    [HideInInspector]public Vector3 horizontalVector;
    [HideInInspector]public Vector3 cameraRight;
    public LayerMask checkHitLayerMask;

    void Start()
    {
        horizontalVector = lookTarget.transform.forward;        
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
        Debug.Log("horizontalAngle"+horizontalAngle); 
        Debug.Log("verticalAngle"+verticalAngle); 

        //vector = Quaternion.AngleAxis(角度, 旋轉軸向量) * 欲旋轉向量;
        horizontalVector = Quaternion.AngleAxis(horizontalAngle, Vector3.up) * horizontalVector;
        horizontalVector.Normalize();
        cameraRight = Vector3.Cross(Vector3.up, horizontalVector);
        Vector3 cameraForward = Quaternion.AngleAxis(verticalAngle, -cameraRight) * horizontalVector;
        cameraForward.Normalize();
        Vector3 cameraPosition = lookTarget.position;

        //牆壁檢測
        if(Physics.Raycast(lookTarget.position, -cameraForward, out RaycastHit rh, followDistance, checkHitLayerMask))
        {
            Vector3 hitRayDir = rh.point - lookTarget.position;
            float hitRayLength = hitRayDir.magnitude;
            Debug.Log("hitLength:" + hitRayLength);

            Vector3 newCameraPosition = rh.point + cameraForward * 0.3f;//固定住攝影機的位置(不要再後退了)     

            if(hitRayLength < minMaxFollowDistance.x)
            {
                float upDistance = minMaxFollowDistance.x - hitRayLength;
                cameraPosition = newCameraPosition + Vector3.up * upDistance;
            }
            else
            {
                cameraPosition = newCameraPosition;
            }
            cameraForward = lookTarget.position - cameraPosition;
        }
        else
        {
            cameraPosition = lookTarget.position - (cameraForward * followDistance);
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
