using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class FadeManager : MonoBehaviour
{
    public static FadeManager Instance { get; private set; }

    public Animator fadeAnimator;

    public void FadeAndLoadScene(string sceneName)
    {
        StartCoroutine(FadeAndLoadSceneCor(sceneName));
    }

    public IEnumerator FadeAndLoadSceneCor(string sceneName)
    {
        // ���������, ��� fadeAnimator �� ���������
        if (fadeAnimator == null)
        {
            Debug.LogError("FadeAnimator is missing!");
            yield break;
        }

        // ��������� �������� ����������
        fadeAnimator.SetTrigger("FadeIn");
        yield return new WaitForSeconds(2f);

        // ���������� ��������� ��������� �����
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // ��������� �������� �����������
        fadeAnimator.SetTrigger("FadeOut");
        yield return new WaitForSeconds(2f);
    }
}