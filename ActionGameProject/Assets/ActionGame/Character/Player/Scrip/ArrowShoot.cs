using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowShoot : MonoBehaviour
{
    Transform cameraTrasform;
    public GameObject prefab;

    public float charge;//蓄力時間(由Player)

    private ArrowLoad load;
    private Arrow arrow;

    private Vector3 targetDirection;//準星目標方向(相機正前方)
    private Vector3 arrowDirection;//箭矢飛行方向
    private Vector3 arrowPosition;//箭矢生成方向
    private float targetDistance;//目標到攝影機的距離

    private void Awake()
    {
        load = new ArrowLoad();
        load.creatArrow(prefab, 30);
    }
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        cameraTrasform = Camera.main.transform;
        arrowDirection = transform.forward;

        Ray r = new Ray(cameraTrasform.position, cameraTrasform.forward);
        if (Physics.Raycast(r, out RaycastHit hit, 80f))
        {
            targetDirection = hit.point - cameraTrasform.position;
            arrowDirection = targetDirection;
            targetDistance = targetDirection.magnitude;
            if (targetDistance < 15f)                      //準星目標離太近時
            {
                targetDirection = cameraTrasform.position + cameraTrasform.forward * 15f;
            }
        }
    }
    void Shoot()
    {
        GameObject go = load.LoadArrow();        
        arrow=go.GetComponent<Arrow>();
        if (charge < 1.5f)          //決定這一箭是普通箭還是爆炸箭
        {
            arrow.transform.name = "Arrow(Clone)";
            arrow.IsNormal();
        }
        else
        {
            //arrow.transform.name = "explode";
            arrow.IsExplode();
        }

        go.transform.position = transform.position;//調整箭矢位置為弓的位置
        go.transform.forward = arrowDirection;//調整箭矢前方為弓得前方
        go.SetActive(true);
    }
    public void GetCharge(float ch)
    {
        charge = ch;
    }
}
