using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimeUI : MonoBehaviour
{
    [SerializeField] bool _changeColor;
    [SerializeField] string _preScript;
    [SerializeField] string _postScript;

    TextMeshProUGUI _text;

    // Start is called before the first frame update
    void Awake()
    {
        _text = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_changeColor)
        {
            _text.color = GameControl.LevelColor;
        }
        _text.text = _preScript + MathF.Floor(GameControl.RunTimer) + ":" + MathF.Abs((int)((GameControl.RunTimer % 1) * 100)) + _postScript;
    }
}
