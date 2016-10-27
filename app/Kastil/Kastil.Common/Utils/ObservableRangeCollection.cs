using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Kastil.Common.Utils
{
    public class ObservableRangeCollection<T> : ObservableCollection<T>, IBatchable
    {
		public void InsertRange(IEnumerable<T> collection, int position = 0)
		{
			if (collection == null) throw new ArgumentNullException(nameof(collection));

			foreach (var i in collection) Items.Insert(position, i);
            TryRaiseCollectionChanged();
		}

        public void AddRange(IEnumerable<T> collection)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));

            foreach (var i in collection) Items.Add(i);
            TryRaiseCollectionChanged();
        }

        public void RemoveRange(IEnumerable<T> collection)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));

            foreach (var i in collection) Items.Remove(i);
            TryRaiseCollectionChanged();
        }

        public void Replace(T item)
        {
            ReplaceRange(new T[] { item });
        }

        public void ReplaceRange(IEnumerable<T> collection)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));

            Items.Clear();
            foreach (var i in collection) Items.Add(i);
            TryRaiseCollectionChanged();
        }

        public ObservableRangeCollection()
        { }

        public ObservableRangeCollection(IEnumerable<T> collection)
            : base(collection) { }

        private UpdateBatch _updateBatch;
        public UpdateBatch BatchRange()
        {
            if (_updateBatch != null)
                return _updateBatch;
            _updateBatch = new UpdateBatch(this);
            return _updateBatch;
        }

        public void Commit()
        {
            _updateBatch = null;
            TryRaiseCollectionChanged();
        }

        private void TryRaiseCollectionChanged()
        {
            if (_updateBatch == null)
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
    }

    public interface IBatchable
    {
        void Commit();
    }

    public class UpdateBatch : IDisposable
    {
        private readonly IBatchable _batchable;

        public UpdateBatch(IBatchable batchable)
        {
            _batchable = batchable;
        }

        public void Dispose()
        {
            _batchable.Commit();
        }
    }

}
