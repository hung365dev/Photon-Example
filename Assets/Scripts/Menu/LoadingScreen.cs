using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
	[SerializeField] private Text LoadingText;
	[SerializeField] private Slider ProgressBar;
	[SerializeField] private Image FadeOverlay;

	[SerializeField] private float FadeDuration = 0.25f;

	[SerializeField] private ThreadPriority LoadThreadPriority = ThreadPriority.High;

    private float m_loadingOp;

	private static int m_sceneToLoad = -1;
	//Loading Scene, Build Settings Index
	private static int m_loadingSceneIndex = 1;

	public static void LoadScene(int sceneIndex)
	{
		Application.backgroundLoadingPriority = ThreadPriority.High;
		m_sceneToLoad = sceneIndex;
		PhotonNetwork.IsMessageQueueRunning = false;
		SceneManager.LoadScene(m_loadingSceneIndex);
    }

	private void Start()
	{
		FadeOverlay.gameObject.SetActive(true);
		StartCoroutine(LoadAsync(m_sceneToLoad));
		Debug.Log("Loading Screen");
	}

	private IEnumerator LoadAsync(int sceneIndex)
	{
		OnLoadingVisuals();
		
		FadeIn();
		yield return new WaitForSeconds(FadeDuration);
		StartAsynOp(sceneIndex);

		float previousProgress = 0;
		while (DoneLoading() == false)
		{
			m_loadingOp = PhotonNetwork.LevelLoadingProgress;
			if (Mathf.Approximately(m_loadingOp, previousProgress) == false)
			{
				ProgressBar.value = m_loadingOp;
				previousProgress = m_loadingOp;
			}
			OnDoneLoading();
			
			Application.backgroundLoadingPriority = ThreadPriority.Normal;
            
			FadeOut();
			yield return new WaitForSeconds(FadeDuration);
			
		}
	}

	private void StartAsynOp(int sceneIndex)
	{
		Application.backgroundLoadingPriority = LoadThreadPriority;
        PhotonNetwork.LoadLevel(sceneIndex);
		m_loadingOp = PhotonNetwork.LevelLoadingProgress;
	}
	private bool DoneLoading()
	{
		return m_loadingOp >= 0.9f;
	}

	private void FadeIn()
	{
		FadeOverlay.CrossFadeAlpha(0, FadeDuration, true);
	}

	private void FadeOut()
	{
		FadeOverlay.CrossFadeAlpha(1, FadeDuration, true);
	}

	private void OnLoadingVisuals()
	{
		ProgressBar.value = 0f;
		LoadingText.text = "Loading...";
	}

	private void OnDoneLoading()
	{
		ProgressBar.value = 1f;
		LoadingText.text = "Loading done";
	}
}
