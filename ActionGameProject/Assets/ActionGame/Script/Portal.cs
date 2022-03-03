using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public GameObject portalPosition;
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            StartCoroutine(StartTeleport());
        }
    }
    public IEnumerator StartTeleport()
    {
        StartCoroutine(GameOverUI.ScreenFadeOut(GameOverUI.FadeType.Teleport));
        yield return new WaitForSeconds(1.0f);
        GameManager.Instance.GetPlayer().transform.position = portalPosition.transform.position;
        StartCoroutine(GameOverUI.ScreenFadeIn(GameOverUI.FadeType.Teleport));
    }
}
