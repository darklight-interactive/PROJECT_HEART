using System;
using System.Collections.Generic;
using Darklight.Editor;
using Darklight.Inky;
using NaughtyAttributes;
using UnityEngine;

[Serializable]
public class InkyStoryStitchData
{
    [Dropdown("_dropdown_knotList"), SerializeField]
    string _knot = "scene_default";

    [Dropdown("_dropdown_interactionStitchList"), SerializeField]
    string _stitch = "interaction_default";

    List<string> _dropdown_knotList
    {
        get
        {
            List<string> knots = new List<string>(100);
            if (InkyStoryManager.Instance != null)
            {
                knots = InkyStoryManager.GetAllKnots();
            }
            return knots;
        }
    }
    List<string> _dropdown_interactionStitchList
    {
        get
        {
            List<string> stitches = new List<string>(100);
            if (InkyStoryManager.Instance != null && !string.IsNullOrEmpty(_knot))
            {
                stitches = InkyStoryManager.GetAllStitchesInKnot(_knot);
            }
            return stitches;
        }
    }

    public string Knot => _knot;
    public string Stitch => _stitch;
}
