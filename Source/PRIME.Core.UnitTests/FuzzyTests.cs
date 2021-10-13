using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using PRIME.Core.DSS.Fuzzy;
using PRIME.Core.DSS.Treatment;

namespace PRIME.Core.UnitTests
{
    [TestClass]
    public class FuzzyTests
    {
        [TestMethod]
        public void ParseTest1()
        {
            var collection = new FuzzyCollection();

            collection.Rules = new List<FuzzyRule>
            {
                new FuzzyRule
                {
                    Name = "TEST",
                    Fuzzy = false,
                    OrRules = new List<OrRule>
                    {
                        new OrRule
                        {
                            AndVariables = new List<FuzzyFunc>
                            {
                                new FuzzyFunc
                                {
                                    Property = "TEST",
                                    Name = "TEST",
                                    Not = false
                                }
                            }
                        }
                    }
                }
            };

            collection.Variables = new List<FuzzyVariable>()
            {

                new FuzzyVariable() {Name = "TEST", Code = "TEST"}
            };

            var s = JsonConvert.SerializeObject(collection);

            var model = JsonConvert.DeserializeObject<FuzzyCollection>(s);

            Assert.IsNotNull(model);

            Assert.AreEqual(1, model.Rules.Count);


            Assert.AreEqual(1, model.Rules.FirstOrDefault()?.OrRules.Count());

            Assert.AreEqual(1, model.Rules.FirstOrDefault()?.OrRules.FirstOrDefault()?.AndVariables.Count());
        }

        [TestMethod]
        public void ParseTest3()
        {

            var s =
                "{\"id\":\"2\",\"naiveModel\":null,\"ruleModel\":{\"rules\":[{\"orrules\":[{\"name\":\"TEST\",\"andvariables\":[{\"name\":\"TEST\",\"property\":\"TEST\",\"not\":false}]}]}],\"variables\":[{\"name\":\"TEST\",\"code\":\"TEST\"}]},\"name\":\"TEST\",\"code\":\"TEST\",\"codeNamespace\":\"TEST\",\"replacementCode\":\"TEST\",\"replacementCodeNamespace\":\"TEST\",\"option\":0,\"summary\":\"TEST\",\"id\":\"2\"}";


            var model = JsonConvert.DeserializeObject<TreatmentClassifier>(s);

            Assert.IsNotNull(model);

        }
        [TestMethod]
        public void ParseTest2()
        {
            var s =
                "{\"rules\":[{\"rules\":[{\"andvariables\":[{ \"name\":\"TEST\",\"code\":\"TEST\",\"not\":false}],\"name\":\"test\"]},\"fuzzy\":true,\"name\":\"test\"}],\"variables\":[{\"name\":\"TEST\",\"code\":\"TEST\"},{\"name\":\"TEST\",\"code\":\"TEST\"}],\"id\":\"2\"}";


            var model = JsonConvert.DeserializeObject<FuzzyCollection>(s);

            Assert.IsNotNull(model);

            Assert.AreEqual(1, model.Rules.Count);


            Assert.AreEqual(1, model.Rules.FirstOrDefault()?.OrRules.Count());

            Assert.AreEqual(1, model.Rules.FirstOrDefault()?.OrRules.FirstOrDefault()?.AndVariables.Count());
        }
    }
}

//        [Serializable, XmlRoot]
//        public class HonestAssesment
//        {
//            [XmlElement]
//            public double IntegrityPercentage { get; set; }

//            [XmlElement]
//            public double TruthPercentage { get; set; }

//            [XmlElement]
//            public double JusticeSensePercentage { get; set; }

//            [XmlElement]
//            public double MistakesAVG
//            {
//                get
//                {
//                    return (IntegrityPercentage + TruthPercentage - JusticeSensePercentage) / 3;
//                }
//            }
//        }

//        //Crisp Logic expression that represents Honesty Profiles:
//        static Expression<Func<HonestAssesment, bool>> _honestyProfile = (h) =>
//            (h.IntegrityPercentage > 75 && h.JusticeSensePercentage > 75 && h.TruthPercentage > 75) || //First group
//            (h.IntegrityPercentage > 90 && h.JusticeSensePercentage > 60 && h.TruthPercentage > 50) || //Second group
//            (h.IntegrityPercentage > 70 && h.JusticeSensePercentage > 90 && h.TruthPercentage > 80) || //Third group
//            (h.IntegrityPercentage > 65 && h.JusticeSensePercentage == 100 && h.TruthPercentage > 95); //Last group


