using System.Collections.Generic;
using System.Linq;

namespace PRIME.Core.DSS.Treatment
{
    /// <summary>
    /// Abstract Naive Bayes class
    /// </summary>
    public abstract class NaiveBayes
    {
        #region Protected Properties
        /// <summary>
        /// Classifier Variables
        /// </summary>
        public List<NaiveVar> Variables { get; set; }
        #endregion Abstract Properties
        

        /// <summary>
        /// Get Value
        /// </summary>
        /// <param name="val"></param>
        /// <param name="prior"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        private double GetProb(bool? val, double prior, double weight)
        {
            if (val.HasValue)
            {
                return val.Value ? weight : (1 - weight);
            }
            else
            {
                return weight * prior;
            }
        }
        /// <summary>
        /// Get Output
        /// </summary>
        /// <param name="x">feature vector</param>
        /// <returns>class probabilities</returns>
        public virtual double GetOutput(List<NaiveVarInstance> x)
        {
            //Need probably an optimization
            double pPos = 1;
            double pNeg = 1;
            
            foreach (var c in Variables)
            {
                var vb = new bool?();
                var v = x.FirstOrDefault(e => e.Code.ToLower() == c.Code.ToLower());
                if (v != null)
                    vb = v.Present;
                var p = GetProb(vb, c.Prior, c.Weight);
                pPos *= p;
                pNeg *= (1 - p);
            }

            var s1 = pPos + pNeg;
            return pPos / s1;
        }
    }
}