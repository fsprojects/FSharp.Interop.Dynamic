//
//  Copyright 2010  Ekon Benefits
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
// THIRD-PARTY-NOTICES.txt. Dropped: CreateCallSite, Linq, unary operators,
// InvokeSetAll, Curry, CoerceToDelegate, CoerceConvert, IsDBNull,
// ApplyEquivalentType, ConvertAll/ConvertEach, InvokeConstructor,
// FastDynamicInvoke, GenericDelegateType, GetMemberNames, InvokeCallSite and
// the COM / ImpromptuInterface / TypeDescriptor late-bound hooks.
// Changed: InvokeGetChain / InvokeSetChain read the Regex groups directly
// (upstream went through FluentRegex and CoerceConvert).

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using FSharp.Interop.Dynamic.BridgeSupport.Internal;

namespace FSharp.Interop.Dynamic.BridgeSupport
{
    /// <summary>
    /// Main API
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class Dynamic
    {
        /// <summary>
        /// Clears the dynamic binding caches.
        /// </summary>
        public static void ClearCaches()
        {
            InvokeHelper.ClearAllCaches();
        }

        /// <summary>
        /// Dynamically Invokes a member method using the DLR
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="name">The name. Can be a string it will be implicitly converted</param>
        /// <param name="args">The args.</param>
        /// <returns> The result</returns>
        public static dynamic InvokeMember(object target, String_OR_InvokeMemberName name, params object[] args)
        {
            target = target.GetTargetContext(out var context, out var staticContext);
            args = Util.GetArgsAndNames(args, out var argNames);
            CallSite callSite = null;

            return InvokeHelper.InvokeMemberCallSite(target, (InvokeMemberName)name, args, argNames, context, staticContext,
                                                     ref callSite);
        }

        /// <summary>
        /// Invokes the binary operator.
        /// </summary>
        /// <param name="leftArg">The left arg.</param>
        /// <param name="op">The op.</param>
        /// <param name="rightArg">The right Arg.</param>
        /// <returns></returns>
        public static dynamic InvokeBinaryOperator(dynamic leftArg, ExpressionType op, dynamic rightArg)
        {
            switch (op)
            {
                case ExpressionType.Add:
                    return leftArg + rightArg;
                case ExpressionType.AddAssign:
                    leftArg += rightArg;
                    return leftArg;
                case ExpressionType.AndAssign:
                    leftArg &= rightArg;
                    return leftArg;
                case ExpressionType.Divide:
                    return leftArg / rightArg;
                case ExpressionType.DivideAssign:
                    leftArg /= rightArg;
                    return leftArg;
                case ExpressionType.Equal:
                    return leftArg == rightArg;
                case ExpressionType.ExclusiveOr:
                    return leftArg ^ rightArg;
                case ExpressionType.ExclusiveOrAssign:
                    leftArg ^= rightArg;
                    return leftArg;
                case ExpressionType.GreaterThan:
                    return leftArg > rightArg;
                case ExpressionType.GreaterThanOrEqual:
                    return leftArg >= rightArg;
                case ExpressionType.LeftShift:
                    return leftArg << rightArg;
                case ExpressionType.LeftShiftAssign:
                    leftArg <<= rightArg;
                    return leftArg;
                case ExpressionType.LessThan:
                    return leftArg < rightArg;
                case ExpressionType.LessThanOrEqual:
                    return leftArg <= rightArg;
                case ExpressionType.Modulo:
                    return leftArg % rightArg;
                case ExpressionType.ModuloAssign:
                    leftArg %= rightArg;
                    return leftArg;
                case ExpressionType.Multiply:
                    return leftArg * rightArg;
                case ExpressionType.MultiplyAssign:
                    leftArg *= rightArg;
                    return leftArg;
                case ExpressionType.NotEqual:
                    return leftArg != rightArg;
                case ExpressionType.OrAssign:
                    leftArg |= rightArg;
                    return leftArg;
                case ExpressionType.RightShift:
                    return leftArg >> rightArg;
                case ExpressionType.RightShiftAssign:
                    leftArg >>= rightArg;
                    return leftArg;
                case ExpressionType.Subtract:
                    return leftArg - rightArg;
                case ExpressionType.SubtractAssign:
                    leftArg -= rightArg;
                    return leftArg;
                case ExpressionType.Or:
                    return leftArg | rightArg;
                case ExpressionType.And:
                    return leftArg & rightArg;
                case ExpressionType.OrElse:
                    return leftArg || rightArg;
                case ExpressionType.AndAlso:
                    return leftArg && rightArg;
                default:
                    throw new ArgumentException("Unsupported Operator", nameof(op));
            }
        }

        /// <summary>
        /// Invokes the specified target using the DLR;
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="args">The args.</param>
        /// <returns></returns>
        public static dynamic Invoke(object target, params object[] args)
        {
            target = target.GetTargetContext(out var context, out var staticContext);
            args = Util.GetArgsAndNames(args, out var argNames);
            CallSite callSite = null;

            return InvokeHelper.InvokeDirectCallSite(target, args, argNames, context, staticContext, ref callSite);
        }

        /// <summary>
        /// Dynamically Invokes indexer using the DLR.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="indexes">The indexes.</param>
        /// <returns></returns>
        public static dynamic InvokeGetIndex(object target, params object[] indexes)
        {
            target = target.GetTargetContext(out var tContext, out var tStaticContext);
            indexes = Util.GetArgsAndNames(indexes, out var tArgNames);
            CallSite tCallSite = null;

            return InvokeHelper.InvokeGetIndexCallSite(target, indexes, tArgNames, tContext, tStaticContext,
                                                       ref tCallSite);
        }

        /// <summary>
        /// Convenience version of InvokeSetIndex that separates value and indexes.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="value">The value</param>
        /// <param name="indexes">The indexes </param>
        /// <returns></returns>
        public static object InvokeSetValueOnIndexes(object target, object value, params object[] indexes)
        {
            var tList = new List<object>(indexes) { value };
            return InvokeSetIndex(target, indexesThenValue: tList.ToArray());
        }

        /// <summary>
        /// Invokes setindex.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="indexesThenValue">The indexes then value.</param>
        public static object InvokeSetIndex(object target, params object[] indexesThenValue)
        {
            if (indexesThenValue.Length < 2)
            {
                throw new ArgumentException("Requires at least one index and one value", nameof(indexesThenValue));
            }

            target = target.GetTargetContext(out var tContext, out var tStaticContext);
            indexesThenValue = Util.GetArgsAndNames(indexesThenValue, out var tArgNames);

            CallSite tCallSite = null;
            return InvokeHelper.InvokeSetIndexCallSite(target, indexesThenValue, tArgNames, tContext, tStaticContext,
                                                ref tCallSite);
        }

        /// <summary>
        /// Dynamically Invokes a member method which returns void using the DLR
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="name">The name.</param>
        /// <param name="args">The args.</param>
        public static void InvokeMemberAction(object target, String_OR_InvokeMemberName name, params object[] args)
        {
            target = target.GetTargetContext(out var tContext, out var tStaticContext);
            args = Util.GetArgsAndNames(args, out var tArgNames);

            CallSite tCallSite = null;
            InvokeHelper.InvokeMemberActionCallSite(target, (InvokeMemberName)name, args, tArgNames, tContext, tStaticContext,
                                                    ref tCallSite);
        }

        /// <summary>
        /// Invokes the action using the DLR
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="args">The args.</param>
        public static void InvokeAction(object target, params object[] args)
        {
            target = target.GetTargetContext(out var tContext, out var tStaticContext);
            args = Util.GetArgsAndNames(args, out var tArgNames);

            CallSite tCallSite = null;
            InvokeHelper.InvokeDirectActionCallSite(target, args, tArgNames, tContext, tStaticContext, ref tCallSite);
        }

        /// <summary>
        /// Dynamically Invokes a set member using the DLR.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <remarks>
        /// if you call a static property off a type with a static context the csharp dlr binder won't do it, so this method reverts to reflection
        /// </remarks>
        public static object InvokeSet(object target, string name, object value)
        {
            target = target.GetTargetContext(out var tContext, out var tStaticContext);
            tContext = tContext.FixContext();

            CallSite tCallSite = null;
            return InvokeHelper.InvokeSetCallSite(target, name, value, tContext, tStaticContext, ref tCallSite);
        }

        /// <summary>
        /// Invokes the set on the end of a property chain.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="propertyChain">The property chain.</param>
        /// <param name="value">The value.</param>
        public static object InvokeSetChain(object target, string propertyChain, object value)
        {
            var tProperties = _chainRegex.Matches(propertyChain);
            if (tProperties.Count == 0)
            {
                throw new FormatException($"Could Not Parse :'{propertyChain}'");
            }

            var tTarget = target;
            for (var i = 0; i < tProperties.Count - 1; i++)
            {
                tTarget = InvokeChainLink(tTarget, tProperties[i], propertyChain);
            }

            var tSetProperty = tProperties[tProperties.Count - 1];

            var tSetGetter = ChainGroup(tSetProperty, "Getter");
            var tSetIntIndexer = ChainGroup(tSetProperty, "IntIndexer");
            var tSetStringIndexer = ChainGroup(tSetProperty, "StringIndexer");

            if (tSetGetter != null)
                return InvokeSet(tTarget, tSetGetter, value);
            if (tSetIntIndexer != null)
                return InvokeSetIndex(tTarget, ParseChainIndex(tSetIntIndexer), value);
            if (tSetStringIndexer != null)
                return InvokeSetIndex(tTarget, tSetStringIndexer, value);

            throw new FormatException($"Could Not Parse :'{propertyChain}'");
        }

        /// <summary>
        /// Dynamically Invokes a get member using the DLR.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="name">The name.</param>
        /// <returns>The result.</returns>
        public static dynamic InvokeGet(object target, string name)
        {
            target = target.GetTargetContext(out var tContext, out var tStaticContext);
            CallSite tSite = null;
            return InvokeHelper.InvokeGetCallSite(target, name, tContext, tStaticContext, ref tSite);
        }

        private static readonly Regex _chainRegex
            = new Regex(@"((\.?(?<Getter>\w+))|(\[(?<IntIndexer>\d+)\])|(\['(?<StringIndexer>\w+)'\]))");

        /// <summary>
        /// Invokes the getter property chain.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="propertyChain">The property chain.</param>
        /// <returns></returns>
        public static dynamic InvokeGetChain(object target, string propertyChain)
        {
            var tProperties = _chainRegex.Matches(propertyChain);
            var tTarget = target;
            foreach (Match tProperty in tProperties)
            {
                tTarget = InvokeChainLink(tTarget, tProperty, propertyChain);
            }
            return tTarget;
        }

        private static object InvokeChainLink(object target, Match property, string propertyChain)
        {
            var tGetter = ChainGroup(property, "Getter");
            var tIntIndexer = ChainGroup(property, "IntIndexer");
            var tStringIndexer = ChainGroup(property, "StringIndexer");

            if (tGetter != null)
                return InvokeGet(target, tGetter);
            if (tIntIndexer != null)
                return InvokeGetIndex(target, ParseChainIndex(tIntIndexer));
            if (tStringIndexer != null)
                return InvokeGetIndex(target, tStringIndexer);

            throw new FormatException($"Could Not Parse :'{propertyChain}'");
        }

        // Upstream read these through FluentRegex / RegexMatch, which returned
        // null for a group that did not participate in the match.
        private static string ChainGroup(Match match, string group)
        {
            var g = match.Groups[group];
            return g.Success ? g.Value : null;
        }

        // Upstream went through Dynamic.CoerceConvert(text, typeof(int)); for a
        // \d+ capture that reduces to an invariant int parse.
        private static int ParseChainIndex(string text)
        {
            return int.Parse(text, NumberStyles.Integer, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Determines whether the specified name on target is event. This allows you to know whether to InvokeMemberAction
        ///  add_{name} or a combo of {invokeGet, +=, invokeSet} and the corresponding remove_{name}
        /// or a combo of {invokeGet, -=, invokeSet}
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="name">The name.</param>
        /// <returns>
        /// 	<c>true</c> if the specified target is event; otherwise, <c>false</c>.
        /// </returns>
        public static bool InvokeIsEvent(object target, string name)
        {
            target = target.GetTargetContext(out var tContext, out var tStaticContext);
            tContext = tContext.FixContext();
            CallSite tCallSite = null;
            return InvokeHelper.InvokeIsEventCallSite(target, name, tContext, ref tCallSite);
        }

        /// <summary>
        /// Invokes add assign with correct behavior for events.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public static void InvokeAddAssignMember(object target, string name, object value)
        {
            CallSite callSiteAdd = null;
            CallSite callSiteGet = null;
            CallSite callSiteSet = null;
            CallSite callSiteIsEvent = null;
            target = target.GetTargetContext(out var context, out var staticContext);

            var args = new[] { value };
            args = Util.GetArgsAndNames(args, out var argNames);

            InvokeHelper.InvokeAddAssignCallSite(target, name, args, argNames, context, staticContext, ref callSiteIsEvent, ref callSiteAdd, ref callSiteGet, ref callSiteSet);
        }

        /// <summary>
        /// Invokes subtract assign with correct behavior for events.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public static void InvokeSubtractAssignMember(object target, string name, object value)
        {
            target = target.GetTargetContext(out var context, out var staticContext);

            var args = new[] { value };

            args = Util.GetArgsAndNames(args, out var argNames);

            CallSite callSiteIsEvent = null;
            CallSite callSiteRemove = null;
            CallSite callSiteGet = null;
            CallSite callSiteSet = null;

            InvokeHelper.InvokeSubtractAssignCallSite(target, name, args, argNames, context, staticContext, ref callSiteIsEvent, ref callSiteRemove, ref callSiteGet, ref callSiteSet);
        }

        /// <summary>
        /// Invokes  convert using the DLR.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="type">The type.</param>
        /// <param name="explicit">if set to <c>true</c> [explicit].</param>
        /// <returns></returns>
        public static dynamic InvokeConvert(object target, Type type, bool @explicit = false)
        {
            target = target.GetTargetContext(out var tContext, out var tDummy);

            CallSite tCallSite = null;
            return InvokeHelper.InvokeConvertCallSite(target, @explicit, type, tContext, ref tCallSite);
        }
    }
}
