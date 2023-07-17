using System;
using System.Collections.Generic;
using System.Text;

namespace PRIME.Core.DSS.MedProposal
{
    /// <summary>
    /// Medication
    /// </summary>
    public class Medication
    {
        /// <summary>
        /// Medication Code
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// Medication Dose
        /// </summary>
        public double Dose { get; set; }
        /// <summary>
        /// Time of medications in minutes from start of day
        /// </summary>
        public int Time { get; set; }

    }

    /// <summary>
    /// Evaluation Of Medication Effectiveness
    /// </summary>
    public class MedicationEvaluation : Medication
    {
        /// <summary>
        /// Off Time
        /// </summary>
        public double OffTime { get; set; }
        /// <summary>
        /// Dyskinesia Time
        /// </summary>
        public double DyskinesiaTime { get; set; }

    }

    public class MedicationScheduleGenerator
    {


        //public static IEnumerable<MedicationSchedule> Generate(MedicationSchedule schedile)
        //{

        //}

    }


    public class MedicationSchedule
    {

        public DateTime StartDate { get; set; }

        public List<Medication> Medications { get; set; }

    }
}
