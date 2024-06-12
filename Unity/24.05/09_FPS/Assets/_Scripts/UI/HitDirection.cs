using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HitDirection : MonoBehaviour
{
    // 플레이어가 맞았을때 duration동안만 보이는 UI
    public float duration = 0.5f;

    float inverseDuration;

    Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
        inverseDuration = 1 / duration;
    }

    private void Start()
    {
        Player player = GameManager.Instance.Player;
        player.onAttacked += OnPlayerAttacked;
        image.color = Color.clear;
    }

    void OnPlayerAttacked(float angle)
    {
        StopAllCoroutines();
        StartCoroutine(Hit());
        image.transform.rotation = Quaternion.Euler(new(0, 0, angle));
    }

    IEnumerator Hit()
    {
        image.color = new Color(1f, 1f, 1f, 1f);
        float elapsedTime = 0.0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            image.color = new Color(1f, 1f, 1f, 1-elapsedTime* inverseDuration);
            yield return null;

        }
    }
}
