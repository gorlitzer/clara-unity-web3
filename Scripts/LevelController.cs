using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    public LevelPiece[] levelPieces;
    //public Transform _camera;
    private Camera _camera;
    public int drawDistance;

    //public float speed;
    public float pieceLength;

    Queue<GameObject> activePieces = new Queue<GameObject>();
    List<int> probabilityList = new List<int>();
    int currentCamStep = 0;
    int lastCamStep = 0;

    void Start()
    {
       _camera = GetComponent<Camera>();
        BuildProbabilityList();

        // Spawn starting level pieces.
        for (int i = 1; i < drawDistance; i++)
        {
            SpawnNewLevelPiece();
        }

        currentCamStep = (int)(_camera.transform.position.z / pieceLength);
        lastCamStep = currentCamStep;
    }

    // Update is called once per frame
    void Update()
    {
        //_camera.transform.position = Vector3.MoveTowards(_camera.transform.position, _camera.transform.position + Vector3.forward, Time.deltaTime * speed);
        currentCamStep = (int)(_camera.transform.position.z / pieceLength);
        if(currentCamStep != lastCamStep) {
            lastCamStep = currentCamStep;
            DespawnLevelPiece();
            SpawnNewLevelPiece();
        }
    }

    void SpawnNewLevelPiece()
    {
        int pieceIndex = probabilityList[Random.Range(0,probabilityList.Count)];
        // Instantiate to (0, 0, pieceLength) with no rotation (Quaternion)
        GameObject newLevelPiece = 
        Instantiate(
            levelPieces[pieceIndex].prefab, 
            new Vector3(0f, 0f, (currentCamStep + activePieces.Count) *  pieceLength), 
            Quaternion.identity
        );
        activePieces.Enqueue(newLevelPiece); // check how many pieces are active
    }

    void DespawnLevelPiece() 
    {
        GameObject oldLevelPiece = activePieces.Dequeue();
        Destroy(oldLevelPiece);
    }

    void  BuildProbabilityList() 
    {
        int index = 0;
        foreach(LevelPiece piece in levelPieces) {
            for (int i =0; i < piece.probability; i++) {
                probabilityList.Add(index);
            }
            index++;
        }
    }
}

[System.Serializable]
public class LevelPiece
{
    public string name;

    public GameObject prefab;

    public int probability = 1;
}
