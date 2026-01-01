using System.Collections.Generic;
using R3;
using TicTacToe.Presentation.Base;
using TicTacToe.Presentation.Cell;
using UnityEngine;

namespace TicTacToe.Presentation.Board
{
    /// <summary>
    /// 盤面全体のView
    /// 3x3のグリッドレイアウトでCellViewを配置・管理
    /// </summary>
    public class BoardView : ViewBase<BoardViewModel>
    {
        [Header("Cell References")]
        [SerializeField] private List<CellView> _cellViews = new List<CellView>(9);

        [Header("Optional Components")]
        [SerializeField] private CanvasGroup _canvasGroup;

        protected override void OnBind(BoardViewModel viewModel)
        {
            // セル数の検証
            if (_cellViews.Count != 9)
            {
                Debug.LogError($"[BoardView] Expected 9 CellViews, but found {_cellViews.Count}");
                return;
            }

            // 各CellViewにCellViewModelをバインド
            for (int i = 0; i < 9; i++)
            {
                var cellView = _cellViews[i];
                var cellViewModel = viewModel.Cells[i];

                if (cellView != null)
                {
                    // 手動でViewModelを設定（Factory経由でない場合）
                    cellView.SetViewModel(cellViewModel);
                }
                else
                {
                    Debug.LogWarning($"[BoardView] CellView at index {i} is null");
                }
            }

            // 盤面のインタラクション状態を購読
            viewModel.IsBoardInteractable
                .Subscribe(interactable => UpdateBoardInteractable(interactable))
                .AddTo(Disposables);
        }

        /// <summary>
        /// 盤面のインタラクション状態を更新
        /// </summary>
        private void UpdateBoardInteractable(bool interactable)
        {
            if (_canvasGroup != null)
            {
                _canvasGroup.interactable = interactable;
            }
        }

        /// <summary>
        /// インデックスでセルビューを取得
        /// </summary>
        /// <param name="index">0-8のセルインデックス</param>
        /// <returns>CellView、範囲外の場合はnull</returns>
        public CellView GetCellView(int index)
        {
            if (index < 0 || index >= _cellViews.Count)
            {
                return null;
            }
            return _cellViews[index];
        }

        /// <summary>
        /// 行・列でセルビューを取得
        /// </summary>
        /// <param name="row">行（0-2）</param>
        /// <param name="col">列（0-2）</param>
        /// <returns>CellView、範囲外の場合はnull</returns>
        public CellView GetCellView(int row, int col)
        {
            if (row < 0 || row > 2 || col < 0 || col > 2)
            {
                return null;
            }
            return GetCellView(row * 3 + col);
        }

#if UNITY_EDITOR
        /// <summary>
        /// エディタ用：子オブジェクトからCellViewを自動収集
        /// </summary>
        [ContextMenu("Auto Collect Cell Views")]
        private void AutoCollectCellViews()
        {
            _cellViews.Clear();
            var cells = GetComponentsInChildren<CellView>(true);
            
            // インデックス順にソート（名前に数字が含まれている想定）
            var sortedCells = new List<CellView>(cells);
            sortedCells.Sort((a, b) => string.Compare(a.name, b.name, System.StringComparison.Ordinal));
            
            _cellViews.AddRange(sortedCells);
            
            Debug.Log($"[BoardView] Collected {_cellViews.Count} CellViews");
        }
#endif
    }
}
