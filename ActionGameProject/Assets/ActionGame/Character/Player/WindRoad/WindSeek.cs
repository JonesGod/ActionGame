using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindSeek : MonoBehaviour
{
    private ParticleSystem windSysem;

    private Vector3 nextPlace;
    private Vector3 currentPlace;
    private Vector3 windDirection;

    public GameObject firstPlace;
    public GameObject SecondPlace;
    public GameObject ThirdPlace;
    public GameObject FourthPlace;
    public GameObject FifthPlace;
    public GameObject SixPlace;
    public GameObject SeventhPlace;
    public GameObject EighthPlace;
    private void Awake()
    {
        windSysem=GetComponent<ParticleSystem>();
    }
    // Start is called before the first frame update
    void Start()
    {
        nextPlace = firstPlace.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        currentPlace = transform.position;
        windDirection = nextPlace - currentPlace;
        transform.right = (windDirection).normalized;
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name+"Seek");
        switch(other.name)
        {
            case "P_AncientRuins_Sword":
                nextPlace = SecondPlace.transform.position;
                break;
            case "SecondPlace":
                nextPlace = ThirdPlace.transform.position;
                break;
            case "ThirdPlace":
                nextPlace = FourthPlace.transform.position;
                break;
            case "FourthPlace":
                nextPlace = FifthPlace.transform.position;
                break;
            case "FifthPlace":
                nextPlace = SixPlace.transform.position;
                break;
            case "SixthPlace":
                nextPlace = SeventhPlace.transform.position;
                break;
            case "SeventhPlace":
                nextPlace = EighthPlace.transform.position;
                break;
        }
    }
    public void PlayWindSeek()
    {
        windSysem.Play();
    }
}
