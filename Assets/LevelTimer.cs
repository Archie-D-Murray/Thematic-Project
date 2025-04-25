using System.Collections;
using System.Collections.Generic;

using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class LevelTimer : MonoBehaviour
{
    [SerializeField] TMP_Text timerText;

    public float timer;

    private void Update() {
        timer += Time.deltaTime;
        timerText.text = (Mathf.Round(timer * 100)/ 100.0).ToString();
    }
}