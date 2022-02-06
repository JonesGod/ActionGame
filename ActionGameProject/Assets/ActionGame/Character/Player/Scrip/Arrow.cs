using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float arrowSpeed = 30.0f;//箭矢飛行速度
    public float gravity ;//重力

    private float fallSpeed;//墜落速度
  
    // Start is called before the first frame update
    private void Awake()
    {
       
    }
    void Start()
    {
        gravity = 6.0f;
        fallSpeed = 0.0f;
    }
    // Update is called once per frame
    void Update()
    {
        transform.position += transform.forward * arrowSpeed * Time.deltaTime;//箭矢往前飛行速度
        fallSpeed += gravity*Time.deltaTime;//重力計算
        transform.position-= fallSpeed * Vector3.up*Time.deltaTime;//箭矢墜落速度
    }
    private void OnTriggerEnter(Collider other)
    {
        fallSpeed = 0.0f;//初始化箭矢的墜落速度
        gameObject.SetActive(false);
    }
}
