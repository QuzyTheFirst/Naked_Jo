using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NewsLine : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scrollingTMP;
    [SerializeField] private float scrollSpeed;
    private TextMeshProUGUI scrollingTMPClone;
    private RectTransform textRT;
    private float width;
    private Vector3 startPos;

    private void Awake()
    {
        textRT = scrollingTMP.GetComponent<RectTransform>();
        width = scrollingTMP.preferredWidth;
        startPos = textRT.anchoredPosition;

        scrollingTMPClone = Instantiate(scrollingTMP);
        RectTransform cloneRT = scrollingTMPClone.GetComponent<RectTransform>();
        cloneRT.SetParent(textRT);
        cloneRT.anchorMin = new Vector2(1, .5f);
        cloneRT.localScale = new Vector3(1, 1, 1);
        cloneRT.anchoredPosition = new Vector2(cloneRT.anchoredPosition.x, textRT.anchoredPosition.y);
    }

    private void Update()
    {
        if (scrollingTMP.havePropertiesChanged)
        {
            width = scrollingTMP.preferredWidth;
            scrollingTMPClone.text = scrollingTMP.text;
        }

        textRT.anchoredPosition = Vector2.MoveTowards(textRT.anchoredPosition, new Vector2(textRT.anchoredPosition.x - startPos.x, textRT.anchoredPosition.y), scrollSpeed * Time.deltaTime);

        if (textRT.anchoredPosition.x <= width * -1 + startPos.x)
        {
            textRT.anchoredPosition = new Vector2(startPos.x, textRT.anchoredPosition.y);
        }
    }
}
