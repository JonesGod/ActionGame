using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class SeaMonsterArea : MonoBehaviour
{
    public GameObject[] trapMonsters;
    public PlayableDirector seaMonsterTimeline;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            seaMonsterTimeline.Play();
            // for(int i = 0; i < trapMonsters.Length; i++)
            // {
            //     trapMonsters[i].SetActive(true);;
            // }       
            Destroy(this.gameObject);     
        }
    }
}
