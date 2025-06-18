using UnityEditor;
using UnityEngine;
using TMPro;

public class CanvasRendererCleaner : EditorWindow
{
    [MenuItem("도구/CanvasRenderer 정리")]
    public static void ShowWindow()
    {
        GetWindow<CanvasRendererCleaner>("CanvasRenderer 정리기");
    }

    private void OnGUI()
    {
        GUILayout.Label("CanvasRenderer 정리 도구", EditorStyles.boldLabel);
        GUILayout.Label("TextMeshPro 없이 붙어 있는 CanvasRenderer를 자동으로 제거합니다.", EditorStyles.wordWrappedLabel);
        GUILayout.Space(10);

        if (GUILayout.Button("전체 씬에서 정리 실행"))
        {
            CleanSceneCanvasRenderers();
        }
    }

    private void CleanSceneCanvasRenderers()
    {
        int removedCount = 0;

        CanvasRenderer[] allRenderers = FindObjectsOfType<CanvasRenderer>(true);
        foreach (CanvasRenderer renderer in allRenderers)
        {
            GameObject go = renderer.gameObject;

            // TextMeshProUGUI가 없으면 삭제
            if (go.GetComponent<TextMeshProUGUI>() == null)
            {
                Undo.DestroyObjectImmediate(renderer);
                removedCount++;
            }
        }

        Debug.Log($"✅ 정리 완료: CanvasRenderer {removedCount}개 제거됨");
    }
}
