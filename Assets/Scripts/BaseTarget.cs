using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Naudojate abstrakcia klase - 0.5t
public abstract class BaseTarget : MonoBehaviour
{
    public abstract void TakeDamage(float amount);
}
