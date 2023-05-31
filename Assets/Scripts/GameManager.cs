using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public GameObject tablePrefab;
    public GameObject[] playersPrefab;
    public List<GameObject> cardsPrefab;
    private GameObject[] players;
    private List<GameObject> deck;
    public List<GameObject> discards;
    private List<GameObject> pit1;
    private List<GameObject> pit2;
    public bool isGameActive;

    private List<GameObject> deckInstances;
    private List<GameObject> discardsInstances;
    public bool hasToRerender;

    // Start is called before the first frame update
    void Start()
    {
        // Instantiate the table
        Debug.Log("Croupier arranging table...");
        GameObject tableInstance = InstantiateTable();

        // Instantiate the players around the table
        Debug.Log("Croupier invited the players to sit...");
        players = new GameObject[4];
        InstantiatePlayersAroundTable(tableInstance);

        // Assign cards to each player
        Debug.Log("Croupier is assigning each player the cards...");
        for(int i=0; i<4; i++)
        {
            Player script = players[i].GetComponent<Player>();
            script.cards = new List<GameObject>();
            GiveCards(script.cards,11);
        }

        // Create the deck
        Debug.Log("Croupier is creating the deck...");
        deck = new List<GameObject>();
        deckInstances = new List<GameObject>();
        GiveCards(deck,41);
        InstantiateDeck();

        // Create the pits
        Debug.Log("Croupier is creating the pits...");
        pit1 = new List<GameObject>();
        GiveCards(pit1,11);
        InstantiatePit1();

        pit2 = new List<GameObject>();
        GiveCards(pit2,11);
        InstantiatePit2();

        // Discard one card
        Debug.Log("Croupier is discarding one card...");
        discards = new List<GameObject>();
        discardsInstances = new List<GameObject>();
        GiveCards(discards,1);
        InstantiateDiscards();

        // Game starting
        Debug.Log("Game starting!");
        isGameActive = true;
        hasToRerender = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Check if rerender is needed
        if(hasToRerender)
        {
            DestoryAll(deckInstances, null);
            DestoryAll(discardsInstances, null);
            InstantiateDeck();
            InstantiateDiscards();
            hasToRerender = false;
        }
    }

    // Instantiate the table and return the instantiated object
    GameObject InstantiateTable()
    {
        Vector3 tablePosition = new Vector3(0,0,0);
        return Instantiate(tablePrefab, tablePosition, tablePrefab.transform.rotation);
    }

    // Instantiate the players around the table
    void InstantiatePlayersAroundTable(GameObject tableInstance)
    {
        Vector3 tableSize = tableInstance.GetComponent<Renderer>().bounds.size;
        int numberOfPlayers = 4;

        for (int i = 0; i < numberOfPlayers; i++)
        {
            // Calculate the position of the player on the table
            Vector3 position;
            int angle;

            switch (i)
            {
                case 0:
                    // Place player on the top side of the table
                    position = new Vector3(tableSize.x * 3/4, tableSize.y, 0);
                    angle = 0;
                    break;

                case 1:
                    // Place player on the right side of the table
                    position = new Vector3(0, tableSize.y, tableSize.z * 3/4);
                    angle = 90;
                    break;

                case 2:
                    // Place player on the bottom side of the table
                    position = new Vector3(-tableSize.x * 3/4, tableSize.y, 0);
                    angle = 180;
                    break;

                default:
                    // Place player on the left side of the table
                    position = new Vector3(0, tableSize.y, -tableSize.z * 3/4);
                    angle = 90;
                    break;
            }

            // Instantiate player prefab at the calculated position and angle
            GameObject player = Instantiate(playersPrefab[i],position,Quaternion.identity);
            player.transform.eulerAngles = new Vector3(0, angle, 0);

            // Store the player instance in the players array
            players[i] = player;
        }
    }

    // Instantiate the deck game object and position the cards
    void InstantiateDeck()
    {
        GameObject deckGameObject;
        // Check if deck game object already exists, if not create it
        if((deckGameObject = GameObject.Find("deck")) == null)
        {
            deckGameObject = new GameObject("deck");
            deckGameObject.transform.position = new Vector3(0, 0.85f, 0);
        }

        // Iterate through the cards in the deck and instantiate them
        for(int i=0; i<deck.Count; i++)
        {
            Vector3 cardPosition = deckGameObject.transform.position + new Vector3(0, i * 0.001f, 0);
            deck[i].transform.rotation = Quaternion.Euler(0,0,180);
            GameObject card = Instantiate(deck[i],cardPosition,deck[i].transform.rotation,deckGameObject.transform);
            deckInstances.Add(card);
        }
    }

    // Instantiate the pit1 game object and position the cards
    void InstantiatePit1()
    {
        GameObject pit1GameObject = new GameObject("pit1");
        pit1GameObject.transform.position = new Vector3(-0.7f, 0.85f, -0.44f);

        // Iterate through the cards in pit1 and instantiate them
        for(int i=0; i<pit1.Count; i++)
        {
            Vector3 cardPosition = pit1GameObject.transform.position + new Vector3(0, i * 0.001f, 0);
            pit1[i].transform.rotation = Quaternion.Euler(0,0,180);
            Instantiate(pit1[i],cardPosition,pit1[i].transform.rotation,pit1GameObject.transform);
        }
    }

    // Instantiate the pit2 game object and position the cards
    void InstantiatePit2()
    {
        GameObject pit2GameObject = new GameObject("pit2");
        pit2GameObject.transform.position = new Vector3(-0.7f, 0.86f, -0.45f);

        // Iterate through the cards in pit2 and instantiate them
        for(int i=0; i<pit2.Count; i++)
        {
            Vector3 cardPosition = pit2GameObject.transform.position + new Vector3(0, i * 0.001f, 0);
            pit2[i].transform.rotation = Quaternion.Euler(0,90,180);
            Instantiate(pit2[i],cardPosition,pit2[i].transform.rotation,pit2GameObject.transform);
        }
    }

    // Instantiate the discards game object and position the cards
    void InstantiateDiscards()
    {
        GameObject discardsGameObject;
        // Check if discards game object already exists, if not create it
        if((discardsGameObject = GameObject.Find("discards")) == null)
        {
            discardsGameObject = new GameObject("discards");
            discardsGameObject.transform.position = new Vector3(0f, 0.86f, -0.18f);
        }

        // Iterate through the cards in discards and instantiate them
        for(int i=0; i<discards.Count; i++)
        {
            Vector3 cardPosition = discardsGameObject.transform.position + new Vector3(0, i * 0.001f, 0);
            discards[i].transform.rotation = Quaternion.Euler(0,90,0);
            GameObject card = Instantiate(discards[i],cardPosition,discards[i].transform.rotation,discardsGameObject.transform);
            discardsInstances.Add(card);
        }
    }

    // Assign a specified number of random cards from the cardsPrefab list to a destination list
    void GiveCards(List<GameObject> destination, int numberOfCards)
    {
        for(int i=0; i<numberOfCards; i++)
        {
            int random = Random.Range(0,cardsPrefab.Count);
            GameObject card = cardsPrefab[random];
            destination.Add(card);
            cardsPrefab.RemoveAt(random);
        }
    }

    // Add a card to the discards list
    public void AddDiscards(GameObject card)
    {
        discards.Add(card);
    }

    // Destroy all the instances in the given list and optionally clear the collection
    void DestoryAll(List<GameObject> instances, List<GameObject> collection)
    {
        foreach(GameObject card in instances)
        {
            Destroy(card);
        }
        if(collection!=null)
            collection.Clear();
    }

}