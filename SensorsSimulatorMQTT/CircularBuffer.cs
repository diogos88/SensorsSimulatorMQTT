using System;
using System.Collections;
using System.Collections.Generic;

namespace diogos88.MQTT.SensorsSimulator
{
   public class CircularBuffer<T> : IEnumerable<T>
   {
      public delegate void BufferContentChangedEvent();
      public event BufferContentChangedEvent BufferContentChanged;

      private T[] m_Buffer;
      private int m_LatestIndex = -1;
      private bool m_BufferFull = false;
      
      public int BufferSize { get; private set; }

      public int Count
      {
         get
         {
            if (m_BufferFull)
               return BufferSize;
            else
               return m_LatestIndex + 1;
         }
      }

      public CircularBuffer(int size)
      {
         BufferSize = size;
         m_LatestIndex = -1;
         m_Buffer = new T[BufferSize];
      }

      public void Add(T item)
      {
         m_LatestIndex++;
         if (m_LatestIndex == BufferSize)
         {
            m_BufferFull = true;
            m_LatestIndex = 0;
         }
         m_Buffer[m_LatestIndex] = item;

         RaiseBufferContentChanged();
      }

      public void Clear()
      {
         m_Buffer = new T[BufferSize];
         m_LatestIndex = -1;

         RaiseBufferContentChanged();
      }

      public T First()
      {
         if (m_LatestIndex < 0)
            return default(T);
         else
            return m_Buffer[m_LatestIndex];
      }

      public IEnumerator<T> GetEnumerator()
      {
         if (m_LatestIndex < 0)
            yield break;
         int max = m_BufferFull ? BufferSize : m_LatestIndex + 1;
         for (int i = m_LatestIndex + 1; Wrap(i) < max - 1; i++)
         {
            yield return m_Buffer[Wrap(i)];
         }
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
         return GetEnumerator();
      }

      public T this[int i]
      {
         get
         {
            if ((!m_BufferFull && i > m_LatestIndex) || i < 0)
               throw new IndexOutOfRangeException();
            else
               return m_Buffer[Wrap(i)];
         }

         set
         {
            if ((!m_BufferFull && i > m_LatestIndex) || i < 0)
               throw new IndexOutOfRangeException();
            else
               m_Buffer[Wrap(i)] = value;

         }
      }

      internal int Wrap(int i)
      {
         int max = m_BufferFull ? BufferSize : m_LatestIndex + 1;
         if (i < 0)
            i += max * (-i / max + 1);
         return i % max;
      }

      private void RaiseBufferContentChanged()
      {
         if (BufferContentChanged != null)
            BufferContentChanged();
      }
   }
}