//        private class InfResult
//        {
//            public string InferenceResult { get; set; }
//        }

//        [TestMethod]
//        public void TestHonest1()
//        {
//            HonestAssesment profile1 = new HonestAssesment()
//            {
//                IntegrityPercentage = 90,
//                JusticeSensePercentage = 80,
//                TruthPercentage = 70
//            };


//            string inference_p1 = FuzzyLogic<HonestAssesment>.GetInference(_honestyProfile, ResponseType.Json, profile1);

//            var res=JsonConvert.DeserializeObject<InfResult>(inference_p1);

//            Assert.AreEqual("0,67",res.InferenceResult);


//        }


//        private static FuzzyDefinition CreatePIGDDefinition()
//        {
//            return new FuzzyDefinition()
//            {
//                Properties = new List<string>() { "GAIT", "FOG","POSTURE" },
//                Rules = new List<OrRule>()
//                {

//                    new OrRule()
//                    {
//                        new FuzzyFunc(){ Property = "FOG",Operator = "GreaterThan",Value=1},
//                        new FuzzyFunc(){ Property = "GAIT",Operator = "GreaterThan",Value=1}
//                    },
//                    new OrRule()
//                    {
//                        new FuzzyFunc(){ Property = "FOG",Operator = "GreaterThan",Value=1},
//                        new FuzzyFunc(){ Property = "POSTURE",Operator = "GreaterThan",Value=1}
//                    },
//                    new OrRule()
//                    {
//                        new FuzzyFunc(){ Property = "GAIT",Operator = "GreaterThan",Value=1},
//                        new FuzzyFunc(){ Property = "POSTURE",Operator = "GreaterThan",Value=1}
//                    },
//                    new OrRule()
//                    {
//                        new FuzzyFunc(){ Property = "FOG",Operator = "GreaterThan",Value=2},

//                    },
//                    new OrRule()
//                    {
//                        new FuzzyFunc(){ Property = "GAIT",Operator = "GreaterThan",Value=2},

//                    },
//                    new OrRule()
//                    {
//                        new FuzzyFunc(){ Property = "POSTURE",Operator = "GreaterThan",Value=2},

//                    },

//                }

//            };
//        }
//        private static FuzzyDefinition CreateDefinition()
//        {
//            return new FuzzyDefinition()
//            {
//                Properties = new List<string>() { "Tremor", "Dyskinesia" },
//                Rules = new List<OrRule>()
//                {

//                    new OrRule()
//                    {
//                        new FuzzyFunc(){ Property = "Tremor",Operator = "GreaterThan",Value=1},
//                        new FuzzyFunc(){ Property = "Dyskinesia",Operator = "GreaterThan",Value=5}
//                    },
//                    new OrRule()
//                    {
//                        new FuzzyFunc(){ Property = "Tremor",Operator = "GreaterThan",Value=5},
//                        new FuzzyFunc(){ Property = "Dyskinesia",Operator = "GreaterThan",Value=1}
//                    },

//                }

//            };
//        }
//        [TestMethod]
//        public void TestSerializeExpression()
//        {

//            FuzzyDefinition profile = CreateDefinition();

//            //serializerSettings.ReferenceLoopHandling =  ReferenceLoopHandling.Ignore;
//            string json = JsonConvert.SerializeObject(profile);
//            var copy = JsonConvert.DeserializeObject<dynamic>(json);

//            Assert.IsTrue(copy.Properties!=null);
//            Assert.IsTrue(copy.Rules != null);
//        }

//        [TestMethod]
//        public void TestFuzzyFromExpression1()
//        {

//            FuzzyDefinition profile = CreateDefinition();
//            var d = new { Tremor = 2, Dyskinesia = 1 };
//            //serializerSettings.ReferenceLoopHandling =  ReferenceLoopHandling.Ignore;

//            Expression left = null;

