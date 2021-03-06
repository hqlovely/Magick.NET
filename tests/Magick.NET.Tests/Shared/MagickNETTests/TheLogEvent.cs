﻿// Copyright 2013-2020 Dirk Lemstra <https://github.com/dlemstra/Magick.NET/>
//
// Licensed under the ImageMagick License (the "License"); you may not use this file except in
// compliance with the License. You may obtain a copy of the License at
//
//   https://www.imagemagick.org/script/license.php
//
// Unless required by applicable law or agreed to in writing, software distributed under the
// License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND,
// either express or implied. See the License for the specific language governing permissions
// and limitations under the License.

using System;
using ImageMagick;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Magick.NET.Tests
{
    public partial class MagickNETTests
    {
        [TestClass]
        public class TheLogEvent
        {
            [TestMethod]
            public void ShouldPassOrderedTests()
            {
                ShouldNotCallLogDelegeteWhenLogEventsAreNotSet();

                ShouldCallLogDelegateWhenLogEventsAreSet();

                ShouldLogTraceEventsWhenLogEventsIsSetToAll();

                ShouldStopCallingLogDelegateWhenLogDelegateIsRemoved();
            }

            private void ShouldNotCallLogDelegeteWhenLogEventsAreNotSet()
            {
                using (var image = new MagickImage(Files.SnakewarePNG))
                {
                    int count = 0;
                    EventHandler<LogEventArgs> logDelegate = (sender, arguments) =>
                    {
                        count++;
                    };

                    MagickNET.Log += logDelegate;

                    image.Flip();

                    MagickNET.Log -= logDelegate;

                    Assert.AreEqual(0, count);
                }
            }

            private void ShouldCallLogDelegateWhenLogEventsAreSet()
            {
                using (var image = new MagickImage(Files.SnakewarePNG))
                {
                    int count = 0;
                    EventHandler<LogEventArgs> logDelegate = (sender, arguments) =>
                    {
                        Assert.IsNull(sender);
                        Assert.IsNotNull(arguments);
                        Assert.AreNotEqual(LogEvents.None, arguments.EventType);
                        Assert.IsNotNull(arguments.Message);
                        Assert.AreNotEqual(0, arguments.Message.Length);

                        count++;
                    };

                    MagickNET.Log += logDelegate;

                    MagickNET.SetLogEvents(LogEvents.Detailed);

                    image.Flip();

                    MagickNET.Log -= logDelegate;

                    Assert.AreNotEqual(0, count);
                    count = 0;

                    image.Flip();
                    Assert.AreEqual(0, count);
                }
            }

            private void ShouldLogTraceEventsWhenLogEventsIsSetToAll()
            {
                int traceEvents = 0;
                EventHandler<LogEventArgs> logDelegate = (sender, arguments) =>
                {
                    if (arguments.EventType == LogEvents.Trace)
                        traceEvents++;
                };

                MagickNET.SetLogEvents(LogEvents.All);

                MagickNET.Log += logDelegate;

                using (var image = new MagickImage(Files.SnakewarePNG))
                {
                }

                MagickNET.Log -= logDelegate;

                Assert.AreNotEqual(0, traceEvents);
            }

            private void ShouldStopCallingLogDelegateWhenLogDelegateIsRemoved()
            {
                using (var image = new MagickImage(Files.SnakewarePNG))
                {
                    int count = 0;
                    EventHandler<LogEventArgs> logDelegate = (sender, arguments) =>
                    {
                        count++;
                    };

                    MagickNET.Log += logDelegate;

                    MagickNET.SetLogEvents(LogEvents.Detailed);

                    MagickNET.Log -= logDelegate;

                    image.Flip();
                    Assert.AreEqual(0, count);
                }
            }
        }
    }
}
