using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneShift : MonoBehaviour
{
    [SerializeField] private bool isMenu;
    [SerializeField] private float waiter = 2;

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneIEnum(sceneName));
    }

    private IEnumerator LoadSceneIEnum(string sceneName)
    {
        yield return new WaitForSeconds(waiter);
        SceneManager.LoadScene(sceneName);
    }
}
