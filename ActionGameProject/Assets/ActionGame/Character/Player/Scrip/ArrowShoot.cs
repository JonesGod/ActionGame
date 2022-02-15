using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowShoot : MonoBehaviour
{
    Transform cameraTrasform;
    public GameObject prefab;

    private ArrowLoad load;
    private Vector3 targetDirection;//�ǬP�ؼФ�V(�۾����e��)
    private Vector3 arrowDirection;//�b�ڭ����V
    private float targetDistance;//�ؼШ���v�����Z��

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
            if (targetDistance < 15f)                      //�ǬP�ؼ����Ӫ��
            {
                targetDirection = cameraTrasform.position + cameraTrasform.forward * 15f;
            }
        }
    }
    void Shoot()
    {
        GameObject go = load.LoadArrow();
        go.transform.position = transform.position;//�վ�b�ڦ�m���}����m
        go.transform.forward = arrowDirection;//�վ�b�ګe�謰�}�o�e��
        go.SetActive(true);
    }
}
