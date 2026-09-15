//
//  Copyright 2011  Ekon Benefits
//
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//
//        http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
//
// Vendored from Dynamitey v3.0.3 (https://github.com/ekonbenefits/dynamitey,
// commit 38fcb82). Trimmed to what FSharp.Interop.Dynamic calls; see
// THIRD-PARTY-NOTICES.txt. Changed: the static factory fields are plain
// lambdas; upstream built them with Return<T>.Arguments, which is not vendored.

using System;
using System.ComponentModel;
using System.Linq;

namespace FSharp.Interop.Dynamic.BridgeSupport
{
    /// <summary>
    /// String or InvokeMemberName
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public abstract class String_OR_InvokeMemberName
    {
        /// <summary>
        /// Performs an implicit conversion from <see cref="System.String"/> to <see cref="String_OR_InvokeMemberName"/>.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator String_OR_InvokeMemberName(string name)
        {
            return new InvokeMemberName(name, null);
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; protected set; }

        /// <summary>
        /// Gets the generic args.
        /// </summary>
        /// <value>The generic args.</value>
        public Type[] GenericArgs { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether this member is special name.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is special name; otherwise, <c>false</c>.
        /// </value>
        public bool IsSpecialName { get; protected set; }
    }

    /// <summary>
    /// Name of Member with associated Generic parameters
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public sealed class InvokeMemberName : String_OR_InvokeMemberName
    {
        /// <summary>
        /// Create Function can set to variable to make cleaner syntax;
        /// </summary>
        public static readonly Func<string, Type[], InvokeMemberName> Create =
            (n, a) => new InvokeMemberName(n, a);

        /// <summary>
        /// Create Function can set to variable to make cleaner syntax;
        /// </summary>
        public static readonly Func<string, InvokeMemberName> CreateSpecialName =
            n => new InvokeMemberName(n, true);

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.String"/> to <see cref="InvokeMemberName"/>.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator InvokeMemberName(string name)
        {
            return new InvokeMemberName(name, null);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvokeMemberName"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="genericArgs">The generic args.</param>
        public InvokeMemberName(string name, params Type[] genericArgs)
        {
            Name = name;
            GenericArgs = genericArgs;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvokeMemberName"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="isSpecialName">if set to <c>true</c> [is special name].</param>
        public InvokeMemberName(string name, bool isSpecialName)
        {
            Name = name;
            GenericArgs = Array.Empty<Type>();
            IsSpecialName = isSpecialName;
        }

        /// <summary>
        /// Equalses the specified other.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns></returns>
        public bool Equals(InvokeMemberName other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return EqualsHelper(other);
        }

        private bool EqualsHelper(InvokeMemberName other)
        {
            var tGenArgs = GenericArgs;
            var tOtherGenArgs = other.GenericArgs;

            return Equals(other.Name, Name)
                && !(other.IsSpecialName ^ IsSpecialName)
                && !(tOtherGenArgs == null ^ tGenArgs == null)
                && (tGenArgs == null ||
                //Exclusive Or makes sure this doesn't happen
                tGenArgs.SequenceEqual(tOtherGenArgs));
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        /// 	<c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (!(obj is InvokeMemberName)) return false;
            return EqualsHelper((InvokeMemberName)obj);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return (GenericArgs != null ? GenericArgs.Length.GetHashCode() * 397 : 0) ^ (Name.GetHashCode());
            }
        }
    }
}
