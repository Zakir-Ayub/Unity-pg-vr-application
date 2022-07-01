using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public string nextScene;
    
    public void LoadScene()
    {
        SceneManager.LoadScene("Laboratory"); //Loads the Laboratory scene
    } 
    
}
