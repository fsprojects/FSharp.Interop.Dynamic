// 
//  Copyright 2011 Ekon Benefits
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
// commit 38fcb82, Internal/Optimization/InvokeHelper.cs, the T4 output).
// Trimmed to what FSharp.Interop.Dynamic calls; see THIRD-PARTY-NOTICES.txt.
// Kept: the per-kind binder caches and the 0..14 argument call-site switches.
// Dropped: BinderConstructorCache, Func/Action/Tuple kind tables, TupleItem,
// WrapFuncHelper, WrapAction, FastDynamicInvoke*. Changed: the default (more
// than 14 arguments) branch calls InvokeLongCallSite instead of emitting a
// delegate type through ImpromptuInterface.

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace FSharp.Interop.Dynamic.BridgeSupport.Internal
{
    internal static class BinderCache<T> where T : class
    {
        private static IDictionary<BinderHash<T>, CallSite<T>> _cache;

        private static readonly object _cacheLock = new object();

        internal static IDictionary<BinderHash<T>, CallSite<T>> Cache
        {
            get
            {
                lock (_cacheLock)
                {
                    return _cache ?? (_cache = new Dictionary<BinderHash<T>, CallSite<T>>());
                }
            }
        }

        internal static readonly Action ClearCache = () =>
        {
            lock (_cacheLock)
            {
                _cache = null;
            }
        };
    }
    internal static class BinderGetCache<T> where T : class
    {
        private static IDictionary<BinderHash<T>, CallSite<T>> _cache;

        private static readonly object _cacheLock = new object();

        internal static IDictionary<BinderHash<T>, CallSite<T>> Cache
        {
            get
            {
                lock (_cacheLock)
                {
                    return _cache ?? (_cache = new Dictionary<BinderHash<T>, CallSite<T>>());
                }
            }
        }

        internal static readonly Action ClearCache = () =>
        {
            lock (_cacheLock)
            {
                _cache = null;
            }
        };
    }
    internal static class BinderSetCache<T> where T : class
    {
        private static IDictionary<BinderHash<T>, CallSite<T>> _cache;

        private static readonly object _cacheLock = new object();

        internal static IDictionary<BinderHash<T>, CallSite<T>> Cache
        {
            get
            {
                lock (_cacheLock)
                {
                    return _cache ?? (_cache = new Dictionary<BinderHash<T>, CallSite<T>>());
                }
            }
        }

        internal static readonly Action ClearCache = () =>
        {
            lock (_cacheLock)
            {
                _cache = null;
            }
        };
    }
    internal static class BinderMemberCache<T> where T : class
    {
        private static IDictionary<BinderHash<T>, CallSite<T>> _cache;

        private static readonly object _cacheLock = new object();

        internal static IDictionary<BinderHash<T>, CallSite<T>> Cache
        {
            get
            {
                lock (_cacheLock)
                {
                    return _cache ?? (_cache = new Dictionary<BinderHash<T>, CallSite<T>>());
                }
            }
        }

        internal static readonly Action ClearCache = () =>
        {
            lock (_cacheLock)
            {
                _cache = null;
            }
        };
    }
    internal static class BinderDirectCache<T> where T : class
    {
        private static IDictionary<BinderHash<T>, CallSite<T>> _cache;

        private static readonly object _cacheLock = new object();

        internal static IDictionary<BinderHash<T>, CallSite<T>> Cache
        {
            get
            {
                lock (_cacheLock)
                {
                    return _cache ?? (_cache = new Dictionary<BinderHash<T>, CallSite<T>>());
                }
            }
        }

        internal static readonly Action ClearCache = () =>
        {
            lock (_cacheLock)
            {
                _cache = null;
            }
        };
    }

    internal static partial class InvokeHelper
    {
        internal static void InvokeMemberAction(ref CallSite callsite,
                                                Type binderType,
                                                int knownType,
                                                LazyBinder binder,
                                                InvokeMemberName name,
                                                bool staticContext,
                                                Type context,
                                                string[] argNames,
                                                object target,
                                                params object[] args)
        {
            var tSwitch = args.Length;
            switch (tSwitch)
            {
#region Optimizations
                case 0:
                    {
                        var tCallSite = (CallSite<Action<CallSite, object>>)callsite;
                        if (tCallSite == null)
                        {
                            tCallSite = CreateCallSite<Action<CallSite, object>>(binderType, knownType, binder, name, context, argNames, staticContext);
                            callsite = tCallSite;
                        }
                        tCallSite.Target(tCallSite, target);
                        break;
                    }
                case 1:
                    {
                        var tCallSite = (CallSite<Action<CallSite, object, object>>)callsite;
                        if (tCallSite == null)
                        {
                            tCallSite = CreateCallSite<Action<CallSite, object, object>>(binderType, knownType, binder, name, context, argNames, staticContext);
                            callsite = tCallSite;
                        }
                        tCallSite.Target(tCallSite, target, args[0]);
                        break;
                    }
                case 2:
                    {
                        var tCallSite = (CallSite<Action<CallSite, object, object, object>>)callsite;
                        if (tCallSite == null)
                        {
                            tCallSite = CreateCallSite<Action<CallSite, object, object, object>>(binderType, knownType, binder, name, context, argNames, staticContext);
                            callsite = tCallSite;
                        }
                        tCallSite.Target(tCallSite, target, args[0], args[1]);
                        break;
                    }
                case 3:
                    {
                        var tCallSite = (CallSite<Action<CallSite, object, object, object, object>>)callsite;
                        if (tCallSite == null)
                        {
                            tCallSite = CreateCallSite<Action<CallSite, object, object, object, object>>(binderType, knownType, binder, name, context, argNames, staticContext);
                            callsite = tCallSite;
                        }
                        tCallSite.Target(tCallSite, target, args[0], args[1], args[2]);
                        break;
                    }
                case 4:
                    {
                        var tCallSite = (CallSite<Action<CallSite, object, object, object, object, object>>)callsite;
                        if (tCallSite == null)
                        {
                            tCallSite = CreateCallSite<Action<CallSite, object, object, object, object, object>>(binderType, knownType, binder, name, context, argNames, staticContext);
                            callsite = tCallSite;
                        }
                        tCallSite.Target(tCallSite, target, args[0], args[1], args[2], args[3]);
                        break;
                    }
                case 5:
                    {
                        var tCallSite = (CallSite<Action<CallSite, object, object, object, object, object, object>>)callsite;
                        if (tCallSite == null)
                        {
                            tCallSite = CreateCallSite<Action<CallSite, object, object, object, object, object, object>>(binderType, knownType, binder, name, context, argNames, staticContext);
                            callsite = tCallSite;
                        }
                        tCallSite.Target(tCallSite, target, args[0], args[1], args[2], args[3], args[4]);
                        break;
                    }
                case 6:
                    {
                        var tCallSite = (CallSite<Action<CallSite, object, object, object, object, object, object, object>>)callsite;
                        if (tCallSite == null)
                        {
                            tCallSite = CreateCallSite<Action<CallSite, object, object, object, object, object, object, object>>(binderType, knownType, binder, name, context, argNames, staticContext);
                            callsite = tCallSite;
                        }
                        tCallSite.Target(tCallSite, target, args[0], args[1], args[2], args[3], args[4], args[5]);
                        break;
                    }
                case 7:
                    {
                        var tCallSite = (CallSite<Action<CallSite, object, object, object, object, object, object, object, object>>)callsite;
                        if (tCallSite == null)
                        {
                            tCallSite = CreateCallSite<Action<CallSite, object, object, object, object, object, object, object, object>>(binderType, knownType, binder, name, context, argNames, staticContext);
                            callsite = tCallSite;
                        }
                        tCallSite.Target(tCallSite, target, args[0], args[1], args[2], args[3], args[4], args[5], args[6]);
                        break;
                    }
                case 8:
                    {
                        var tCallSite = (CallSite<Action<CallSite, object, object, object, object, object, object, object, object, object>>)callsite;
                        if (tCallSite == null)
                        {
                            tCallSite = CreateCallSite<Action<CallSite, object, object, object, object, object, object, object, object, object>>(binderType, knownType, binder, name, context, argNames, staticContext);
                            callsite = tCallSite;
                        }
                        tCallSite.Target(tCallSite, target, args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7]);
                        break;
                    }
                case 9:
                    {
                        var tCallSite = (CallSite<Action<CallSite, object, object, object, object, object, object, object, object, object, object>>)callsite;
                        if (tCallSite == null)
                        {
                            tCallSite = CreateCallSite<Action<CallSite, object, object, object, object, object, object, object, object, object, object>>(binderType, knownType, binder, name, context, argNames, staticContext);
                            callsite = tCallSite;
                        }
                        tCallSite.Target(tCallSite, target, args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8]);
                        break;
                    }
                case 10:
                    {
                        var tCallSite = (CallSite<Action<CallSite, object, object, object, object, object, object, object, object, object, object, object>>)callsite;
                        if (tCallSite == null)
                        {
                            tCallSite = CreateCallSite<Action<CallSite, object, object, object, object, object, object, object, object, object, object, object>>(binderType, knownType, binder, name, context, argNames, staticContext);
                            callsite = tCallSite;
                        }
                        tCallSite.Target(tCallSite, target, args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8], args[9]);
                        break;
                    }
                case 11:
                    {
                        var tCallSite = (CallSite<Action<CallSite, object, object, object, object, object, object, object, object, object, object, object, object>>)callsite;
                        if (tCallSite == null)
                        {
                            tCallSite = CreateCallSite<Action<CallSite, object, object, object, object, object, object, object, object, object, object, object, object>>(binderType, knownType, binder, name, context, argNames, staticContext);
                            callsite = tCallSite;
                        }
                        tCallSite.Target(tCallSite, target, args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8], args[9], args[10]);
                        break;
                    }
                case 12:
                    {
                        var tCallSite = (CallSite<Action<CallSite, object, object, object, object, object, object, object, object, object, object, object, object, object>>)callsite;
                        if (tCallSite == null)
                        {
                            tCallSite = CreateCallSite<Action<CallSite, object, object, object, object, object, object, object, object, object, object, object, object, object>>(binderType, knownType, binder, name, context, argNames, staticContext);
                            callsite = tCallSite;
                        }
                        tCallSite.Target(tCallSite, target, args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8], args[9], args[10], args[11]);
                        break;
                    }
                case 13:
                    {
                        var tCallSite = (CallSite<Action<CallSite, object, object, object, object, object, object, object, object, object, object, object, object, object, object>>)callsite;
                        if (tCallSite == null)
                        {
                            tCallSite = CreateCallSite<Action<CallSite, object, object, object, object, object, object, object, object, object, object, object, object, object, object>>(binderType, knownType, binder, name, context, argNames, staticContext);
                            callsite = tCallSite;
                        }
                        tCallSite.Target(tCallSite, target, args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8], args[9], args[10], args[11], args[12]);
                        break;
                    }
                case 14:
                    {
                        var tCallSite = (CallSite<Action<CallSite, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object>>)callsite;
                        if (tCallSite == null)
                        {
                            tCallSite = CreateCallSite<Action<CallSite, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object>>(binderType, knownType, binder, name, context, argNames, staticContext);
                            callsite = tCallSite;
                        }
                        tCallSite.Target(tCallSite, target, args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8], args[9], args[10], args[11], args[12], args[13]);
                        break;
                    }
