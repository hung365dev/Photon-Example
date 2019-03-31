using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : Singleton<CameraShake>
{
    [SerializeField] Transform m_camera;
    Coroutine m_shakeCoroutine;
    [System.Serializable]
    public class ShakeSettings
    {
        public string Name;
        public AnimationCurve XAxis = AnimationCurve.EaseInOut(0, 0, 1, 1);
        public AnimationCurve YAxis = AnimationCurve.EaseInOut(0, 0, 1, 1);
        public float CurveValueMultiplier = 1;
        public float AnimationSpeed = 1;
        public float RandomMultiplier = 1;
        public bool OverrideShakeDuration = false;
        public float newShakeDuration = 0;
        [HideInInspector] public float ShakeDuration;
    }
    [SerializeField] List<ShakeSettings> m_shakeList = new List<ShakeSettings>();
    Vector3 m_cachedPosition;
    //Get the Information from AnimationCurve x and y

    private void Start()
    {
        m_cachedPosition = m_camera.localPosition;
        GetTotalShakeTime();
    }

    private void GetTotalShakeTime()
    {

        for (int i = 0; i < m_shakeList.Count; i++)
        {
            //Get the last key in the ANimation curve to get the duration of the whole animationcurve
            float m_durationX = m_shakeList[i].XAxis.keys[m_shakeList[i].XAxis.length - 1].time;
            float m_durationY = m_shakeList[i].YAxis.keys[m_shakeList[i].YAxis.length - 1].time;

            if (m_shakeList[i].OverrideShakeDuration)
            {
                m_shakeList[i].ShakeDuration = m_shakeList[i].newShakeDuration;
                continue;
            }
            //compare both lenghts and assign the longest = total shake/animation Time
            m_shakeList[i].ShakeDuration = m_durationX > m_durationY ? m_durationX : m_durationY;
        }

    }
    ///<Summary>Start shake with index of ShakeSettings</Summary>
	public void StartShake(int shakeSettingsIndex)
    {
        m_shakeCoroutine = StartCoroutine(ShakeIt(shakeSettingsIndex));
    }
    ///<Summary>Stop current Shake Animation.</Summary>
    public void StopShake()
    {
        StopCoroutine(m_shakeCoroutine);
    }

    IEnumerator ShakeIt(int shakeSettingsIndex)
    {
        var m_shakeSettings = m_shakeList[shakeSettingsIndex];
        float m_timer = 0;
        float m_xCurve = 0;
        float m_yCurve = 0;

        while (m_timer <= m_shakeSettings.ShakeDuration)
        {
            m_xCurve = m_shakeSettings.XAxis.Evaluate(m_timer);
            m_yCurve = m_shakeSettings.YAxis.Evaluate(m_timer);

            var m_randomInside = Random.insideUnitSphere * m_shakeSettings.RandomMultiplier;
            var m_randomResult = new Vector3(m_randomInside.x * m_xCurve, m_randomInside.y * m_yCurve, 0);

            m_camera.localPosition = m_cachedPosition + (new Vector3(m_xCurve, m_yCurve, 0) * m_shakeSettings.CurveValueMultiplier) + m_randomResult;


            m_timer += Time.deltaTime * m_shakeSettings.AnimationSpeed;
            yield return null;
        }
        m_camera.localPosition = m_cachedPosition;

    }
}
