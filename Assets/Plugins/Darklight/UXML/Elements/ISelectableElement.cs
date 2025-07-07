using System;

public interface ISelectableElement
{
    public const string SELECTED_CLASS = "selected";
    public const string DISABLED_CLASS = "disabled";
    public event Action OnSelect;
    void Select();
    void Deselect();
    void Enable();
    void Disable();
}
