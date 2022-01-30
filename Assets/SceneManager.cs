using System.Collections;  
using System.Collections.Generic;  
using UnityEngine;  
using UnityEngine.SceneManagement;

public class SceneManager : MonoBehaviour
{
    public void LoadMain()
    {
        Invoke("LoadScene", 3);
    }

   private void LoadScene()
    {
        Application.LoadLevel("Main");
    }
}
