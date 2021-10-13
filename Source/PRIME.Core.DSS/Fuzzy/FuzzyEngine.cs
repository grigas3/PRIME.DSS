using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace PRIME.Core.DSS.Fuzzy
{
    /// <summary>
    ///     Fuzzy Function
    /// </summary>
    public class FuzzyFunc
    {
        public string Name { get; set; }
        public string Property { get; set; }
        public string Operator { get; set; }
        public object Value { get; set; }
        public bool Not { get; set; }
    }

    /// <summary>
    ///     And Rules
    /// </summary>
    public class OrRule 
    {

        public OrRule()
        {
            

        }
        public OrRule(string n)
        {
            Name = n;
        }

        public List<FuzzyFunc> AndVariables { get; set; }
        public string Name { get; set; }
    }

    public class FuzzyEngine
    {
        private const double eps = 0.000001;

        public static FuzzyResult GetFuzzyInference(FuzzyRule profile, Dictionary<string, object> d)
        {
            var result = new FuzzyResult();
            Expression left = null;
            double max = 0;
            foreach (var r in profile.OrRules)
            {
                Expression right = null;
                var min = double.MaxValue;
                foreach (var a in r.AndVariables)
                {
                    if (!d.ContainsKey(a.Property))
                    {
                      
                        result.MissingVariables.Add(a.Property);
                        continue;
                    }


                    if (!(d[a.Property] is double?) && !(d[a.Property] is double))
                        throw new Exception("Only double values supported");
                    var v = d[a.Property] as double?;
                    if (a.Not)
                        v = 1 - v;
                    if (v.HasValue && v.Value < min)
                    {
                        min = v.Value;
                        result.Variable = string.IsNullOrEmpty(a.Name) ? a.Property : a.Name;
                    }
                }

                if (max < min)
                {
                    max = min;
                    result.Rule = r.Name;
                }
            }

            result.Result = max;
            return result;
        }

        public static FuzzyResult GetFuzzyInference(FuzzyRule profile, object d)
        {
            var result = new FuzzyResult();
            Expression left = null;
            double max = 0;
            foreach (var r in profile.OrRules)
            {
                Expression right = null;
                var min = double.MaxValue;
                bool assigned = false;
                foreach (var a in r.AndVariables)
                {
                    if (d.GetType().GetProperty(a.Property) == null)
                    {
                       
                        result.MissingVariables.Add(a.Property);
                        continue;
                    }

                    assigned = true;

                    if (d.GetType().GetProperty(a.Property).PropertyType != typeof(double))
                        throw new Exception("Only double values supported");
                    var v = d.GetType().GetProperty(a.Property)?.GetValue(d, null) as double?;
                    if (a.Not)
                        v = 1 - v;
                    if (v.HasValue && v.Value < min)
                    {
                        min = v.Value;
                        result.Variable = string.IsNullOrEmpty(a.Name)?a.Property:a.Name;
                    }
                }

                if (max < min&&assigned)
                {
                    max = min;
                    result.Rule = r.Name;
                }
            }

            result.Result = max;
            return result;
        }


        public static FuzzyResult GetCrispInference(FuzzyRule profile, Dictionary<string, object> d)
        {
            var result = new FuzzyResult();
            Expression left = null;

            foreach (var r in profile.OrRules)
            {
                Expression right = null;
                foreach (var a in r.AndVariables)
                {
                    if (!d.ContainsKey(a.Property))
                    {
                        result.MissingVariables.Add(a.Property);
                        continue;
                    }

                    if(string.IsNullOrEmpty(a.Operator))
                        throw new ArgumentNullException("Operator");
                    if (a.Value is null)
                        throw new ArgumentNullException("Value");


                    var op = a.Operator; //d.GetType().GetProperty(a.Operator).GetValue(d, null);
                    var value = Expression.Constant(d[a.Property]);
                    var constant = Expression.Constant(a.Value);
                    Expression q = null;
                    switch (op)
                    {
                        case "GreaterThan":
                            q = Expression.GreaterThan(value, constant);
                            break;
                        case "LessThan":
                            q = Expression.LessThan(value, constant);
                            break;
                        case "Equals":
                            q = Expression.Equal(value, constant);
                            break;
                        default:
                            q = Expression.Equal(value, constant);
                            break;
                    }


                    if (right == null)
                        right = q;
                    else
                        right = Expression.And(right, q);
                }

                if (left != null && right != null)
                    left = Expression.Or(left, right);
                else
                    left = right;
            }

            var ret = true;
            if (left != null)
                ret = (bool) Expression.Lambda(left).Compile().DynamicInvoke();
            result.Result = ret ? 1.0 : 0;

            return result;
        }


        public static FuzzyResult GetCrispInference(FuzzyRule profile, object d)
        {
            var result = new FuzzyResult();
            Expression left = null;

            foreach (var r in profile.OrRules)
            {
                Expression right = null;
                foreach (var a in r.AndVariables)
                {
                    if (d.GetType().GetProperty(a.Property) == null)
                    {
                        result.MissingVariables.Add(a.Property);
                        continue;
                    }

                    var v = d.GetType().GetProperty(a.Property).GetValue(d, null);

                    var op = a.Operator; //d.GetType().GetProperty(a.Operator).GetValue(d, null);
                    var value = Expression.Constant(v);
                    var constant = Expression.Constant(a.Value);
                    Expression q = null;
                    switch (op)
                    {
                        case "GreaterThan":
                            q = Expression.GreaterThan(value, constant);
                            break;
                        case "LessThan":
                            q = Expression.LessThan(value, constant);
                            break;
                        case "Equals":
                            q = Expression.Equal(value, constant);
                            break;
                    }


                    if (right == null)
                        right = q;
                    else
                        right = Expression.And(right, q);
                }

                if (left != null && right != null)
                    left = Expression.Or(left, right);
                else
                    left = right;
            }

            var ret = true;
            if (left != null)
                ret = (bool) Expression.Lambda(left).Compile().DynamicInvoke();
            result.Result = ret ? 1.0 : 0;

            return result;
        }

        /// <summary>
        ///     Get Inference
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="d"></param>
        /// <returns></returns>
        public static FuzzyResult GetInference(FuzzyCollection profile, object d)
        {
            var totalRes = new FuzzyResult();
            var anyFuzzy = profile.Rules.Any(e => e.Fuzzy);
            double max = 0;
            foreach (var f in profile.Rules)
            {
                FuzzyResult res = null;
                if (f.Fuzzy)
                    res = GetFuzzyInference(f, d);
                else
                    res = GetCrispInference(f, d);

                totalRes.MissingVariables.AddRange(res.MissingVariables);
                if (anyFuzzy && !f.Fuzzy && res.Result < eps)
                {
                    totalRes.Result = 0;
                    return totalRes;
                }

                if (anyFuzzy && f.Fuzzy && res.Result > max)
                {
                    max = res.Result;
                    totalRes.Variable = res.Variable;
                    totalRes.Rule = res.Rule;
                }
                else if (!anyFuzzy && res.Result > max)
                {
                    max = res.Result;
                    totalRes.Variable = res.Variable;
                    totalRes.Rule = res.Rule;
                }
            }

            totalRes.Result = max;

            return totalRes;
        }


        /// <summary>
        ///     Get Inference
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="d"></param>
        /// <returns></returns>
        public static FuzzyResult GetInference(FuzzyCollection profile, Dictionary<string, object> d)
        {
            var totalRes = new FuzzyResult();
            var anyFuzzy = profile.Rules.Any(e => e.Fuzzy);
            double min = double.MaxValue;
            foreach (var f in profile.Rules)
            {
                FuzzyResult res = null;
                if (f.Fuzzy)
                    res = GetFuzzyInference(f, d);
                else
                    res = GetCrispInference(f, d);

                totalRes.MissingVariables.AddRange(res.MissingVariables);
                if (anyFuzzy && !f.Fuzzy && res.Result < eps)
                {
                    totalRes.Result = 0;
                    return totalRes;
                }

                if (anyFuzzy && f.Fuzzy && res.Result < min)
                {
                    min = res.Result;
                    totalRes.Variable = res.Variable;
                    totalRes.Rule = res.Rule;
                }
                else if (!anyFuzzy && res.Result < min)
                {
                    min = res.Result;
                    totalRes.Variable = res.Variable;
                    totalRes.Rule = res.Rule;
                }
            }

            totalRes.Result = min;

            return totalRes;
        }
    }
}