using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class EngineController : SimpleEngineController
{

    [Header("Advanced Inputs")]
    // inputs
    public KeyCode boost; // touche pour augmenter la puissance
    public KeyCode slow; // touche pour diminuer la puissance

    public bool maintainKey; // si true, le moteur reste allumé tant que la touche est appuyée

    // percentage management
    [Range(0, 1)] public float boost_percentage = 0.75f;
    [Range(0, 1)] public float slow_percentage = 0.25f;

    [Header("Back Input")]
    public EngineAlignmentHandler alignmentHandler;
    public KeyCode back_key; // touche pour aller en arrière
    [Range(0, 1)] public float back_percentage = 0.5f;

    // START
    protected override void Start()
    {
        base.Start();
        alignmentHandler = GetComponent<EngineAlignmentHandler>();
    }

    // UPDATE
    void Update()
    {
        // on récupère les inputs
        if (maintainKey)
        {
            UpdateOnOffMaintain();
        }
        else
        {
            UpdateOnOff();
        }
        
        if (!engine.isOn) { return; }
        
        UpdatePower();
    }

    void UpdatePower()
    {

        // todo : check even if not on and keep the inputs in memory

        // on change la puissance du moteur
        if (Input.GetKeyDown(boost))
        {
            engine.SetPercentage(boost_percentage);
        }
        else if (Input.GetKey(slow))
        {
            engine.SetPercentage(slow_percentage);
        }
        else if (Input.GetKeyUp(boost) || Input.GetKeyUp(slow))
        {
            engine.SetPercentage(default_percentage);
        }
    }

    void UpdateOnOffMaintain()
    {
        if (Input.GetKeyDown(key))
        {
            engine.TurnOn();
            engine.SetPercentage(default_percentage);
            alignmentHandler.SetForwardAlignment(Vector3.forward);
        }
        else if (Input.GetKeyUp(key))
        {
            engine.TurnOff();
        }
        else if (Input.GetKeyDown(back_key))
        {
            engine.TurnOn();
            engine.SetPercentage(back_percentage);
            alignmentHandler.SetForwardAlignment(Vector3.back);
        }
        else if (Input.GetKeyUp(back_key))
        {
            engine.TurnOff();
        }
    }
}