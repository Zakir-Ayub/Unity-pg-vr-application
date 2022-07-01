using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Dissolve_Effect : MonoBehaviour
{
    
    public float speed;// speed of shrinking effect

    private bool playDissolvingAnimation;
    
    void Start()
    {
        playDissolvingAnimation = false;
    }

    private IEnumerator SetInactive() // IEnumerator type custom function created specially for pausing time
    {
        float randomTime = Random.Range(5.0f, 12.0f);
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
                this.gameObject.SetActive(false);// After randomTime seconds the object where this script is attached will disappear but it will remain in the scene.
                playDissolvingAnimation = false;
            }
        }
    }

    void OnCollisionEnter(Collision collision)// Built in fucntion which checks if the object where this script is attached is touching any collider or not
    {
            if (collision.gameObject.name.Contains("Beaker_300ml")) //Condition for checking if the object where this script is attached is touching the object with name including Water
            {
                if(collision.gameObject.GetComponent<WaterController>().WaterAmount > 0) StartCoroutine(SetInactive());// If the above condition is fullfiled then the "StartDestroy" function will be executed. To be noted IEnumerator function has to be called in Coroutine function otherwise it will not work
            }
    }
}
