using Unity.Mathematics;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    public void OnCollisionEnter(Collision objecthit)
    {
        if (objecthit.gameObject.CompareTag("Target"))
        {
            print("hit" + objecthit.gameObject.name + "!");

            CreateBulletImpactEffect(objecthit);

            Destroy(gameObject);
        }

        if (objecthit.gameObject.CompareTag("Wall"))
        {
            print("hit a wall");

            CreateBulletImpactEffect(objecthit);

            Destroy(gameObject);
        }
        if (objecthit.gameObject.CompareTag("Bottle"))
        {
            print("hit a bottle");
            objecthit.gameObject.GetComponent<BreakableBottle>().Shatter();
        }


    }
    void CreateBulletImpactEffect(Collision objecthit)
    {
        ContactPoint contact = objecthit.contacts[0];

        GameObject hole = Instantiate(GlobalReferences.Instance.bulletimpactEffectPrefab,contact.point,Quaternion.LookRotation(contact.normal));

        hole.transform.SetParent(objecthit.gameObject.transform);

    }
}
