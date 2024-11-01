using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.IO;

public class GameManager : MonoBehaviour
{
    public int stage;
    public Animator stageAnim;
    public Animator clearAnim;
    public Animator fadeAnim;
    public Transform playerPos;

    public string[] enemyObjs;
    public Transform[] spawnPoints;

    public float nextSpawnDelay;
    public float curSpawnDelay;

    public GameObject player;
    public TextMeshProUGUI scoreText;
    public Image[] lifeImage;
    public Image[] boomImage;
    public GameObject gameOverSet;
    public ObjectManager objectManager;


    public List<Spawn> spawnList;
    public int spawnIndex;
    public bool spawnEnd;

    void Awake()
    {
        spawnList = new List<Spawn>();
        enemyObjs = new string[] { "EnemyS", "EnemyM", "EnemyL", "EnemyB" };
        StageStart();
    }

    public void StageStart() 
    {
        // Stage UI Load
        stageAnim.SetTrigger("On");
        stageAnim.GetComponent<TextMeshProUGUI>().text = "Stage " + stage + "\nStart!!";
        stageAnim.GetComponent<TextMeshProUGUI>().text = "Stage " + stage + "\nClear";
        // Enemy Spawn File Read
        ReadSpawnFile();

        // Fade In
        fadeAnim.SetTrigger("In");

    }

    public void StageEnd()
    {
        // Clear UI Load
        clearAnim.SetTrigger("On");

        // Fade Out
        fadeAnim.SetTrigger("Out");

        // Plyer Repos
        player.transform.position = playerPos.position;

        // Stage Increament
        stage++;
        if (stage > 2)
        {
            Invoke("GameOver", 6);
        }
        else
        {
            Invoke("StageStart", 5);
        }
    }
    void ReadSpawnFile()
    {
        // 1. 변수 초기화
        spawnList.Clear();
        spawnIndex = 0;
        spawnEnd = false;

        // 2. 리스폰 파일 읽기 TextAsset => 텍스트 파일 에셋 클래스, 텍스트로 하는거 신기하다..  using System.IO; 이거 꼭 추가하기
        TextAsset textFile = Resources.Load("Stage " + stage) as TextAsset;  // as TextAsset => 텍스트 파일이 맞는지 확인 틀리면 null값 반환
        StringReader stringReader = new StringReader(textFile.text); // StringReader => 텍스트 안의 내용 읽기, using System.IO 필요

        // 3. 리스폰 데이터 생성
        while (stringReader != null)
        {
            string line = stringReader.ReadLine();// 텍스트 데이터를 한줄 씩 반환(자동 줄바꿈)
            Debug.Log(line);

            if (line == null)
            {   
                break;  
            }
           
            Spawn spawnData = new Spawn();
            spawnData.delay = float.Parse(line.Split(',')[0]);
            spawnData.type = line.Split(',')[1];
            spawnData.point = int.Parse(line.Split(',')[2]);
            spawnList.Add(spawnData);  //ㅇㅎ C#의 리스트는 ArrayList구나.
        }

        // 4. 텍스트 파일 닫기
        stringReader.Close(); // 파일 열면 무조건 닫아줘야함.

        // 5. 첫번째 스폰 딜레이 적용
        nextSpawnDelay = spawnList[0].delay;
    }

    void Update()
    {
        curSpawnDelay += Time.deltaTime;
        if(curSpawnDelay > nextSpawnDelay && !spawnEnd)
        {
            SpawnEnemy();
            curSpawnDelay = 0;
        }

        // UI Score Update
        Player playerLogic = player.GetComponent<Player>();
        scoreText.text = string.Format("{0:n0}", playerLogic.score);
    }

    void SpawnEnemy()
    {
        int enemyIndex = 0;
        switch (spawnList[spawnIndex].type)
        {
            case "S" :
                enemyIndex = 0;
                break;
            case "M":
                enemyIndex = 1;
                break;
            case "L":
                enemyIndex = 2;
                break;
            case "B":
                enemyIndex = 3;
                break;
        }
        int enemyPoint = spawnList[spawnIndex].point;
        GameObject enemy = objectManager.MakeObj(enemyObjs[enemyIndex]);
        enemy.transform.position = spawnPoints[enemyPoint].position;

        Rigidbody2D rigid = enemy.GetComponent<Rigidbody2D>();
        Enemy enemyLogic = enemy.GetComponent<Enemy>();
        enemyLogic.player = player;
        enemyLogic.gameManager = this;
        enemyLogic.objectManager = objectManager;

        if(enemyPoint == 5 || enemyPoint == 6) // Right Spawn
        {
            enemy.transform.Rotate(Vector3.back * 90);
            rigid.velocity = new Vector2(enemyLogic.speed * (-1), -1);
            
        }
        else if(enemyPoint == 7 || enemyPoint == 8) // Left Spawn
        {
            enemy.transform.Rotate(Vector3.forward * 90);
            rigid.velocity = new Vector2(enemyLogic.speed, -1);
        }
        else // Front Spawn 
        {
            rigid.velocity = new Vector2(0, enemyLogic.speed *  (-1));
        }

        // 리스폰 인덱스 증가
        spawnIndex++;
        if(spawnIndex == spawnList.Count)
        {
            spawnEnd = true;
            return;
        }

        // 다음 리스폰 딜레이 갱신
        nextSpawnDelay = spawnList[spawnIndex].delay;
    }   

    public void UpdateLifeIcon(int life)
    {
        //UI Life Init Disable
        for(int index = 0; index<3; index++)
        {
            lifeImage[index].color = new Color(1, 1, 1, 0);
        }

        //UI Life Init Active
        for (int index = 0; index < life; index++)
        {
            lifeImage[index].color = new Color(1, 1, 1, 1);
        }
    }

    public void UpdateBoomIcon(int boom)
    {
        //UI Boom Init Disable
        for (int index = 0; index < 3; index++)
        {
            boomImage[index].color = new Color(1, 1, 1, 0);
        }

        //UI Boom Init Active
        for (int index = 0; index < boom; index++)
        {
            boomImage[index].color = new Color(1, 1, 1, 1);
        }
    }

    public void RespawnPlayer()
    {
        Invoke("RespawnPlayerExe", 2f);
    }

    void RespawnPlayerExe()
    {
        player.transform.position = Vector3.down * 3.5f;
        player.SetActive(true);

        Player playerLogic = player.GetComponent<Player>();
        playerLogic.isHit = false;
    }

    public void CallExplosion(Vector3 pos, string type)
    {
        GameObject exposion = objectManager.MakeObj("Explosion");
        Explosion explosionLogic = exposion.GetComponent<Explosion>();

        exposion.transform.position = pos;
        explosionLogic.StartExplosion(type);
    }

    public void GameOver()
    {
        gameOverSet.SetActive(true);
    }

    public void GameRetry()
    {
        SceneManager.LoadScene(0);
    }




}
