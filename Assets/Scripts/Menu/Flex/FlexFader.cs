using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SradnickDev.FlexGUI
{
    public class FlexFader : MonoBehaviour
    {
        [SerializeField] private Image FadeImage;
        [SerializeField] private float FadeInDuration = 0.25f;
        [SerializeField] private float FadeOutDuration = 0.25f;
        private bool m_isRunning = false;


        public void StartFade()
        {
            if (m_isRunning) return;
            SetAlpha(0);
            gameObject.SetActive(true);
            StartCoroutine(Fade());
        }
        private IEnumerator Fade()
        {
            m_isRunning = true;
            FadeImage.CrossFadeAlpha(1,FadeInDuration,false);
            yield return new WaitForSeconds(FadeInDuration);
            
            FadeImage.CrossFadeAlpha(0,FadeOutDuration,false);
            yield return new WaitForSeconds(FadeOutDuration);

            m_isRunning = false;
            gameObject.SetActive(false);
        }
        
        private void SetAlpha(float alpha)
        {
            Color tempColor = FadeImage.color;
            tempColor.a = alpha;
            FadeImage.color = tempColor;
        }
    }
}
