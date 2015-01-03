using System;
using System.Collections;
using System.Collections.Generic;

namespace diogos88.MQTT.SensorsSimulator
{
   /// <summary>
   /// Circular buffer.
   /// https://github.com/joaoportela/CircullarBuffer-CSharp/tree/master/CircularBuffer
   /// 
   /// When writting to a full buffer:
   /// PushBack -> removes this[0] / Front()
   /// PushFront -> removes this[Size-1] / Back()
   /// 
   /// this implementation is inspired by
   /// http://www.boost.org/doc/libs/1_53_0/libs/circular_buffer/doc/circular_buffer.html
   /// because I liked their interface.
   /// </summary>
   public class CircularBuffer<T> : IEnumerable<T>
   {
      public delegate void BufferContentChangedEvent();
      public event BufferContentChangedEvent BufferContentChanged;

      private readonly T[] m_Buffer;
      private int m_Start;
      private int m_End;
      private int m_Size;

      public CircularBuffer(int capacity) : this(capacity, new T[] { })
      {
      }

      /// <summary>
      /// Initializes a new instance of the <see cref="CircularBuffer{T}"/> class.
      /// 
      /// </summary>
      /// <param name='capacity'>
      /// Buffer capacity. Must be positive.
      /// </param>
      /// <param name='items'>
      /// Items to fill buffer with. Items length must be less than capacity.
      /// Sugestion: use Skip(x).Take(y).ToArray() to build this argument from
      /// any enumerable.
      /// </param>
      public CircularBuffer(int capacity, T[] items)
      {
         if (capacity < 1)
         {
            throw new ArgumentException("Circular buffer cannot have negative or zero capacity.", "capacity");
         }
         if (items == null)
         {
            throw new ArgumentNullException("items");
         }
         if (items.Length > capacity)
         {
            throw new ArgumentException("Too many items to fit circular buffer", "items");
         }

         m_Buffer = new T[capacity];

         Array.Copy(items, m_Buffer, items.Length);
         m_Size = items.Length;

         m_Start = 0;
         m_End = m_Size == capacity ? 0 : m_Size;
      }

      /// <summary>
      /// Maximum capacity of the buffer. Elements pushed into the buffer after
      /// maximum capacity is reached (IsFull = true), will remove an element.
      /// </summary>
      public int Capacity { get { return m_Buffer.Length; } }

      public bool IsFull
      {
         get
         {
            return Size == Capacity;
         }
      }

      public bool IsEmpty
      {
         get
         {
            return Size == 0;
         }
      }

      /// <summary>
      /// Current buffer size (the number of elements that the buffer has).
      /// </summary>
      public int Size { get { return m_Size; } }

      /// <summary>
      /// Element at the front of the buffer - this[0].
      /// </summary>
      /// <returns>The value of the element of type T at the front of the buffer.</returns>
      public T Front()
      {
         ThrowIfEmpty();
         return m_Buffer[m_Start];
      }

      /// <summary>
      /// Element at the back of the buffer - this[Size - 1].
      /// </summary>
      /// <returns>The value of the element of type T at the back of the buffer.</returns>
      public T Back()
      {
         ThrowIfEmpty();
         return m_Buffer[(m_End != 0 ? m_End : m_Size) - 1];
      }

      public T this[int index]
      {
         get
         {
            if (IsEmpty)
            {
               throw new IndexOutOfRangeException(string.Format("Cannot access index {0}. Buffer is empty", index));
            }
            if (index >= m_Size)
            {
               throw new IndexOutOfRangeException(string.Format("Cannot access index {0}. Buffer size is {1}", index, m_Size));
            }
            int actualIndex = InternalIndex(index);
            return m_Buffer[actualIndex];
         }
         set
         {
            if (IsEmpty)
            {
               throw new IndexOutOfRangeException(string.Format("Cannot access index {0}. Buffer is empty", index));
            }
            if (index >= m_Size)
            {
               throw new IndexOutOfRangeException(string.Format("Cannot access index {0}. Buffer size is {1}", index, m_Size));
            }
            int actualIndex = InternalIndex(index);
            m_Buffer[actualIndex] = value;
         }
      }

      /// <summary>
      /// Pushes a new element to the back of the buffer. Back()/this[Size-1]
      /// will now return this element.
      /// 
      /// When the buffer is full, the element at Front()/this[0] will be 
      /// popped to allow for this new element to fit.
      /// </summary>
      /// <param name="item">Item to push to the back of the buffer</param>
      public void PushBack(T item)
      {
         if (IsFull)
         {
            m_Buffer[m_End] = item;
            Increment(ref m_End);
            m_Start = m_End;
         }
         else
         {
            m_Buffer[m_End] = item;
            Increment(ref m_End);
            ++m_Size;
         }

         if (BufferContentChanged != null)
            BufferContentChanged();
      }

