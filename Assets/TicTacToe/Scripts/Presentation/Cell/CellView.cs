using System.Collections;
using R3;
using TMPro;
using TicTacToe.Core.Domain;
using TicTacToe.Presentation.Base;
using UnityEngine;
using UnityEngine.UI;

namespace TicTacToe.Presentation.Cell
{
    /// <summary>
    /// 個別セルのView
    /// ボタンとマーク表示を担当
    /// </summary>
    public class CellView : ViewBase<CellViewModel>
    {
        [Header("UI Components")]
        [SerializeField] private Button _button;
        [SerializeField] private TextMeshProUGUI _markText;
        [SerializeField] private Image _backgroundImage;

        [Header("Visual Settings")]
        [SerializeField] private Color _emptyColor = new Color(0.9f, 0.9f, 0.9f);
        [SerializeField] private Color _xColor = new Color(0.2f, 0.6f, 1f);
        [SerializeField] private Color _oColor = new Color(1f, 0.4f, 0.4f);
        [SerializeField] private Color _disabledColor = new Color(0.7f, 0.7f, 0.7f);

        [Header("Animation Settings")]
        [SerializeField] private float _popDuration = 0.3f;
        [SerializeField] private float _highlightScale = 1.2f;

        private Coroutine _currentAnimation;

        protected override void OnBind(CellViewModel viewModel)
        {
            // セル状態の変更を購読してUIを更新
            viewModel.CellState
                .Subscribe(state => UpdateCellVisual(state))
                .AddTo(Disposables);

            // クリック可能状態を購読してボタン状態を更新
            viewModel.IsClickable
                .Subscribe(clickable => UpdateButtonInteractable(clickable))
                .AddTo(Disposables);

            // ボタンクリックをViewModelに通知
            _button.OnClickAsObservable()
                .Subscribe(_ => viewModel.OnClick())
                .AddTo(Disposables);

            // ハイライト状態を購読
            viewModel.IsHighlighted
                .Subscribe(highlighted => UpdateHighlight(highlighted))
                .AddTo(Disposables);
        }

        private void UpdateHighlight(bool highlighted)
        {
            StopCurrentAnimation();
            
            if (highlighted)
            {
                // 勝利ラインの強調アニメーション（ループ）
                _currentAnimation = StartCoroutine(HighlightPulseAnimation());
            }
            else
            {
                // 通常状態に戻す
                _markText.transform.localScale = Vector3.one;
            }
        }

        private IEnumerator HighlightPulseAnimation()
        {
            while (true)
            {
                // 拡大
                yield return ScaleAnimation(_markText.transform, Vector3.one, Vector3.one * _highlightScale, 0.4f);
                // 縮小
                yield return ScaleAnimation(_markText.transform, Vector3.one * _highlightScale, Vector3.one, 0.4f);
            }
        }

        /// <summary>
        /// セルの表示を更新
        /// </summary>
        private void UpdateCellVisual(CellState state)
        {
            switch (state)
            {
                case CellState.Empty:
                    _markText.text = "";
                    _markText.transform.localScale = Vector3.one;
                    if (_backgroundImage != null)
                    {
                        _backgroundImage.color = _emptyColor;
                    }
                    break;

                case CellState.X:
                    _markText.text = "X";
                    _markText.color = _xColor;
                    if (_backgroundImage != null)
                    {
                        _backgroundImage.color = Color.white;
                    }
                    AnimateMark();
                    break;

                case CellState.O:
                    _markText.text = "O";
                    _markText.color = _oColor;
                    if (_backgroundImage != null)
                    {
                        _backgroundImage.color = Color.white;
                    }
                    AnimateMark();
                    break;
            }
        }

        private void AnimateMark()
        {
            if (_markText == null) return;

            StopCurrentAnimation();
            _currentAnimation = StartCoroutine(PopAnimation());
        }

        private IEnumerator PopAnimation()
        {
            // OutBack 風のポップアップアニメーション
            _markText.transform.localScale = Vector3.zero;
            yield return ScaleAnimationWithOvershoot(_markText.transform, Vector3.zero, Vector3.one, _popDuration);
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

        private IEnumerator ScaleAnimationWithOvershoot(Transform target, Vector3 from, Vector3 to, float duration)
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                // OutBack easing (overshoot)
                float c1 = 1.70158f;
                float c3 = c1 + 1f;
                t = 1f + c3 * Mathf.Pow(t - 1f, 3f) + c1 * Mathf.Pow(t - 1f, 2f);
                target.localScale = Vector3.LerpUnclamped(from, to, t);
                yield return null;
            }
            target.localScale = to;
        }

        private void StopCurrentAnimation()
        {
            if (_currentAnimation != null)
            {
                StopCoroutine(_currentAnimation);
                _currentAnimation = null;
            }
        }

        /// <summary>
        /// ボタンのインタラクション状態を更新
        /// </summary>
        private void UpdateButtonInteractable(bool interactable)
        {
            _button.interactable = interactable;

            // 無効時は視覚的フィードバック
            if (!interactable && ViewModel.CellState.CurrentValue == CellState.Empty)
            {
                if (_backgroundImage != null)
                {
                    _backgroundImage.color = _disabledColor;
                }
            }
        }

#if UNITY_EDITOR
        /// <summary>
        /// エディタ用：コンポーネント自動取得
        /// </summary>
        private void Reset()
        {
            if (_button == null)
            {
                _button = GetComponent<Button>();
            }
            if (_markText == null)
            {
                _markText = GetComponentInChildren<TextMeshProUGUI>();
            }
            if (_backgroundImage == null)
            {
                _backgroundImage = GetComponent<Image>();
            }
        }
#endif
    }
}
