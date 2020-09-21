using System;
using EawModinfo.Model;
using EawModinfo.Spec;

namespace EawModinfo.Utilities
{
    /// <summary>
    /// Provides various utilities for an <see cref="IModinfo"/>.
    /// </summary>
    public static class ModinfoDataUtilities
    {
        /// <summary>
        /// Validates both inputs then creates a copy of
        /// <paramref name="baseModinfo"/> and overrides existing data from <paramref name="target"/> into the new copy.
        /// If <paramref name="baseModinfo"/> is <see langword="null"/> this method will return <paramref name="target"/>.
        /// </summary>
        /// <remarks>Subsequent data such as <see cref="IModinfo.Languages"/> will be replaced entirely and not merged by property.
        /// Exceptions is made for <see cref="IModinfo.Custom"/>, where items will get merged individually.
        /// <br></br>
        /// Subsequent data get replaced by creating a new copy of that element. This means the new and the merged property are not equal by reference.
        /// </remarks>
        /// <param name="target">the data source from which data will get merged.</param>
        /// <param name="baseModinfo">Original data which will get updated.</param>
        /// <returns>A new instance of an <see cref="IModinfo"/> or <paramref name="target"/> if <paramref name="baseModinfo"/> was <see langword="null"/></returns>
        public static IModinfo MergeInto(this IModinfo target, IModinfo? baseModinfo)
        {
            if (target is null)
                throw new ArgumentNullException(nameof(target));
            if (baseModinfo is null)
                return target;
            target.Validate();
            baseModinfo.Validate();
            var newModinfo = new ModinfoData(baseModinfo);
            newModinfo.MergeFrom(target);
            return newModinfo;
        }
    }
}
