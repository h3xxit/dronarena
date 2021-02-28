using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour
{
    public Queue<BulletLogic> pool = new Queue<BulletLogic>();
    public GameObject bulletPrefab;

    public BulletLogic GetNewBullet()
    {
        if (pool.Count == 0)
        {
            GameObject g = Instantiate(bulletPrefab, transform);
            return g.GetComponent<BulletLogic>();
        }
        return pool.Dequeue();
    }
}
