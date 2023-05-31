using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Burraco.Utilities;

public class Pile : MonoBehaviour
{

    List<GameObject> cards;
    public int highest;
    public int lowest;
    public Utils.Seed seed;
    public bool hasJoker;

    // Start is called before the first frame update
    void Start()
    {
        cards = new List<GameObject>();
        highest = -1;
        lowest = -1;
        hasJoker = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddCard(GameObject card)
    {
        Card script = card.GetComponent<Card>();

        if(script.isJoker)
        {
            hasJoker = true;
        }
        else if(script.value > highest && seed != Utils.Seed.NoSeed && seed == script.seed)
        {
            highest = script.value;
        }
        else if(script.value < lowest && seed != Utils.Seed.NoSeed && seed == script.seed)
        {
            lowest = script.value;
        }

        cards.Add(card);

    }

    public int Count()
    {
        return cards.Count;
    }
}
