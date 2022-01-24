using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FolowCamera : MonoBehaviour
{
    public float cameraSpeed = 20.0f;
    public Transform lookTarget;
    public float followDistance;
    public Vector2 minMaxFollowDistance;
    public float cameraHeight;
    public float relativeDistance=2.0f;
    public float bowStateY = 2.25f;

    private float horizontalAngle;
    private float verticalAngle;

    private bool isSwitch=false;

    Vector2 mouseInput;
    Vector2 moveInput;

    Vector3 lookTargetPosition;
    Vector3 cameraForward;
    Vector3 relativeVector;
    Vector3 relativePoint;
    Vector3 relativeForward;
    Vector3 bowPosition;

    [HideInInspector] public Vector3 horizontalVector;
    [HideInInspector] public Vector3 cameraRight;
    public LayerMask checkHitLayerMask;

    Vector3 cameraPosition;
    void Start()
    {
        horizontalVector = lookTarget.transform.forward;
        lookTargetPosition = lookTarget.position + new Vector3(0.0f, 2.0f, 0.0f);

        relativePoint = lookTarget.position + new Vector3(-1.6f, 0.0f, -0.4f);       
        relativeVector = relativePoint-lookTarget.position;
    }

    void Update()
    {
        UpdateCamera();
    }

    void UpdateCamera()
    {
        mouseInput = PlayerInput.Instance.MouseInput;
        moveInput = PlayerInput.Instance.MoveInput / cameraSpeed;
        horizontalAngle = mouseInput.x;
        verticalAngle += mouseInput.y;
        if (verticalAngle > 20.0f)
            verticalAngle = 20.0f;
        if (verticalAngle < -60.0f)
            verticalAngle = -60.0f;
        
        CameraRotate();
        BowCameraRotate();

        if (Input.GetButtonDown("Switch") && !isSwitch)
        {
            isSwitch = true;
            StartCoroutine(BowSwitch());
        }
        else
        {
            if (PlayerInput.Instance.bowState)
            {
                bowPosition = lookTarget.position + new Vector3(0.0f, bowStateY, 0.0f) + relativeForward * relativeDistance;
                cameraPosition = bowPosition;

                var xc = Quaternion.LookRotation(horizontalVector);
                lookTarget.rotation = xc;
            }
            else
            {
                WallDetect(); //牆壁檢測
            }
        }

        transform.forward = cameraForward;
        transform.position = cameraPosition;
    }

    void OnDrawGizmos()
    {
  
        Gizmos.color = new Color(1.0f, 0.0f, 0.0f);
        Gizmos.DrawLine(cameraRight, cameraRight * 3);
        Gizmos.color = new Color(0.0f, 1.0f, 0.0f);
        Gizmos.DrawLine(relativePoint, cameraPosition);
    }
    void WallDetect()
    {
        lookTargetPosition = lookTarget.position + new Vector3(0.0f, 2.0f, 0.0f);
        if (Physics.Raycast(lookTargetPosition, -cameraForward, out RaycastHit rh, followDistance, checkHitLayerMask))
        {
            Vector3 hitRayDir = rh.point - lookTarget.position;
            float hitRayLength = hitRayDir.magnitude;

            Vector3 newCameraPosition = rh.point + rh.normal * 0.5f;//固定住攝影機的位置(不要再後退了) 

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
    }
    void BowCameraRotate()
    {     
        relativeVector = Quaternion.AngleAxis(horizontalAngle, Vector3.up) * relativeVector;
        relativeVector.Normalize();
        relativeForward = Quaternion.AngleAxis(verticalAngle, -cameraRight) * relativeVector;
        relativeForward.Normalize();
    }
    /// <summary>
    /// vector = Quaternion.AngleAxis(角度, 旋轉軸向量) * 欲旋轉向量;
    /// </summary>
    void CameraRotate()
    {
        if (PlayerInput.Instance.attackState || PlayerInput.Instance.bowState)
            moveInput.x = 0f;

        horizontalVector = Quaternion.AngleAxis(horizontalAngle + moveInput.x, Vector3.up) * horizontalVector;
        horizontalVector.Normalize();
        cameraRight = Vector3.Cross(Vector3.up, horizontalVector);
        cameraForward = Quaternion.AngleAxis(verticalAngle, -cameraRight) * horizontalVector;
        cameraForward.Normalize();
    }
    IEnumerator BowSwitch()
    {
        float time = 0.0f;
        while (time <= 0.5f)
        {
            time += Time.deltaTime;
            cameraPosition = Vector3.Lerp(cameraPosition, bowPosition, 0.2f);
            Debug.Log(time);
            yield return null;
        }
        isSwitch = false;
    }
}
