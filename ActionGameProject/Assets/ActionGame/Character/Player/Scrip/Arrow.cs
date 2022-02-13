using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    private float arrowSpeed = 40.0f;//�b�ڭ���t��
    private float gravity=3f ;//���O
    private float liveTime=0.0f;//�s�b�ɶ�
    private float fallSpeed=0.0f;//�Y���t��
  
    // Start is called before the first frame update
    private void Awake()
    {
       
    }
    void Start()
    {

    }
    // Update is called once per frame
    void Update()
    {
        transform.position += transform.forward * arrowSpeed * Time.deltaTime;//�b�ک��e����t��
        fallSpeed += gravity*Time.deltaTime;//���O�p��
        transform.position-= fallSpeed * Vector3.up*Time.deltaTime;//�b�ڼY���t��

        liveTime += Time.deltaTime;//�p��s�b�ɶ�
        if(liveTime>=10f)//�s�b�W�L10��ɮ���
        {
            ArrowDestory();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        ArrowDestory();
    }
    /// <summary>
    /// �b�ڮ���
    /// </summary>
    void ArrowDestory()
    {
        liveTime = 0.0f;//��l�Ʀs�b�ɶ�
        fallSpeed = 0.0f;//��l�ƽb�ڪ��Y���t��
        gameObject.SetActive(false);
    }
}
