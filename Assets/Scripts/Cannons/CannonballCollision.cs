using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonballCollision : MonoBehaviour
{
    [SerializeField] public int damage;
    [SerializeField] public bool isEnemyOwned;
    [SerializeField] GameObject ExplosionEffect;
    [SerializeField] GameObject SplashEffect;

    // Do this when colliding w/ a ship
    void OnCollisionEnter(Collision other)
    {
        string colTag = other.gameObject.tag;

        if (colTag == "Ship")
        {
            // Spawn Explosion
            GameObject explosion = Instantiate(ExplosionEffect);
            explosion.transform.position = this.transform.position;
            Destroy(gameObject);

            // Get script of the colliding ship
            Transform otherShip = other.gameObject.transform.parent;
            var otherShipStats = otherShip.GetComponent<ShipStats>();

            // Adjust HP of colliding ship if applicable
            if (isEnemyOwned != otherShipStats.isEnemy)
            {
                otherShipStats.adjustHP(-damage);
            }
        }
    }

    // Do this when colliding w/ the ocean
    void OnTriggerStay(Collider other)
    {
        string colTag = other.gameObject.tag;
        
        if (colTag == "Ocean")
        {
            // Spawn Splash
            GameObject splash = Instantiate(SplashEffect);
            splash.transform.position = this.transform.position;
        }
    }

    // As to only play splash sound once
    void OnTriggerEnter(Collider other)
    {
        string colTag = other.gameObject.tag;
        
        if (colTag == "Ocean")
        {
            // Play audio
            var audioData = GetComponent<AudioSource>();
            audioData.Play(0);
        }
    }
}
