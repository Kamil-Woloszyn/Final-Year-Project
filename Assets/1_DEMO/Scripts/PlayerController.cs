using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    List<Sprite> spritesPlayer;
    [SerializeField]
    Camera cameraRefernace;
    float updateSpriteCooldown = 0.15f;

    int currentSprite;
    float timer;
    int spriteOffset;
    bool keepUpdating = true;
    // Start is called before the first frame update
    void Start()
    {
        currentSprite = 0;
        timer = 0;
        cameraRefernace.transform.parent = transform;
    }

    // Update is called once per frame
    //Every 4 direction changes SWEN South West East North facing sprite every 4
    void Update()
    {
        keepUpdating = true;
        if (Input.GetKeyUp(KeyCode.S))
        {
            spriteOffset = 0;
            //Check if the tile is available
            transform.position += new Vector3(0, -1, 0);
        }
        else if (Input.GetKeyUp(KeyCode.A))
        {
            spriteOffset = 4;
            //Check if the tile is available
            transform.position += new Vector3(-1, 0, 0);
        }
        else if(Input.GetKeyUp(KeyCode.D))
        {
            spriteOffset = 8;
            //Check if the tile is available
            transform.position += new Vector3(1, 0, 0);
        }
        else if(Input.GetKeyUp(KeyCode.W))
        {
            spriteOffset = 12;
            //Check if the tile is available
            transform.position += new Vector3(0, 1, 0);
        }
        else
        {
            keepUpdating = false;
        }
        timer += Time.deltaTime;
        if (updateSpriteCooldown < timer && keepUpdating)
        {
            timer = 0;
            currentSprite++;
            currentSprite = currentSprite == 4 ? 0 : currentSprite;
            GetComponent<SpriteRenderer>().sprite = spritesPlayer[currentSprite + spriteOffset];
        }
        
    }
}
