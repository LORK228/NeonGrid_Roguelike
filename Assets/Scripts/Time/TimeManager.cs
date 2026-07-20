using System.Collections.Generic;
using Time.Interfaces;
using UnityEngine;

namespace Time
{
    public class TimeManager : MonoBehaviour
    {
        private HashSet<ITimeTrackable> _trackablesSet = new HashSet<ITimeTrackable>();
        private List<ITimeTrackable> _trackablesList = new List<ITimeTrackable>(); 
        
        // Очереди отложенных операций
        private List<ITimeTrackable> _pendingAdds = new List<ITimeTrackable>();
        private List<ITimeTrackable> _pendingRemoves = new List<ITimeTrackable>();
        private bool _isIterating = false;

        public bool IsRewinding { get; private set; }

        public void Register(ITimeTrackable trackable)
        {
            if (_isIterating)
            {
                _pendingAdds.Add(trackable);
            }
            else if (_trackablesSet.Add(trackable))
            {
                _trackablesList.Add(trackable);
            }
        }

        public void Unregister(ITimeTrackable trackable)
        {
            if (_isIterating)
            {
                _pendingRemoves.Add(trackable);
            }
            else if (_trackablesSet.Remove(trackable))
            {
                _trackablesList.Remove(trackable);
            }
        }

        // Вернул методы на место и обернул их в безопасный try/finally
        public void StartRewind()
        {
            IsRewinding = true;
            _isIterating = true;
            
            try
            {
                for (int i = _trackablesList.Count - 1; i >= 0; i--)
                {
                    _trackablesList[i].StartRewind();
                }
            }
            finally
            {
                _isIterating = false;
                ProcessPendingChanges();
            }
        }

        public void StopRewind()
        {
            IsRewinding = false;
            _isIterating = true;
            
            try
            {
                for (int i = _trackablesList.Count - 1; i >= 0; i--)
                {
                    _trackablesList[i].StopRewind();
                }
            }
            finally
            {
                _isIterating = false;
                ProcessPendingChanges();
            }
        }

        private void FixedUpdate()
        {
            _isIterating = true; 

            try
            {
                if (IsRewinding)
                {
                    for (int i = _trackablesList.Count - 1; i >= 0; i--)
                        _trackablesList[i].RewindState();
                }
                else
                {
                    for (int i = _trackablesList.Count - 1; i >= 0; i--)
                        _trackablesList[i].RecordState();
                }
            }
            finally
            {
                _isIterating = false; 
                ProcessPendingChanges(); 
            }
        }

        private void ProcessPendingChanges()
        {
            if (_pendingAdds.Count > 0)
            {
                foreach (var add in _pendingAdds) Register(add);
                _pendingAdds.Clear();
            }

            if (_pendingRemoves.Count > 0)
            {
                foreach (var remove in _pendingRemoves) Unregister(remove);
                _pendingRemoves.Clear();
            }
        }
    }
}