using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordHitEffect : MonoBehaviour
{
    public GameObject hitEffect;
    private ParticleSystem hitSystem;
    public GameObject specialHitEffect01;
    private ParticleSystem specialHitSystem;

    [HideInInspector]public int effectId;
    // Start is called before the first frame update
    void Start()
    {
        hitSystem = hitEffect.GetComponent<ParticleSystem>();
        specialHitSystem=specialHitEffect01.GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "HitBox")
        {
            switch(effectId)
            {
                case 1:
                    hitSystem.Play();
                    break;
                case 2:
                    specialHitSystem.Play();
                    break;
            }           
        }
    }
    public void SelectHitEffect(int id)
    {
        effectId = id;
    }
}
