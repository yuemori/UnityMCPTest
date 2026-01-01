using UnityEngine;
using UnityEngine.UI;

namespace TicTacToe.Presentation.UI
{
    /// <summary>
    /// 角丸UIシェーダーのRectSizeを自動更新するコンポーネント
    /// マテリアルインスタンスを作成して、個別のサイズを設定可能にする
    /// </summary>
    [ExecuteAlways]
    [RequireComponent(typeof(Image))]
    public class RoundedUIUpdater : MonoBehaviour
    {
        private static readonly int RectSizeProperty = Shader.PropertyToID("_RectSize");
        
        private Image _image;
        private RectTransform _rectTransform;
        private Material _materialInstance;
        private Vector2 _lastSize;

        private void Awake()
        {
            _image = GetComponent<Image>();
            _rectTransform = GetComponent<RectTransform>();
            CreateMaterialInstance();
        }

        private void OnEnable()
        {
            UpdateMaterialSize();
        }

        private void OnDestroy()
        {
            // マテリアルインスタンスをクリーンアップ
            if (_materialInstance != null)
            {
#if UNITY_EDITOR
                if (!Application.isPlaying)
                {
                    DestroyImmediate(_materialInstance);
                }
                else
#endif
                {
                    Destroy(_materialInstance);
                }
                _materialInstance = null;
            }
        }

        private void Update()
        {
            // サイズが変わった場合のみ更新
            var currentSize = _rectTransform.rect.size;
            if (currentSize != _lastSize)
            {
                UpdateMaterialSize();
            }
        }

        private void CreateMaterialInstance()
        {
            if (_image == null || _image.material == null) return;
            
            // すでにインスタンスがある場合はスキップ
            if (_materialInstance != null) return;
            
            // マテリアルインスタンスを作成
            _materialInstance = new Material(_image.material);
            _materialInstance.name = _image.material.name + " (Instance)";
            _image.material = _materialInstance;
        }

        private void UpdateMaterialSize()
        {
            if (_materialInstance == null)
            {
                CreateMaterialInstance();
            }
            
            if (_materialInstance == null || _rectTransform == null) return;
            
            var size = _rectTransform.rect.size;
            _lastSize = size;
            
            // インスタンスにサイズを設定
            _materialInstance.SetVector(RectSizeProperty, new Vector4(size.x, size.y, 0, 0));
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_image == null) _image = GetComponent<Image>();
            if (_rectTransform == null) _rectTransform = GetComponent<RectTransform>();
            
            // Editorでは遅延実行
            UnityEditor.EditorApplication.delayCall += () =>
            {
                if (this != null && gameObject != null)
                {
                    CreateMaterialInstance();
                    UpdateMaterialSize();
                }
            };
        }
#endif
    }
}
