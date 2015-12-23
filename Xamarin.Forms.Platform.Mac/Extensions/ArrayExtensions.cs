using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace Xamarin.Forms.Platform.Mac
{
	internal static class ArrayExtensions
	{
		public static T[] Insert<T> (this T[] self, int index, T item)
		{
			T[] objArray = new T[self.Length + 1];
			if (index > 0)
				Array.Copy ((Array)self, (Array)objArray, index);
			objArray [index] = item;
			if (index < self.Length)
				Array.Copy ((Array)self, index, (Array)objArray, index + 1, objArray.Length - index - 1);
			return objArray;
		}

		public static T[] RemoveAt<T> (this T[] self, int index)
		{
			T[] objArray = new T[self.Length - 1];
			if (index > 0)
				Array.Copy ((Array)self, (Array)objArray, index);
			if (index < self.Length - 1)
				Array.Copy ((Array)self, index + 1, (Array)objArray, index, self.Length - index - 1);
			return objArray;
		}

		public static T[] Remove<T> (this T[] self, T item)
		{
			return ArrayExtensions.RemoveAt<T> (self, IndexOf<T> (self, item));
		}

		public static int IndexOf<T> (this IEnumerable<T> source, T value)
		{
			int index = 0;
			var comparer = EqualityComparer<T>.Default; // or pass in as a parameter
			foreach (T item in source)
			{
				if (comparer.Equals (item, value))
					return index;
				index++;
			}
			return -1;
		}
	}
}
