using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;

namespace Xunit
{
    /// <summary>
    /// 
    /// </summary>
    public static class ReflectionHelper
    {
        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="exp"></param>
        /// <returns></returns>
        public static string GetFullPropertyName<T, TProperty>
        (this T instance, Expression<Func<T, TProperty>> exp)
        {
            MemberExpression memberExp;
            if (!TryFindMemberExpression(exp.Body, out memberExp))
                return string.Empty;

            var memberNames = new Stack<string>();
            do
            {
                memberNames.Push(memberExp.Member.Name);
            }
            while (TryFindMemberExpression(memberExp.Expression, out memberExp));

            return string.Join(".", memberNames.ToArray());
        }
        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="exp"></param>
        /// <returns></returns>
        public static string GetFullPropertyName<T, TProperty>
        ( Expression<Func<T, TProperty>> exp)
        {
            MemberExpression memberExp;
            if (!TryFindMemberExpression(exp.Body, out memberExp))
                return string.Empty;

            var memberNames = new Stack<string>();
            do
            {
                memberNames.Push(memberExp.Member.Name);
            }
            while (TryFindMemberExpression(memberExp.Expression, out memberExp));

            return string.Join(".", memberNames.ToArray());
        }
        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="exp"></param>
        /// <returns></returns>
        public static TAttribute GetPropertyAttribute<T, TAttribute>
        (this T instnace, Expression<Func<T, object>> exp) where TAttribute : Attribute
        {
            MemberExpression memberExp;
            if (!TryFindMemberExpression(exp.Body, out memberExp))
                return null;

            return (TAttribute)Attribute.GetCustomAttribute(memberExp.Member, typeof(TAttribute));
        }
        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="exp"></param>
        /// <returns></returns>
        public static TAttribute GetPropertyAttribute<T, TAttribute>
        ( Expression<Func<T, object>> exp) where TAttribute : Attribute
        {
            MemberExpression memberExp;
            if (!TryFindMemberExpression(exp.Body, out memberExp))
                return null;

            return (TAttribute)Attribute.GetCustomAttribute(memberExp.Member, typeof(TAttribute));
        }

        /// <summary>
        /// Given a lambda expression, attempts to append another property to the expression.
        ///
        /// <example>
        ///
        /// x=> Model.MyClass
        ///
        /// want to add the name property of myclass
        ///
        /// x=> Model.MyClass.Name
        ///
        ///
        ///
        /// </example>
        /// </summary>
        /// <typeparam name="TModel">The model for the expression.  This is the root type in the object chanin</typeparam>
        /// <typeparam name="TProperty">The original type of the property expression.  This is the type you will append an expression</typeparam>
        /// <typeparam name="TNewProperty">The return type of the new property expression</typeparam>
        /// <param name="expression">The original lambda expression</param>
        /// <param name="Name">The name of the property</param>
        /// <returns></returns>
        public static Expression<Func<TModel, TNewProperty>> AppendProperty<TModel, TProperty, TNewProperty>(Expression<Func<TModel, TProperty>> expression, string Name)
        {
            var pe = Expression.Property(expression.Body, Name);
            return LambdaExpression.Lambda<Func<TModel, TNewProperty>>(pe, expression.Parameters);
        }

