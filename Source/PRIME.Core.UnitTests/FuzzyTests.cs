using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;
using System.Text;
using System.Xml.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using PRIME.Core.DSS.Fuzzy;
using PRIME.Core.DSS.Fuzzy.FuzzyLogicApi;
using Remote.Linq;
using Remote.Linq.ExpressionVisitors;

namespace PRIME.Core.UnitTests
{
    [TestClass]
    public class FuzzyTests
    {
        [Serializable, XmlRoot]
        public class HonestAssesment
        {
            [XmlElement]
            public double IntegrityPercentage { get; set; }

            [XmlElement]
            public double TruthPercentage { get; set; }

            [XmlElement]
            public double JusticeSensePercentage { get; set; }

            [XmlElement]
            public double MistakesAVG
            {
                get
                {
                    return (IntegrityPercentage + TruthPercentage - JusticeSensePercentage) / 3;
                }
            }
        }

        //Crisp Logic expression that represents Honesty Profiles:
        static Expression<Func<HonestAssesment, bool>> _honestyProfile = (h) =>
            (h.IntegrityPercentage > 75 && h.JusticeSensePercentage > 75 && h.TruthPercentage > 75) || //First group
            (h.IntegrityPercentage > 90 && h.JusticeSensePercentage > 60 && h.TruthPercentage > 50) || //Second group
            (h.IntegrityPercentage > 70 && h.JusticeSensePercentage > 90 && h.TruthPercentage > 80) || //Third group
            (h.IntegrityPercentage > 65 && h.JusticeSensePercentage == 100 && h.TruthPercentage > 95); //Last group


    
        private class InfResult
        {
            public string InferenceResult { get; set; }
        }

        [TestMethod]
        public void TestHonest1()
        {
            HonestAssesment profile1 = new HonestAssesment()
            {
                IntegrityPercentage = 90,
                JusticeSensePercentage = 80,
                TruthPercentage = 70
            };

            
            string inference_p1 = FuzzyLogic<HonestAssesment>.GetInference(_honestyProfile, ResponseType.Json, profile1);

            var res=JsonConvert.DeserializeObject<InfResult>(inference_p1);

            Assert.AreEqual("0,67",res.InferenceResult);


        }

        public class FuzzyFunc
        {

            public string Property { get; set; }
            public string Operator { get; set; }
            public object Value { get; set; }

        }

        public class AndRules:List<FuzzyFunc>
        {
            

        }
        public class FuzzyDefinition
        {

            public List<string> Properties { get; set; }

            public List<AndRules> OrRules { get; set; }

            

        }

        private static Func<dynamic, FuzzyDefinition, bool> profile = (e, d) =>
        {
            foreach (var c in d.Properties)
            {

                var p=e.GetType().GetProperty(c).GetValue(d, null);

            }
            return false;
        };

        private static FuzzyDefinition CreatePIGDDefinition()
        {
            return new FuzzyDefinition()
            {
                Properties = new List<string>() { "GAIT", "FOG","POSTURE" },
                OrRules = new List<AndRules>()
                {

                    new AndRules()
                    {
                        new FuzzyFunc(){ Property = "FOG",Operator = "GreaterThan",Value=1},
                        new FuzzyFunc(){ Property = "GAIT",Operator = "GreaterThan",Value=1}
                    },
                    new AndRules()
                    {
                        new FuzzyFunc(){ Property = "FOG",Operator = "GreaterThan",Value=1},
                        new FuzzyFunc(){ Property = "POSTURE",Operator = "GreaterThan",Value=1}
                    },
                    new AndRules()
                    {
                        new FuzzyFunc(){ Property = "GAIT",Operator = "GreaterThan",Value=1},
                        new FuzzyFunc(){ Property = "POSTURE",Operator = "GreaterThan",Value=1}
                    },
                    new AndRules()
                    {
                        new FuzzyFunc(){ Property = "FOG",Operator = "GreaterThan",Value=2},
                        
                    },
                    new AndRules()
                    {
                        new FuzzyFunc(){ Property = "GAIT",Operator = "GreaterThan",Value=2},

                    },
                    new AndRules()
                    {
                        new FuzzyFunc(){ Property = "POSTURE",Operator = "GreaterThan",Value=2},

                    },

                }

            };
        }
        private static FuzzyDefinition CreateDefinition()
        {
            return new FuzzyDefinition()
            {
                Properties = new List<string>() { "Tremor", "Dyskinesia" },
                OrRules = new List<AndRules>()
                {

                    new AndRules()
                    {
                        new FuzzyFunc(){ Property = "Tremor",Operator = "GreaterThan",Value=1},
                        new FuzzyFunc(){ Property = "Dyskinesia",Operator = "GreaterThan",Value=5}
                    },
                    new AndRules()
                    {
                        new FuzzyFunc(){ Property = "Tremor",Operator = "GreaterThan",Value=5},
                        new FuzzyFunc(){ Property = "Dyskinesia",Operator = "GreaterThan",Value=1}
                    },

                }

            };
        }
        [TestMethod]
        public void TestSerializeExpression()
        {

            FuzzyDefinition profile = CreateDefinition();

            //serializerSettings.ReferenceLoopHandling =  ReferenceLoopHandling.Ignore;
            string json = JsonConvert.SerializeObject(profile);
            var copy = JsonConvert.DeserializeObject<dynamic>(json);

            Assert.IsTrue(copy.Properties!=null);
            Assert.IsTrue(copy.OrRules != null);
        }

