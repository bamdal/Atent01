using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {

        RecycleObject obj = collision.GetComponent<RecycleObject>(); // 부모 컴포넌트 가져오기

        if (obj != null)
        {
            collision.gameObject.SetActive(false); // 풀에 있는 오브젝트 비활성화
        }
        else
        {

            Destroy(collision.gameObject);

        }


    }

}