        /// <summary>
        /// Retrieves the dotted notation path of a lambda expression
        ///
        /// </summary>
        /// <example>
        /// GetFullPropertyName(x=> x.Employee.Address.Street)
        /// would yield the string "Employee.Address.Street";
        /// </example>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static string GetFullPropertyName(Expression expression)
        {
            MemberExpression memberExp;
            if (!TryFindMemberExpression(expression, out memberExp))
                return string.Empty;

            var memberNames = new Stack<string>();
            do
            {
                memberNames.Push(memberExp.Member.Name);
            }
            while (TryFindMemberExpression(memberExp.Expression, out memberExp));

            return string.Join(".", memberNames.ToArray());
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="exp"></param>
        /// <param name="memberExp"></param>
        /// <returns></returns>
        private static bool TryFindMemberExpression
        (Expression exp, out MemberExpression memberExp)
        {
            memberExp = exp as MemberExpression;
            if (memberExp != null)
            {
                // heyo! that was easy enough
                return true;
            }

            // if the compiler created an automatic conversion,
            // it'll look something like...
            // obj => Convert(obj.Property) [e.g., int -> object]
            // OR:
            // obj => ConvertChecked(obj.Property) [e.g., int -> long]
            // ...which are the cases checked in IsConversion
            if (IsConversion(exp) && exp is UnaryExpression)
            {
                memberExp = ((UnaryExpression)exp).Operand as MemberExpression;
                if (memberExp != null)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        private static bool IsConversion(Expression exp)
        {
            return (
                exp.NodeType == ExpressionType.Convert ||
                exp.NodeType == ExpressionType.ConvertChecked
            );
        }
        /// <summary>
        /// Returns a _private_ Property Value from a given Object. Uses Reflection.
        /// Throws a ArgumentOutOfRangeException if the Property is not found.
        /// </summary>
        /// <typeparam name="T">Type of the Property</typeparam>
        /// <param name="obj">Object from where the Property Value is returned</param>
        /// <param name="propName">Propertyname as string.</param>
        /// <returns>PropertyValue</returns>
        public static T GetPrivatePropertyValue<T>(this object obj, string propName)
        {
            if (obj == null) { throw new ArgumentNullException("obj"); }
            PropertyInfo pi = obj.GetType().GetProperty(propName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (pi == null) { throw new ArgumentOutOfRangeException("propName", string.Format("Property {0} was not found in Type {1}", propName, obj.GetType().FullName)); }
            return (T)pi.GetValue(obj, null);
        }

        /// <summary>
        /// Returns a private Property Value from a given Object. Uses Reflection.
        /// Throws a ArgumentOutOfRangeException if the Property is not found.
        /// </summary>
        /// <typeparam name="T">Type of the Property</typeparam>
        /// <param name="obj">Object from where the Property Value is returned</param>
        /// <param name="propName">Propertyname as string.</param>
        /// <returns>PropertyValue</returns>
        public static T GetPrivateFieldValue<T>(this object obj, string propName)
        {
            if (obj == null) { throw new ArgumentNullException("obj"); }
            Type t = obj.GetType();
            FieldInfo fi = null;
            while (fi == null && t != null)
            {
                fi = t.GetField(propName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                t = t.BaseType;
            }
            if (fi == null) { throw new ArgumentOutOfRangeException("propName", string.Format("Field {0} was not found in Type {1}", propName, obj.GetType().FullName)); }
            return (T)fi.GetValue(obj);
        }

        /// <summary>
        /// Sets a _private_ Property Value from a given Object. Uses Reflection.
        /// Throws a ArgumentOutOfRangeException if the Property is not found.
        /// </summary>
        /// <typeparam name="T">Type of the Property</typeparam>
        /// <param name="obj">Object from where the Property Value is set</param>
        /// <param name="propName">Propertyname as string.</param>
        /// <param name="val">Value to set.</param>
        /// <returns>PropertyValue</returns>
        public static void SetPrivatePropertyValue<T>(this object obj, string propName, T val)
        {
            Type t = obj.GetType();
            if (t.GetProperty(propName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance) == null)
            {
                throw new ArgumentOutOfRangeException("propName", string.Format("Property {0} was not found in Type {1}", propName, obj.GetType().FullName));
            }

            t.InvokeMember(propName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.SetProperty | BindingFlags.Instance, null, obj, new object[] { val });
        }

        /// <summary>
        /// Set a private Property Value on a given Object. Uses Reflection.
        /// </summary>
        /// <typeparam name="T">Type of the Property</typeparam>
        /// <param name="obj">Object from where the Property Value is returned</param>
        /// <param name="propName">Propertyname as string.</param>
        /// <param name="val">the value to set</param>
        /// <exception cref="ArgumentOutOfRangeException">if the Property is not found</exception>
        public static void SetPrivateFieldValue<T>(this object obj, string propName, T val)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            Type t = obj.GetType();
            FieldInfo fi = null;
            while (fi == null && t != null)
            {
                fi = t.GetField(propName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                t = t.BaseType;
            }
            if (fi == null)
            {
                throw new ArgumentOutOfRangeException("propName", string.Format("Field {0} was not found in Type {1}", propName, obj.GetType().FullName));
            }

            fi.SetValue(obj, val);
        }
    }


   
}