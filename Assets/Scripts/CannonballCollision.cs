using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonballCollision : MonoBehaviour
{
    [SerializeField] GameObject ExplosionEffect;

    void OnCollisionEnter(Collision other)
    {
        GameObject explosion = Instantiate(ExplosionEffect);
        explosion.transform.position = this.transform.position;
        Destroy(gameObject);
    }
}
