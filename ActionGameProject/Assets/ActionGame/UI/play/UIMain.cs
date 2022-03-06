using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMain : MonoBehaviour
{
    private static UIMain mInstance = null;
    public static UIMain Instance() { return mInstance; }

    public Image m_HpBar;
    public Image m_MpBar;

    public Texture2D cursorImage;

    public GameObject m_PlayerObject;
    public GameObject skillWindow;
    public GameObject licened;
    public GameObject Scope;

    private void Awake()
    {
        mInstance = this;

        Cursor.SetCursor(cursorImage,Vector2.zero,CursorMode.Auto);
    }
    public void ScopeOpen()
    {
        Scope.SetActive(true);
    }
    public void ScopeClose()
    {
        Scope.SetActive(false);
    }
    //更新血條
    public void UpdateHpBar(float fValue)
    {
        
        m_HpBar.fillAmount = fValue;
    }
    //更新魔槽
    public void UpdateMpBar(float fValue)
    {        
        m_MpBar.fillAmount = fValue;
    }
    public void ToggleGroupUpdate(ToggleGroup tg)
    {
        Toggle t = tg.GetFirstActiveToggle();
        if(t != null)
        {
            Debug.Log(t.name + ": On");
        }
    }
    /// <summary>
    /// 開關技能視窗
    /// </summary>
    public bool OpenSkillWindow()
    {
        if(skillWindow.activeSelf==false)
        {
            skillWindow.SetActive(true);
            licened.SetActive(false);

            Cursor.visible = true;
            return true;
        }
        else
        {
            skillWindow.SetActive(false);
            Cursor.visible = false;
            return false;
        }
    }
    public IEnumerator SwordSkillUnLock01()
    {
        GameObject swordSkillLock = skillWindow.transform.GetChild(3).gameObject;

        licened.SetActive(true);
        while (skillWindow.activeSelf==false)
        {
            yield return null;
        }

        yield return StartCoroutine(GameOverUI.ScreenFadeIn(GameOverUI.FadeType.swordSkill01));
        swordSkillLock.SetActive(false);     
    }
    public IEnumerator SwordSkillUnLock02()
    {
        GameObject swordSkillLock = skillWindow.transform.GetChild(8).gameObject;

        licened.SetActive(true);
        while (skillWindow.activeSelf == false)
        {
            yield return null;
        }

        yield return StartCoroutine(GameOverUI.ScreenFadeIn(GameOverUI.FadeType.swordSkill02));
        swordSkillLock.SetActive(false);
    }
    public IEnumerator BowUnlock()
    {
        GameObject BowLock = skillWindow.transform.GetChild(4).gameObject;

        licened.SetActive(true);
        while (skillWindow.activeSelf == false)
        {
            yield return null;
        }

        yield return StartCoroutine(GameOverUI.ScreenFadeIn(GameOverUI.FadeType.bow));
        BowLock.SetActive(false);
    }
    public IEnumerator ExplodeArrowUnlock()
    {
        GameObject ArrowLock = skillWindow.transform.GetChild(5).gameObject;

        licened.SetActive(true);
        while (skillWindow.activeSelf == false)
        {
            yield return null;
        }

        yield return StartCoroutine(GameOverUI.ScreenFadeIn(GameOverUI.FadeType.eplodeArrow));
        ArrowLock.SetActive(false);     
    }  
}
