using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float damage = 10f;
    private void OnCollisionEnter(Collision objectWeHit)
    {
        if (objectWeHit.gameObject.CompareTag("Target"))
        {
            print("hit " + objectWeHit.gameObject.name + " !");

            CreateBulletImpactEffect(objectWeHit);

            Destroy(gameObject);
        }

        if (objectWeHit.gameObject.CompareTag("Wall"))
        {
            Debug.Log("Missed shot!");
            //ScreenShake shake = Camera.main.GetComponent<ScreenShake>();
            //if (shake != null)
            //{
            //    StartCoroutine(shake.Shake(0.2f, 0.1f));
            //}
            CreateBulletImpactEffect(objectWeHit);

            Destroy(gameObject);
        }

        if (objectWeHit.gameObject.CompareTag("Ground"))
        {
            print("hit a ground");

            CreateBulletImpactEffect(objectWeHit);

            Destroy(gameObject);
        }

        if (objectWeHit.gameObject.CompareTag("Beer"))
        {
            print("hit a beer bottle");

            objectWeHit.gameObject.GetComponent<BeerBottle>().Shatter();
        }

        if (objectWeHit.gameObject.CompareTag("ShootingRangeTarget"))
        {
            CreateBulletImpactEffect(objectWeHit);


            //Naudojami operatoriai ?. ?[] ?? arba ??= - 0.5 t
            ShootingTarget target = objectWeHit.gameObject.GetComponent<ShootingTarget>() ?? new ShootingTarget();
            
           if (target != null)
            {
                target.TakeDamage(damage);
            }

        }

        if (objectWeHit.gameObject.CompareTag("PlayButton"))
        {
            Debug.Log("Play button shot");
            ShootingTarget[] allTargets = FindObjectsOfType<ShootingTarget>();
            foreach (ShootingTarget target in allTargets)
            {
                target.StartGame();
            }
        }

        if (objectWeHit.gameObject.CompareTag("AgainButton"))
        {
            Debug.Log("Again button shot! Restarting game...");

            ShootingTarget[] allTargets = FindObjectsOfType<ShootingTarget>();
            foreach (ShootingTarget target in allTargets)
            {
                target.RestartGame();
            }
        }
    }

    void CreateBulletImpactEffect(Collision objectWeHit)
    {
        ContactPoint contact = objectWeHit.contacts[0];

        GameObject hole = Instantiate(
            GlobalReferences.Instance.bulletImpactEffectPrefab,
            contact.point,
            Quaternion.LookRotation(contact.normal)
            );

        hole.transform.SetParent(objectWeHit.gameObject.transform);
    }
}
