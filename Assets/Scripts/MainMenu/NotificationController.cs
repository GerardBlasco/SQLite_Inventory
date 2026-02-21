using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NotificationController : MonoBehaviour
{
    public static NotificationController instance;

    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text text;

    private void Awake()
    {
        if (!instance)
        {
            instance = this;
        }
    }

    public void GenerateNotification(string content)
    {
        text.text = content;
        panel.SetActive(true);
    }

    public void CloseNotification()
    {
        panel.SetActive(false);
    }
}
