using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneShift : MonoBehaviour
{
    [SerializeField] private bool isMenu;

    private void Awake()
    {
        if (isMenu) { StartCoroutine(LoadScene(1, 2)); }
    }

    public IEnumerator LoadScene(int buildIndex, float waiter = 0)
    {
        yield return new WaitForSeconds(waiter);
        SceneManager.LoadScene(buildIndex);
    }
}
