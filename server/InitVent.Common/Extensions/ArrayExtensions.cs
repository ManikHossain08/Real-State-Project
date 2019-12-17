using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InitVent.Common.Extensions
{
    public static class ArrayExtensions
    {
        //public static Array[] ByDimension(Array array, int dimension)
        //{
        //}

        public static int GetRowLength<T>(this T[,] array)
        {
            return array.GetLength(0);
        }

        public static int GetColumnLength<T>(this T[,] array)
        {
            return array.GetLength(1);
        }

        public static T[] GetRow<T>(this T[,] array, int row)
        {
            return Enumerable.Range(0, GetColumnLength(array))
                .Select(col => array[row, col])
                .ToArray();
        }

        public static T[][] GetRows<T>(this T[,] array)
        {
            return Enumerable.Range(0, GetRowLength(array))
                .Select(col => GetRow(array, col))
                .ToArray();
        }

        public static T[] GetColumn<T>(this T[,] array, int col)
        {
            return Enumerable.Range(0, GetRowLength(array))
                .Select(row => array[row, col])
                .ToArray();
        }

        public static T[][] GetColumns<T>(this T[,] array)
        {
            return Enumerable.Range(0, GetColumnLength(array))
                .Select(col => GetColumn(array, col))
                .ToArray();
        }

        public static T[,] To2DArray<T>(this T[][] array)
        {
            if (array == null)
                return null;

            var rowCount = array.Length;
            var columnCount = array.Select(row => row.Length).FirstOrDefault();

            if (array.Any(row => row.Length != columnCount))
                throw new ArgumentException("Input array is jagged.", "array");

            var result = new T[rowCount, columnCount];
            for (int r = 0; r < rowCount; r++)
            {
                for (int c = 0; c < columnCount; c++)
                    result[r, c] = array[r][c];
            }

            return result;
        }

        public static T[,] Transpose<T>(this T[,] array)
        {
            var rowCount = GetRowLength(array);
            var columnCount = GetColumnLength(array);

            var result = new T[columnCount, rowCount];
            for (int r = 0; r < rowCount; r++)
            {
                for (int c = 0; c < columnCount; c++)
                    result[c, r] = array[r, c];
            }

            return result;
        }

        public static T[][] Transpose<T>(this T[][] array)
        {
            return array.To2DArray().Transpose().GetRows();
        }
    }
}
