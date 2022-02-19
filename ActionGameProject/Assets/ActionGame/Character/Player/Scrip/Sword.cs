using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    public static Sword Instance
    {
        get { return s_Instance; }
    }

    protected static Sword s_Instance;

    public Transform[] link;
    public Transform sword;
    public bool battleRunIsNext;//給PlayerControl判定
    public bool runIsNext;//給PlayerControl判定
    public bool bowIsNext;//給PlayerControl判定

    private BoxCollider swordBoxCollider;

    [HideInInspector] public bool attackState;
    
    private void Awake()
    {
        s_Instance = this;
    }
    void Start()
    {
        swordBoxCollider = sword.GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (battleRunIsNext)
            SwordOn();

        if (runIsNext || bowIsNext)
            SwordOff();
    }
    void SwordOn()
    {
        sword.gameObject.SetActive(true);
    }
    public void SwordOff()
    {
        sword.gameObject.SetActive(false);
    }
    void SwordColliderOn()
    {
        Debug.Log("ON123");
        swordBoxCollider.enabled = true;
    }
    public void SwordColliderOff()
    {
        swordBoxCollider.enabled = false;
    }

}
