using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonControl : MonoBehaviour
{
    // Values
    [SerializeField] public float shotSpeed;
    [SerializeField] public int   shotDamage;
    [SerializeField] public float shotRandomness;

    // Assets
    public GameObject cannonball;
    public GameObject muzzleFlash;

    // Muzzle Object
    public Transform muzzle;

    // Start is called before the first frame update
    void Start()
    {
        // Get child
        muzzle = transform.GetChild(0);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {   
            // Fire cannonball
            fireCannonball();
        }
    }

    public void fireEffect()
    {
         // ----- Audio -----
        var audioData = GetComponent<AudioSource>();
        audioData.Play(0);

        // ----- Muzzle Flash -----
        GameObject mFlash = Instantiate(muzzleFlash);
        mFlash.transform.position = muzzle.position;
    }

    public void fireCannonball()
    {
        fireEffect();

        // ----- Cannonball Spawning -----
        GameObject newCannonball = Instantiate(cannonball);
        newCannonball.transform.position = muzzle.position;
        newCannonball.transform.rotation = muzzle.rotation;
        var rb = newCannonball.GetComponent<Rigidbody>();
        newCannonball.GetComponent<CannonballCollision>().damage = shotDamage;

        // Calculate cannonball velocity
        Vector3 shotVelocity = muzzle.forward * shotSpeed;
        shotVelocity.x += (Random.value * shotRandomness) - shotRandomness/2;
        shotVelocity.y += (Random.value * shotRandomness) - shotRandomness/2;
        shotVelocity.z += (Random.value * shotRandomness) - shotRandomness/2;

        // Apply calculated velocity
        rb.velocity = shotVelocity;
    }

    public void fireObject(GameObject obj)
    {
        fireEffect();

        // ----- Object Positioning -----
        obj.transform.position = muzzle.position;
        obj.transform.rotation = muzzle.rotation;
        var rb = obj.GetComponent<Rigidbody>();

        // Calculate Object velocity
        Vector3 shotVelocity = muzzle.forward * shotSpeed;
        shotVelocity.x += (Random.value * shotRandomness) - shotRandomness/2;
        shotVelocity.y += (Random.value * shotRandomness) - shotRandomness/2;
        shotVelocity.z += (Random.value * shotRandomness) - shotRandomness/2;

        // Apply calculated velocity
        rb.velocity = shotVelocity;
    }
}
