using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace SradnickDev.FlexGUI
{
    public class FlexControl : MonoBehaviour
    {
        #region Main Properties

        [SerializeField] private FlexScreen m_startScreen;

        public FlexScreen StartScreen
        {
            get
            {
                if (m_startScreen == null)
                {
                    var fallbackScreen = transform.GetChild(0).GetComponent<FlexScreen>();
                    Debug.LogWarning("No Start Screen Found, please assign one!");
                    return fallbackScreen;
                }
                else
                {
                    return m_startScreen;
                }
            }
        }

        #endregion

        #region  Events

        public UnityEvent OnSwitchScreens;

        #endregion


        private FlexScreen[] m_screens;

        private FlexScreen m_previousScreen;

        public FlexScreen PreviousScreen
        {
            get { return m_previousScreen; }
        }

        private FlexScreen m_currentScreen;

        public FlexScreen CurrentScreen
        {
            get { return m_currentScreen; }
        }

        private void OnEnable()
        {
            Initialize();
        }

        private void Initialize()
        {
            m_screens = GetComponentsInChildren<FlexScreen>(true);
            foreach (var screen in m_screens)
            {
                screen.gameObject.SetActive(false);
            }
        }

        private void Start()
        {
            OnSwitchScreen(m_startScreen);
        }

        public void OnSwitchScreen(FlexScreen screen)
        {
            if (!screen) return;
            
            if (m_currentScreen)
            {
                m_currentScreen.Close();
                m_previousScreen = m_currentScreen;
            }

            m_currentScreen = screen;
            m_currentScreen.gameObject.SetActive(true);
            m_currentScreen.Open();

            if (OnSwitchScreens != null)
            {
                OnSwitchScreens.Invoke();
            }
        }
    }
}