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
// commit 38fcb82, Internal/Optimization/Util.cs). Trimmed to what
// FSharp.Interop.Dynamic calls; see THIRD-PARTY-NOTICES.txt. Dropped: IsMono,
// IsAnonymousType, NameArgsIfNecessary, MassageResultBasedOnInterface. The
// class is internal here (upstream it was public).

using System;

namespace FSharp.Interop.Dynamic.BridgeSupport.Internal
{
    /// <summary>
    /// Utility Class
    /// </summary>
    internal static class Util
    {
        /// <summary>
        /// Gets the target context.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="context">The context.</param>
        /// <param name="staticContext">if set to <c>true</c> [static context].</param>
        /// <returns></returns>
        public static object GetTargetContext(this object target, out Type context, out bool staticContext)
        {
            var tInvokeContext = target as InvokeContext;
            staticContext = false;
            if (tInvokeContext != null)
            {
                staticContext = tInvokeContext.StaticContext;
                context = tInvokeContext.Context;
                context = context.FixContext();
                return tInvokeContext.Target;
            }

            context = target as Type ?? target.GetType();
            context = context.FixContext();
            return target;
        }

        /// <summary>
        /// Fixes the context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public static Type FixContext(this Type context)
        {
            if (context.IsArray)
            {
                return typeof(object);
            }
            return context;
        }

        internal static object[] GetArgsAndNames(object[] args, out string[] argNames)
        {
            if (args == null)
                args = new object[] { null };

            //Optimization: linq statement creates a slight overhead in this case
            argNames = new string[args.Length];

            var tArgSet = false;
            var tNewArgs = new object[args.Length];
            for (int i = 0; i < args.Length; i++)
            {
                var tArg = args[i];
                string tName = null;

                if (tArg is InvokeArg)
                {
                    tName = ((InvokeArg)tArg).Name;

                    tNewArgs[i] = ((InvokeArg)tArg).Value;
                    tArgSet = true;
                }
                else
                {
                    tNewArgs[i] = tArg;
                }
                argNames[i] = tName;
            }

            if (!tArgSet)
                argNames = null;
            return tNewArgs;
        }
    }
}
