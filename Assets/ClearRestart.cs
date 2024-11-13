using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClearRestart : MonoBehaviour
{
    // Start is called before the first frame update

    public void RebootGame()
    {

        SceneManager.LoadScene(1);
    }
}
