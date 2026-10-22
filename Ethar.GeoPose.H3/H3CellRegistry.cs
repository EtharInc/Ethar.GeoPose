// <copyright file="H3CellRegistry.cs" company="Ethar">
// Copyright (c) Ethar. All rights reserved.
// </copyright>

namespace Ethar.GeoPose.H3
{
    using System;
    using System.Collections.Generic;
    using Ethar.GeoPose.DataTypes;
    using Ethar.GeoPose.Extensions;

    /// <summary>
    /// A registry of items placed at GeoPose positions that answers "which items are inside this H3 cell" at any resolution.
    /// </summary>
    /// <typeparam name="T">The item type, for example a GeoPose structural data unit or an application object.</typeparam>
    /// <remarks>
    /// Every item is stored once, keyed by its resolution 15 cell. Because the H3 index is hierarchical, all descendants of a cell at resolution r form one contiguous range of resolution 15 values, so a query is a binary search plus a scan of the matches.
    /// Adding is O(1); the first query after an add sorts the registry, O(n log n); each query is then O(log n + matches). The registry is not thread safe.
    /// Containment is by the index hierarchy, the same rule as <see cref="H3Index.Contains"/>. A point close to a cell edge can be projected directly into a neighbouring cell at a coarse resolution while its resolution 15 cell descends from this one; see the package readme.
    /// </remarks>
    public sealed class H3CellRegistry<T>
    {
        private const int FinestResolution = 15;

        private readonly List<Entry> entries = new List<Entry>();

        private bool sorted = true;

        /// <summary>
        /// Gets the number of registered items.
        /// </summary>
        public int Count => this.entries.Count;

        /// <summary>
        /// Gets the registered items in no particular order.
        /// </summary>
        public IEnumerable<T> Items
        {
            get
            {
                foreach (var entry in this.entries)
                {
                    yield return entry.Item;
                }
            }
        }

        /// <summary>
        /// Registers an item at a position. The height is ignored.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="position">The position in degrees.</param>
        public void Add(T item, TangentPointPosition position)
        {
            this.Add(item, position.ToH3Cell(FinestResolution));
        }

        /// <summary>
        /// Registers an item at a cell. A cell coarser than resolution 15 is stored by its centre child, so it is found by queries for that cell and its ancestors, but only by queries for finer cells that contain the centre.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="cell">The cell.</param>
        public void Add(T item, H3Index cell)
        {
            var key = cell.Resolution == FinestResolution ? cell : cell.CenterChild(FinestResolution);
            this.entries.Add(new Entry(key.Value, item));
            this.sorted = false;
        }

        /// <summary>
        /// Removes the first registration of an item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>True if the item was registered.</returns>
        public bool Remove(T item)
        {
            var comparer = EqualityComparer<T>.Default;
            for (var i = 0; i < this.entries.Count; i++)
            {
                if (comparer.Equals(this.entries[i].Item, item))
                {
                    this.entries.RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Removes every registration.
        /// </summary>
        public void Clear()
        {
            this.entries.Clear();
            this.sorted = true;
        }

        /// <summary>
        /// Returns the items whose position lies in the cell.
        /// </summary>
        /// <param name="cell">The cell at any resolution.</param>
        /// <returns>The items, in ascending order of their resolution 15 cell.</returns>
        public List<T> FindWithin(H3Index cell)
        {
            this.EnsureSorted();
            Range(cell, out var min, out var max);
            var first = this.LowerBound(min);
            var end = this.UpperBound(max);
            var found = new List<T>(Math.Max(0, end - first));
            for (var i = first; i < end; i++)
            {
                found.Add(this.entries[i].Item);
            }

            return found;
        }

        /// <summary>
        /// Returns the items that share the cell of a position at the given resolution.
        /// </summary>
        /// <remarks>
        /// The cell is taken from the position's resolution 15 cell by the index hierarchy, the same rule the registry uses for its keys, so a registered position always finds itself.
        /// </remarks>
        /// <param name="position">The position in degrees. The height is ignored.</param>
        /// <param name="resolution">The H3 resolution, 0 to 15.</param>
        /// <returns>The items.</returns>
        public List<T> FindWithin(TangentPointPosition position, int resolution)
        {
            return this.FindWithin(position.ToH3Cell(FinestResolution).Parent(resolution));
        }

        /// <summary>
        /// Counts the items whose position lies in the cell without materialising them.
        /// </summary>
        /// <param name="cell">The cell at any resolution.</param>
        /// <returns>The number of items.</returns>
        public int CountWithin(H3Index cell)
        {
            this.EnsureSorted();
            Range(cell, out var min, out var max);
            return this.UpperBound(max) - this.LowerBound(min);
        }

        /// <summary>
        /// Computes the inclusive range of resolution 15 values that are descendants of the cell.
        /// The minimum is the centre child; the maximum has every digit below the cell's resolution set to 6, the highest child digit.
        /// </summary>
        private static void Range(H3Index cell, out ulong min, out ulong max)
        {
            min = cell.CenterChild(FinestResolution).Value;
            max = min;
            for (var resolution = cell.Resolution + 1; resolution <= FinestResolution; resolution++)
            {
                max = H3.SetIndexDigit(max, resolution, Direction.IjAxes);
            }
        }

        private void EnsureSorted()
        {
            if (!this.sorted)
            {
                this.entries.Sort(Entry.CompareKeys);
                this.sorted = true;
            }
        }

        /// <summary>
        /// First position whose key is at least <paramref name="key"/>.
        /// </summary>
        private int LowerBound(ulong key)
        {
            var low = 0;
            var high = this.entries.Count;
            while (low < high)
            {
                var mid = low + ((high - low) >> 1);
                if (this.entries[mid].Key < key)
                {
                    low = mid + 1;
                }
                else
                {
                    high = mid;
                }
            }

            return low;
        }

        /// <summary>
        /// First position whose key is greater than <paramref name="key"/>.
        /// </summary>
        private int UpperBound(ulong key)
        {
            var low = 0;
            var high = this.entries.Count;
            while (low < high)
            {
                var mid = low + ((high - low) >> 1);
                if (this.entries[mid].Key <= key)
                {
                    low = mid + 1;
                }
                else
                {
                    high = mid;
                }
            }

            return low;
        }

        private readonly struct Entry
        {
            public Entry(ulong key, T item)
            {
                this.Key = key;
                this.Item = item;
            }

            public ulong Key { get; }

            public T Item { get; }

            public static int CompareKeys(Entry a, Entry b) => a.Key.CompareTo(b.Key);
        }
    }
}
