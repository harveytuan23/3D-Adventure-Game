using UnityEngine;
using System.Collections;

public class LocationTrigger : MonoBehaviour
{
    public CanvasGroup locationTextGroup;
    public string locationName = "蛋捲廣場";
    public AudioClip locationSound;  // 🎵 音效
    public AudioSource audioSource;  // 🎵 播放器

    private bool hasShown = false;

    private void OnTriggerEnter(Collider other)
    {
        if (hasShown) return;
        if (other.CompareTag("Player"))
        {
            hasShown = true;

            StartCoroutine(ShowLocationText());

            // 🔊 播放音樂
            if (audioSource != null && locationSound != null)
            {
                audioSource.PlayOneShot(locationSound);
            }
        }
    }

    IEnumerator ShowLocationText()
    {
        // 淡入
        locationTextGroup.alpha = 0;
        locationTextGroup.gameObject.SetActive(true);
        for (float t = 0; t < 1f; t += Time.deltaTime)
        {
            locationTextGroup.alpha = t;
            yield return null;
        }
        locationTextGroup.alpha = 1;

        yield return new WaitForSeconds(2f); // 停留時間

        // 淡出
        for (float t = 1f; t > 0; t -= Time.deltaTime)
        {
            locationTextGroup.alpha = t;
            yield return null;
        }

        locationTextGroup.gameObject.SetActive(false);
    }
}
