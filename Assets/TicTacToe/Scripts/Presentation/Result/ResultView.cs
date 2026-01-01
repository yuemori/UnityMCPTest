using System.Collections;
using System.Collections.Generic;
using R3;
using TMPro;
using TicTacToe.Core.Domain;
using TicTacToe.Presentation.Base;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TicTacToe.Presentation.Result
{
    /// <summary>
    /// ゲーム結果表示のView
    /// 勝敗結果とリスタートボタンを表示
    /// </summary>
    public class ResultView : ViewBase<ResultViewModel>
    {
        [Header("UI Components")]
        [SerializeField] private TextMeshProUGUI _resultText;
        [SerializeField] private Button _restartButton;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private GameObject _panelRoot;
        
        private Image _overlayImage;

        [Header("Visual Settings")]
        [SerializeField] private Color _xWinColor = new Color(0.2f, 0.6f, 1f);
        [SerializeField] private Color _oWinColor = new Color(1f, 0.4f, 0.4f);
        [SerializeField] private Color _drawColor = new Color(0.7f, 0.7f, 0.7f);
        [SerializeField] private Color _loseOverlayColor = new Color(0f, 0f, 0f, 0.6f);

        [Header("Animation Settings")]
        [SerializeField] private float _fadeDuration = 0.3f;
        [SerializeField] private float _buttonHoverScale = 1.1f;
        [SerializeField] private float _buttonPressScale = 0.95f;
        [SerializeField] private float _buttonAnimDuration = 0.15f;

        [Header("Confetti Settings")]
        [SerializeField] private int _confettiCount = 30;
        [SerializeField] private float _confettiDuration = 2f;
        [SerializeField] private RectTransform _confettiContainer;

        private Coroutine _currentAnimation;
        private Coroutine _buttonAnimation;
        private Coroutine _confettiAnimation;
        private Vector3 _buttonOriginalScale = Vector3.one;
        private bool _isButtonHovered;
        private bool _isButtonPressed;
        private ResultType _currentResultType = ResultType.None;
        private readonly List<GameObject> _confettiParticles = new List<GameObject>();

        // Confetti colors
        private readonly Color[] _confettiColors = new Color[]
        {
            new Color(1f, 0.84f, 0f),      // Gold
            new Color(1f, 0.5f, 0f),       // Orange
            new Color(0f, 0.8f, 0.4f),     // Green
            new Color(0.4f, 0.6f, 1f),     // Blue
            new Color(1f, 0.4f, 0.7f),     // Pink
            new Color(0.6f, 0.4f, 1f),     // Purple
        };

        protected override void OnBind(ResultViewModel viewModel)
        {
            // オーバーレイを動的に作成
            CreateOverlay();

            // 結果テキストの変更を購読
            viewModel.ResultText
                .Subscribe(text => UpdateResultText(text))
                .AddTo(Disposables);

            // 勝者のマークの変更を購読（色分け用）
            viewModel.WinnerMark
                .Subscribe(mark => UpdateTextColor(mark))
                .AddTo(Disposables);

            // 引き分け状態の変更を購読
            viewModel.IsDraw
                .Subscribe(isDraw => OnDrawStateChanged(isDraw))
                .AddTo(Disposables);

            // 結果タイプの変更を購読
            viewModel.CurrentResultType
                .Subscribe(resultType => _currentResultType = resultType)
                .AddTo(Disposables);

            // 表示/非表示の変更を購読
            viewModel.IsVisible
                .Subscribe(visible => SetVisible(visible))
                .AddTo(Disposables);

            // リスタートボタンのクリックをViewModelに通知
            if (_restartButton != null)
            {
                _restartButton.OnClickAsObservable()
                    .Subscribe(_ => viewModel.OnRestartClick())
                    .AddTo(Disposables);

                // ボタンアニメーション用のEventTriggerを設定
                SetupButtonEvents();
            }
        }

        /// <summary>
        /// オーバーレイを動的に作成
        /// </summary>
        private void CreateOverlay()
        {
            if (_overlayImage != null) return;

            // Canvas直下にオーバーレイを作成
            var canvas = GetComponentInParent<Canvas>();
            if (canvas == null) return;

            var overlayGo = new GameObject("ResultOverlay");
            overlayGo.transform.SetParent(canvas.transform, false);
            
            // このResultPanelより下（後ろ）に配置
            overlayGo.transform.SetSiblingIndex(transform.GetSiblingIndex());

            var overlayRect = overlayGo.AddComponent<RectTransform>();
            overlayRect.anchorMin = Vector2.zero;
            overlayRect.anchorMax = Vector2.one;
            overlayRect.offsetMin = Vector2.zero;
            overlayRect.offsetMax = Vector2.zero;

            _overlayImage = overlayGo.AddComponent<Image>();
            _overlayImage.color = new Color(0f, 0f, 0f, 0f);
            _overlayImage.raycastTarget = false;

            overlayGo.SetActive(false);
        }

        /// <summary>
        /// ボタンのホバー/プレスイベントを設定
        /// </summary>
        private void SetupButtonEvents()
        {
            if (_restartButton == null) return;

            _buttonOriginalScale = _restartButton.transform.localScale;

            var eventTrigger = _restartButton.gameObject.GetComponent<EventTrigger>();
            if (eventTrigger == null)
            {
                eventTrigger = _restartButton.gameObject.AddComponent<EventTrigger>();
            }

            // PointerEnter
            var enterEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
            enterEntry.callback.AddListener(_ => OnButtonPointerEnter());
            eventTrigger.triggers.Add(enterEntry);

            // PointerExit
            var exitEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
            exitEntry.callback.AddListener(_ => OnButtonPointerExit());
            eventTrigger.triggers.Add(exitEntry);

            // PointerDown
            var downEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerDown };
            downEntry.callback.AddListener(_ => OnButtonPointerDown());
            eventTrigger.triggers.Add(downEntry);

            // PointerUp
            var upEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerUp };
            upEntry.callback.AddListener(_ => OnButtonPointerUp());
            eventTrigger.triggers.Add(upEntry);
        }

        /// <summary>
        /// 結果テキストを更新
        /// </summary>
        private void UpdateResultText(string text)
        {
            if (_resultText != null)
            {
                _resultText.text = text;
            }
        }

        /// <summary>
        /// テキスト色を更新
        /// </summary>
        private void UpdateTextColor(CellState winnerMark)
        {
            if (_resultText == null) return;

            _resultText.color = winnerMark switch
            {
                CellState.X => _xWinColor,
                CellState.O => _oWinColor,
                _ => _drawColor
            };
        }

        /// <summary>
        /// 引き分け状態が変更されたとき
        /// </summary>
        private void OnDrawStateChanged(bool isDraw)
        {
            if (isDraw && _resultText != null)
            {
                _resultText.color = _drawColor;
            }
        }

        /// <summary>
        /// 表示/非表示を設定
        /// </summary>
        private void SetVisible(bool visible)
        {
            StopCurrentAnimation();
            StopConfetti();

            if (visible)
            {
                if (_panelRoot != null)
                {
                    _panelRoot.SetActive(true);
                }
                _currentAnimation = StartCoroutine(ShowAnimation());
            }
            else
            {
                if (_panelRoot != null)
                {
                    _panelRoot.SetActive(false);
                }

                if (_canvasGroup != null)
                {
                    _canvasGroup.alpha = 0f;
                    _canvasGroup.interactable = false;
                    _canvasGroup.blocksRaycasts = false;
                }

                // オーバーレイを非表示
                if (_overlayImage != null)
                {
                    _overlayImage.gameObject.SetActive(false);
                }
            }
        }

        private IEnumerator ShowAnimation()
        {
            // 結果タイプに応じた演出を開始
            switch (_currentResultType)
            {
                case ResultType.HumanWin:
                    yield return StartCoroutine(ShowWinAnimation());
                    break;
                case ResultType.AIWin:
                    yield return StartCoroutine(ShowLoseAnimation());
                    break;
                case ResultType.Draw:
                    yield return StartCoroutine(ShowDrawAnimation());
                    break;
                default:
                    yield return StartCoroutine(ShowDefaultAnimation());
                    break;
            }
        }

        /// <summary>
        /// 勝利時のアニメーション（クラッカー演出）
        /// </summary>
        private IEnumerator ShowWinAnimation()
        {
            // パネルを初期化
            if (_panelRoot != null)
            {
                _panelRoot.transform.localScale = Vector3.zero;
            }
            if (_canvasGroup != null)
            {
                _canvasGroup.alpha = 0f;
                _canvasGroup.interactable = true;
                _canvasGroup.blocksRaycasts = true;
            }

            // オーバーレイは薄い金色
            if (_overlayImage != null)
            {
                _overlayImage.gameObject.SetActive(true);
                _overlayImage.color = new Color(1f, 0.9f, 0.5f, 0f);
            }

            // クラッカー演出開始
            _confettiAnimation = StartCoroutine(SpawnConfetti());

            // メインアニメーション
            float elapsed = 0f;
            float duration = _fadeDuration * 1.5f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);

                // EaseOutElastic for bouncy entrance
                float elasticT = ElasticEaseOut(t);

                if (_panelRoot != null)
                {
                    _panelRoot.transform.localScale = Vector3.LerpUnclamped(Vector3.zero, Vector3.one, elasticT);
                }
                if (_canvasGroup != null)
                {
                    _canvasGroup.alpha = Mathf.Clamp01(t * 2f);
                }
                if (_overlayImage != null)
                {
                    _overlayImage.color = new Color(1f, 0.9f, 0.5f, t * 0.15f);
                }
                yield return null;
            }

            // 最終状態
            if (_panelRoot != null)
            {
                _panelRoot.transform.localScale = Vector3.one;
            }
            if (_canvasGroup != null)
            {
                _canvasGroup.alpha = 1f;
            }
        }

        /// <summary>
        /// 敗北時のアニメーション（トーンダウン）
        /// </summary>
        private IEnumerator ShowLoseAnimation()
        {
            // パネルを初期化
            if (_panelRoot != null)
            {
                _panelRoot.transform.localScale = Vector3.one * 0.8f;
            }
            if (_canvasGroup != null)
            {
                _canvasGroup.alpha = 0f;
                _canvasGroup.interactable = true;
                _canvasGroup.blocksRaycasts = true;
            }

            // オーバーレイを有効化（暗いオーバーレイ）
            if (_overlayImage != null)
            {
                _overlayImage.gameObject.SetActive(true);
                _overlayImage.color = new Color(0f, 0f, 0f, 0f);
            }

            float elapsed = 0f;
            float duration = _fadeDuration * 1.5f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);

                // Slow ease for sad feeling
                float easeT = t * t;

                if (_panelRoot != null)
                {
                    _panelRoot.transform.localScale = Vector3.Lerp(Vector3.one * 0.8f, Vector3.one, easeT);
                }
                if (_canvasGroup != null)
                {
                    _canvasGroup.alpha = easeT;
                }
                if (_overlayImage != null)
                {
                    // トーンダウン（暗くする）
                    _overlayImage.color = Color.Lerp(
                        new Color(0f, 0f, 0f, 0f),
                        _loseOverlayColor,
                        easeT
                    );
                }
                yield return null;
            }

            // 最終状態
            if (_panelRoot != null)
            {
                _panelRoot.transform.localScale = Vector3.one;
            }
            if (_canvasGroup != null)
            {
                _canvasGroup.alpha = 1f;
            }
        }

        /// <summary>
        /// 引き分け時のアニメーション
        /// </summary>
        private IEnumerator ShowDrawAnimation()
        {
            // パネルを初期化
            if (_panelRoot != null)
            {
                _panelRoot.transform.localScale = Vector3.one * 0.9f;
            }
            if (_canvasGroup != null)
            {
                _canvasGroup.alpha = 0f;
                _canvasGroup.interactable = true;
                _canvasGroup.blocksRaycasts = true;
            }

            // 薄いグレーオーバーレイ
            if (_overlayImage != null)
            {
                _overlayImage.gameObject.SetActive(true);
                _overlayImage.color = new Color(0.5f, 0.5f, 0.5f, 0f);
            }

            float elapsed = 0f;
            while (elapsed < _fadeDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / _fadeDuration);

                if (_panelRoot != null)
                {
                    _panelRoot.transform.localScale = Vector3.Lerp(Vector3.one * 0.9f, Vector3.one, t);
                }
                if (_canvasGroup != null)
                {
                    _canvasGroup.alpha = t;
                }
                if (_overlayImage != null)
                {
                    _overlayImage.color = new Color(0.5f, 0.5f, 0.5f, t * 0.2f);
                }
                yield return null;
            }

            // 最終状態
            if (_panelRoot != null)
            {
                _panelRoot.transform.localScale = Vector3.one;
            }
            if (_canvasGroup != null)
            {
                _canvasGroup.alpha = 1f;
            }
        }

        /// <summary>
        /// デフォルトのアニメーション
        /// </summary>
        private IEnumerator ShowDefaultAnimation()
        {
            // 初期状態
            if (_panelRoot != null)
            {
                _panelRoot.transform.localScale = Vector3.one * 0.8f;
            }
            if (_canvasGroup != null)
            {
                _canvasGroup.alpha = 0f;
                _canvasGroup.interactable = true;
                _canvasGroup.blocksRaycasts = true;
            }

            float elapsed = 0f;
            while (elapsed < _fadeDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / _fadeDuration);

                // OutBack easing for scale
                float c1 = 1.70158f;
                float c3 = c1 + 1f;
                float scaleT = 1f + c3 * Mathf.Pow(t - 1f, 3f) + c1 * Mathf.Pow(t - 1f, 2f);

                if (_panelRoot != null)
                {
                    _panelRoot.transform.localScale = Vector3.LerpUnclamped(Vector3.one * 0.8f, Vector3.one, scaleT);
                }
                if (_canvasGroup != null)
                {
                    _canvasGroup.alpha = t;
                }
                yield return null;
            }

            // 最終状態
            if (_panelRoot != null)
            {
                _panelRoot.transform.localScale = Vector3.one;
            }
            if (_canvasGroup != null)
            {
                _canvasGroup.alpha = 1f;
            }
        }

        #region Confetti Effect

        /// <summary>
        /// クラッカー演出（紙吹雪）
        /// </summary>
        private IEnumerator SpawnConfetti()
        {
            var container = _confettiContainer != null ? _confettiContainer : (RectTransform)transform;
            var containerRect = container.rect;

            // 紙吹雪を生成
            for (int i = 0; i < _confettiCount; i++)
            {
                var confetti = CreateConfettiParticle(container, containerRect);
                _confettiParticles.Add(confetti);
            }

            // アニメーション
            float elapsed = 0f;
            while (elapsed < _confettiDuration)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / _confettiDuration;

                foreach (var particle in _confettiParticles)
                {
                    if (particle == null) continue;

                    var data = particle.GetComponent<ConfettiData>();
                    if (data == null) continue;

                    var rect = particle.GetComponent<RectTransform>();
                    if (rect == null) continue;

                    // 落下 + 横揺れ
                    float y = data.StartY - (data.FallSpeed * elapsed * 100f);
                    float x = data.StartX + Mathf.Sin(elapsed * data.SwaySpeed) * data.SwayAmount;
                    rect.anchoredPosition = new Vector2(x, y);

                    // 回転
                    rect.Rotate(0f, 0f, data.RotationSpeed * Time.deltaTime);

                    // フェードアウト
                    var image = particle.GetComponent<Image>();
                    if (image != null && progress > 0.7f)
                    {
                        float fadeProgress = (progress - 0.7f) / 0.3f;
                        var color = image.color;
                        color.a = 1f - fadeProgress;
                        image.color = color;
                    }
                }

                yield return null;
            }

            // クリーンアップ
            ClearConfetti();
        }

        private GameObject CreateConfettiParticle(RectTransform container, Rect containerRect)
        {
            var go = new GameObject("Confetti");
            go.transform.SetParent(container, false);

            var rect = go.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(Random.Range(8f, 16f), Random.Range(8f, 16f));

            // ランダムな開始位置（上部から）
            float startX = Random.Range(-containerRect.width * 0.5f, containerRect.width * 0.5f);
            float startY = containerRect.height * 0.5f + Random.Range(20f, 100f);
            rect.anchoredPosition = new Vector2(startX, startY);

            // ランダムな回転
            rect.rotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));

            var image = go.AddComponent<Image>();
            image.color = _confettiColors[Random.Range(0, _confettiColors.Length)];

            // データを保存
            var data = go.AddComponent<ConfettiData>();
            data.StartX = startX;
            data.StartY = startY;
            data.FallSpeed = Random.Range(2f, 5f);
            data.SwaySpeed = Random.Range(3f, 8f);
            data.SwayAmount = Random.Range(20f, 50f);
            data.RotationSpeed = Random.Range(-200f, 200f);

            return go;
        }

        private void ClearConfetti()
        {
            foreach (var particle in _confettiParticles)
            {
                if (particle != null)
                {
                    Destroy(particle);
                }
            }
            _confettiParticles.Clear();
        }

        private void StopConfetti()
        {
            if (_confettiAnimation != null)
            {
                StopCoroutine(_confettiAnimation);
                _confettiAnimation = null;
            }
            ClearConfetti();
        }

        #endregion

        /// <summary>
        /// Elastic ease out
        /// </summary>
        private float ElasticEaseOut(float t)
        {
            const float c4 = (2f * Mathf.PI) / 3f;
            return t <= 0f ? 0f : t >= 1f ? 1f : Mathf.Pow(2f, -10f * t) * Mathf.Sin((t * 10f - 0.75f) * c4) + 1f;
        }

        private void StopCurrentAnimation()
        {
            if (_currentAnimation != null)
            {
                StopCoroutine(_currentAnimation);
                _currentAnimation = null;
            }
        }

        #region Button Hover/Press Animation

        private void OnButtonPointerEnter()
        {
            _isButtonHovered = true;
            AnimateButton(_buttonHoverScale);
        }

        private void OnButtonPointerExit()
        {
            _isButtonHovered = false;
            if (!_isButtonPressed)
            {
                AnimateButton(1f);
            }
        }

        private void OnButtonPointerDown()
        {
            _isButtonPressed = true;
            AnimateButton(_buttonPressScale);
        }

        private void OnButtonPointerUp()
        {
            _isButtonPressed = false;
            AnimateButton(_isButtonHovered ? _buttonHoverScale : 1f);
        }

        private void AnimateButton(float targetScale)
        {
            if (_restartButton == null) return;

            StopButtonAnimation();
            _buttonAnimation = StartCoroutine(ButtonScaleAnimation(targetScale));
        }

        private IEnumerator ButtonScaleAnimation(float targetScale)
        {
            var buttonTransform = _restartButton.transform;
            var startScale = buttonTransform.localScale;
            var endScale = _buttonOriginalScale * targetScale;

            float elapsed = 0f;
            while (elapsed < _buttonAnimDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / _buttonAnimDuration);
                // EaseOutBack for snappy feel
                float c1 = 1.70158f;
                float c3 = c1 + 1f;
                t = 1f + c3 * Mathf.Pow(t - 1f, 3f) + c1 * Mathf.Pow(t - 1f, 2f);

                buttonTransform.localScale = Vector3.LerpUnclamped(startScale, endScale, t);
                yield return null;
            }

            buttonTransform.localScale = endScale;
        }

        private void StopButtonAnimation()
        {
            if (_buttonAnimation != null)
            {
                StopCoroutine(_buttonAnimation);
                _buttonAnimation = null;
            }
        }

        #endregion

#if UNITY_EDITOR
        /// <summary>
        /// エディタ用：コンポーネント自動取得
        /// </summary>
        private void Reset()
        {
            if (_resultText == null)
            {
                _resultText = GetComponentInChildren<TextMeshProUGUI>();
            }
            if (_restartButton == null)
            {
                _restartButton = GetComponentInChildren<Button>();
            }
            if (_canvasGroup == null)
            {
                _canvasGroup = GetComponent<CanvasGroup>();
            }
            if (_panelRoot == null)
            {
                _panelRoot = gameObject;
            }
        }
#endif
    }

    /// <summary>
    /// 紙吹雪パーティクル用データ
    /// </summary>
    public class ConfettiData : MonoBehaviour
    {
        public float StartX;
        public float StartY;
        public float FallSpeed;
        public float SwaySpeed;
        public float SwayAmount;
        public float RotationSpeed;
    }
}
