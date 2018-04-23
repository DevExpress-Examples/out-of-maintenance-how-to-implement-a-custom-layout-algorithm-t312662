#region #CustomLayoutAlgorithmImpl
using DevExpress.Xpf.TreeMap;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace CustomLayoutAlgorithmSample {
    class CustomSliceAndDiceLayoutAlgorithm : TreeMapLayoutAlgorithmBase, IComparer<ITreeMapLayoutItem> {
        // Cut the slice depending on the non-filled space width/height ratio.
        public override void Calculate(IList<ITreeMapLayoutItem> items, Size size) {
            double unlayoutedItemsWeight = 0;
            foreach (var item in items)
                unlayoutedItemsWeight += item.Weight;

            var sortedItems = items.ToList();
            sortedItems.Sort(this);

            Rect emptySpace = new Rect(0, 0, size.Width, size.Height);
            foreach (var item in sortedItems) {
                double itemWidth;
                double itemHeight;

                double newEmptySpaceX;
                double newEmptySpaceY;
                double newEmptySpaceWidth;
                double newEmptySpaceHeight;

                if (emptySpace.Width / emptySpace.Height > 1) {
                    itemWidth = emptySpace.Width * item.Weight / unlayoutedItemsWeight;
                    itemHeight = emptySpace.Height;

                    newEmptySpaceX = emptySpace.X + itemWidth;
                    newEmptySpaceY = emptySpace.Y;
                    newEmptySpaceHeight = emptySpace.Height;

                    newEmptySpaceWidth = emptySpace.Width - itemWidth;
                    newEmptySpaceWidth = newEmptySpaceWidth < 0 ? 0 : newEmptySpaceWidth;
                }
                else {
                    itemWidth = emptySpace.Width;
                    itemHeight = emptySpace.Height * item.Weight / unlayoutedItemsWeight;

                    newEmptySpaceX = emptySpace.X;
                    newEmptySpaceY = emptySpace.Y + itemHeight;
                    newEmptySpaceWidth = emptySpace.Width;

                    newEmptySpaceHeight = emptySpace.Height - itemHeight;
                    newEmptySpaceHeight = newEmptySpaceHeight < 0 ? 0 : newEmptySpaceHeight;
                }
                item.Layout = new Rect(emptySpace.X, emptySpace.Y, itemWidth, itemHeight);
                emptySpace = new Rect(
                    newEmptySpaceX,
                    newEmptySpaceY,
                    newEmptySpaceWidth,
                    newEmptySpaceHeight);
                unlayoutedItemsWeight -= item.Weight;
            }
        }

        public int Compare(ITreeMapLayoutItem x, ITreeMapLayoutItem y) {
            if (x.Weight > y.Weight) return -1;
            else if (x.Weight < y.Weight) return 1;
            else return 0;
        }

        protected override TreeMapDependencyObject CreateObject() {
            return new CustomSliceAndDiceLayoutAlgorithm();
        }
    }
}
#endregion #CustomLayoutAlgorithmImpl