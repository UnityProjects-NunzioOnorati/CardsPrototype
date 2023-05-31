using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public List<GameObject> cards;
    public GameManager croupier;
    public string tag;
    //Change to private when testing is ended
    public float rangeCircle;
    public float angleTotal;
    public Vector3 distanceFromPlayer; 
    public float xOffset;

    private bool isCardsShown = false;
    public List<GameObject> instantiatedCards;

    void Start()
    {
        // Find the GameManager object and get its GameManager component
        croupier = GameObject.Find("GameManager").GetComponent<GameManager>();

        // Set the 'tag' variable to the tag of the current GameObject
        tag = gameObject.tag;

        // Initialize the 'instantiatedCards' list
        instantiatedCards = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the game is active and the cards haven't been shown yet
        if (croupier.isGameActive && !isCardsShown)
        {
            // Show the cards
            ShowCards();
            isCardsShown = true;
        }
    }

    void ShowCards()
    {
        // Calculate the angle in radians
        float angle = angleTotal / 180f * Mathf.PI;

        // Calculate the start and end angles for distributing the cards
        float startAngle = (Mathf.PI - angle) / 2f;
        float endAngle = Mathf.PI - startAngle;

        // Calculate the angular increment for each card
        float increment = (endAngle - startAngle) / cards.Count;

        // Loop through each card
        for (int i = 0; i < cards.Count; i++)
        {
            Vector3 position;
            Quaternion rotation;

            switch (tag)
            {
                case "Player":
                    // Calculate the position and rotation for player cards
                    position = new Vector3(-distanceFromPlayer.x + xOffset * i, Mathf.Pow(rangeCircle, 2) * Mathf.Sin(startAngle + increment * i) + distanceFromPlayer.y, Mathf.Pow(rangeCircle, 2) * Mathf.Cos(startAngle + increment * i));
                    rotation = Quaternion.Euler((-increment * cards.Count + increment * i) * 180f / Mathf.PI, 180f, 90f);
                    break;

                case "Enemy":
                    // Calculate the position and rotation for enemy cards
                    bool isOnTheLeft = transform.position.z > 0;
                    position = new Vector3(Mathf.Pow(rangeCircle, 2) * Mathf.Cos(startAngle + increment * i), Mathf.Pow(rangeCircle, 2) * Mathf.Sin(startAngle + increment * i) + distanceFromPlayer.y, (isOnTheLeft ? -1f : 1f) * distanceFromPlayer.x + (isOnTheLeft ? 1f : -1f) * xOffset * i);
                    rotation = Quaternion.Euler((isOnTheLeft ? -1f : 1f) * (increment * cards.Count - increment * i) * -180f / Mathf.PI, (isOnTheLeft ? 2f : 1f) * 180f, 90f);
                    break;

                case "Ally":
                    // Calculate the position and rotation for ally cards
                    position = new Vector3(distanceFromPlayer.x + xOffset * i, Mathf.Pow(rangeCircle, 2) * Mathf.Sin(startAngle + increment * i) + distanceFromPlayer.y, Mathf.Pow(rangeCircle, 2) * Mathf.Cos(startAngle + increment * i));
                    rotation = Quaternion.Euler((increment * cards.Count - increment * i) * 180f / Mathf.PI, 180f, 90f);
                    break;

                default:
                    // Log an error if the tag is unknown and skip to the next card
                    Debug.LogError("Unknown tag: " + tag);
                    continue;
            }

            // Instantiate the card at the calculated position and rotation
            GameObject instantiatedCard = Instantiate(cards[i], transform.position + position, transform.rotation * rotation, transform);
            Card script = instantiatedCard.GetComponent<Card>();
            script.owner = this;
            instantiatedCards.Add(instantiatedCard);
        }
    }

    public void Fold(string name)
    {
        // Create a list to store the indices of cards to remove
        List<int> objToRemove = new List<int>();

        // Loop through each card
        for (int i = 0; i < cards.Count; i++)
        {
            GameObject card = cards[i];

            // Check if the name of the card to fold matches the current card's name
            if (name.Contains(card.name))
            {
                // Destroy the cards and add the index to the list of cards to remove
                DestroyCards();
                objToRemove.Add(i);
            }
        }

        // Loop through the indices in reverse order to remove the cards from the 'cards' list
        for (int j = objToRemove.Count - 1; j >= 0; j--)
        {
            int index = objToRemove[j];
            croupier.AddDiscards(cards[index]);
            cards.RemoveAt(index);
        }

        // Set the 'hasToRerender' flag in the GameManager to indicate that a rerender is needed
        croupier.hasToRerender = true;
    }

    public void DestroyCards()
    {
        // Loop through each instantiated card and destroy it
        foreach (GameObject instantiatedCard in instantiatedCards)
        {
            Destroy(instantiatedCard);
        }

        // Clear the list of instantiated cards and reset the 'isCardsShown' flag
        instantiatedCards.Clear();
        isCardsShown = false;
    }

}
