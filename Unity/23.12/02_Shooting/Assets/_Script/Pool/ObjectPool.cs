using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T> : MonoBehaviour // 제네릭을 쓸거면 클래스 뒤에 <>를 쓰고 대신 쓸 T나 이름들을 쓰면된다.
{
    public GameObject originalPrefeb;

    /// <summary>
    /// 풀의 크기, 처음에 생성하는 오브제그의 개수 모든 개수는 2^n으로 잡는 것이 좋다.
    /// </summary>
    public int poolSize = 64;

    /// <summary>
    /// T타입으로 지정된 오브젝트의 배열
    /// </summary>
    T[] pool;
    
    /// <summary>
    /// 현재 사용가능한(비활성화되어있는) 오브젝트들을 관리하는 큐
    /// </summary>
    Queue<T> readyQueue;


  
    public void Initialize()
    {
        pool = new T[poolSize]; // 배열의 크기만큼 new
        readyQueue = new Queue<T>(poolSize); // 레디큐를 만들고 capactiy를 poolSize로 지정
        
        GenerateObjects(0,poolSize,pool);
    }

    /// <summary>
    /// 풀에서 사용할 오브젝트를 생성하는 함수
    /// </summary>
    /// <param name="start">새로 생성 시작할 인덱스</param>
    /// <param name="end">새로 생성이 끝나는 인덱스 +1</param>
    /// <param name="result">새로 생성된 오브젝트가 들어갈 배열</param>
    /// <returns></returns>
    void GenerateObjects(int start, int end, T[] result)
    {
        for (int i = start; i < end; i++)
        {
            GameObject obj = Instantiate(originalPrefeb, transform);
            obj.name = $"{originalPrefeb.name}_{i}";

            T comp = obj.GetComponent<T>();

            result[i] = comp;
            obj.SetActive(false);
        }
    }

}