#endregion
                default:
                    InvokeLongCallSite(ref callsite, binderType, knownType, binder, name, staticContext, context, argNames, typeof(object), typeof(void), target, args);
                    break;
            }
        }

        internal static TReturn InvokeMemberTargetType<TTarget, TReturn>(
                                        ref CallSite callsite,
                                        Type binderType,
                                        int knownType,
                                        LazyBinder binder,
                                        InvokeMemberName name,
                                        bool staticContext,
                                        Type context,
                                        string[] argNames,
                                        TTarget target, params object[] args)
        {
            var tSwitch = args.Length;

            switch (tSwitch)
            {
#region Optimizations
                case 0:
                    {
                        var tCallSite = (CallSite<Func<CallSite, TTarget, TReturn>>)callsite;
                        if (tCallSite == null)
                        {
                            tCallSite = CreateCallSite<Func<CallSite, TTarget, TReturn>>(binderType, knownType, binder, name, context, argNames, staticContext);
                            callsite = tCallSite;
                        }
                        return tCallSite.Target(tCallSite, target);
                    }
                case 1:
                    {
                        var tCallSite = (CallSite<Func<CallSite, TTarget, object, TReturn>>)callsite;
                        if (tCallSite == null)
                        {
                            tCallSite = CreateCallSite<Func<CallSite, TTarget, object, TReturn>>(binderType, knownType, binder, name, context, argNames, staticContext);
                            callsite = tCallSite;
                        }
                        return tCallSite.Target(tCallSite, target, args[0]);
                    }
                case 2:
                    {
                        var tCallSite = (CallSite<Func<CallSite, TTarget, object, object, TReturn>>)callsite;
                        if (tCallSite == null)
                        {
                            tCallSite = CreateCallSite<Func<CallSite, TTarget, object, object, TReturn>>(binderType, knownType, binder, name, context, argNames, staticContext);
                            callsite = tCallSite;
                        }
                        return tCallSite.Target(tCallSite, target, args[0], args[1]);
                    }
                case 3:
                    {
                        var tCallSite = (CallSite<Func<CallSite, TTarget, object, object, object, TReturn>>)callsite;
                        if (tCallSite == null)
                        {
                            tCallSite = CreateCallSite<Func<CallSite, TTarget, object, object, object, TReturn>>(binderType, knownType, binder, name, context, argNames, staticContext);
                            callsite = tCallSite;
                        }
                        return tCallSite.Target(tCallSite, target, args[0], args[1], args[2]);
                    }
                case 4:
                    {
                        var tCallSite = (CallSite<Func<CallSite, TTarget, object, object, object, object, TReturn>>)callsite;
                        if (tCallSite == null)
                        {
                            tCallSite = CreateCallSite<Func<CallSite, TTarget, object, object, object, object, TReturn>>(binderType, knownType, binder, name, context, argNames, staticContext);
                            callsite = tCallSite;
                        }
                        return tCallSite.Target(tCallSite, target, args[0], args[1], args[2], args[3]);
                    }
                case 5:
                    {
                        var tCallSite = (CallSite<Func<CallSite, TTarget, object, object, object, object, object, TReturn>>)callsite;
                        if (tCallSite == null)
                        {
                            tCallSite = CreateCallSite<Func<CallSite, TTarget, object, object, object, object, object, TReturn>>(binderType, knownType, binder, name, context, argNames, staticContext);
                            callsite = tCallSite;
                        }
                        return tCallSite.Target(tCallSite, target, args[0], args[1], args[2], args[3], args[4]);
                    }
                case 6:
                    {
                        var tCallSite = (CallSite<Func<CallSite, TTarget, object, object, object, object, object, object, TReturn>>)callsite;
                        if (tCallSite == null)
                        {
                            tCallSite = CreateCallSite<Func<CallSite, TTarget, object, object, object, object, object, object, TReturn>>(binderType, knownType, binder, name, context, argNames, staticContext);
                            callsite = tCallSite;
                        }
                        return tCallSite.Target(tCallSite, target, args[0], args[1], args[2], args[3], args[4], args[5]);
                    }
                case 7:
                    {
                        var tCallSite = (CallSite<Func<CallSite, TTarget, object, object, object, object, object, object, object, TReturn>>)callsite;
                        if (tCallSite == null)
                        {
                            tCallSite = CreateCallSite<Func<CallSite, TTarget, object, object, object, object, object, object, object, TReturn>>(binderType, knownType, binder, name, context, argNames, staticContext);
                            callsite = tCallSite;
                        }
                        return tCallSite.Target(tCallSite, target, args[0], args[1], args[2], args[3], args[4], args[5], args[6]);
                    }
                case 8:
                    {
                        var tCallSite = (CallSite<Func<CallSite, TTarget, object, object, object, object, object, object, object, object, TReturn>>)callsite;
                        if (tCallSite == null)
                        {
                            tCallSite = CreateCallSite<Func<CallSite, TTarget, object, object, object, object, object, object, object, object, TReturn>>(binderType, knownType, binder, name, context, argNames, staticContext);
                            callsite = tCallSite;
                        }
                        return tCallSite.Target(tCallSite, target, args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7]);
                    }
                case 9:
                    {
                        var tCallSite = (CallSite<Func<CallSite, TTarget, object, object, object, object, object, object, object, object, object, TReturn>>)callsite;
                        if (tCallSite == null)
                        {
                            tCallSite = CreateCallSite<Func<CallSite, TTarget, object, object, object, object, object, object, object, object, object, TReturn>>(binderType, knownType, binder, name, context, argNames, staticContext);
                            callsite = tCallSite;
                        }
                        return tCallSite.Target(tCallSite, target, args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8]);
                    }
                case 10:
                    {
                        var tCallSite = (CallSite<Func<CallSite, TTarget, object, object, object, object, object, object, object, object, object, object, TReturn>>)callsite;
                        if (tCallSite == null)
                        {
                            tCallSite = CreateCallSite<Func<CallSite, TTarget, object, object, object, object, object, object, object, object, object, object, TReturn>>(binderType, knownType, binder, name, context, argNames, staticContext);
                            callsite = tCallSite;
                        }
                        return tCallSite.Target(tCallSite, target, args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8], args[9]);
                    }
                case 11:
                    {
                        var tCallSite = (CallSite<Func<CallSite, TTarget, object, object, object, object, object, object, object, object, object, object, object, TReturn>>)callsite;
                        if (tCallSite == null)
                        {
                            tCallSite = CreateCallSite<Func<CallSite, TTarget, object, object, object, object, object, object, object, object, object, object, object, TReturn>>(binderType, knownType, binder, name, context, argNames, staticContext);
                            callsite = tCallSite;
                        }
                        return tCallSite.Target(tCallSite, target, args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8], args[9], args[10]);
                    }
                case 12:
                    {
                        var tCallSite = (CallSite<Func<CallSite, TTarget, object, object, object, object, object, object, object, object, object, object, object, object, TReturn>>)callsite;
                        if (tCallSite == null)
                        {
                            tCallSite = CreateCallSite<Func<CallSite, TTarget, object, object, object, object, object, object, object, object, object, object, object, object, TReturn>>(binderType, knownType, binder, name, context, argNames, staticContext);
                            callsite = tCallSite;
                        }
                        return tCallSite.Target(tCallSite, target, args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8], args[9], args[10], args[11]);
                    }
                case 13:
                    {
                        var tCallSite = (CallSite<Func<CallSite, TTarget, object, object, object, object, object, object, object, object, object, object, object, object, object, TReturn>>)callsite;
                        if (tCallSite == null)
                        {
                            tCallSite = CreateCallSite<Func<CallSite, TTarget, object, object, object, object, object, object, object, object, object, object, object, object, object, TReturn>>(binderType, knownType, binder, name, context, argNames, staticContext);
                            callsite = tCallSite;
                        }
                        return tCallSite.Target(tCallSite, target, args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8], args[9], args[10], args[11], args[12]);
                    }
                case 14:
                    {
                        var tCallSite = (CallSite<Func<CallSite, TTarget, object, object, object, object, object, object, object, object, object, object, object, object, object, object, TReturn>>)callsite;
                        if (tCallSite == null)
                        {
                            tCallSite = CreateCallSite<Func<CallSite, TTarget, object, object, object, object, object, object, object, object, object, object, object, object, object, object, TReturn>>(binderType, knownType, binder, name, context, argNames, staticContext);
                            callsite = tCallSite;
                        }
                        return tCallSite.Target(tCallSite, target, args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8], args[9], args[10], args[11], args[12], args[13]);
                    }
#endregion
                default:
                    return (TReturn)InvokeLongCallSite(ref callsite, binderType, knownType, binder, name, staticContext, context, argNames, typeof(TTarget), typeof(TReturn), target, args);
            }
        }
    }
}
