using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FolowCamera : MonoBehaviour
{
    public static FolowCamera Instance
    {
        get { return s_Instance; }
    }

    protected static FolowCamera s_Instance;

    public float cameraSpeed = 20.0f;
    public Transform lookTarget;
    public float followDistance;
    public Vector2 minMaxFollowDistance;
    public float cameraHeight;
    public float relativeDistance = 2.0f;
    public float bowStateY = 2.25f;

    public bool isSwitch = false;

    private float horizontalAngle;
    private float verticalAngle;
    private float zeroPoint;
    private float distance;

    Quaternion playerRotate;

    Vector2 mouseInput;
    Vector2 moveInput;

    Vector3 lookTargetPosition;
    Vector3 cameraForward;
    Vector3 relativeVector;

    Vector3 relativeForward;
    Vector3 bowPosition;
    Vector3 normalPosition;
    Vector3 nextPosition;
    Vector3 lastPosition;
    Vector3 direct;


    [HideInInspector] public Vector3 horizontalVector;
    [HideInInspector] public Vector3 cameraRight;
    public LayerMask checkHitLayerMask;

    Vector3 cameraPosition;
    private void Awake()
    {
        s_Instance = this;
    }
    void Start()
    {
        horizontalVector = lookTarget.transform.forward;
        lookTargetPosition = lookTarget.position + new Vector3(0.0f, 2.0f, 0.0f);

        relativeVector = Quaternion.AngleAxis(165f, Vector3.up) * horizontalVector;
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

        CameraRotate();
        BowCameraRotate();

        if (Input.GetButtonDown("Switch") && !isSwitch && PlayerInput.Instance.cantBowState
            && PlayerInput.Instance.rollToBow)                                   //切換中判斷
        {
            isSwitch = true;
            zeroPoint = 0f;
        }
        if (PlayerInput.Instance.bowState)
        {
            BowVisionLimit();
            lastPosition = normalPosition;
            nextPosition = bowPosition;
            
            Switch();

            playerRotate = Quaternion.LookRotation(horizontalVector);
            lookTarget.rotation = playerRotate;
        }
        else
        {
            NormalVisionLimit();
            lastPosition = bowPosition;
            nextPosition = normalPosition;

            Switch();

            WallDetect(); //牆壁檢測
        }


        transform.forward = cameraForward;
        transform.position = cameraPosition;
    }

    void OnDrawGizmos()
    {

        Gizmos.color = new Color(1.0f, 0.0f, 0.0f);
        Gizmos.DrawLine(cameraRight, cameraRight * 3);
        Gizmos.color = new Color(0.0f, 1.0f, 0.0f);
        Gizmos.DrawLine(transform.position, cameraPosition);
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
    }
    void BowCameraRotate()
    {
        relativeVector = Quaternion.AngleAxis(horizontalAngle + moveInput.x, Vector3.up) * relativeVector;
        relativeVector.Normalize();
        relativeForward = Quaternion.AngleAxis(verticalAngle, -cameraRight) * relativeVector;
        relativeForward.Normalize();

        bowPosition = lookTarget.position + new Vector3(0.0f, bowStateY, 0.0f) + relativeForward * relativeDistance;
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

        normalPosition = lookTarget.position + new Vector3(0.0f, cameraHeight, 0.0f) - (cameraForward * followDistance);
    }
    void Switch()
    {
        direct = nextPosition - lastPosition;
        distance = direct.magnitude;
        direct.Normalize();
        zeroPoint = Mathf.Lerp(zeroPoint, distance, 0.1f);
        cameraPosition = lastPosition + zeroPoint * direct;

        if ((cameraPosition - nextPosition).magnitude < 0.05f)
        {
            isSwitch = false;
            cameraPosition = nextPosition;
        }
    }
    void NormalVisionLimit()
    {
        if (verticalAngle > 20.0f)
            verticalAngle = 20.0f;
        if (verticalAngle < -60.0f)
            verticalAngle = -60.0f;
    }
    void BowVisionLimit()
    {
        if(verticalAngle > 20.0f)
            verticalAngle = 20.0f;
        if (verticalAngle < -21f)
            verticalAngle = -21f;
    }
}
