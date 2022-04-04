using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MenuSludge : Sludge {
    public event Action eatenAction = delegate { };
    protected override void CustomStart() {}

    protected override void Eaten() {
        gameObject.SetActive(false);
        if (eatenAction != null) {
            eatenAction();
        }
        playerSwing.GetComponent<Swing>().playerHand.SludgeAbsorb(this);
    }

    protected override void Movement() {}
}
