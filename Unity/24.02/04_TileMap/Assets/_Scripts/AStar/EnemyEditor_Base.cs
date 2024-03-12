using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// Editor를 상속받으면 에디터에서만 작동함
[CustomEditor(typeof(EnemyBase_))]
public class EnemyEditor : Editor
{
    // EnemyEditor와 Enemy는 별개의 클래스이므로 실제 선택된 Enemy를 찾아올수 있어야함
    public EnemyBase_ selected;

    // Editor에서 OnEnable은 실제 에디터에서 오브젝트를 눌렀을때 활성화됨
    private void OnEnable()
    {
        // target은 Editor에 있는 변수로 선택한 오브젝트를 받아옴.
        if (AssetDatabase.Contains(target))
        {
            selected = null;
        }
        else
        {
            // target은 Object형이므로 Enemy로 형변환
            selected = (EnemyBase_)target;
        }
    }

    // 유니티가 인스펙터를 GUI로 그려주는함수
    public override void OnInspectorGUI()
    {
        if (selected == null)
            return;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("rseree ");
        EditorGUILayout.Space();

        GUI.color = Color.white;
        selected.maxHp = EditorGUILayout.IntField("몬스터 체력", selected.Hp);
        if (selected.maxHp < 0)
            selected.maxHp = 1;

        selected.mobDamage = EditorGUILayout.IntField("몬스터 공격력", selected.mobDamage);

        selected.mobMoveSpeed = EditorGUILayout.FloatField("몬스터 이동속도", selected.mobMoveSpeed);
        if (selected.mobMoveSpeed < 0)
            selected.mobMoveSpeed = 0;

        // 크기 무작위로 변경
        if (GUILayout.Button("Resize"))
        {
            selected.transform.localScale = Vector3.one * Random.Range(0.5f, 1f);
        }
    }
}

