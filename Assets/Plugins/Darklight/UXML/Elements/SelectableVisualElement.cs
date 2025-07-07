using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace Darklight.UXML
{
    #region ---- [[ SELECTABLE VISUAL ELEMENT ]] ----
    /// <summary>
    /// Base class for controlled elements that encapsulates common functionalities for UXML elements.
    /// </summary>
    /// <typeparam name="TElement">The Type of the </typeparam>
    // Base class for controlled elements that encapsulates common functionalities for UXML elements.
    [UxmlElement]
    public partial class SelectableVisualElement : VisualElement, ISelectableElement
    {
        const string SELECTED_CLASS = "selected";
        const string DISABLED_CLASS = "disabled";
        bool _selected = false;
        bool _disabled = false;

        public VisualElement Element { get; protected set; }

        [UxmlAttribute]
        public bool Selected
        {
            get => _selected;
            set
            {
                _selected = value;
                if (value)
                {
                    Select();
                }
                else
                {
                    Deselect();
                }
            }
        }

        [UxmlAttribute]
        public bool Disabled
        {
            get => _disabled;
            set
            {
                _disabled = value;
                if (value)
                {
                    Disable();
                }
                else
                {
                    Enable();
                }
            }
        }

        public event Action OnSelect;

        public SelectableVisualElement()
        {
            Element = this;
            this.focusable = true;
        }

        public virtual void Select()
        {
            this.AddToClassList(SELECTED_CLASS);
            this.Focus();
            OnSelect?.Invoke();
        }

        public virtual void Deselect()
        {
            this.RemoveFromClassList(SELECTED_CLASS);
        }

        public virtual void Enable()
        {
            this.RemoveFromClassList(DISABLED_CLASS);
        }

        public virtual void Disable()
        {
            this.AddToClassList(DISABLED_CLASS);
        }

        public class SelectableVisualElementFactory : UxmlFactory<SelectableVisualElement> { }
    }
    #endregion
}
