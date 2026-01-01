using R3;
using TMPro;
using TicTacToe.Core.Domain;
using TicTacToe.Presentation.Base;
using UnityEngine;
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

        [Header("Visual Settings")]
        [SerializeField] private Color _xWinColor = new Color(0.2f, 0.6f, 1f);
        [SerializeField] private Color _oWinColor = new Color(1f, 0.4f, 0.4f);
        [SerializeField] private Color _drawColor = new Color(0.7f, 0.7f, 0.7f);

        [Header("Animation Settings")]
        [SerializeField] private float _fadeDuration = 0.3f;

        protected override void OnBind(ResultViewModel viewModel)
        {
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
            }
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
            if (_panelRoot != null)
            {
                _panelRoot.SetActive(visible);
            }

            if (_canvasGroup != null)
            {
                _canvasGroup.alpha = visible ? 1f : 0f;
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
}
