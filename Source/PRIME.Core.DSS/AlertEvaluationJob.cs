using PRIME.Core.Common.Interfaces;
using System.Collections.Generic;
using System.Linq;
using PRIME.Core.Common.Enums;
using System.Threading.Tasks;
using PRIME.Core.Common.Models;

namespace PRIME.Core.DSS
{
    /// <summary>
    /// Alert Evaluation
    /// </summary>
    public class AlertEvaluationJob : IRecurringJob
    {
        private readonly IAlertEvaluator _alertEvaluator;
        private readonly IAlertInputProvider _alertInputProvider;
        private readonly INotificationService _communicationManager;
        private readonly IPatientProvider _patientProvider;
        private readonly IGenericLogger _logger;
        private const int MaxPatients = 100;

        /// <summary>
        /// Alert Evaluation Job
        /// </summary>
        /// <param name="alertEvaluator"></param>
        /// <param name="alertInputProvider"></param>
        /// <param name="patientProvider"></param>
        /// <param name="commManager"></param>
        /// <param name="logger"></param>
        public AlertEvaluationJob(IAlertEvaluator alertEvaluator, IAlertInputProvider alertInputProvider,
            IPatientProvider patientProvider, INotificationService commManager, IGenericLogger logger)
        {
            this._alertEvaluator = alertEvaluator;
            this._patientProvider = patientProvider;
            this._logger = logger;
            this._alertInputProvider = alertInputProvider;
            this._communicationManager = commManager;
        }

        /// <summary>
        /// Run Job
        /// </summary>
        public async Task<bool> Run()
        {
            int take = MaxPatients;
            int currentNumberOfPatients = 0;
            int n = 0;

            var alertInputs = _alertInputProvider.GetAlertInputs();
            var enumerable = alertInputs as IAlertInput[] ?? alertInputs.ToArray();
            do
            {
                var patientList = _patientProvider.GetPatientIds(MaxPatients, currentNumberOfPatients);
                var patIds = patientList as string[] ?? patientList.ToArray();
                n = patIds.Count();

                // Alert Patient List
                foreach (var patId in patIds)
                {
                    //Iterate Alert Input Models
                    foreach (var alertInput in enumerable)
                    {
                        var alertLevel = await _alertEvaluator.GetAlertLevel(alertInput, patId);

                        if (alertLevel != AlertLevel.High)
                            continue;

                        IEnumerable<NotificationContact> contacts = _patientProvider.GetPatientContacts(patId);
                        foreach (var contact in contacts)
                        {
                            _communicationManager.SendMessage(new PDMessage()
                            {
                                Sender = "PRIME",
                                Subject = alertInput.Name,
                                Body = alertInput.Message,
                                ReceiverUri = contact.Uri,
                                MessageType = contact.PreferredMessageType,
                                Receiver = contact.Name
                            });
                        }
                    }
                }
            } while (n == take);

            return true;
        }

        /// <summary>
        /// Get Job Id
        /// </summary>
        /// <returns></returns>
        public string GetId()
        {
            return "A8230448-3BAB-4B49-94D2-733975DD43DD";
        }
    }
}