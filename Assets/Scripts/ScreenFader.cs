using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ScreenFader : MonoBehaviour
{
    public static ScreenFader Instance;

    [SerializeField] public Image fadeImage;
    [SerializeField] private float fadeDuration = 1f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {

        if (fadeImage != null)
            StartCoroutine(FadeInImmediate());
    }

    private IEnumerator FadeInImmediate()
    {
        Color c = fadeImage.color;
        c.a = 1f; 
        fadeImage.color = c;

        yield return Fade(1f, 0f); 
    }

    public IEnumerator FadeOut()
    {
        yield return Fade(fadeImage.color.a, 1f);
    }

    public IEnumerator FadeIn()
    {
        yield return Fade(fadeImage.color.a, 0f);
    }

    private IEnumerator Fade(float from, float to)
    {
        float t = 0f;
        Color c = fadeImage.color;

        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            c.a = Mathf.Lerp(from, to, t / fadeDuration);
            fadeImage.color = c;
            yield return null;
        }

        c.a = to;
        fadeImage.color = c;
    }
}
