using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetBowPoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Player")
        {
            PlayerInput playerInput = other.transform.GetComponent<PlayerInput>();
            playerInput.BowUnlock();
            WorldEvManager.Instance.ShowGetSkillUI();
        }
    }
}
