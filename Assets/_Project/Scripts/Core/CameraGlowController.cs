using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal; // Обязательно для URP
using System.Collections;

public class CameraGlowController : MonoBehaviour
{
    public static CameraGlowController Instance { get; private set; }

    private Volume volume;
    private Vignette vignette;
    private Coroutine fadeCoroutine;

    [Header("Настройки анимации")]
    public float maxIntensity = 0.45f; // Насколько сильно будет видно синее свечение
    public float fadeSpeed = 4f;       // Скорость появления/исчезновения

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        volume = GetComponent<Volume>();
        // Достаем профиль виньетки
        if (volume.profile.TryGet(out Vignette outVignette))
        {
            vignette = outVignette;
            vignette.intensity.value = 0f; // При старте свечения нет
        }
    }

    // Метод для включения свечения
    public void SetGlow(bool active)
    {
        if (vignette == null) return;

        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeGlow(active ? maxIntensity : 0f));
    }

    private IEnumerator FadeGlow(float targetIntensity)
    {
        while (!Mathf.Approximately(vignette.intensity.value, targetIntensity))
        {
            vignette.intensity.value = Mathf.MoveTowards(vignette.intensity.value, targetIntensity, fadeSpeed * Time.deltaTime);
            yield return null;
        }
    }
}