﻿using System;
using System.Linq;
using NUnit.Framework;
using NUnitReporter.EventReport;
using NUnitReporter.EventReport.Events;

namespace NUnitReporterTests.EventReport
{
    public class DefaultEventReportTests
    {
        [Test]
        public void ShouldCreateNestedEvents()
        {
            var report = new DefaultEventReport();

            var guid = report.RecordActivityStarted("Dancing with a spouse", "Anna");
            report.RecordEvent("Taking a drink", "Wine");
            report.RecordActivityFinished(guid);
            report.RecordEvent("Going to bathroom", "Wine");

            Assert.That(report.RootActivity, Is.Not.Null);
            Assert.That(report.RootActivity.Parent, Is.Null);
            Assert.That(report.RootActivity.Guid, Is.Not.Null);
            Assert.That(report.RootActivity.Guid.ToString(), Is.Not.Empty);
            Assert.That(report.RootActivity.Nested, Is.Not.Empty);

            var rootNestedActions = report.RootActivity.Nested.ToList();

            Assert.That(report.RootActivity.Nested, Has.Count.EqualTo(2));

            Assert.That(rootNestedActions, Has.Some.Matches(
                Is.InstanceOf<BasicEvent>()
                    .And.Property("Description").EqualTo("Going to bathroom")
                    .And.Property("Arguments").EquivalentTo(new [] {"Wine"})
                ));

            Assert.That(report.RootActivity.Nested, Has.Some.Matches(
                Is.InstanceOf<Activity>()
                    .And.Property("Description").EqualTo("Dancing with a spouse")
                    .And.Property("Arguments").EquivalentTo(new [] {"Anna"})
                    .And.Property("Nested").Count.EqualTo(1)
                    .And.Property("Nested").Some.Matches(
                        Is.InstanceOf<BasicEvent>()
                            .And.Property("Description").EqualTo("Taking a drink")
                            .And.Property("Arguments").EquivalentTo(new [] {"Wine"})
                    )
                ));
        }

        [Test]
        public void ShouldRecordSreenshotEvent()
        {
            var report = new DefaultEventReport();
            report.RecordScreenshot("sun_flower.jpg");

            Assert.That(report.RootActivity.Nested, Has.Count.EqualTo(1).And.Some.Matches(
                Is.InstanceOf<ScreenshotEvent>().And.Property("FilePath").EqualTo("sun_flower.jpg")
            ));
        }

        [Test]
        public void ShouldRecordErrorEvent()
        {
            var report = new DefaultEventReport();

            Exception exception = null;

            try
            {
                throw new Exception("The system have cut it's finger and cannot proceed");
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            report.RecordError(exception);

            Assert.That(report.RootActivity.Nested, Has.Count.EqualTo(1).And.Some.Matches(
                Is.InstanceOf<ErrorEvent>()
                    .And.Property("Message").EqualTo(exception.Message)
                    .And.Property("StackTrace").EqualTo(exception.StackTrace)
            ));
        }
    }
}