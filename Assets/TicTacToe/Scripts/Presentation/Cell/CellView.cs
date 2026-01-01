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
                    break;

                case CellState.O:
                    _markText.text = "O";
                    _markText.color = _oColor;
                    if (_backgroundImage != null)
                    {
                        _backgroundImage.color = Color.white;
                    }
                    break;
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
