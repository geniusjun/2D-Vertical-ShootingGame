using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public class Follower : MonoBehaviour
{
    public float maxShotDelay;
    public float curShotDelay;
    public ObjectManager objectManager;

    public Vector3 followPos;
    public int followDelay;
    public Transform parent;
    public Queue<Vector3> parentPos;


    void Awake()
    {
        parentPos = new Queue<Vector3>();    //Quque 선입선출(FIFO)
    }
    void Update()
    {
        Watch();
        Follow();
        Fire();
        Reload();

    }

    void Watch()
    {
        // Input Pos
        if (!parentPos.Contains(parent.position)) // 플레이어가 멈췄을때 = 같은 값을 프레임마다 넣을 때.
        parentPos.Enqueue(parent.position);

        // Output Pos
        if (parentPos.Count > followDelay) // 선입선출 하는거를 딜레이를 걸어서 따라오는 거처럼 보이게 만들어버리네;; 
        {
            followPos = parentPos.Dequeue();
        } else if(parentPos.Count < followDelay)
        {
            followPos = parent.position;  // Queue 자료구조가 Delay만큼 채워지기 전까지는 부모위치를 적용. 가만히 멈춰있으면 좀 그렇잖아.
        }
    }

    void Follow()
    {
        transform.position = followPos;
    }

    void Fire()
    {
        if (!Input.GetButton("Fire1"))
        {
            return;
        }

        if (curShotDelay < maxShotDelay)
        {
            return;
        }

        GameObject bullet = objectManager.MakeObj("BulletFollower");
        bullet.transform.position = transform.position;

        Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
        rigid.AddForce(Vector2.up * 10, ForceMode2D.Impulse);


        curShotDelay = 0;
    }

    void Reload()
    {
        curShotDelay += Time.deltaTime;
    }
}
