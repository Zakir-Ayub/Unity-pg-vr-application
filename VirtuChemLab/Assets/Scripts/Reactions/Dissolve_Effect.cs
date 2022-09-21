using System.Collections;
using UnityEngine;

/// <summary>
/// Used to play a dissolving effect on a chemical gameobject
/// </summary>
public class Dissolve_Effect : MonoBehaviour
{
    
    public float speed;// speed of shrinking effect

    private bool playDissolvingAnimation = false;

    private IEnumerator SetInactive() // IEnumerator type custom function created specially for pausing time
    {
        float randomTime = Random.Range(1.0f, 4.0f);
        yield return new WaitForSeconds(randomTime); // This parameter is used to hold time for 5 seconds when "StartDestroy" fucntion is called
        playDissolvingAnimation = true;
    }

    void Update()
    {
        if (playDissolvingAnimation) 
        {
            // scale down the element 
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Max(scale.x - Time.deltaTime * speed, 0f);
            scale.y = Mathf.Max(scale.y - Time.deltaTime * speed, 0f);
            scale.z = Mathf.Max(scale.z - Time.deltaTime * speed, 0f);
            transform.localScale = scale;
            if (scale == new Vector3(0, 0, 0))
            {
                Destroy(gameObject);
            }
        }
    }

    public void StartDissolveAnimation()
    {
        StartCoroutine(SetInactive());
    }
}
