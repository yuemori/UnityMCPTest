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

        private float _targetAlpha = 1f;

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
            }
        }

        /// <summary>
        /// テキスト色を更新
        /// </summary>
        private void UpdateTextColor(CellState mark)
        {
            if (_turnText == null) return;

            _turnText.color = mark switch
            {
                CellState.X => _xColor,
                CellState.O => _oColor,
                _ => Color.white
            };
        }

        /// <summary>
        /// AI思考状態が変更されたとき
        /// </summary>
        private void OnAIThinkingChanged(bool isThinking)
        {
            if (_turnText == null) return;

            if (isThinking)
            {
                _turnText.color = _aiThinkingColor;
            }
            else
            {
                // 現在のマークに基づいて色を復元
                UpdateTextColor(ViewModel.CurrentMark.CurrentValue);
            }
        }

        /// <summary>
        /// 表示/非表示を設定
        /// </summary>
        private void SetVisible(bool visible)
        {
            _targetAlpha = visible ? 1f : 0f;

            if (_canvasGroup != null)
            {
                // アニメーションなしで即座に変更（シンプル実装）
                _canvasGroup.alpha = _targetAlpha;
                _canvasGroup.interactable = visible;
                _canvasGroup.blocksRaycasts = visible;
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
