using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float arrowSpeed = 30.0f;//�b�ڭ���t��
    public float gravity ;//���O

    private float fallSpeed;//�Y���t��
  
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
        transform.position += transform.forward * arrowSpeed * Time.deltaTime;//�b�ک��e����t��
        fallSpeed += gravity*Time.deltaTime;//���O�p��
        transform.position-= fallSpeed * Vector3.up*Time.deltaTime;//�b�ڼY���t��
    }
    private void OnTriggerEnter(Collider other)
    {
        fallSpeed = 0.0f;//��l�ƽb�ڪ��Y���t��
        gameObject.SetActive(false);
    }
}
