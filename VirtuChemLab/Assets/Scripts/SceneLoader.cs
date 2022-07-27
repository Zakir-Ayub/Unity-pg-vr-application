using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public string nextScene;

    private AudioSource source;

    [Tooltip("Sound that plays when the Next-button is hit")]
    public AudioClip nextButtonSound;

    void Start()
    {
        source = GetComponent<AudioSource>();
    }

        public void LoadScene()
    {
        source.PlayOneShot(nextButtonSound, 1.0F);
        SceneManager.LoadScene("Laboratory"); //Loads the Laboratory scene
    } 
    
}
