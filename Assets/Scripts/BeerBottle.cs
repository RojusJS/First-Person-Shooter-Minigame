using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeerBottle : MonoBehaviour
{
    //Naudojamos duomenu strukturos iš System.Collections arba System.Collections.Generic (yra ir daugiau panaudota) - 1 t.
    public List<Rigidbody> allParts = new List<Rigidbody>();

    public void Shatter()
    {
        foreach (Rigidbody part in allParts)
        {
            part.isKinematic = false;
        }
    }
}
