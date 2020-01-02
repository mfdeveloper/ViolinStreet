using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class UpdateBPM : MonoBehaviour
{

    public MetronomePro_Player metronomePlayer;
    public MetronomePro metronomePro;

    protected Button btnSet;
    protected InputField bpmInputField;

    void Awake()
    {
        btnSet = GetComponent<Button>();
        bpmInputField = transform.parent.GetComponentInChildren<InputField>();
    }

    // Start is called before the first frame update
    void Start()
    {
        btnSet?.onClick.AddListener(Change);
    }

    protected virtual void Change()
    {

        metronomePro?.UpdateBPM();
        
        if (metronomePlayer != null && bpmInputField != null)
        {
            metronomePlayer.SetNewBPM(Convert.ToDouble(bpmInputField.text));
        }
    }
}
