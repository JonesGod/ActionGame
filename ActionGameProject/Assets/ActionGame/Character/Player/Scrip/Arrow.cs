using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    private float arrowSpeed = 50.0f;//�b�ڭ���t��
    private float gravity=3f ;//���O
    private float liveTime=0.0f;//�s�b�ɶ�
    private float fallSpeed=0.0f;//�Y���t��
    private float explodeRadius = 15f;//�z���b�|

    private bool explodeFlag=false;//�O�_���z���b

    private List<FSMBase> monster;//�s���Ǫ���T
    private SphereCollider collider;

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

        collider = transform.GetComponent<SphereCollider>();
    }
    // Update is called once per frame
    void Update()
    {
        transform.position += transform.forward * arrowSpeed * Time.deltaTime;//�b�ک��e����t��
        fallSpeed += gravity*Time.deltaTime;//���O�p��
        transform.position-= fallSpeed * Vector3.up*Time.deltaTime;//�b�ڼY���t��

        liveTime += Time.deltaTime;//�p��s�b�ɶ�
        if(liveTime>=10f)//�s�b�W�L10���ɮ���
        {
            ArrowDestory();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name);
        if (explodeFlag)
        {
            //ArrowExplode();
            ExplodeTest();
        }
        else
        {
            ArrowDestory();
        }
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
    public void IsExplode()
    {
        explodeFlag = true;
        transform.name = "ExplosiveArrow";
    }
    public void IsNormal()
    {
        explodeFlag = false;
    }
    /// <summary>
    /// trigger���z���b
    /// </summary>
    void ExplodeTest()
    {
        liveTime = 0.0f;//��l�Ʀs�b�ɶ�
        fallSpeed = 0.0f;//��l�ƽb�ڪ��Y���t��
        
        StartCoroutine(ExplodeTime());
    }
    protected IEnumerator ExplodeTime()
    {
        collider.radius = 25.0f;
        arrowSpeed = 0.0f;
        gravity = 0.0f;
        yield return new WaitForSeconds(0.2f);
        collider.radius = 0.5f;
        arrowSpeed = 40f;
        gravity = 3.0f;
        gameObject.SetActive(false);
    }
}
