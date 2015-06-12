using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Spawner : MonoBehaviour
{
    public const float oddRowDifference = 0.5f;

    public static Spawner instance = null;
    public float rowDifference;
    public float camSpeed = 2.0f;
    public int rows = 9;
    public int cols = 7;
    public GameObject[] tiles;
    public Camera cam;

    private bool oddRow = false;
    private bool spawning = false;
    private bool spawnTurn = false;
    private int rowCount = 0;
    private List<List<GameObject>> spawnedTiles;
    private Mover mov;
    private Vector3 newCamPosition;



    void Start()
    {
        /*if (instance == null)
            instance = this;
        else if(instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);*/

        spawnedTiles = new List<List<GameObject>>();
        mov = GetComponent<Mover>();
        newCamPosition = cam.transform.position;
        StartCoroutine(SpawnRow(rows, false));
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) { Application.Quit(); }

        /*if (Input.GetKeyDown(KeyCode.Alpha1)) { Application.LoadLevel(0); }
        if (Input.GetKeyDown(KeyCode.Alpha2)) { Application.LoadLevel(1); }
        if (Input.GetKeyDown(KeyCode.Alpha3)) { Application.LoadLevel(2); }*/

        if (Input.GetKeyDown(KeyCode.R)) { Application.LoadLevel(Application.loadedLevel); }

        if (Input.GetMouseButtonDown(0) && !spawning)
        {
            if (mov.Move(cam))
            {
                if (spawnTurn)
                    SpawnNew();
                spawnTurn = !spawnTurn ? true : false;
            }
        }
        if (mov.currentPosition.y >= cam.transform.position.y + rowDifference && !spawning)
        {
            SpawnNew();
        }

        StartCoroutine(GameOver()); // WIP
        cam.transform.position = Vector3.MoveTowards(cam.transform.position, newCamPosition, camSpeed * Time.deltaTime);
    }

    void SpawnNew()
    {
        newCamPosition += new Vector3(0f, rowDifference, 0f);

        StartCoroutine(SpawnRow());

        for (int i = 0; i < spawnedTiles[0].Count; i++)
            Destroy(spawnedTiles[0][i].gameObject);
        spawnedTiles.RemoveAt(0);
    }

    IEnumerator SpawnRow(int spawnCount = 1, bool wait = true)
    {
        spawning = true;

        for (int i = 0; i < spawnCount; i++)
        {
            int count = oddRow ? cols : cols + 1;
            List<GameObject> newHexagonRow = new List<GameObject>();

            for (int j = 0; j < count; j++)
            {
                float xPos = j;
                float yPos = rowCount * rowDifference;
                if (!oddRow)
                    xPos -= oddRowDifference;

                GameObject instance = tiles[Random.Range(0, tiles.Length)];
                instance = Instantiate(instance, new Vector3(xPos, yPos, 0f), Quaternion.identity) as GameObject;
                newHexagonRow.Add(instance);

                if (wait)
                    yield return new WaitForSeconds(0.05f);
            }

            spawnedTiles.Add(newHexagonRow);
            oddRow = !oddRow ? true : false;
            rowCount++;

            // WIP
            if (i == 1)
            {
                mov.currentTile = spawnedTiles[i][2];
                mov.currentTile.GetComponent<BoxCollider2D>().enabled = false;
                mov.currentTile.GetComponent<Animator>().enabled = true;
            }
            // WIP

            yield return new WaitForSeconds(0.1f);
        }

        spawning = false;
    }

    IEnumerator GameOver()
    {
        if (cam.transform.position.y - cam.orthographicSize > mov.currentPosition.y)
        {
            spawning = true;
            yield return new WaitForSeconds(1f);
            Application.LoadLevel(Application.loadedLevel);
        }
    }
}
