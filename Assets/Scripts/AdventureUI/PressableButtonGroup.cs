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

    public void Enable(IEnumerable<PressableButton> buttons)
    {
        foreach (var btn in buttons)
        {
            btn.gameObject.SetActive(true);
        }
    }

    public PressableButton[] DisableAll()
    {
        var buttons = _pressableButtons.ToArray();
        foreach (var btn in buttons)
        {
            btn.Deactivate();
            btn.gameObject.SetActive(false);
        }
        
        return buttons;
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
