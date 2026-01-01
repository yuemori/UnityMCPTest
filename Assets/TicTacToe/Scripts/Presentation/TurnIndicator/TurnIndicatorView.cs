using System.Collections;
using R3;
using TMPro;
using TicTacToe.Core.Domain;
using TicTacToe.Presentation.Base;
using UnityEngine;

namespace TicTacToe.Presentation.TurnIndicator
{
    /// <summary>
    /// ターン表示のView
    /// 現在のターン情報をUIに表示
    /// </summary>
    public class TurnIndicatorView : ViewBase<TurnIndicatorViewModel>
    {
        [Header("UI Components")]
        [SerializeField] private TextMeshProUGUI _turnText;
        [SerializeField] private CanvasGroup _canvasGroup;

        [Header("Visual Settings")]
        [SerializeField] private Color _xColor = new Color(0.2f, 0.6f, 1f);
        [SerializeField] private Color _oColor = new Color(1f, 0.4f, 0.4f);
        [SerializeField] private Color _aiThinkingColor = new Color(0.5f, 0.5f, 0.5f);

        [Header("Animation Settings")]
        [SerializeField] private float _fadeDuration = 0.3f;

        private Coroutine _textAnimation;
        private Coroutine _pulseAnimation;
        private Coroutine _fadeAnimation;

        protected override void OnBind(TurnIndicatorViewModel viewModel)
        {
            // ターンテキストの変更を購読
            viewModel.TurnText
                .Subscribe(text => UpdateTurnText(text))
                .AddTo(Disposables);

            // 現在のマークの変更を購読（色分け用）
            viewModel.CurrentMark
                .Subscribe(mark => UpdateTextColor(mark))
                .AddTo(Disposables);

            // AI思考状態の変更を購読
            viewModel.IsAIThinking
                .Subscribe(isThinking => OnAIThinkingChanged(isThinking))
                .AddTo(Disposables);

            // 表示/非表示の変更を購読
            viewModel.IsVisible
                .Subscribe(visible => SetVisible(visible))
                .AddTo(Disposables);
        }

        /// <summary>
        /// ターンテキストを更新
        /// </summary>
        private void UpdateTurnText(string text)
        {
            if (_turnText != null)
            {
                _turnText.text = text;
                
                // ターン交代時のアニメーション
                StopAnimation(ref _textAnimation);
                _textAnimation = StartCoroutine(TurnChangeAnimation());
            }
        }

        private IEnumerator TurnChangeAnimation()
        {
            _turnText.transform.localPosition = new Vector3(0, -10f, 0);
            
            float elapsed = 0f;
            float duration = 0.4f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                // OutBack easing
                float c1 = 1.70158f;
                float c3 = c1 + 1f;
                t = 1f + c3 * Mathf.Pow(t - 1f, 3f) + c1 * Mathf.Pow(t - 1f, 2f);
                
                _turnText.transform.localPosition = new Vector3(0, Mathf.LerpUnclamped(-10f, 0f, t), 0);
                yield return null;
            }
            _turnText.transform.localPosition = Vector3.zero;
        }

        /// <summary>
        /// テキスト色を更新
        /// </summary>
        private void UpdateTextColor(CellState mark)
        {
            if (_turnText == null) return;

            Color targetColor = mark switch
            {
                CellState.X => _xColor,
                CellState.O => _oColor,
                _ => Color.white
            };
            
            StartCoroutine(ColorAnimation(_turnText, targetColor, _fadeDuration));
        }

        private IEnumerator ColorAnimation(TextMeshProUGUI text, Color targetColor, float duration)
        {
            Color startColor = text.color;
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                text.color = Color.Lerp(startColor, targetColor, t);
                yield return null;
            }
            text.color = targetColor;
        }

        /// <summary>
        /// AI思考状態が変更されたとき
        /// </summary>
        private void OnAIThinkingChanged(bool isThinking)
        {
            if (_turnText == null) return;

            if (isThinking)
            {
                StartCoroutine(ColorAnimation(_turnText, _aiThinkingColor, _fadeDuration));
                // 思考中のパルスアニメーション
                StopAnimation(ref _pulseAnimation);
                _pulseAnimation = StartCoroutine(PulseAnimation());
            }
            else
            {
                StopAnimation(ref _pulseAnimation);
                _turnText.transform.localScale = Vector3.one;
                // 現在のマークに基づいて色を復元
                UpdateTextColor(ViewModel.CurrentMark.CurrentValue);
            }
        }

        private IEnumerator PulseAnimation()
        {
            while (true)
            {
                // 拡大
                yield return ScaleAnimation(_turnText.transform, Vector3.one, Vector3.one * 1.05f, 0.5f);
                // 縮小
                yield return ScaleAnimation(_turnText.transform, Vector3.one * 1.05f, Vector3.one, 0.5f);
            }
        }

        private IEnumerator ScaleAnimation(Transform target, Vector3 from, Vector3 to, float duration)
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                // EaseInOutSine
                t = -(Mathf.Cos(Mathf.PI * t) - 1f) / 2f;
                target.localScale = Vector3.Lerp(from, to, t);
                yield return null;
            }
            target.localScale = to;
        }

        /// <summary>
        /// 表示/非表示を設定
        /// </summary>
        private void SetVisible(bool visible)
        {
            if (_canvasGroup != null)
            {
                StopAnimation(ref _fadeAnimation);
                _fadeAnimation = StartCoroutine(FadeAnimation(visible ? 1f : 0f));
                _canvasGroup.interactable = visible;
                _canvasGroup.blocksRaycasts = visible;
            }
        }

        private IEnumerator FadeAnimation(float targetAlpha)
        {
            float startAlpha = _canvasGroup.alpha;
            float elapsed = 0f;
            while (elapsed < _fadeDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / _fadeDuration);
                _canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);
                yield return null;
            }
            _canvasGroup.alpha = targetAlpha;
        }

        private void StopAnimation(ref Coroutine coroutine)
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
                coroutine = null;
            }
        }

#if UNITY_EDITOR
        /// <summary>
        /// エディタ用：コンポーネント自動取得
        /// </summary>
        private void Reset()
        {
            if (_turnText == null)
            {
                _turnText = GetComponentInChildren<TextMeshProUGUI>();
            }
            if (_canvasGroup == null)
            {
                _canvasGroup = GetComponent<CanvasGroup>();
            }
        }
#endif
    }
}
