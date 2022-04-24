using System;

namespace GameJam.BB2018
{
    public class MenuSludge : Sludge
    {
        public event Action eatenAction;

        protected override void CustomStart()
        {
            // do nothing
        }

        protected override void Eaten()
        {
            gameObject.SetActive(false);

            if (eatenAction != null)
            {
                eatenAction();
            }

            _playerSwing.GetComponent<Swing>().playerHand.SludgeAbsorb(this);
        }

        protected override void Movement()
        {
            // do nothing
        }
    }
}