using UnityEngine;
using UnityEngine.UIElements;





[UxmlElement]
public partial class ScreenTransition : VisualElement
{
    public enum TransitionType
    {
        FadeIn,
        FadeOut,
        WipeOpen,
        WipeClose,
    }

    public enum Alignment
    {
        Left,
        Right,
    }

    TransitionType _transitionType = TransitionType.FadeIn;
    Alignment _alignment = Alignment.Left;
    VisualElement _fullscreenBlackElement;

    [Header("(( Transition Type )) ---- >>")]
    [UxmlAttribute]
    public TransitionType TypeKey
    {
        get { return _transitionType; }
        set
        {
            _transitionType = value;
            SetTransitionType(_transitionType);
        }
    }

    [UxmlAttribute]
    public Alignment screenAlignment
    {
        get { return _alignment; }
        set
        {
            _alignment = value;
            SetAlignment(_alignment);
        }
    }

    public ScreenTransition()
    {
        this.style.flexGrow = 1;
        this.style.alignSelf = Align.Stretch;

        _fullscreenBlackElement = new VisualElement()
        {
            name = "fullscreen-black",
            style =
            {
                backgroundColor = Color.black,
                position = Position.Relative,
                top = 0,
                left = 0,
                right = 0,
                bottom = 0,
                flexGrow = 1,
                alignSelf = Align.Stretch
            }
        };

        this.Add(_fullscreenBlackElement);
    }

    public void SetTransitionType(TransitionType type)
    {
        switch (type)
        {
            case TransitionType.FadeIn:
                FadeIn();
                break;
            case TransitionType.FadeOut:
                FadeOut();
                break;
            case TransitionType.WipeOpen:
                WipeOpen();
                break;
            case TransitionType.WipeClose:
                WipeClose();
                break;
        }
    }

    public void SetToDefaults()
    {
        RemoveAllClasses();
        _fullscreenBlackElement.style.opacity = 1;
        _fullscreenBlackElement.style.width = Length.Percent(100);
    }

    public void RemoveAllClasses()
    {
        _fullscreenBlackElement.RemoveFromClassList("fade-in");
        _fullscreenBlackElement.RemoveFromClassList("fade-out");
        _fullscreenBlackElement.RemoveFromClassList("wipe-open");
        _fullscreenBlackElement.RemoveFromClassList("wipe-close");
    }

    public void FadeIn()
    {
        RemoveAllClasses();
        _fullscreenBlackElement.style.width = Length.Percent(100);

        _fullscreenBlackElement.AddToClassList("fade-in");
    }

    public void FadeOut()
    {
        RemoveAllClasses();
        _fullscreenBlackElement.style.width = Length.Percent(100);
        _fullscreenBlackElement.AddToClassList("fade-out");
    }

    public void WipeOpen()
    {
        RemoveAllClasses();
        //_fullscreenBlackElement.style.width = Length.Percent(100);
        //_fullscreenBlackElement.style.opacity = 1;
        _fullscreenBlackElement.AddToClassList("wipe-open");
    }

    public void WipeClose()
    {
        RemoveAllClasses();
        //_fullscreenBlackElement.style.opacity = 1;
        //_fullscreenBlackElement.style.width = Length.Percent(0);
        _fullscreenBlackElement.AddToClassList("wipe-close");
    }

    void SetAlignment(Alignment alignment)
    {
        switch (alignment)
        {
            case Alignment.Left:
                _fullscreenBlackElement.style.alignSelf = Align.FlexStart;
                break;
            case Alignment.Right:
                _fullscreenBlackElement.style.alignSelf = Align.FlexEnd;
                break;
        }
    }
}
