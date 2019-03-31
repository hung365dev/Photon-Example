using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SradnickDev.FlexGUI
{
    public abstract class FlexScreen : MonoBehaviourPunCallbacks, IFlexScreen
    {
        #region Variables

        public Selectable StartSelectable;

        public FlexFader Fader;

        #endregion

        #region UnityEvents

        public UnityEvent onOpen;
        public UnityEvent onClose;

        #endregion

        private void Start()
        {
            if (StartSelectable)
            {
                EventSystem.current.SetSelectedGameObject(StartSelectable.gameObject);
            }
        }

        public virtual void Open()
        {
            if (onOpen != null)
            {
                onOpen.Invoke();
            }

            gameObject.SetActive(true);
            if (Fader)
            {
                Fader.StartFade();
            }
        }

        public virtual void Close()
        {
            if (onClose != null)
            {
                onClose.Invoke();
            }
            
            if (Fader)
            {
                Fader.StartFade();
            }
            gameObject.SetActive(false);
        }

    }

}