      /// <summary>
      /// Pushes a new element to the front of the buffer. Front()/this[0]
      /// will now return this element.
      /// 
      /// When the buffer is full, the element at Back()/this[Size-1] will be 
      /// popped to allow for this new element to fit.
      /// </summary>
      /// <param name="item">Item to push to the front of the buffer</param>
      public void PushFront(T item)
      {
         if (IsFull)
         {
            Decrement(ref m_Start);
            m_End = m_Start;
            m_Buffer[m_Start] = item;
         }
         else
         {
            Decrement(ref m_Start);
            m_Buffer[m_Start] = item;
            ++m_Size;
         }

         if (BufferContentChanged != null)
            BufferContentChanged();
      }

      /// <summary>
      /// Removes the element at the back of the buffer. Decreassing the 
      /// Buffer size by 1.
      /// </summary>
      public void PopBack()
      {
         ThrowIfEmpty("Cannot take elements from an empty buffer.");
         Decrement(ref m_End);
         m_Buffer[m_End] = default(T);
         --m_Size;

         if (BufferContentChanged != null)
            BufferContentChanged();
      }

      /// <summary>
      /// Removes the element at the front of the buffer. Decreassing the 
      /// Buffer size by 1.
      /// </summary>
      public void PopFront()
      {
         ThrowIfEmpty("Cannot take elements from an empty buffer.");
         m_Buffer[m_Start] = default(T);
         Increment(ref m_Start);
         --m_Size;

         if (BufferContentChanged != null)
            BufferContentChanged();
      }

      /// <summary>
      /// Copies the buffer contents to an array, acording to the logical
      /// contents of the buffer (i.e. independent of the internal 
      /// order/contents)
      /// </summary>
      /// <returns>A new array with a copy of the buffer contents.</returns>
      public T[] ToArray()
      {
         T[] newArray = new T[Size];
         int newArrayOffset = 0;
         var segments = new ArraySegment<T>[2] { ArrayOne(), ArrayTwo() };
         foreach (ArraySegment<T> segment in segments)
         {
            Array.Copy(segment.Array, segment.Offset, newArray, newArrayOffset, segment.Count);
            newArrayOffset += segment.Count;
         }
         return newArray;
      }

      #region IEnumerable<T> implementation
      public IEnumerator<T> GetEnumerator()
      {
         var segments = new ArraySegment<T>[2] { ArrayOne(), ArrayTwo() };
         foreach (ArraySegment<T> segment in segments)
         {
            for (int i = 0; i < segment.Count; i++)
            {
               yield return segment.Array[segment.Offset + i];
            }
         }
      }
      #endregion
      #region IEnumerable implementation
      IEnumerator IEnumerable.GetEnumerator()
      {
         return (IEnumerator)GetEnumerator();
      }
      #endregion

      private void ThrowIfEmpty(string message = "Cannot access an empty buffer.")
      {
         if (IsEmpty)
         {
            throw new InvalidOperationException(message);
         }
      }

      /// <summary>
      /// Increments the provided index variable by one, wrapping
      /// around if necessary.
      /// </summary>
      /// <param name="index"></param>
      private void Increment(ref int index)
      {
         if (++index == Capacity)
         {
            index = 0;
         }
      }

      /// <summary>
      /// Decrements the provided index variable by one, wrapping
      /// around if necessary.
      /// </summary>
      /// <param name="index"></param>
      private void Decrement(ref int index)
      {
         if (index == 0)
         {
            index = Capacity;
         }
         index--;
      }

      /// <summary>
      /// Converts the index in the argument to an index in <code>_buffer</code>
      /// </summary>
      /// <returns>
      /// The transformed index.
      /// </returns>
      /// <param name='index'>
      /// External index.
      /// </param>
      private int InternalIndex(int index)
      {
         return m_Start + (index < (Capacity - m_Start) ? index : index - Capacity);
      }

      // doing ArrayOne and ArrayTwo methods returning ArraySegment<T> as seen here: 
      // http://www.boost.org/doc/libs/1_37_0/libs/circular_buffer/doc/circular_buffer.html#classboost_1_1circular__buffer_1957cccdcb0c4ef7d80a34a990065818d
      // http://www.boost.org/doc/libs/1_37_0/libs/circular_buffer/doc/circular_buffer.html#classboost_1_1circular__buffer_1f5081a54afbc2dfc1a7fb20329df7d5b
      // should help a lot with the code.

      // The array is composed by at most two non-contiguous segments, 
      // the next two methods allow easy access to those.

      private ArraySegment<T> ArrayOne()
      {
         if (m_Start < m_End)
         {
            return new ArraySegment<T>(m_Buffer, m_Start, m_End - m_Start);
         }
         else
         {
            return new ArraySegment<T>(m_Buffer, m_Start, m_Buffer.Length - m_Start);
         }
      }

      private ArraySegment<T> ArrayTwo()
      {
         if (m_Start < m_End)
         {
            return new ArraySegment<T>(m_Buffer, m_End, 0);
         }
         else
         {
            return new ArraySegment<T>(m_Buffer, 0, m_End);
         }
      }
   }
}