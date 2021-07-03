using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] Button m_ButtonGenerate;
    [SerializeField] Slider m_Slider;
    [SerializeField] Text m_Text;

    void Start()
    {
        m_ButtonGenerate.onClick.AddListener(() =>
        {
            GameManager.Instance.Generate((int)m_Slider.value);
        });

        m_Slider.minValue = 2;
        m_Slider.maxValue = 50;

        m_Slider.onValueChanged.AddListener((value) =>
        {
            m_Text.text = m_Slider.value.ToString();
        });

        m_Slider.value = 10;
    }
}
