using UnityEngine;
using System.Collections;

namespace Kubility
{
    [ExecuteInEditMode]
    public class MonoDelegate : MonoSingleTon<MonoDelegate>
    {

        public void Coroutine_Delay(float time, VoidDelegate ev)
        {

            StartCoroutine(Delay(time, ev));
        }

        IEnumerator Delay(float time, VoidDelegate ev)
        {
            yield return new WaitForSeconds(time);
            if (ev != null)
                ev();
        }
    }
}

