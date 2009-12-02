﻿#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009 Jon Skeet
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
using System;
using NUnit.Framework;
using NodaTime.Format;

namespace NodaTime.Test
{
    public partial class PeriodFormatterTest
    {
        [Test]
        public void WithProviderCreatesNewInstance()
        {
            var sutDefault = new PeriodFormatter(null, null, daysPeriodType);
            Assert.IsNull(sutDefault.Provider);
            var sutWithProvider = sutDefault.WithProvider(provider);
            Assert.AreEqual(provider, sutWithProvider.Provider);
            Assert.AreNotSame(sutDefault, sutWithProvider);
        }

        [Test]
        public void WithPeriodTypeCreatesNewInstance()
        {
            var sutDefault = new PeriodFormatter(null, null, daysPeriodType);
            Assert.AreEqual(daysPeriodType, sutDefault.PeriodType);
            var sutWithPeriodType = sutDefault.WithPeriodType(monthsPeriodType);
            Assert.AreEqual(monthsPeriodType, sutWithPeriodType.PeriodType);
            Assert.AreNotSame(sutDefault, sutWithPeriodType);
        }
    }
}
