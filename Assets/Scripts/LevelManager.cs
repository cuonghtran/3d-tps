using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    public Camera gameCamera;
    public GameObject playerPrefab;

    public float minX;
    public float maxX;
    public float minZ;
    public float maxZ;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

    }

    private void Start()
    {
        gameCamera.gameObject.SetActive(false);
        //SpawnPlayers();
    }

    void SpawnPlayers()
    {
        Vector3 randomPos = new Vector3(Random.Range(minX, maxX), 1, Random.Range(minZ, maxZ));
        PhotonNetwork.Instantiate(playerPrefab.name, randomPos, Quaternion.identity);
    }
}
