﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kros.KORM.Query
{
    /// <summary>
    /// Represent result of filter (where) operation.
    /// </summary>
    /// <typeparam name="T">Type of model class.</typeparam>
    /// <seealso cref="Kros.KORM.Query.IQueryBase{T}" />
    public interface IFilteredQuery<T> : IQueryBase<T>
    {
        /// <summary>
        /// Add order by statement to sql.
        /// </summary>
        /// <param name="orderBy">The order by statement.</param>
        /// <returns>
        /// Query for enumerable models.
        /// </returns>
        /// <example>
        /// <code source="..\Examples\Kros.KORM.Examples\IQueryExample.cs" title="OrderBy" region="OrderBy" lang="C#"  />
        /// </example>
        /// <exception cref="ArgumentNullException">if <c>orderBy</c> is null or white string.</exception>
        IOrderedQuery<T> OrderBy(string orderBy);

        /// <summary>
        /// Add group by statement to sql query.
        /// </summary>
        /// <param name="groupBy">The group by statement.</param>
        /// <returns>
        /// Query for enumerable models.
        /// </returns>
        /// <example>
        /// <code source="..\Examples\Kros.KORM.Examples\IQueryExample.cs" title="GroupBy" region="GroupBy" lang="C#"  />
        /// </example>
        /// <exception cref="ArgumentNullException">if <c>groupBy</c> is null or white string.</exception>
        IGroupedQuery<T> GroupBy(string groupBy);
    }
}
