﻿using System;
using Castle.DynamicProxy;

namespace NUnitReporter.EventReport.ProxyFactory
{
    public class ReportingInerceptor : IInterceptor
    {
        private readonly IEventReportFactory _reportFactory;

        public ReportingInerceptor(IEventReportFactory reportFactory)
        {
            _reportFactory = reportFactory;
        }

        public void Intercept(IInvocation invocation)
        {
            var report = _reportFactory.CurrentTestReport;

            if (report != null && IsAligibleForReporting(invocation))
            {
                ReportAndProceed(invocation, report);
            }
            else
            {
                invocation.Proceed();
            }
        }

        private static void ReportAndProceed(IInvocation invocation, IEventReport report)
        {
            String actionGuid = report.RecordActivityStarted(
                String.Format("{0}::{1}", invocation.TargetType.Name, invocation.Method.Name), 
                invocation.Arguments
            );

            try
            {
                invocation.Proceed();
            }
            catch (Exception ex)
            {
                report.RecordError(ex);
                throw;
            }
            finally
            {
                if (!String.IsNullOrEmpty(actionGuid))
                {
                    report.RecordActivityFinished(actionGuid);
                }
            }
        }

        private static Boolean IsAligibleForReporting(IInvocation invocation)
        {
            if (invocation.Method.IsSpecialName)
            {
                return false;
            }

            if (invocation.Method.GetBaseDefinition().DeclaringType == typeof (Object))
            {
                return false;
            }

            return true;
        }
    }
}