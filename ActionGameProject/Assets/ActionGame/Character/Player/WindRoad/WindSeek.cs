using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindSeek : MonoBehaviour
{
    private ParticleSystem windSysem;

    private Vector3 nextPlace;
    private Vector3 currentPlace;
    private Vector3 windDirection;
    private void Awake()
    {
        windSysem=GetComponent<ParticleSystem>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        currentPlace = transform.position;
        nextPlace = other.transform.position;
    }
}
