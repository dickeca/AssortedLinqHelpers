using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinqHelpers
{
    public static class LinqExtensions
    {
        /// <summary>
        /// Distinct Operator which takes a Predicate instead of an IEqualityOperator
        /// </summary>
        /// <typeparam name="TSource">Source type of the Collection you're performing the distinct operation on.</typeparam>
        /// <param name="source">The collection you're perfroming the distinct operation on. This is not modified in the process</param>
        /// <param name="predicate">A comparator function. Proper use means this functions determines if the two TSources are "equivalent."</param>
        /// <returns>An IEnumerable of type TSource that contains all elements that meet the "distinct" criteria provided by the predicate.</returns>

        public static IEnumerable<TSource> Distinct<TSource>(this IEnumerable<TSource> source, Func<TSource, TSource, bool> predicate)
        {
            if (source == null)
                throw new ArgumentException("source");
            if (predicate == null)
                throw new ArgumentException("predicate");
            var output = new List<TSource>();
            using (IEnumerator<TSource> enumerator = source.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    var add = output.All(t => !predicate(enumerator.Current, t));
                    if (add)
                    {
                        output.Add(enumerator.Current);
                    }
                }
            }
            return output.AsEnumerable();
        }

        /// <summary>
        /// Sums a collection 
        /// </summary>
        /// <typeparam name="TSource">The type of the items being summed from the collection</typeparam>
        /// <typeparam name="TOutput">The desired output type. Requires a default constructor so the function sum operates correctly on an empty collection.</typeparam>
        /// <param name="source">The collection you are summing.</param>
        /// <param name="predicate">The function that defines how to a TSource to the TOutput</param>
        /// <param name="converter">Converts the first member of the collection into TOutput form</param>
        /// <returns>The sum of the collection, as defined by the provided predicate.</returns>
        public static TOutput Sum<TSource, TOutput>(this IEnumerable<TSource> source,
            Func<TOutput, TSource, TOutput> predicate, Func<TSource, TOutput> converter) where TOutput : new()
        {
            if (source == null)
                throw new ArgumentException("source");
            if (predicate == null)
                throw new ArgumentException("predicate");
            using (IEnumerator<TSource> enumerator = source.GetEnumerator())
            {
                if (!enumerator.MoveNext()) return new TOutput();
                var first = converter(enumerator.Current);
                if (!enumerator.MoveNext()) return first;
                var second = enumerator.Current;
                var output = predicate(first, second);
                while (enumerator.MoveNext())
                {
                    predicate(output, enumerator.Current);
                }
                return output;
            }
        }


    }

    public static class Reflection
    {
        /// <summary>
        /// Converts an anonymous type to a dynamic type so that properties can be added dynamically.
        /// </summary>
        /// <param name="value">The object to be converted.</param>
        /// <returns>System.Dynamic.ExpandoObject</returns>
        public static dynamic ToDynamic(this object value)
        {
            IDictionary<string, object> expando = new ExpandoObject();

            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(value.GetType()))
                expando.Add(property.Name, property.GetValue(value));

            return expando as ExpandoObject;
        }
    }
}
