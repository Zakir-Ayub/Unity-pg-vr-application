using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Breakable_Glass : MonoBehaviour
{

    public ParticleSystem GlassParticle;//object type for particle
    private bool Collisioncheck;// Boolean variable for making condition so that only one particle system will be Instantiated

    void Start() //Start is called exactly once in the lifetime of the script
    {
        Collisioncheck = false;
    }

    private void OnCollisionEnter(Collision collision)// Built in fucntion which checks if the object where this script is attached is colliding with any other object or not
    {
        if (collision.collider.CompareTag("Floor")) //Condition for checking if the object where this script is attached is colliding the object with tag "Ground"
        {
            DestroyGlass();// Custom function called
            if (Collisioncheck == false)
            {
                Instantiate(GlassParticle, transform.position, Quaternion.identity);//This parameter is responsible for instantiating particle from asset folder to the scene at the same axis where this script is attached.
                Collisioncheck = true;
            }
        }

    }

    private void DestroyGlass()// Custom function
    {
        Destroy(this.gameObject);//This parameter is resposible for disappearing object where this script is attached
    }
}
