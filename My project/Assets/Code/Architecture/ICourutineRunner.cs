using System.Collections;
using UnityEngine;

namespace Code.Infrastructure
{
    public interface ICourutineRunner
    {
        Coroutine StartCoroutine(IEnumerator courutine);
        void StopCoroutine(Coroutine courutine);


    }
}