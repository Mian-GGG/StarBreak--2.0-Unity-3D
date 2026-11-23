using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LandingCutscene : MonoBehaviour
{
    [Header("Effects")]
    [SerializeField] private ParticleSystem _smokeEffect;
    [SerializeField] private ParticleSystem _hyperspeedEffect;
    [SerializeField] private Image _fadePanel;

    [Header("Screen Shake")]
    [SerializeField] private Transform _camera;
    [SerializeField] private float _shakeIntensity = 0.1f;

    [Header("Scene")]
    [SerializeField] private string _groundScene = "GroundLevel";
    [SerializeField] private float _cutsceneDuration = 5f;

    void Start()
    {
        StartCoroutine(PlayCutscene());
    }

    IEnumerator PlayCutscene()
    {
        if (_smokeEffect != null) _smokeEffect.Play();
        if (_hyperspeedEffect != null) _hyperspeedEffect.Play();

        float elapsed = 0f;
        Vector3 originalCamPos = _camera.localPosition;

        while (elapsed < _cutsceneDuration)
        {
            elapsed += Time.deltaTime;

            float shakeAmount = _shakeIntensity * (elapsed / _cutsceneDuration);
            _camera.localPosition = originalCamPos +
                Random.insideUnitSphere * shakeAmount;

            if (elapsed > _cutsceneDuration - 2f)
            {
                float fadeProgress = (elapsed - (_cutsceneDuration - 2f)) / 2f;
                if (_fadePanel != null)
                    _fadePanel.color = new Color(0f, 0f, 0f, fadeProgress);
            }

            yield return null;
        }

        SceneManager.LoadScene(_groundScene);
    }
}