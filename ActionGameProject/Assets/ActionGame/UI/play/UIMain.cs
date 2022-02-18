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

    public GameObject m_PlayerObject;
    
    


    private void Awake()
    {
        mInstance = this;
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
}
