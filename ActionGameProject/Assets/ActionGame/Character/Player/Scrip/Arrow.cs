using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    private float arrowSpeed = 40.0f;//箭矢飛行速度
    private float gravity=3f ;//重力
    private float liveTime=0.0f;//存在時間
    private float fallSpeed=0.0f;//墜落速度
    private float explodeRadius = 15f;//爆炸半徑

    private bool explodeFlag=false;//是否為爆炸箭

    private List<FSMBase> monster;//存取怪物資訊

    // Start is called before the first frame update
    private void Awake()
    {

    }
    void Start()
    {
        monster = new List<FSMBase>();
        GameObject[] allMonster = GameManager.Instance.allMonster;//將場景裡tag為Monster的物件存起來
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
        transform.position += transform.forward * arrowSpeed * Time.deltaTime;//箭矢往前飛行速度
        fallSpeed += gravity*Time.deltaTime;//重力計算
        transform.position-= fallSpeed * Vector3.up*Time.deltaTime;//箭矢墜落速度

        liveTime += Time.deltaTime;//計算存在時間
        if(liveTime>=10f)//存在超過10秒時消失
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
    /// 箭矢消失
    /// </summary>
    void ArrowDestory()
    {
        liveTime = 0.0f;//初始化存在時間
        fallSpeed = 0.0f;//初始化箭矢的墜落速度
        gameObject.SetActive(false);
    }
    void ArrowExplode()
    {
        liveTime = 0.0f;//初始化存在時間
        fallSpeed = 0.0f;//初始化箭矢的墜落速度
       
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
