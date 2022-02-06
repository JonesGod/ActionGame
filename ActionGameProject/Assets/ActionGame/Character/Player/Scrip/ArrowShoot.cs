using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowShoot : MonoBehaviour
{
    public GameObject prefab;
    private ArrowLoad load;

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
        //if (Input.GetMouseButtonDown(0) && PlayerInput.Instance.bowShoot)
        //{
        //    GameObject go = load.LoadArrow();
        //    go.transform.position = transform.position;
        //    go.transform.forward = transform.forward;
        //    go.SetActive(true);
        //}
    }
    void Shoot()
    {
        GameObject go = load.LoadArrow();
        go.transform.position = transform.position;//�վ�b�ڦ�m���}����m
        go.transform.forward = transform.forward;//�վ�b�ګe�謰�}�o�e��
        go.SetActive(true);
    }
}
