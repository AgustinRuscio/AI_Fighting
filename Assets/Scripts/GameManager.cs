using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public HashSet<IBoid> allBoids = new HashSet<IBoid>();

    public static GameManager instance;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public void AddBoid(IBoid b)
    {
        if (!allBoids.Contains(b))
            allBoids.Add(b);
    }
}
