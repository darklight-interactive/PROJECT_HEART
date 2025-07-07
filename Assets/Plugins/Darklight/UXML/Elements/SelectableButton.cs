using System;
using UnityEngine.UIElements;

namespace Darklight.UXML
{
    [UxmlElement]
    public partial class SelectableButton : Button, ISelectableElement
    {
        public event Action OnSelect;
        public event Action OnClick;

        public SelectableButton()
        {
            if (text == null)
                text = "selectable-button";
            this.clickable.clicked += () => InvokeClickAction();
        }

        public void Select()
        {
            AddToClassList(ISelectableElement.SELECTED_CLASS);
            OnSelect?.Invoke();
        }

        public void Deselect() => RemoveFromClassList(ISelectableElement.SELECTED_CLASS);

        public void Enable() => RemoveFromClassList(ISelectableElement.DISABLED_CLASS);

        public void Disable() => AddToClassList(ISelectableElement.DISABLED_CLASS);

        public void InvokeClickAction() => OnClick?.Invoke();

        public class SelectableButtonFactory : UxmlFactory<SelectableButton> { }
    }
}
