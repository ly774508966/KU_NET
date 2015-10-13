using UnityEngine;
using System.Collections;


namespace Kubility
{
    [System.Serializable]
    public class AnimationTrans : AbstractTrans
    {
        [SerializeField]
        private Animator _ani;

        public Animator ani
        {
            get
            {
                return _ani;
            }
            protected set
            {
                _ani = value;
            }
        }

        public AnimationTrans()
        {
            this._ani = null;
        }

        public AnimationTrans(Animator an)
        {
            this._ani = an;
        }

        public override void OnCreateTrans()
        {
            base.OnCreateTrans();

        }

        public override void OnEnterTrans()
        {
            base.OnEnterTrans();
        }

        public override void OnExitTrans()
        {
            base.OnExitTrans();
        }

        public override void OnDestroyTrans()
        {
            base.OnDestroyTrans();
        }

        public override void OnPauseTrans()
        {
            base.OnPauseTrans();
        }

        public override void OnResumeTrans()
        {
            base.OnResumeTrans();
        }

    }
}


