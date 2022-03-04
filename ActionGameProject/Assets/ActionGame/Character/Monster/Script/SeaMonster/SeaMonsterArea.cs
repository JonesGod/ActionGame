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
            StartCoroutine(PlaySeaMonsterTimeline());
            // for(int i = 0; i < trapMonsters.Length; i++)
            // {
            //     trapMonsters[i].SetActive(true);;
            // }                    
        }
    }
    IEnumerator PlaySeaMonsterTimeline()
    {
        seaMonsterTimeline.Play();
        PlayerInput.Instance.isPlayingTimeline = true;
        yield return new WaitForSeconds((float) seaMonsterTimeline.duration - 0.5f);
        PlayerInput.Instance.isPlayingTimeline = false;
        Destroy(this.gameObject);    
    }
}
