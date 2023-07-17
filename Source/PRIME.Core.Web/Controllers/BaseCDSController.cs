using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PRIME.Core.Common.Interfaces;
using PRIME.Core.Context.Entities;
using PRIME.Core.Models;

namespace PRIME.Core.Web.Controllers
{
    public abstract class BaseCDSController:Controller
    {
        public class CDSInput
        {

            /// <summary>
            /// Id
            /// </summary>
            [JsonProperty("id")]
            public string Id { get; set; }

            /// <summary>
            /// Input
            /// </summary>
            [JsonProperty("input")]
            public string Input { get; set; }
        }

        public class ConditionRepository : IConditionRepository
        {
            private readonly List<Condition> _codes = new List<Condition>();



            public void Add(Condition c)
            {
                _codes.Add(c);
            }

            public async Task Init(string id)
            {

            }

            public async Task Aggregate(string patientId, IAggregator aggregator, List<AggrModel> aggregators)
            {
                var obs = _codes.Where(e=>e.Value.HasValue).Select(e=>new PDObservation(){ Value=e.Value.Value,Code=e.Code,CodeNameSpace = "PRIME"});

                foreach (var aggr in aggregators)
                {
                    var r = await aggregator.RunSingle(patientId, aggr.Code, "PRIME", obs, aggr.Config);
                    if (r == null)
                    {
                        //TODO: Log
                        continue;

                    }
                    if (_codes.Any(e => e.CodeNameSpace == r.CodeNameSpace && e.Code == r.Code))
                    {
                        var m = _codes.FirstOrDefault(
                            e => e.CodeNameSpace == r.CodeNameSpace && e.Code == r.Code);
                        if (m != null) m.Value = r.Value;
                    }
                    else
                    {
                        _codes.Add(new Condition()
                        {
                            Code=r.Code,
                            Value=r.Value,
                            CodeNameSpace = r.CodeNameSpace
                        });
                    }

                }

            }

            public bool? HasCondition(string code, string system)
            {
                var v= _codes.Where(e => e.Code.ToLower() == code.ToLower()).Select(e => e.Value).FirstOrDefault();

                if (v.HasValue)
                    return v.Value > 0;
                return new bool?();
            }

            public bool? HasCondition(string code, string system, Func<object, bool> convert)
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// Get Condition Value
            /// </summary>
            /// <param name="code"></param>
            /// <param name="system"></param>
            /// <returns></returns>
            public double? GetCondition(string code, string system)
            {
                var v=_codes.Where(e => e.Code.ToLower() == code.ToLower()).Select(e => e.Value).FirstOrDefault();

                if (v.HasValue)
                {
                    return v.Value;
                }

                return new double();

            }
            /// <summary>
            /// Add Condition
            /// </summary>
            /// <param name="oCode"></param>
            /// <param name="codeNamespace"></param>
            /// <param name="value"></param>
            public bool AddCondition(string oCode, string codeNamespace,double value=1.0)
            {

                if (_codes.Any(e => Match(e, oCode)))
                {
                    var cond = _codes.FirstOrDefault(e => Match(e, oCode));
                    if (cond == null)
                        return false;

                    cond.Value = value;
                    return true;

                }
                else
                {
                    _codes.Add(new Condition() { Code = oCode, Value = value });
                    return false;
                }


            }
            private bool Match(Condition obs, string code)
            {

                if (obs.Code == null)
                    return false;
                if (code == null)
                    return false;
                return code.ToLower() == obs.Code.ToLower();

                
            }

            public void RemoveCondition(string oCode, string codeNamespace)
            {
                if (string.IsNullOrEmpty(codeNamespace))
                    codeNamespace = "PRIME";
                var c= _codes.FirstOrDefault(e=>e.Code.ToLower() == oCode.ToLower());
                if(c!=null)
                    _codes.Remove(c);
            }

            public ConditionResult GetConditionRes(string code, string system)
            {
                var v = _codes.Where(e => e.Code.ToLower() == code.ToLower()).Select(e => e.Value).FirstOrDefault();

                if (v.HasValue)
                {
                    return new ConditionResult()
                    {
                        Value=v.Value,
                        Code=code,
                        CodeNameSpace =system
                    };
                }

                return new ConditionResult()
                {
                    
                    Code = code,
                    CodeNameSpace = system
                };
            }

            public Dictionary<string, string> ToDict(IValueMapping mapping)
            {
                Dictionary<string,string> dictionary=new Dictionary<string, string>();

                if (mapping == null)
                    return dictionary;

                foreach (var c in _codes)
                {
                    if (c.Value.HasValue)
                    {

                        if (!dictionary.ContainsKey(c.Code))
                            dictionary.Add(c.Code,
                                c.Value.Value.ToString(CultureInfo.InvariantCulture)); //mapping.GetValue(c.Code, c.Value.Value).ToString());
                        else
                            dictionary[c.Code] = c.Value.Value.ToString(CultureInfo.InvariantCulture);

                    }
                }

                return dictionary;
            }
        }

        public class Condition
        {
            public string Name { get; set; }
            public string Code { get; set; }
            public string CodeNameSpace { get; set; }
            public double? Value { get; set; }
            public string Category { get; set; }
            public List<string> Values { get; set; }
        }
    }
}