//            foreach (var r in profile.Rules)
//            {
//                Expression right = null;
//                foreach (var a in r)
//                {


//                    var v = d.GetType().GetProperty(a.Property).GetValue(d, null);

//                    var value = Expression.Constant(v);
//                    var constant = Expression.Constant(a.Value);
//                    var q = Expression.GreaterThan(value, constant);
//                    if (right == null)
//                        right = q;
//                    else
//                        right = Expression.And(right, q);

//                }

//                if (left != null && right != null)
//                    left = Expression.Or(left, right);
//                else
//                    left = right;

//            }

//            var ret = (bool)Expression.Lambda(left).Compile().DynamicInvoke();

//            Assert.IsFalse(ret);


//            //ar body = Expression.Equal(
//            //    len, Expression.Constant(5));
//            //var lambda = Expression.Lambda<Func<dynamic, bool>>(
//            //    body, param);
//            //Assert.IsTrue(copy.Properties != null);
//            //Assert.IsTrue(copy.Rules != null);
//        }


//        [TestMethod]
//        public void PIGDFromExpression_Test1()
//        {

//            FuzzyDefinition profile = CreatePIGDDefinition();
//            var d = new { FOG = 2};
//            //serializerSettings.ReferenceLoopHandling =  ReferenceLoopHandling.Ignore;

//            Expression left = null;

//            foreach (var r in profile.Rules)
//            {
//                Expression right = null;
//                foreach (var a in r)
//                {


//                    var v = d.GetType().GetProperty(a.Property).GetValue(d, null);

//                    var value = Expression.Constant(v);
//                    var constant = Expression.Constant(a.Value);
//                    var q = Expression.GreaterThan(value, constant);
//                    if (right == null)
//                        right = q;
//                    else
//                        right = Expression.And(right, q);

//                }

//                if (left != null && right != null)
//                    left = Expression.Or(left, right);
//                else
//                    left = right;

//            }

//            var ret = (bool)Expression.Lambda(left).Compile().DynamicInvoke();

//            Assert.IsTrue(ret);
//        }

//        /// <summary>
//        /// Object Property Helper
//        /// </summary>
//        private class ObjectPropertyHelper
//        {

//            public int Variable1 { get; set; }
//            public int Variable2 { get; set; }
//            public int Variable3 { get; set; }
//            public int Variable4 { get; set; }
//            public int Variable5 { get; set; }
//            public int Variable6 { get; set; }
//            public int Variable7 { get; set; }
//            public int Variable8 { get; set; }
//            public int Variable9 { get; set; }
//            public int Variable10 { get; set; }
//            public int Variable11 { get; set; }
//            public int Variable12 { get; set; }
//            public int Variable13 { get; set; }
//            public int Variable14 { get; set; }
//            public int Variable15 { get; set; }
//            public int Variable16 { get; set; }
//            public int Variable17 { get; set; }
//            public int Variable18 { get; set; }
//            public int Variable19 { get; set; }
//            public int Variable20 { get; set; }

//            public double DVariable1 { get; set; }
//            public double DVariable2 { get; set; }
//            public double DVariable3 { get; set; }
//            public double DVariable4 { get; set; }
//            public double DVariable5 { get; set; }
//            public double DVariable6 { get; set; }
//            public double DVariable7 { get; set; }
//            public double DVariable8 { get; set; }
//            public double DVariable9 { get; set; }
//            public double DVariable10 { get; set; }
//            public double DVariable11 { get; set; }
//            public double DVariable12 { get; set; }
//            public double DVariable13 { get; set; }
//            public double DVariable14 { get; set; }
//            public double DVariable15 { get; set; }
//            public double DVariable16 { get; set; }
//            public double DVariable17 { get; set; }
//            public double DVariable18 { get; set; }
//            public double DVariable19 { get; set; }
//            public double DVariable20 { get; set; }

