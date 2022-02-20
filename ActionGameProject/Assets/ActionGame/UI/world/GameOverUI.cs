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

    //GameObject back;
    public bool m_IsFading;
    public CanvasGroup gameOver;
    // Start is called before the first frame update
    private void Awake()
    {
        s_Instance = this;
    }
    private void Start()
    {
        //back = transform.GetChild(2).gameObject;
    }
    public static IEnumerator OpenGameOverScreen()
    {
        var canvers = Instance.gameOver;

        canvers.gameObject.SetActive(true);
        yield return Instance.StartCoroutine(Instance.Fade(1f));
        Cursor.visible = true;
    }
    public static IEnumerator CloseGameOverScreen()
    {
        var canvers = Instance.gameOver;

        yield return Instance.StartCoroutine(Instance.Fade(0f));
        canvers.gameObject.SetActive(false);
        Cursor.visible = false;
    }
    protected IEnumerator Fade(float finalAlpha)
    {
        gameOver.blocksRaycasts = false;
        m_IsFading = true;

        while(!Mathf.Approximately(gameOver.alpha,finalAlpha))
        {
            gameOver.alpha = Mathf.MoveTowards(gameOver.alpha, finalAlpha, 1f * Time.deltaTime);
            yield return null;
        }
        gameOver.alpha = finalAlpha;

        gameOver.blocksRaycasts = true;
        m_IsFading = false;

    }
}
