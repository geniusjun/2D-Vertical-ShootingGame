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
        parentPos = new Queue<Vector3>();    //Quque ���Լ���(FIFO)
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
        if (!parentPos.Contains(parent.position)) // �÷��̾ �������� = ���� ���� �����Ӹ��� ���� ��.
        parentPos.Enqueue(parent.position);

        // Output Pos
        if (parentPos.Count > followDelay) // ���Լ��� �ϴ°Ÿ� �����̸� �ɾ ������� ��ó�� ���̰� ����������;; 
        {
            followPos = parentPos.Dequeue();
        } else if(parentPos.Count < followDelay)
        {
            followPos = parent.position;  // Queue �ڷᱸ���� Delay��ŭ ä������ �������� �θ���ġ�� ����. ������ ���������� �� �׷��ݾ�.
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
