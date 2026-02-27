using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject level0Map;      // map1
    public GameObject[] level1Maps;   // map2, map3

    public PlayerMove player;

    GameObject currentMap;

    void Start()
    {
        currentMap = level0Map;
        level0Map.SetActive(true);

        foreach (GameObject map in level1Maps)
            map.SetActive(false);
    }

    public void MoveToLevel1Random()
    {
        // 현재 맵 끄기
        currentMap.SetActive(false);

        // 레벨1 중 랜덤 선택
        int randomIndex = Random.Range(0, level1Maps.Length);

        currentMap = level1Maps[randomIndex];
        currentMap.SetActive(true);

        PlayerReposition();
    }

    void PlayerReposition()
    {
        GameObject spawnPoint = GameObject.FindWithTag("spawnPoint");

        if (spawnPoint == null)
        {
            return;
        }

        player.transform.position = spawnPoint.transform.position;
        player.VelocityZero();
    }
}