using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMPlayer : MonoBehaviour
{
    public static BGMPlayer Instance { get; private set; }

    public AudioSource audioSource; // 🔥 Inspector에서 연결 필요

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void OnEnable()
    {
        Debug.Log("📡 BGMPlayer 활성화됨 (씬 유지 중)");
    }

}