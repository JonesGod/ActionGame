using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMain : MonoBehaviour
{
    private static UIMain mInstance = null;
    public static UIMain Instance() { return mInstance; }

    public Image m_HpBar;

    public GameObject m_PlayerObject;
  
    


    //private List<FloatingBar> m_FloatingBarsList;

    // Start is called before the first frame update
    private void Awake()
    {
        mInstance = this;
    }
       

    
    public void UpdateHpBar(float fValue)
    {
        Debug.Log("UpdateHpBar " + fValue);
        m_HpBar.fillAmount = fValue;
    }

  

    public void ToggleGroupUpdate(ToggleGroup tg)
    {
        Toggle t = tg.GetFirstActiveToggle();
        if(t != null)
        {
            Debug.Log(t.name + ": On");
        }
    }

    public void MyButtonClick(Button b)
    {
        Debug.Log(b.name + ": click");
        m_PlayerObject.SendMessage("Hit", 10.0f);
    }
}
