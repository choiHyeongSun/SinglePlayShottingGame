using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ItemUI : MonoBehaviour
{
    [SerializeField] private Transform targetTransform;
    [SerializeField] private RectTransform rootTrans;
    [SerializeField] private Vector2 posOffset;
    [SerializeField] private TMP_Text descriptionField;
    [SerializeField] private TMP_Text itemName;
    [SerializeField] private Image image;
    public ItemInfo itemInfo
    {
        set
        {
            image.sprite = value.itemImage;
            descriptionField.text = value.itemDescription;
            itemName.text = value.itemName;
        }
    }
    public void Update()
    {
        Vector2 pos = Camera.main.WorldToViewportPoint(targetTransform.position);
        pos += new Vector2(-0.5f, -0.5f);
        pos = Vector2.Scale(pos, new Vector2(Screen.width, Screen.height));
        rootTrans.anchoredPosition = pos + posOffset;
    }
}
