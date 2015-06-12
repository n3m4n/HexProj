using UnityEngine;
using System.Collections;
using System;
using Random = UnityEngine.Random;

public class Mover : MonoBehaviour
{
    public Transform player;
    public Sprite destroyed;
    [HideInInspector]
    public Vector3 currentPosition;
    [HideInInspector]
    public GameObject currentTile;

    private Spawner spawner;

    void Start()
    {
        spawner = GetComponent<Spawner>();
        currentPosition = new Vector3((spawner.cols - 1) / 2, spawner.rowDifference, 0f);
        player.position = currentPosition;
    }


    void Update()
    {
        player.position = Vector3.MoveTowards(player.position, currentPosition, 5 * Time.deltaTime);
    }

    public bool Move(Camera cam)
    {
        RaycastHit2D hit = Physics2D.Raycast(cam.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

        if (hit.collider != null)
        {
            Vector3 hitPosition = hit.collider.transform.position;

            if (hitPosition.x >= 0 && hitPosition.x <= spawner.cols - 1)
                if (hitPosition.y - currentPosition.y <= spawner.rowDifference + 0.01f && hitPosition.y - currentPosition.y >= 0)
                    if (Mathf.Abs(hitPosition.x - currentPosition.x) <= 1)
                    {
                        currentTile.GetComponent<Animator>().enabled = false;
                        currentTile.GetComponent<SpriteRenderer>().sprite = destroyed;

                        currentTile = hit.collider.gameObject;
                        currentPosition = hitPosition;

                        currentTile.GetComponent<Animator>().enabled = true;
                        hit.collider.enabled = false;

                        return true;
                    }
        }

        return false;
    }
}
