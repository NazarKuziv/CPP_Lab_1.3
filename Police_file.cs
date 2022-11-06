using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace CPP_Lab_1._3
{
    // 1. Оголосити інтерфейс контейнера з підтримкою узагальненого типу T
    interface IPolice_file<T>
    {
        // Повернути ітератор на контейнер
        IIterator<T> CreateIterator();

        // Кількість елементів контейнера
        long Count();

        // Повернути елемент за його індексом
        T GetItem(long index);
    }

    // 2. Оголосити інтерфейс ітератора для типу T
    interface IIterator<T>
    {
        // Перехід на перший елемент контейнера
        void First();

        // Перехід на наступний елемент контейнера
        void Next();

        // Отримати поточний елемент
        T CurrentItem();

        // Перевірка, чи курсор вказує на кінець контейнера
        bool IsDone();
    }
    // 3. Конкретний ітератор
    class Police_file_Iterator<T> : IIterator<T>
    {
        // внутрішні дані
        private IPolice_file<T> prioner; // Посилання на агрегат
        private long current; // поточна позиція

        // Конструктор, який отримує агрегатний об'єкт
        public Police_file_Iterator(IPolice_file<T> obj)
        {
            prioner = obj;
            current = 0;
        }

        // Перевід курсору на початок списку
        virtual public void First()
        {
            current = 0;
        }

        // Перевід курсору на наступний елемент списку
        virtual public void Next()
        {
            current++;
        }

        // Перевірка, чи не кінець списку
        virtual public bool IsDone()
        {
            return current >= prioner.Count();
        }

        // Повернути поточний елемент списку
        virtual public T CurrentItem()
        {
            if (!IsDone())
                return prioner.GetItem(current);
            else
            {
                throw new NotImplementedException("Error");
            }
        }
    }


    // 4. Конкретний контейнер для елементів типу T
    [Serializable]
    public class Police_file<T> : IPolice_file<T>
    {
        // Внутрішні поля
        public T[] array; // динамічний масив елементів типу T
        public T arr;

        public T[] Get_arr() { return array; }
        // Конструктори
        // Конструктор, що отримує зовнішній масив

        public Police_file(T[] _array)
        {
            array = _array;
        }

        // Конструктор, що створює пустий масив
        public Police_file()
        {
            array = null;
        }

        // Метод, що додає елемент в кінець контейнера
        public void Append(T item)
        {
            T[] array2 = array;
            array = new T[array2.Length + 1];
            array2.CopyTo(array, 0);
            array[array.Length - 1] = item;
        }
        public void Edit(T item, long index)
        {
            array[index] = item;  
        }

        // Видаляє елемент з контейнера в позиції index
        public void Remove(long index)
        {
            T[] array2 = array;
            array = new T[array2.Length - 1];

            for (long i = 0; i < index; i++)
                array[i] = array2[i];

            for (long i = index + 1; i < array2.Length; i++)
                array[i - 1] = array2[i];
        }

        // Вивести вміст контейнеру
        public void Print()
        {
           
        }

        // Реалізація методів інтерфейсу 
        // Повернути ітератор
        IIterator<T> IPolice_file<T>.CreateIterator()
        {
            return new Police_file_Iterator<T>(this);
        }

        // Кількість елементів
        public long Count()
        {
            return array.Length;
        }
        
        // Повернути окремий елемент
        public T GetItem(long index)
        {
            if ((index >= 0) && (index < array.Length))
                return array[index];

            return arr;
        }
    }

    
}
