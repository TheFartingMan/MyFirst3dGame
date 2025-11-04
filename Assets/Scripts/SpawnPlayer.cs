using UnityEngine;

public class SpawnPlayer : MonoBehaviour
{
    public GameObject player;
    public Vector3 spawnPoint = new Vector3(0, 10, 0);
    private float timer = 0f;
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= .01)
        {
            Instantiate(player, spawnPoint, Quaternion.identity);
            timer = 0;
        }
    }
}
