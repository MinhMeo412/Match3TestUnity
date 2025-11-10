using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPanelGame : MonoBehaviour,IMenu
{
    public Text LevelConditionView;

    [SerializeField] private Button btnPause;
    [SerializeField] private Button btnAutoWin;
    [SerializeField] private Button btnAutoLose;

    private UIMainManager m_mngr;
    private AutoPlay m_autoPlay;

    private void Awake()
    {
        btnPause.onClick.AddListener(OnClickPause);
        btnAutoWin.onClick.AddListener(OnClickAutoWin);
        btnAutoLose.onClick.AddListener(OnClickAutoLose);
    }

    public void SetAutoPlay(AutoPlay autoPlay)
    {
        m_autoPlay = autoPlay;
        SetAutoButtonsVisible(autoPlay != null);
    }

    public void SetAutoButtonsVisible(bool visible)
    {
        btnAutoWin.gameObject.SetActive(visible);
        btnAutoLose.gameObject.SetActive(visible);
    }

    private void OnClickPause()
    {
        m_mngr?.ShowPauseMenu();
    }

    private void OnClickAutoWin()
    {
        if (m_mngr.GetState() != GameManager.eStateGame.AUTO_PLAYING)
        {
            m_autoPlay?.StartAutoWin(); 
        }
    }

    private void OnClickAutoLose()
    {
        if (m_mngr.GetState() != GameManager.eStateGame.AUTO_PLAYING)
        {
            m_autoPlay?.StartAutoLose();
        }
    }

    public void Setup(UIMainManager mngr)
    {
        m_mngr = mngr;
    }

    public void Show()
    {
        this.gameObject.SetActive(true);
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }  
}
