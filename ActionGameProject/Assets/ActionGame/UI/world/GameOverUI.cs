using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverUI : MonoBehaviour
{
    public static GameOverUI Instance
    {
        get { return s_Instance; }
    }

    protected static GameOverUI s_Instance;

    GameObject back;
    // Start is called before the first frame update
    private void Awake()
    {
        s_Instance = this;
    }
    private void Start()
    {
        back = transform.GetChild(1).gameObject;
    }
    public void OpenGameOverScreen()
    {
        back.SetActive(true);
        Cursor.visible = true;
    }
    public void CloseGameOverScreen()
    {
        back.SetActive(false);
    }
}
