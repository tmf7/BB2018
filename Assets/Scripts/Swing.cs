using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swing : MonoBehaviour {
    public float damageDistance = 1;
    [HideInInspector]
    public PlayerHand playerHand;

    public bool canDamage {
        get { return (transform.position - playerHand.gameObject.transform.position).magnitude >= damageDistance; }
    }
}
