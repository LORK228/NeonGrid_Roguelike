using UnityEngine;

namespace Time.Trackers
{
    /// <summary>
    /// Кольцевой буфер нулевой аллокации для записи снимков времени.
    /// Выделяет память только один раз при создании.
    /// </summary>
    public class TimeRewindBuffer<T>
    {
        private readonly T[] _buffer;
        private int _head; // Индекс последней записи

        public int Count { get; private set; }
        public int Capacity => _buffer.Length;

        public TimeRewindBuffer(int capacity)
        {
            _buffer = new T[capacity];
            Count = 0;
            _head = -1;
        }

        // Записываем новый снимок, сдвигая "голову" вперед
        public void Record(T item)
        {
            _head = (_head + 1) % Capacity;
            _buffer[_head] = item;
            
            if (Count < Capacity) 
                Count++;
        }

        // Возвращает последний записанный снимок и сдвигает "голову" назад
        public bool TryRewind(out T item)
        {
            if (Count == 0)
            {
                item = default;
                return false;
            }

            item = _buffer[_head];
            
            // Смещаем индекс назад с учетом закольцованности массива
            _head = (_head - 1 + Capacity) % Capacity;
            Count--;
            
            return true;
        }

        public void Clear()
        {
            Count = 0;
            _head = -1;
        }
    }
}