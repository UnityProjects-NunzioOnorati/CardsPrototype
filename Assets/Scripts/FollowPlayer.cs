using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Vector3 offset;
    public Vector3 direction;
    private GameObject player;
    
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(player != null){
            transform.position = player.transform.position + offset;
            transform.LookAt(player.transform, direction);
        }else
            player = GameObject.Find("Player(Clone)");
    }
}