//            public bool BVariable1 { get; set; }
//            public bool BVariable2 { get; set; }
//            public bool BVariable3 { get; set; }
//            public bool BVariable4 { get; set; }
//            public bool BVariable5 { get; set; }
//            public bool BVariable6 { get; set; }
//            public bool BVariable7 { get; set; }
//            public bool BVariable8 { get; set; }
//            public bool BVariable9 { get; set; }
//            public bool BVariable10 { get; set; }
//            public bool BVariable11 { get; set; }
//            public bool BVariable12 { get; set; }
//            public bool BVariable13 { get; set; }
//            public bool BVariable14 { get; set; }
//            public bool BVariable15 { get; set; }
//            public bool BVariable16 { get; set; }
//            public bool BVariable17 { get; set; }
//            public bool BVariable18 { get; set; }
//            public bool BVariable19 { get; set; }
//            public bool BVariable20 { get; set; }
//            /// <summary>
//            /// Create Instance of ObjectPropertyHelper
//            /// </summary>
//            /// <param name="propMapping"></param>
//            /// <param name="source"></param>
//            /// <returns></returns>
//            public static ObjectPropertyHelper CreateInstanceBinder(Dictionary<string, string> propMapping,
//                object source)
//            {

//                ObjectPropertyHelper target=new ObjectPropertyHelper();
//                foreach (var c in propMapping.Keys)
//                {
//                    target.GetType().GetProperty(propMapping[c]).SetValue(target,source.GetType().GetProperty(c).GetValue(source,null));
//                }

//                return target;

//            }


//        }

//        [TestMethod]
//        public void TestFuzzyFromExpression3()
//        {

//            FuzzyDefinition profile = CreateDefinition();
//            ParameterExpression param = Expression.Parameter(typeof(ObjectPropertyHelper), "param");

//            var d = new { Tremor = 2, Dyskinesia = 10 };
//            //serializerSettings.ReferenceLoopHandling =  ReferenceLoopHandling.Ignore;

//            Expression left = null;

//            int propertyCount = 1;
//            Dictionary<string,string> propertyIndex=new Dictionary<string, string>();
//            foreach (var r in profile.Rules)
//            {
//                Expression right = null;
//                foreach (var a in r)
//                {
//                    var propName = "";
//                    if (!propertyIndex.ContainsKey(a.Property))
//                    {
//                        propName = "Variable" + (propertyCount++);
//                        propertyIndex.Add(a.Property,propName);
//                    }

//                    propName = propertyIndex[a.Property];
//                    //var param = Expression.Parameter(typeof(object),"param");
//                    var value=Expression.Property(param, propName);
//                    var constant = Expression.Constant(a.Value);
//                    var q = Expression.GreaterThan(value, constant);
//                    if (right == null)
//                        right = q;
//                    else
//                        right = Expression.And(right, q);

//                }

//                if (left != null && right != null)
//                    left = Expression.Or(left, right);
//                else
//                    left = right;

//            }

//            var funcExp = Expression.Lambda<Func<ObjectPropertyHelper, bool>>(left, param);


//            Assert.IsTrue(funcExp.Compile().Invoke(ObjectPropertyHelper.CreateInstanceBinder(propertyIndex,d)));
//        }


//        [TestMethod]
//        public void TestFuzzyFromExpression2()
//        {

//            FuzzyDefinition profile = CreateDefinition();
//            var d = new {Tremor = 2,Dyskinesia=10};
//            //serializerSettings.ReferenceLoopHandling =  ReferenceLoopHandling.Ignore;

//            Expression left = null;

//       foreach (var r in profile.Rules)
//       {
//           Expression right = null;
//                foreach (var a in r)
//                {


//                    var v=d.GetType().GetProperty(a.Property).GetValue(d, null);

//               var value = Expression.Constant(v);
//               var constant=Expression.Constant(a.Value);
//               var q = Expression.GreaterThan(value, constant);
//               if (right == null)
//                   right = q;
//               else
//                   right=Expression.And(right, q);

//           }

//                if (left != null&&right!=null)
//                    left = Expression.Or(left, right);
//                else
//                    left = right;

//       }


//       var ret = (bool) Expression.Lambda(left).Compile().DynamicInvoke();

//       Assert.IsTrue(ret);


//            //ar body = Expression.Equal(
//            //    len, Expression.Constant(5));
//            //var lambda = Expression.Lambda<Func<dynamic, bool>>(
//            //    body, param);
//            //Assert.IsTrue(copy.Properties != null);
//            //Assert.IsTrue(copy.Rules != null);
//        }

//    }
//}