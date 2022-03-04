using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;


public class EvColliderRockBossFight : MonoBehaviour
{
    WorldEvManager worldEvManager;

    public PlayableDirector rockMonsterTimeline; //Start RockBoss battle
    private void Awake()
    {
        worldEvManager = FindObjectOfType<WorldEvManager>();
    }


    //��������tag��Ĳ�o
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            StartCoroutine(PlayRockMonsterTimeline());
            //worldEvManager.ActivateRockBossFight();
        }
    }
    IEnumerator PlayRockMonsterTimeline()
    {
        rockMonsterTimeline.Play();
        PlayerInput.Instance.isPlayingTimeline = true;
        yield return new WaitForSeconds((float) rockMonsterTimeline.duration - 0.5f);
        PlayerInput.Instance.isPlayingTimeline = false;
        worldEvManager.ActivateRockBossFight();
    }
}
