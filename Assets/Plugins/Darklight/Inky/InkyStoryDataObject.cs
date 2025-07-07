using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Darklight.Inky.InkyStoryManager;

public class InkyStoryDataObject : ScriptableObject
{
    Dictionary<string, List<string>> _stitchDictionary = new Dictionary<string, List<string>>();
    Dictionary<string, object> _variableDictionary = new Dictionary<string, object>();

    [SerializeField]
    List<StoryKnotContainer> _knotContainers = new List<StoryKnotContainer>();

    [SerializeField]
    List<StoryVariableContainer> _variableContainers = new List<StoryVariableContainer>();

    public List<StoryKnotContainer> KnotContainers => _knotContainers;
    public List<StoryVariableContainer> VariableContainers => _variableContainers;

    public void RepopulateKnotContainers(Dictionary<string, List<string>> stitchDictionary)
    {
        _knotContainers.Clear();
        _stitchDictionary = stitchDictionary
            .OrderBy(x => x.Key)
            .ToDictionary(x => x.Key, x => x.Value);

        foreach (KeyValuePair<string, List<string>> pair in _stitchDictionary)
        {
            if (pair.Value.Count == 0)
                continue;
            _knotContainers.Add(new StoryKnotContainer(pair));
        }
    }

    public void RepopulateVariableContainers(Dictionary<string, object> variableDictionary)
    {
        _variableDictionary = variableDictionary
            .OrderBy(x => x.Key)
            .ToDictionary(x => x.Key, x => x.Value);

        foreach (KeyValuePair<string, object> pair in _variableDictionary)
        {
            if (pair.Value == null)
                continue;
            var existingContainer = _variableContainers.Find(x => x.Key == pair.Key);
            if (existingContainer != null && existingContainer.Value != pair.Value)
                existingContainer.Value = pair.Value;
            else
                _variableContainers.Add(new StoryVariableContainer(pair.Key, pair.Value));
        }
    }
}
