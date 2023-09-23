using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Live2D.Cubism.Core;
using Live2D.Cubism.Framework;
using TMPro;

public class TypewriterEffect : MonoBehaviour {

    [SerializeField] private float speed = 50f;
    private int charIndex;

    public Coroutine Run(string textToType, TMP_Text textLabel)
    {
        return StartCoroutine(TypeText(textToType, textLabel));
    }

    private IEnumerator TypeText(string textToType, TMP_Text textLabel) {
        float t = 0;
        charIndex = 0;
        while (charIndex < textToType.Length) {
            t += Time.deltaTime * speed;
            charIndex = Mathf.FloorToInt(t);
            charIndex = Mathf.Clamp(charIndex, 0, textToType.Length);
            string text = textToType.Substring(0, charIndex);
            textLabel.text = text;
            yield return null;
        }
        textLabel.text = textToType;
    }
}
