using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A set of reasons, e.g. reasons to not open the inventory.
/// </summary>
public class Reasons
{
    private HashSet<string> _reasons = new();
    private Action<bool> _onChange;
    private Action _onBecomeTrue;
    private Action _onBecomeFalse;

    /// <summary>
    /// Returns true if the set of reasons isn't empty.
    /// </summary>
    public bool AnyReasons => _reasons.Count > 0;

    /// <summary>
    /// The number of reasons.
    /// </summary>
    public int NumReasons => _reasons.Count;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="onChange">A delegate invoked whenever the AnyReasons property changes.</param>
    /// <param name="onBecomeTrue">A delegate invoked whenever the AnyReasons property changes to true.</param>
    /// <param name="onBecomeFalse">A delegate invoked whenever the AnyReasons property changes to false.</param>
    public Reasons(Action<bool> onChange = null, Action onBecomeTrue = null, Action onBecomeFalse = null)
    {
        _onChange = onChange;
        _onBecomeTrue = onBecomeTrue;
        _onBecomeFalse = onBecomeFalse;
    }

    /// <summary>
    /// Adds a reason to the set.
    /// </summary>
    public void AddReason(string reason) => ChangeReason(reason, true);

    /// <summary>
    /// Removes a reason from the set.
    /// </summary>
    public void RemoveReason(string reason) => ChangeReason(reason, false);

    /// <summary>
    /// Adds or removes a reason from the set.
    /// </summary>
    public void ChangeReason(string reason, bool addDontRemove)
    {
        bool anyBefore = AnyReasons;

        if (addDontRemove)
        {
            _reasons.Add(reason);
        }
        else
        {
            _reasons.Remove(reason);
        }

        if (anyBefore != AnyReasons)
        {
            if (_onChange != null)
            {
                _onChange(AnyReasons);
            }

            if (_onBecomeTrue != null && AnyReasons)
            {
                _onBecomeTrue();
            }

            if (_onBecomeFalse != null && !AnyReasons)
            {
                _onBecomeFalse();
            }
        }
    }
}
