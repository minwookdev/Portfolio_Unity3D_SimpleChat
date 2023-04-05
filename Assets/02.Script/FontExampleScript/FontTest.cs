using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FontTest : MonoBehaviour
{
    public TextMeshProUGUI testText;

    private void Start()
    {
        testText.text = "공식 한글 지원 !";
    }
}
