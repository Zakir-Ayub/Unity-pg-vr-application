using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SliderSounds : MonoBehaviour
{
    private AudioSource source;

    [Tooltip("Sound that plays when the Slider value is increased.")]
    public AudioClip increaseSound;

    [Tooltip("Sound that plays when the Slider value is decreased.")]
    public AudioClip decreaseSound;

    private TextSlider[] sliders;

    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
        sliders= FindObjectsOfType<TextSlider>();
    }

    // Update is called once per frame
    void Update()
    {
        foreach (TextSlider slider in sliders)
        {
            if (slider.hasIncreased)
            {
                slider.hasIncreased = false;
                if (!source.isPlaying)
                {
                    source.clip= increaseSound;
                    source.Play();
                }
            
            }
            if (slider.hasDecreased)
            {
                slider.hasDecreased = false;
                if (!source.isPlaying)
                {
                    source.clip = decreaseSound;
                    source.Play();
                }

            }

        }

    }
}
