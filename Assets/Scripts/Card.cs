using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using Burraco.Utilities;

public class Card : MonoBehaviour
{

    public enum State{
        Idle,
        Fold,
        Connected    
    }

    public Utils.Back backColor;
    public Utils.Seed seed;
    public int value;
    public bool isJoker;

    public GameObject pilePrefab;
    public GameObject pileInstance;
    public Pile pile;
    public Player owner;
    public GameObject canvasPrefab;
    public static GameObject canvas;
    public TextMeshProUGUI textUi;

    private Vector3 initialPosition;
    private bool isDragging = false;
    private Vector3 dragStartPosition;
    private Plane dragPlane;
    private float distanceToCamera;
    private State state;
    private Quaternion initialRotation;
    private GameObject connectable;
    public float interactionRange;

    // Start is called before the first frame update
    void Start()
    {
        // Set the card attributes
        SetCardAttributes();
        if(canvas == null)
            canvas = Instantiate(canvasPrefab, canvasPrefab.transform.position, canvasPrefab.transform.rotation); 
    }

    // Update is called once per frame
    void Update()
    {
        if (isDragging)
        {
            // Cast a ray from the mouse position into the scene
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            // Intersect the ray with the drag plane
            if (dragPlane.Raycast(ray, out float distance))
            {
                // Calculate the new position on the XZ plane
                Vector3 newPosition = ray.GetPoint(distance);

                // Update the Y-axis of the new position
                newPosition.y = dragStartPosition.y;

                

                if (newPosition.x <= 0)
                {
                    // If the card position is to the left, set the state to "Fold"
                    textUi.text = "Fold";
                    state = State.Fold;
                    transform.rotation = initialRotation;
                }
                else if((connectable = FindNearestObject()) != null)
                {
                    transform.localRotation = connectable.transform.localRotation;
                    textUi.text = "Connectable";
                    state = State.Connected;
                    Connect(connectable);                    
                }
                else
                {
                    // If the card position is to the right, set the state to "Idle"
                    textUi.text = "Moving Around";
                    state = State.Idle;
                    transform.rotation = initialRotation;
                }

                // Move the object to the new position
                transform.position = newPosition;
            }
        }
    }

    void OnMouseDown()
    {
        if (Input.GetMouseButton(0) && owner.name.Contains("Player"))
        {

            textUi = canvas.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
            textUi.text = "";
            textUi.fontSize = 14;

            canvas.SetActive(true);

            // Store the initial position of the card and start dragging
            initialPosition = transform.position;
            initialRotation = transform.rotation;
            isDragging = true;
            dragStartPosition = transform.position;

            // Calculate the distance from the camera to the object
            distanceToCamera = Vector3.Distance(transform.position, Camera.main.transform.position);

            // Create a plane at the drag start position perpendicular to the Y-axis
            dragPlane = new Plane(Vector3.up, dragStartPosition);
        }
    }

    void OnMouseUp()
    {

        canvas.SetActive(false);

        isDragging = false;

        if (state == State.Idle)
        {
            transform.position = initialPosition;
            transform.rotation = initialRotation;
        }
        else if (state == State.Fold)
        {
            // Destroy the card and notify the owner to fold it
            Destroy(gameObject);
            owner.Fold(gameObject.name);
        }
        else if (state == State.Connected)
        {


            transform.SetParent(pileInstance.transform);
        }
    }

    private GameObject FindNearestObject()
    {
        GameObject nearestObject = null;
        float nearestDistance = Mathf.Infinity;

        foreach (GameObject obj in owner.instantiatedCards)
        {
            // Calculate the distance between the center point of the object and the player
            float distance = Vector3.Distance(obj.transform.position, transform.position);

            // Check if the distance is smaller than the current nearest distance
            if ( distance < nearestDistance && !obj.name.Contains(gameObject.name) && distance <= interactionRange)
            {
                nearestDistance = distance;
                nearestObject = obj;
            }
        }

        return nearestObject;
    }

    void Connect(GameObject thing)
    {
        float offset = 0.0025f;

        if(thing.name.Contains("pile"))
        {
            Pile script = thing.GetComponent<Pile>();
            if((seed == script.seed && value - script.highest == 1 || script.lowest - value == 1) || (value == script.highest && value == script.lowest && script.highest == script.lowest) || (isJoker && !script.hasJoker))
            {
                script.AddCard(gameObject);
                int numberOfCards = script.Count();
                Vector3 newPosition = new Vector3(connectable.transform.position.x + numberOfCards * offset, connectable.transform.position.y, connectable.transform.position.z + numberOfCards * offset );
                transform.position = newPosition;
            }
        }
        else
        {
            Card script = thing.GetComponent<Card>();
            if((seed == script.seed && Mathf.Abs(value - script.value) == 1) || (value == script.value) || (isJoker && !script.isJoker))
            {
                Vector3 newPosition = new Vector3(connectable.transform.position.x + offset, connectable.transform.position.y, connectable.transform.position.z + offset );
                transform.position = newPosition;
                pileInstance = Instantiate(pilePrefab, connectable.transform.position, connectable.transform.rotation);
                connectable.transform.SetParent(pileInstance.transform);
                pileInstance.transform.SetParent(owner.gameObject.transform);
            }
        }
    }

    // Set the attributes of the card based on its name
    void SetCardAttributes()
    {
        string[] nameParts = gameObject.name.Split("_");
        string back = nameParts[0];
        string cardInfo = nameParts[2];

        // Determine the back color of the card
        if (back == "Blue")
        {
            backColor = Utils.Back.Blue;
        }
        else
        {
            backColor = Utils.Back.Red;
        }

        if (cardInfo == "Joker")
        {
            // If the card is a Joker, set the appropriate values
            isJoker = true;
            seed = Utils.Seed.NoSeed;
            value = -1;
        }
        else
        {
            // If the card is not a Joker, parse the seed and value
            isJoker = false;
            Enum.TryParse<Utils.Seed>(cardInfo, out seed);
            value = int.Parse(cardInfo.Substring(cardInfo.Length - 2));
        }
    }
}
