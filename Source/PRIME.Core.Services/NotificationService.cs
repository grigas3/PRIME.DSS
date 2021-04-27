﻿using PRIME.Core.Common.Interfaces;
using PRIME.Core.Common.Models;
using PRIME.Core.Service.Notification;

namespace PRIME.Core.Services
{
    /// <summary>
    ///     Communication Manager
    ///     TODO: Proper Implementation with queue and scheduling
    /// </summary>
    public class NotificationService : INotificationService
    {
        #region Parameter Provider

        private readonly ICommunicationParamProvider _communicationParamProvider;

        #endregion

        private readonly CommunicationParameters commParams;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="communicationParamProvider">The Communication parameters</param>
        public NotificationService(ICommunicationParamProvider communicationParamProvider)
        {
            commParams = communicationParamProvider.GetParameters();
        }


        /// <summary>
        ///     Send Message
        /// </summary>
        /// <param name="message"></param>
        public void SendMessage(IPDMessage message)
        {
            switch (message.MessageType)
            {
                case PDMessageType.EMAIL:
                    SendEmail(message);
                    break;
                case PDMessageType.SMS:
                    SendSMS(message);
                    break;
                case PDMessageType.GCM:
                    SendGCM(message);
                    break;
                case PDMessageType.FCM:
                    SendFCM(message);
                    break;
            }
        }

        /// <summary>
        ///     Send FCM Message
        /// </summary>
        /// <param name="message"></param>
        private void SendFCM(IPDMessage message)
        {
            FCMNotification.Notify(message.ReceiverUri, message.SenderUri, message.Subject, commParams.FCMAppID,
                commParams.FCMSenderID);
        }

        /// <summary>
        ///     Send GCM Message
        /// </summary>
        /// <param name="message"></param>
        private void SendGCM(IPDMessage message)
        {
            GCMNotification.Notify(message.ReceiverUri, message.SenderUri, message.Subject, commParams.GCMAppID,
                commParams.GCMSenderID);
        }

        /// <summary>
        ///     Send SMS Message
        /// </summary>
        /// <param name="message"></param>
        private void SendSMS(IPDMessage message)
        {
            SMSNotification.Notify(message.Sender, message.ReceiverUri, message.Subject, commParams.SMSUserName,
                commParams.SMSPasword, commParams.SMSKey);
        }

        /// <summary>
        ///     Send Email
        /// </summary>
        /// <param name="message"></param>
        private void SendEmail(IPDMessage message)
        {
            EmailNotification.Notify(message.SenderUri, message.ReceiverUri, message.Body, message.Subject,
                commParams.EmailUserName, commParams.EmailPasword, commParams.EmailServer, commParams.EmailPort, null);
        }
    }
}