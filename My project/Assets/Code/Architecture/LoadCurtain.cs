using System.Collections;
using UnityEngine;

namespace Code.ApplicationServices.Client
{
    public class LoadCurtain : MonoBehaviour
    {
        [SerializeField]
        private CanvasGroup _curtain;
        [SerializeField]
        private float _stopTime;
        [SerializeField]
        private float _curtainSpeed = 0.03f;
        [SerializeField]
        private float _step;

        public void Show()
        {
            //gameObject.SetActive(true);
            _curtain.alpha = 1.0f;
        }

        public void Hide()
        {
            //gameObject.SetActive(true);
            StartCoroutine(FadeIn());
        }
        private IEnumerator FadeIn()
        {
            yield return new WaitForSeconds(_stopTime);
            while (_curtain.alpha > 0)
            {
                _curtain.alpha -= _step;

                yield return new WaitForSeconds(_curtainSpeed);
            }
            //gameObject.SetActive(false);
        }
    }
}
