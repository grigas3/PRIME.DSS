using Microsoft.VisualStudio.TestTools.UnitTesting;
using PRIME.Core.Common.Interfaces;
using PRIME.Core.DSS.Treatment;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRIME.Core.UnitTests
{
    public class DumyCondition
    {

        public string Code { get; set; }
        public bool? Value { get; set; }

    }
    public class DummyConditionRepository : IConditionRepository
    {


        private List<DumyCondition> _codes = new List<DumyCondition>();

        public void Add(DumyCondition c)
        {
            _codes.Add(c);
        }
        public bool? HasCondition(string code, string system)
        {
            return _codes.Where(e => e.Code==code).Select(e => e.Value).FirstOrDefault();
        }


        public async Task Init(string id)
        {

        }
    
        public bool? HasCondition(string code, string system, Func<object, bool> convert)
        {
            throw new NotImplementedException();
        }
    }
    [TestClass]
    public class TreatmentTests
    {


        //Do not offer anticholinergics to people with Parkinson's disease who have
        //developed dyskinesia and/or motor fluctuations.


            [TestMethod]
        public void Treatment_Dopamine()
        {

            TreatmentClassifierFactory factory = new TreatmentClassifierFactory();
            ConditionClassifierFactory cfactory = new ConditionClassifierFactory();

            DummyConditionRepository repository = new DummyConditionRepository();

            repository.Add(new DumyCondition() { Code = "FLUCTUATIONS", Value = true });
            repository.Add(new DumyCondition() { Code = "LEVODOPA", Value = true });
            repository.Add(new DumyCondition() { Code = "IMPHISTORY", Value = true });
            repository.Add(new DumyCondition() { Code = "ALCOHOL", Value = true });

            factory.Load(Path.Combine(Environment.CurrentDirectory, "Data\\Treatments"));
            cfactory.Load(Path.Combine(Environment.CurrentDirectory, "Data\\Conditions"));

            var options=factory.GetTreatmentOptions(repository);

            Assert.IsTrue(options.Any(e => e.Code == "DOPAMINE"&&e.Probability>=0.3),"DOPAMINE Has low probability");
            List<Condition> conditions = new List<Condition>();
            foreach (var o in options)
            {
               conditions.AddRange(cfactory.GetConditionOptions(o, repository));


            }
            Assert.IsTrue(conditions.Any(e => e.Code == "IMPULSIVITY" && e.Probability > 0.1), "IMP Has low probability");

        }


        [TestMethod]
        public void Treatment_ImpulsivityLow()
        {

            TreatmentClassifierFactory factory = new TreatmentClassifierFactory();
            ConditionClassifierFactory cfactory = new ConditionClassifierFactory();

            DummyConditionRepository repository = new DummyConditionRepository();

            repository.Add(new DumyCondition() { Code = "FLUCTUATIONS", Value = true });
            repository.Add(new DumyCondition() { Code = "LEVODOPA", Value = true });
            repository.Add(new DumyCondition() { Code = "DOPAMINE", Value = false });
            repository.Add(new DumyCondition() { Code = "IMPHISTORY", Value = true });
            repository.Add(new DumyCondition() { Code = "ALCOHOL", Value = true });

            cfactory.Load(Path.Combine(Environment.CurrentDirectory, "TestData\\Conditions"));

           
            List<Condition> conditions = new List<Condition>();
         
                conditions.AddRange(cfactory.GetConditionOptions(null, repository));


         
            Assert.IsTrue(conditions.Any(e => e.Code == "IMPULSIVITY" && e.Probability < 0.5));

        }

        [TestMethod]
        public void Treatment_ImpulsivityHigh()
        {

            TreatmentClassifierFactory factory = new TreatmentClassifierFactory();
            ConditionClassifierFactory cfactory = new ConditionClassifierFactory();

            DummyConditionRepository repository = new DummyConditionRepository();

            repository.Add(new DumyCondition() { Code = "FLUCTUATIONS", Value = true });
            repository.Add(new DumyCondition() { Code = "LEVODOPA", Value = true });
            repository.Add(new DumyCondition() { Code = "DOPAMINE", Value = true });
            repository.Add(new DumyCondition() { Code = "IMPHISTORY", Value = true });
            repository.Add(new DumyCondition() { Code = "ALCOHOL", Value = true });

            cfactory.Load(Path.Combine(Environment.CurrentDirectory, "TestData\\Conditions"));


            List<Condition> conditions = new List<Condition>();

            conditions.AddRange(cfactory.GetConditionOptions(null, repository));



            Assert.IsTrue(conditions.Any(e => e.Code == "IMPULSIVITY" && e.Probability > 0.5));

        }


        [TestMethod]
        public void Treatment_ImpulsivityVeryLow()
        {

            TreatmentClassifierFactory factory = new TreatmentClassifierFactory();
            ConditionClassifierFactory cfactory = new ConditionClassifierFactory();

            DummyConditionRepository repository = new DummyConditionRepository();

            repository.Add(new DumyCondition() { Code = "FLUCTUATIONS", Value = true });
            repository.Add(new DumyCondition() { Code = "LEVODOPA", Value = true });
            repository.Add(new DumyCondition() { Code = "DOPAMINE", Value = false });
            repository.Add(new DumyCondition() { Code = "IMPHISTORY", Value = false });
            repository.Add(new DumyCondition() { Code = "ALCOHOL", Value = true });

            cfactory.Load(Path.Combine(Environment.CurrentDirectory, "TestData\\Conditions"));


            List<Condition> conditions = new List<Condition>();

            conditions.AddRange(cfactory.GetConditionOptions(null, repository));



            Assert.IsTrue(conditions.Any(e => e.Code == "IMPULSIVITY" && e.Probability <0.1));

        }


        [TestMethod]
        public void Treatment_ReplaceDopamine()
        {

            TreatmentClassifierFactory factory = new TreatmentClassifierFactory();
            ConditionClassifierFactory cfactory = new ConditionClassifierFactory();

            DummyConditionRepository repository = new DummyConditionRepository();

            repository.Add(new DumyCondition() { Code = "FLUCTUATIONS", Value = true });
            repository.Add(new DumyCondition() { Code = "LEVODOPA", Value = true });
            repository.Add(new DumyCondition() { Code = "DOPAMINE", Value = true });
            repository.Add(new DumyCondition() { Code = "IMPHISTORY", Value = true });
            repository.Add(new DumyCondition() { Code = "ALCOHOL", Value = true });

            factory.Load(Path.Combine(Environment.CurrentDirectory, "TestData\\Treatments"));
            cfactory.Load(Path.Combine(Environment.CurrentDirectory, "TestData\\Conditions"));

            var options = factory.GetTreatmentOptions(repository);

            Assert.IsTrue(options.Any(e => e.Code == "MAO" && e.Probability >= 0.5));
            List<Condition> conditions = new List<Condition>();
            foreach (var o in options)
            {
                conditions.AddRange(cfactory.GetConditionOptions(o, repository));


            }
            Assert.IsTrue(conditions.Any(e => e.Code == "IMPULSIVITY" && e.Probability < 0.5));

        }



        [TestMethod]
        public void Treatment_NoDopamine()
        {

            TreatmentClassifierFactory factory = new TreatmentClassifierFactory();
            ConditionClassifierFactory cfactory = new ConditionClassifierFactory();

            DummyConditionRepository repository = new DummyConditionRepository();

            repository.Add(new DumyCondition() { Code = "FLUCTUATIONS", Value = false });
            repository.Add(new DumyCondition() { Code = "LEVODOPA", Value = false });
            repository.Add(new DumyCondition() { Code = "IMPHISTORY", Value = true });
            repository.Add(new DumyCondition() { Code = "ALCOHOL", Value = true });

            factory.Load(Path.Combine(Environment.CurrentDirectory, "TestData\\Treatments"));
            cfactory.Load(Path.Combine(Environment.CurrentDirectory, "TestData\\Conditions"));

            var options = factory.GetTreatmentOptions(repository);

            Assert.IsTrue(options.Any(e => e.Code == "DOPAMINE" && e.Probability < 0.5), "DOPAMINE Has high probability");
           
        }

    }
}
