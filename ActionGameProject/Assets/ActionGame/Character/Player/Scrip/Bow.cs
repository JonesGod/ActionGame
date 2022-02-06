using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow : MonoBehaviour
{
    public static Bow Instance
    {
        get { return s_Instance; }
    }
    protected static Bow s_Instance;

    public Transform bow;
    //public GameObject prefab;
    [HideInInspector]
    public bool bowState;

    // Start is called before the first frame update
    private void Awake()
    {
        s_Instance = this;
    }
    void Start()
    {
        
    }

    void Update()
    {
        
        if (bowState)
            BowOn();
        else
            BowOff();

    }
    void BowOn()
    {
        bow.gameObject.SetActive(true);
    }
    void BowOff()
    {
        bow.gameObject.SetActive(false);
    }
    /// <summary>
    /// ©I¥sArrowShoot ScriptªºShoot¨ç¦¡
    /// </summary>
    void BowFire()
    {
        BroadcastMessage("Shoot");
    }
}