        [TestMethod]
        public void TestFuzzyFromExpression1()
        {

            FuzzyDefinition profile = CreateDefinition();
            var d = new { Tremor = 2, Dyskinesia = 1 };
            //serializerSettings.ReferenceLoopHandling =  ReferenceLoopHandling.Ignore;

            Expression left = null;

            foreach (var r in profile.OrRules)
            {
                Expression right = null;
                foreach (var a in r)
                {


                    var v = d.GetType().GetProperty(a.Property).GetValue(d, null);

                    var value = Expression.Constant(v);
                    var constant = Expression.Constant(a.Value);
                    var q = Expression.GreaterThan(value, constant);
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
     
            var ret = (bool)Expression.Lambda(left).Compile().DynamicInvoke();

            Assert.IsFalse(ret);




            //ar body = Expression.Equal(
            //    len, Expression.Constant(5));
            //var lambda = Expression.Lambda<Func<dynamic, bool>>(
            //    body, param);
            //Assert.IsTrue(copy.Properties != null);
            //Assert.IsTrue(copy.OrRules != null);
        }



        [TestMethod]
        public void PIGDFromExpression_Test1()
        {

            FuzzyDefinition profile = CreatePIGDDefinition();
            var d = new { FOG = 2};
            //serializerSettings.ReferenceLoopHandling =  ReferenceLoopHandling.Ignore;

            Expression left = null;

            foreach (var r in profile.OrRules)
            {
                Expression right = null;
                foreach (var a in r)
                {


                    var v = d.GetType().GetProperty(a.Property).GetValue(d, null);

                    var value = Expression.Constant(v);
                    var constant = Expression.Constant(a.Value);
                    var q = Expression.GreaterThan(value, constant);
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

            var ret = (bool)Expression.Lambda(left).Compile().DynamicInvoke();

            Assert.IsTrue(ret);
        }
  
        /// <summary>
        /// Object Property Helper
        /// </summary>
        private class ObjectPropertyHelper
        {

            public int Variable1 { get; set; }
            public int Variable2 { get; set; }
            public int Variable3 { get; set; }
            public int Variable4 { get; set; }
            public int Variable5 { get; set; }
            public int Variable6 { get; set; }
            public int Variable7 { get; set; }
            public int Variable8 { get; set; }
            public int Variable9 { get; set; }
            public int Variable10 { get; set; }
            public int Variable11 { get; set; }
            public int Variable12 { get; set; }
            public int Variable13 { get; set; }
            public int Variable14 { get; set; }
            public int Variable15 { get; set; }
            public int Variable16 { get; set; }
            public int Variable17 { get; set; }
            public int Variable18 { get; set; }
            public int Variable19 { get; set; }
            public int Variable20 { get; set; }
            


            /// <summary>
            /// Create Instance of ObjectPropertyHelper
            /// </summary>
            /// <param name="propMapping"></param>
            /// <param name="source"></param>
            /// <returns></returns>
            public static ObjectPropertyHelper CreateInstanceBinder(Dictionary<string, string> propMapping,
                object source)
            {

                ObjectPropertyHelper target=new ObjectPropertyHelper();
                foreach (var c in propMapping.Keys)
                {
                    target.GetType().GetProperty(propMapping[c]).SetValue(target,source.GetType().GetProperty(c).GetValue(source,null));
                }

                return target;

            }


        }

        [TestMethod]
        public void TestFuzzyFromExpression3()
        {

            FuzzyDefinition profile = CreateDefinition();
            ParameterExpression param = Expression.Parameter(typeof(ObjectPropertyHelper), "param");

            var d = new { Tremor = 2, Dyskinesia = 10 };
            //serializerSettings.ReferenceLoopHandling =  ReferenceLoopHandling.Ignore;

            Expression left = null;

            int propertyCount = 1;
            Dictionary<string,string> propertyIndex=new Dictionary<string, string>();
            foreach (var r in profile.OrRules)
            {
                Expression right = null;
                foreach (var a in r)
                {
                    var propName = "";
                    if (!propertyIndex.ContainsKey(a.Property))
                    {
                        propName = "Variable" + (propertyCount++);
                        propertyIndex.Add(a.Property,propName);
                    }

                    propName = propertyIndex[a.Property];
                    //var param = Expression.Parameter(typeof(object),"param");
                    var value=Expression.Property(param, propName);
                    var constant = Expression.Constant(a.Value);
                    var q = Expression.GreaterThan(value, constant);
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

            var funcExp = Expression.Lambda<Func<ObjectPropertyHelper, bool>>(left, param);


            Assert.IsTrue(funcExp.Compile().Invoke(ObjectPropertyHelper.CreateInstanceBinder(propertyIndex,d)));
        }

        [TestMethod]
        public void PIGDFromExpression_Test2()
        {

            FuzzyDefinition profile = CreatePIGDDefinition();
            ParameterExpression param = Expression.Parameter(typeof(ObjectPropertyHelper), "param");

            var d = new { FOG = 3,GAIT=0,POSTURE=1};
            //serializerSettings.ReferenceLoopHandling =  ReferenceLoopHandling.Ignore;

            Expression left = null;

            int propertyCount = 1;
            Dictionary<string, string> propertyIndex = new Dictionary<string, string>();
            foreach (var r in profile.OrRules)
            {
                Expression right = null;
                foreach (var a in r)
                {
                    var propName = "";
                    if (!propertyIndex.ContainsKey(a.Property))
                    {
                        propName = "Variable" + (propertyCount++);
                        propertyIndex.Add(a.Property, propName);
                    }

                    propName = propertyIndex[a.Property];
                    //var param = Expression.Parameter(typeof(object),"param");
                    var value = Expression.Property(param, propName);
                    var constant = Expression.Constant(a.Value);
                    var q = Expression.GreaterThan(value, constant);
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

            var funcExp = Expression.Lambda<Func<ObjectPropertyHelper, bool>>(left, param);

            var v = funcExp.Compile().Invoke(ObjectPropertyHelper.CreateInstanceBinder(propertyIndex, d));
            Assert.IsTrue(v);
        }


        [TestMethod]
        public void TestFuzzyFromExpression2()
        {

            FuzzyDefinition profile = CreateDefinition();
            var d = new {Tremor = 2,Dyskinesia=10};
            //serializerSettings.ReferenceLoopHandling =  ReferenceLoopHandling.Ignore;

            Expression left = null;

       foreach (var r in profile.OrRules)
       {
           Expression right = null;
                foreach (var a in r)
                {


                    var v=d.GetType().GetProperty(a.Property).GetValue(d, null);

               var value = Expression.Constant(v);
               var constant=Expression.Constant(a.Value);
               var q = Expression.GreaterThan(value, constant);
               if (right == null)
                   right = q;
               else
                   right=Expression.And(right, q);

           }

                if (left != null&&right!=null)
                    left = Expression.Or(left, right);
                else
                    left = right;

       }

       
       var ret = (bool) Expression.Lambda(left).Compile().DynamicInvoke();
      
       Assert.IsTrue(ret);

       

       
            //ar body = Expression.Equal(
            //    len, Expression.Constant(5));
            //var lambda = Expression.Lambda<Func<dynamic, bool>>(
            //    body, param);
            //Assert.IsTrue(copy.Properties != null);
            //Assert.IsTrue(copy.OrRules != null);
        }

    }
}
