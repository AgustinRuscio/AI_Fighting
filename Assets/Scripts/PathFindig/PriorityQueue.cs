//--------------------------------------------
//          Agustin Ruscio & Merdeces Riego
//--------------------------------------------


using System.Collections.Generic;

public class PriorityQueue <T>
{
    private Dictionary<T, float> _allElement = new Dictionary<T, float>();

    public float Count { get { return _allElement.Count;} }

    public void Enqueue (T elemnt, float cost)
    {
        if(!_allElement.ContainsKey(elemnt)) _allElement.Add(elemnt, cost);

        else _allElement[elemnt] = cost;
    }

    public T Dequeue() 
    {
        T element = default;

        foreach (var item in _allElement)
        {
            element = element == null ? item.Key : item.Value < _allElement[element] ? item.Key : element;
        }

        _allElement.Remove(element);
        return element;
    }
}