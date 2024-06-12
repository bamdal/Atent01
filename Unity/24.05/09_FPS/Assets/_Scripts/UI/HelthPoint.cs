using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HelthPoint : MonoBehaviour
{
    Player player;

    TextMeshProUGUI text;

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }
    void Start()
    {
        player = GameManager.Instance.Player;
        text.text = player.HP.ToString();
        player.onHPChange += (hp) => { text.text = $"{hp}"; };
    }


}
