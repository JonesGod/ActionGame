using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    private float arrowSpeed = 40.0f;//�b�ڭ���t��
    private float gravity=3f ;//���O
    private float liveTime=0.0f;//�s�b�ɶ�
    private float fallSpeed=0.0f;//�Y���t��
    private float explodeRadius = 15f;//�z���b�|

    private bool explodeFlag=false;//�O�_���z���b

    private List<FSMBase> monster;//�s���Ǫ���T

    // Start is called before the first frame update
    private void Awake()
    {

    }
    void Start()
    {
        monster = new List<FSMBase>();
        GameObject[] allMonster = GameManager.Instance.allMonster;//�N������tag��Monster������s�_��
        if (allMonster != null || allMonster.Length > 0)
        {
            foreach (GameObject m in allMonster)
            {
                monster.Add(m.GetComponent<FSMBase>());
            }
        }
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
        if (explodeFlag)
            ArrowExplode();
        else
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
    void ArrowExplode()
    {
        liveTime = 0.0f;//��l�Ʀs�b�ɶ�
        fallSpeed = 0.0f;//��l�ƽb�ڪ��Y���t��
       
        for(int i=0;i<monster.Count;i++)
        {
            float dis = (monster[i].transform.position - transform.position).magnitude;
            //if (dis < explodeRadius)
            //    monster.hurt;
        }
        gameObject.SetActive(false);
    }
    public void IsExplode()
    {
        explodeFlag = true;
    }
    public void IsNormal()
    {
        explodeFlag = false;
    }
}
