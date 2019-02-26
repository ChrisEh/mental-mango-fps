using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    public int StartingAmount; // We want to be able to configure this in the editor.
    public int CurrentOwned;

    public UnityEvent OnValueChanged = new UnityEvent();

    // Unity runs 'Start' when the scene runs
    public void Start()
    {
        CurrentOwned = StartingAmount;
        OnValueChanged.Invoke();
    }
    
    public void AddAmount(int x)
    {
        CurrentOwned += x;
        OnValueChanged.Invoke();
    }

    public void RemoveAmount(int x)
    {
        CurrentOwned -= x;
        OnValueChanged.Invoke();
    }

    [ContextMenu("Add One To Resource")]
    public void Test_AddOne()
    {
        AddAmount(1);
    }
}
