using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PressableButtonGroup : MonoBehaviour
{
    private readonly List<PressableButton> _pressableButtons = new List<PressableButton>();
    public void DeactivateAllExcept(PressableButton button)
    {
        foreach (var btn in _pressableButtons.Where(btn => btn != button))
        {
            btn.Deactivate();
        }
    }

    public void DisableAll()
    {
        foreach (var btn in _pressableButtons.ToArray())
        {
            btn.Deactivate();
            btn.gameObject.SetActive(false);
        }
    }

    public void AddToGroup(PressableButton button)
    {
        if (!_pressableButtons.Contains(button)) _pressableButtons.Add(button);
    }

    public void RemoveFromGroup(PressableButton button)
    {
        if (_pressableButtons.Contains(button)) _pressableButtons.Remove(button);
    }
}
