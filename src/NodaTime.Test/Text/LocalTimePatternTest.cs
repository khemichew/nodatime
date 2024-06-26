// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using NodaTime.Text;
using NodaTime.Text.Patterns;
using NUnit.Framework;

namespace NodaTime.Test.Text
{
    public partial class LocalTimePatternTest : PatternTestBase<LocalTime>
    {
        private static readonly DateTime SampleDateTime = new DateTime(2000, 1, 1, 21, 13, 34, 123, DateTimeKind.Unspecified).AddTicks(4567);
        private static readonly LocalTime SampleLocalTime = LocalTime.FromHourMinuteSecondMillisecondTick(21, 13, 34, 123, 4567);

        // Characters we expect to work the same in Noda Time as in the BCL.
        private const string ExpectedCharacters = "hHms.:fFtT ";

        private static readonly CultureInfo AmOnlyCulture = CreateCustomAmPmCulture("am", "");
        private static readonly CultureInfo PmOnlyCulture = CreateCustomAmPmCulture("", "pm");
        private static readonly CultureInfo NoAmOrPmCulture = CreateCustomAmPmCulture("", "");

        internal static readonly Data[] InvalidPatternData = {
            new Data { Pattern = "", Message = TextErrorMessages.FormatStringEmpty },
            new Data { Pattern = "!", Message = TextErrorMessages.UnknownStandardFormat, Parameters = {'!', typeof(LocalTime).FullName! }},
            new Data { Pattern = "%", Message = TextErrorMessages.UnknownStandardFormat, Parameters = { '%', typeof(LocalTime).FullName! } },
            new Data { Pattern = "\\", Message = TextErrorMessages.UnknownStandardFormat, Parameters = { '\\', typeof(LocalTime).FullName! } },
            new Data { Pattern = "%%", Message = TextErrorMessages.PercentDoubled },
            new Data { Pattern = "%\\", Message = TextErrorMessages.EscapeAtEndOfString },
            new Data { Pattern = "ffffffffff", Message = TextErrorMessages.RepeatCountExceeded, Parameters = { 'f', 9 } },
            new Data { Pattern = "FFFFFFFFFF", Message = TextErrorMessages.RepeatCountExceeded, Parameters = { 'F', 9 } },
            new Data { Pattern = "H%", Message = TextErrorMessages.PercentAtEndOfString },
            new Data { Pattern = "HHH", Message = TextErrorMessages.RepeatCountExceeded, Parameters = { 'H', 2 } },
            new Data { Pattern = "mmm", Message = TextErrorMessages.RepeatCountExceeded, Parameters = { 'm', 2 } },
            new Data { Pattern = "mmmmmmmmmmmmmmmmmmm", Message = TextErrorMessages.RepeatCountExceeded, Parameters = { 'm', 2 } },
            new Data { Pattern = "'qwe", Message = TextErrorMessages.MissingEndQuote, Parameters = { '\'' } },
            new Data { Pattern = "'qwe\\", Message = TextErrorMessages.EscapeAtEndOfString },
            new Data { Pattern = "'qwe\\'", Message = TextErrorMessages.MissingEndQuote, Parameters = { '\'' } },
            new Data { Pattern = "sss", Message = TextErrorMessages.RepeatCountExceeded, Parameters = { 's', 2 } },
            // T isn't valid in a time pattern
            new Data { Pattern = "1970-01-01THH:mm:ss", Message = TextErrorMessages.UnquotedLiteral, Parameters = { 'T' } }
        };

        internal static Data[] ParseFailureData = {
            new Data { Text = "17 6", Pattern = "HH h", Message = TextErrorMessages.InconsistentValues2, Parameters = {'H', 'h', typeof(LocalTime).FullName! }},
            new Data { Text = "17 AM", Pattern = "HH tt", Message = TextErrorMessages.InconsistentValues2, Parameters = {'H', 't', typeof(LocalTime).FullName! }},
            new Data { Text = "5 foo", Pattern = "h t", Message = TextErrorMessages.MissingAmPmDesignator},
            new Data { Text = "04.", Pattern = "ss.FF", Message = TextErrorMessages.MismatchedNumber, Parameters = { "FF" } },
            new Data { Text = "04.", Pattern = "ss;FF", Message = TextErrorMessages.MismatchedNumber, Parameters = { "FF" } },
            new Data { Text = "04.", Pattern = "ss.ff", Message = TextErrorMessages.MismatchedNumber, Parameters = { "ff" } },
            new Data { Text = "04.", Pattern = "ss;ff", Message = TextErrorMessages.MismatchedNumber, Parameters = { "ff" } },
            new Data { Text = "04.x", Pattern = "ss.FF", Message = TextErrorMessages.MismatchedNumber, Parameters = { "FF" } },
            new Data { Text = "04.x", Pattern = "ss;FF", Message = TextErrorMessages.MismatchedNumber, Parameters = { "FF" } },
            new Data { Text = "04.x", Pattern = "ss.ff", Message = TextErrorMessages.MismatchedNumber, Parameters = { "ff" } },
            new Data { Text = "04.x", Pattern = "ss;ff", Message = TextErrorMessages.MismatchedNumber, Parameters = { "ff" } },
            new Data { Text = "05 Foo", Pattern = "HH tt", Message = TextErrorMessages.MissingAmPmDesignator }
        };

        internal static Data[] ParseOnlyData = {
            new Data(0, 0, 0, 400) { Text = "4", Pattern = "%f", },
            new Data(0, 0, 0, 400) { Text = "4", Pattern = "%F", },
            new Data(0, 0, 0, 400) { Text = "4", Pattern = "FF", },
            new Data(0, 0, 0, 400) { Text = "40", Pattern = "FF", },
            new Data(0, 0, 0, 400) { Text = "4", Pattern = "FFF", },
            new Data(0, 0, 0, 400) { Text = "40", Pattern = "FFF" },
            new Data(0, 0, 0, 400) { Text = "400", Pattern = "FFF" },
            new Data(0, 0, 0, 400) { Text = "40", Pattern = "ff", },
            new Data(0, 0, 0, 400) { Text = "400", Pattern = "fff", },
            new Data(0, 0, 0, 400) { Text = "4000", Pattern = "ffff", },
            new Data(0, 0, 0, 400) { Text = "40000", Pattern = "fffff", },
            new Data(0, 0, 0, 400) { Text = "400000", Pattern = "ffffff", },
            new Data(0, 0, 0, 400) { Text = "4000000", Pattern = "fffffff", },
            new Data(0, 0, 0, 450) { Text = "45", Pattern = "ff" },
            new Data(0, 0, 0, 450) { Text = "45", Pattern = "FF" },
            new Data(0, 0, 0, 450) { Text = "45", Pattern = "FFF" },
            new Data(0, 0, 0, 450) { Text = "450", Pattern = "fff" },
            new Data(0, 0, 0, 456) { Text = "456", Pattern = "fff" },
            new Data(0, 0, 0, 456) { Text = "456", Pattern = "FFF" },

            new Data(0, 0, 0, 0) { Text = "0", Pattern = "%f" },
            new Data(0, 0, 0, 0) { Text = "00", Pattern = "ff" },
            new Data(0, 0, 0, 8) { Text = "008", Pattern = "fff" },
            new Data(0, 0, 0, 8) { Text = "008", Pattern = "FFF" },
            new Data(5, 0, 0, 0) { Text = "05", Pattern = "HH" },
            new Data(0, 6, 0, 0) { Text = "06", Pattern = "mm" },
            new Data(0, 0, 7, 0) { Text = "07", Pattern = "ss" },
            new Data(5, 0, 0, 0) { Text = "5", Pattern = "%H" },
            new Data(0, 6, 0, 0) { Text = "6", Pattern = "%m" },
            new Data(0, 0, 7, 0) { Text = "7", Pattern = "%s" },

            // AM/PM designator is case-insensitive for both short and long forms
            new Data(17, 0, 0, 0) { Text = "5 p", Pattern = "h t" },
            new Data(17, 0, 0, 0) { Text = "5 pm", Pattern = "h tt" },

            // Parsing using the semi-colon "comma dot" specifier
            new Data(16, 05, 20, 352) { Pattern = "HH:mm:ss;fff", Text = "16:05:20,352" },
            new Data(16, 05, 20, 352) { Pattern = "HH:mm:ss;FFF", Text = "16:05:20,352" },

            // Empty fractional section
            new Data(0,0,4,0) { Text = "04", Pattern = "ssFF" },
            new Data(0,0,4,0) { Text = "040", Pattern = "ssFF" },
            new Data(0,0,4,0) { Text = "040", Pattern = "ssFFF" },
            new Data(0,0,4,0) { Text = "04", Pattern = "ss.FF"},
        };

        internal static Data[] FormatOnlyData = {
            new Data(5, 6, 7, 8) { Text = "", Pattern = "%F" },
            new Data(5, 6, 7, 8) { Text = "", Pattern = "FF" },
            new Data(1, 1, 1, 400) { Text = "4", Pattern = "%f" },
            new Data(1, 1, 1, 400) { Text = "4", Pattern = "%F" },
            new Data(1, 1, 1, 400) { Text = "4", Pattern = "FF" },
            new Data(1, 1, 1, 400) { Text = "4", Pattern = "FFF" },
            new Data(1, 1, 1, 400) { Text = "40", Pattern = "ff" },
            new Data(1, 1, 1, 400) { Text = "400", Pattern = "fff" },
            new Data(1, 1, 1, 400) { Text = "4000", Pattern = "ffff" },
            new Data(1, 1, 1, 400) { Text = "40000", Pattern = "fffff" },
            new Data(1, 1, 1, 400) { Text = "400000", Pattern = "ffffff" },
            new Data(1, 1, 1, 400) { Text = "4000000", Pattern = "fffffff" },
            new Data(1, 1, 1, 450) { Text = "4", Pattern = "%f" },
            new Data(1, 1, 1, 450) { Text = "4", Pattern = "%F" },
            new Data(1, 1, 1, 450) { Text = "45", Pattern = "ff" },
            new Data(1, 1, 1, 450) { Text = "45", Pattern = "FF" },
            new Data(1, 1, 1, 450) { Text = "45", Pattern = "FFF" },
            new Data(1, 1, 1, 450) { Text = "450", Pattern = "fff" },
            new Data(1, 1, 1, 456) { Text = "4", Pattern = "%f" },
            new Data(1, 1, 1, 456) { Text = "4", Pattern = "%F" },
            new Data(1, 1, 1, 456) { Text = "45", Pattern = "ff" },
            new Data(1, 1, 1, 456) { Text = "45", Pattern = "FF" },
            new Data(1, 1, 1, 456) { Text = "456", Pattern = "fff" },
            new Data(1, 1, 1, 456) { Text = "456", Pattern = "FFF" },
            new Data(0,0,0,0) {Text = "", Pattern = "FF" },

            new Data(5, 6, 7, 8) { Culture = Cultures.EnUs, Text = "0", Pattern = "%f" },
            new Data(5, 6, 7, 8) { Culture = Cultures.EnUs, Text = "00", Pattern = "ff" },
            new Data(5, 6, 7, 8) { Culture = Cultures.EnUs, Text = "008", Pattern = "fff" },
            new Data(5, 6, 7, 8) { Culture = Cultures.EnUs, Text = "008", Pattern = "FFF" },
            new Data(5, 6, 7, 8) { Culture = Cultures.EnUs, Text = "05", Pattern = "HH" },
            new Data(5, 6, 7, 8) { Culture = Cultures.EnUs, Text = "06", Pattern = "mm" },
            new Data(5, 6, 7, 8) { Culture = Cultures.EnUs, Text = "07", Pattern = "ss" },
            new Data(5, 6, 7, 8) { Culture = Cultures.EnUs, Text = "5", Pattern = "%H" },
            new Data(5, 6, 7, 8) { Culture = Cultures.EnUs, Text = "6", Pattern = "%m" },
            new Data(5, 6, 7, 8) { Culture = Cultures.EnUs, Text = "7", Pattern = "%s" },

            // The subminute values are truncated for the short pattern
            new Data(14, 15, 16) { Culture = Cultures.DotTimeSeparator, Text = "14.15", Pattern = "t" },
            new Data(14, 15, 16) { Culture = Cultures.Invariant, Text = "14:15", Pattern = "t" },
        };

        /// <summary>
        /// Common test data for both formatting and parsing. A test should be placed here unless is truly
        /// cannot be run both ways. This ensures that as many round-trip type tests are performed as possible.
        /// </summary>
        internal static readonly Data[] FormatAndParseData = {
            new Data(LocalTime.Midnight) { Culture = Cultures.EnUs, Text = ".", Pattern = "%." },
            new Data(LocalTime.Midnight) { Culture = Cultures.EnUs, Text = ":", Pattern = "%:" },
            new Data(LocalTime.Midnight) { Culture = Cultures.DotTimeSeparator, Text = ".", Pattern = "%." },
            new Data(LocalTime.Midnight) { Culture = Cultures.DotTimeSeparator, Text = ".", Pattern = "%:" },
            new Data(LocalTime.Midnight) { Culture = Cultures.EnUs, Text = "H", Pattern = "\\H" },
            new Data(LocalTime.Midnight) { Culture = Cultures.EnUs, Text = "HHss", Pattern = "'HHss'" },
            new Data(0, 0, 0, 100) { Culture = Cultures.EnUs, Text = "1", Pattern = "%f" },
            new Data(0, 0, 0, 100) { Culture = Cultures.EnUs, Text = "1", Pattern = "%F" },
            new Data(0, 0, 0, 100) { Culture = Cultures.EnUs, Text = "1", Pattern = "FF" },
            new Data(0, 0, 0, 100) { Culture = Cultures.EnUs, Text = "1", Pattern = "FFF" },
            new Data(0, 0, 0, 100) { Culture = Cultures.EnUs, Text = "100000000", Pattern = "fffffffff" },
            new Data(0, 0, 0, 100) { Culture = Cultures.EnUs, Text = "1", Pattern = "FFFFFFFFF" },
            new Data(0, 0, 0, 120) { Culture = Cultures.EnUs, Text = "12", Pattern = "ff" },
            new Data(0, 0, 0, 120) { Culture = Cultures.EnUs, Text = "12", Pattern = "FF" },
            new Data(0, 0, 0, 120) { Culture = Cultures.EnUs, Text = "12", Pattern = "FFF" },
            new Data(0, 0, 0, 123) { Culture = Cultures.EnUs, Text = "123", Pattern = "fff" },
            new Data(0, 0, 0, 123) { Culture = Cultures.EnUs, Text = "123", Pattern = "FFF" },
            new Data(0, 0, 0, 123, 4000) { Culture = Cultures.EnUs, Text = "1234", Pattern = "ffff" },
            new Data(0, 0, 0, 123, 4000) { Culture = Cultures.EnUs, Text = "1234", Pattern = "FFFF" },
            new Data(0, 0, 0, 123, 4500) { Culture = Cultures.EnUs, Text = "12345", Pattern = "fffff" },
            new Data(0, 0, 0, 123, 4500) { Culture = Cultures.EnUs, Text = "12345", Pattern = "FFFFF" },
            new Data(0, 0, 0, 123, 4560) { Culture = Cultures.EnUs, Text = "123456", Pattern = "ffffff" },
            new Data(0, 0, 0, 123, 4560) { Culture = Cultures.EnUs, Text = "123456", Pattern = "FFFFFF" },
            new Data(0, 0, 0, 123, 4567) { Culture = Cultures.EnUs, Text = "1234567", Pattern = "fffffff" },
            new Data(0, 0, 0, 123, 4567) { Culture = Cultures.EnUs, Text = "1234567", Pattern = "FFFFFFF" },
            new Data(0, 0, 0, 123456780L) { Culture = Cultures.EnUs, Text = "12345678", Pattern = "ffffffff" },
            new Data(0, 0, 0, 123456780L) { Culture = Cultures.EnUs, Text = "12345678", Pattern = "FFFFFFFF" },
            new Data(0, 0, 0, 123456789L) { Culture = Cultures.EnUs, Text = "123456789", Pattern = "fffffffff" },
            new Data(0, 0, 0, 123456789L) { Culture = Cultures.EnUs, Text = "123456789", Pattern = "FFFFFFFFF" },
            new Data(0, 0, 0, 600) { Culture = Cultures.EnUs, Text = ".6", Pattern = ".f" },
            new Data(0, 0, 0, 600) { Culture = Cultures.EnUs, Text = ".6", Pattern = ".F" },
            new Data(0, 0, 0, 600) { Culture = Cultures.EnUs, Text = ".6", Pattern = ".FFF" }, // Elided fraction
            new Data(0, 0, 0, 678) { Culture = Cultures.EnUs, Text = ".678", Pattern = ".fff" },
            new Data(0, 0, 0, 678) { Culture = Cultures.EnUs, Text = ".678", Pattern = ".FFF" },
            new Data(0, 0, 12, 0) { Culture = Cultures.EnUs, Text = "12", Pattern = "%s" },
            new Data(0, 0, 12, 0) { Culture = Cultures.EnUs, Text = "12", Pattern = "ss" },
            new Data(0, 0, 2, 0) { Culture = Cultures.EnUs, Text = "2", Pattern = "%s" },
            new Data(0, 12, 0, 0) { Culture = Cultures.EnUs, Text = "12", Pattern = "%m" },
            new Data(0, 12, 0, 0) { Culture = Cultures.EnUs, Text = "12", Pattern = "mm" },
            new Data(0, 2, 0, 0) { Culture = Cultures.EnUs, Text = "2", Pattern = "%m" },
            new Data(1, 0, 0, 0) { Culture = Cultures.EnUs, Text = "1", Pattern = "H.FFF" }, // Missing fraction
            new Data(12, 0, 0, 0) { Culture = Cultures.EnUs, Text = "12", Pattern = "%H" },
            new Data(12, 0, 0, 0) { Culture = Cultures.EnUs, Text = "12", Pattern = "HH" },
            new Data(2, 0, 0, 0) { Culture = Cultures.EnUs, Text = "2", Pattern = "%H" },
            new Data(0, 0, 12, 340) { Culture = Cultures.EnUs, Text = "12.34", Pattern = "ss.FFF"  },

            // Standard patterns
            new Data(14, 15, 16) { Culture = Cultures.EnUs, Text = "14:15:16", Pattern = "r" },
            new Data(14, 15, 16, 700) { Culture = Cultures.EnUs, Text = "14:15:16.7", Pattern = "r" },
            new Data(14, 15, 16, 780) { Culture = Cultures.EnUs, Text = "14:15:16.78", Pattern = "r" },
            new Data(14, 15, 16, 789) { Culture = Cultures.EnUs, Text = "14:15:16.789", Pattern = "r" },
            new Data(14, 15, 16, 789, 1000) { Culture = Cultures.EnUs, Text = "14:15:16.7891", Pattern = "r" },
            new Data(14, 15, 16, 789, 1200) { Culture = Cultures.EnUs, Text = "14:15:16.78912", Pattern = "r" },
            new Data(14, 15, 16, 789, 1230) { Culture = Cultures.EnUs, Text = "14:15:16.789123", Pattern = "r" },
            new Data(14, 15, 16, 789, 1234) { Culture = Cultures.EnUs, Text = "14:15:16.7891234", Pattern = "r" },
            new Data(14, 15, 16, 700) { Culture = Cultures.DotTimeSeparator, Text = "14.15.16.7", Pattern = "r" },
            new Data(14, 15, 16, 780) { Culture = Cultures.DotTimeSeparator, Text = "14.15.16.78", Pattern = "r" },
            new Data(14, 15, 16, 789) { Culture = Cultures.DotTimeSeparator, Text = "14.15.16.789", Pattern = "r" },
            new Data(14, 15, 16, 789, 1000) { Culture = Cultures.DotTimeSeparator, Text = "14.15.16.7891", Pattern = "r" },
            new Data(14, 15, 16, 789, 1200) { Culture = Cultures.DotTimeSeparator, Text = "14.15.16.78912", Pattern = "r" },
            new Data(14, 15, 16, 789, 1230) { Culture = Cultures.DotTimeSeparator, Text = "14.15.16.789123", Pattern = "r" },
            new Data(14, 15, 16, 789, 1234) { Culture = Cultures.DotTimeSeparator, Text = "14.15.16.7891234", Pattern = "r" },
            new Data(14, 15, 16, 789123456L) { Culture = Cultures.DotTimeSeparator, Text = "14.15.16.789123456", Pattern = "r" },

            new Data(14, 15, 0) { Culture = Cultures.DotTimeSeparator, Text = "14.15", Pattern = "t" },
            new Data(14, 15, 0) { Culture = Cultures.Invariant, Text = "14:15", Pattern = "t" },

            new Data(14, 15, 16) { Culture = Cultures.DotTimeSeparator, Text = "14.15.16", Pattern = "T" },
            new Data(14, 15, 16) { Culture = Cultures.Invariant, Text = "14:15:16", Pattern = "T" },
            new Data(5, 0, 0, 0) { Text = "05:00:00", Pattern = "T"},
            new Data(5, 12, 0, 0) { Text = "05:12:00", Pattern = "T" },
            new Data(5, 12, 34, 0) { Text = "05:12:34", Pattern = "T" },
            new Data(17, 0, 0, 0) { Culture = Cultures.EnUs, Text = "5:00:00 PM", Pattern = "T" },
            new Data(5, 0, 0, 0) { Culture = Cultures.EnUs, Text = "5:00:00 AM", Pattern = "T" },
            new Data(5, 12, 0, 0) { Culture = Cultures.EnUs, Text = "5:12:00 AM", Pattern = "T" },
            new Data(5, 12, 34, 0) { Culture = Cultures.EnUs, Text = "5:12:34 AM", Pattern = "T" },

            new Data(14, 15, 16, 789) { StandardPattern = LocalTimePattern.ExtendedIso, Culture = Cultures.DotTimeSeparator, Text = "14:15:16.789", Pattern = "o" },
            new Data(14, 15, 16, 789) { StandardPattern = LocalTimePattern.ExtendedIso, Culture = Cultures.EnUs, Text = "14:15:16.789", Pattern = "o" },
            new Data(14, 15, 16) { StandardPattern = LocalTimePattern.ExtendedIso, Culture = Cultures.Invariant, Text = "14:15:16", Pattern = "o" },

            new Data(14, 15, 16, 789) { StandardPattern = LocalTimePattern.LongExtendedIso, Culture = Cultures.DotTimeSeparator, Text = "14:15:16.789000000", Pattern = "O" },
            new Data(14, 15, 16, 789) { StandardPattern = LocalTimePattern.LongExtendedIso, Culture = Cultures.EnUs, Text = "14:15:16.789000000", Pattern = "O" },
            new Data(14, 15, 16) { StandardPattern = LocalTimePattern.LongExtendedIso, Culture = Cultures.Invariant, Text = "14:15:16.000000000", Pattern = "O" },
            new Data(14, 15, 16) { StandardPattern = LocalTimePattern.GeneralIso, Culture = Cultures.Invariant, Text = "14:15:16", Pattern = "HH:mm:ss" },

            // ------------ Template value tests ----------
            // Mixtures of 12 and 24 hour times
            new Data(18, 0, 0) { Culture = Cultures.EnUs, Text = "18 6 PM", Pattern = "HH h tt" },
            new Data(18, 0, 0) { Culture = Cultures.EnUs, Text = "18 6", Pattern = "HH h" },
            new Data(18, 0, 0) { Culture = Cultures.EnUs, Text = "18 PM", Pattern = "HH tt" },
            new Data(18, 0, 0) { Culture = Cultures.EnUs, Text = "6 PM", Pattern = "h tt" },
            new Data(6, 0, 0) { Culture = Cultures.EnUs, Text = "6", Pattern = "%h" },
            new Data(0, 0, 0) { Culture = Cultures.EnUs, Text = "AM", Pattern = "tt" },
            new Data(12, 0, 0) { Culture = Cultures.EnUs, Text = "PM", Pattern = "tt" },
            new Data(0, 0, 0) { Culture = Cultures.EnUs, Text = "A", Pattern = "%t" },
            new Data(12, 0, 0) { Culture = Cultures.EnUs, Text = "P", Pattern = "%t" },

            // Pattern specifies nothing - template value is passed through
            new Data(LocalTime.FromHourMinuteSecondMillisecondTick(1, 2, 3, 4, 5)) { Culture = Cultures.EnUs, Text = "*", Pattern = "%*", Template = LocalTime.FromHourMinuteSecondMillisecondTick(1, 2, 3, 4, 5) },
            // Tests for each individual field being propagated
            new Data(LocalTime.FromHourMinuteSecondMillisecondTick(1, 6, 7, 8, 9)) { Culture = Cultures.EnUs, Text = "06:07.0080009", Pattern = "mm:ss.FFFFFFF", Template = LocalTime.FromHourMinuteSecondMillisecondTick(1, 2, 3, 4, 5) },
            new Data(LocalTime.FromHourMinuteSecondMillisecondTick(6, 2, 7, 8, 9)) { Culture = Cultures.EnUs, Text = "06:07.0080009", Pattern = "HH:ss.FFFFFFF", Template = LocalTime.FromHourMinuteSecondMillisecondTick(1, 2, 3, 4, 5) },
            new Data(LocalTime.FromHourMinuteSecondMillisecondTick(6, 7, 3, 8, 9)) { Culture = Cultures.EnUs, Text = "06:07.0080009", Pattern = "HH:mm.FFFFFFF", Template = LocalTime.FromHourMinuteSecondMillisecondTick(1, 2, 3, 4, 5) },
            new Data(LocalTime.FromHourMinuteSecondMillisecondTick(6, 7, 8, 4, 5)) { Culture = Cultures.EnUs, Text = "06:07:08", Pattern = "HH:mm:ss", Template = LocalTime.FromHourMinuteSecondMillisecondTick(1, 2, 3, 4, 5) },

            // Hours are tricky because of the ways they can be specified
            new Data(new LocalTime(6, 2, 3)) { Culture = Cultures.EnUs, Text = "6", Pattern = "%h", Template = new LocalTime(1, 2, 3) },
            new Data(new LocalTime(18, 2, 3)) { Culture = Cultures.EnUs, Text = "6", Pattern = "%h", Template = new LocalTime(14, 2, 3) },
            new Data(new LocalTime(2, 2, 3)) { Culture = Cultures.EnUs, Text = "AM", Pattern = "tt", Template = new LocalTime(14, 2, 3) },
            new Data(new LocalTime(14, 2, 3)) { Culture = Cultures.EnUs, Text = "PM", Pattern = "tt", Template = new LocalTime(14, 2, 3) },
            new Data(new LocalTime(2, 2, 3)) { Culture = Cultures.EnUs, Text = "AM", Pattern = "tt", Template = new LocalTime(2, 2, 3) },
            new Data(new LocalTime(14, 2, 3)) { Culture = Cultures.EnUs, Text = "PM", Pattern = "tt", Template = new LocalTime(2, 2, 3) },
            new Data(new LocalTime(17, 2, 3)) { Culture = Cultures.EnUs, Text = "5 PM", Pattern = "h tt", Template = new LocalTime(1, 2, 3) },
            // --------------- end of template value tests ----------------------

            // Only one of the AM/PM designator is present. We should still be able to work out what is meant, by the presence
            // or absence of the non-empty one.
            new Data(5, 0, 0) { Culture = AmOnlyCulture, Text = "5 am", Pattern = "h tt" },
            new Data(15, 0, 0) { Culture = AmOnlyCulture, Text = "3 ", Pattern = "h tt", Description = "Implicit PM" },
            new Data(5, 0, 0) { Culture = AmOnlyCulture, Text = "5 a", Pattern = "h t" },
            new Data(15, 0, 0) { Culture = AmOnlyCulture, Text = "3 ", Pattern = "h t", Description = "Implicit PM"},

            new Data(5, 0, 0) { Culture = PmOnlyCulture, Text = "5 ", Pattern = "h tt" },
            new Data(15, 0, 0) { Culture = PmOnlyCulture, Text = "3 pm", Pattern = "h tt" },
            new Data(5, 0, 0) { Culture = PmOnlyCulture, Text = "5 ", Pattern = "h t" },
            new Data(15, 0, 0) { Culture = PmOnlyCulture, Text = "3 p", Pattern = "h t" },

            // AM / PM designators are both empty strings. The parsing side relies on the AM/PM value being correct on the
            // template value. (The template value is for the wrong actual hour, but in the right side of noon.)
            new Data(5, 0, 0) { Culture = NoAmOrPmCulture, Text = "5 ", Pattern = "h tt", Template = new LocalTime(2, 0, 0) },
            new Data(15, 0, 0) { Culture = NoAmOrPmCulture, Text = "3 ", Pattern = "h tt", Template = new LocalTime(14, 0, 0) },
            new Data(5, 0, 0) { Culture = NoAmOrPmCulture, Text = "5 ", Pattern = "h t", Template = new LocalTime(2, 0, 0) },
            new Data(15, 0, 0) { Culture = NoAmOrPmCulture, Text = "3 ", Pattern = "h t", Template = new LocalTime(14, 0, 0) },

            // Use of the semi-colon "comma dot" specifier
            new Data(16, 05, 20, 352) { Pattern = "HH:mm:ss;fff", Text = "16:05:20.352" },
            new Data(16, 05, 20, 352) { Pattern = "HH:mm:ss;FFF", Text = "16:05:20.352" },
            new Data(16, 05, 20, 352) { Pattern = "HH:mm:ss;FFF 'end'", Text = "16:05:20.352 end" },
            new Data(16, 05, 20) { Pattern = "HH:mm:ss;FFF 'end'", Text = "16:05:20 end" },

            // Check handling of F after non-period.
            new Data(16, 05, 20, 352) { Pattern = "HH:mm:ss'x'FFF", Text = "16:05:20x352" },
        };

        internal static IEnumerable<Data> ParseData = ParseOnlyData.Concat(FormatAndParseData);
        internal static IEnumerable<Data> FormatData = FormatOnlyData.Concat(FormatAndParseData);

        private static CultureInfo CreateCustomAmPmCulture(string amDesignator, string pmDesignator)
        {
            CultureInfo clone = (CultureInfo)CultureInfo.InvariantCulture.Clone();
            clone.DateTimeFormat.AMDesignator = amDesignator;
            clone.DateTimeFormat.PMDesignator = pmDesignator;
            return CultureInfo.ReadOnly(clone);
        }

        [Test]
        public void ParseNull() => AssertParseNull(LocalTimePattern.ExtendedIso);

        [Test]
        [TestCaseSource(typeof(Cultures), nameof(Cultures.AllCultures))]
        public void BclLongTimePatternIsValidNodaPattern(CultureInfo culture)
        {
            if (culture is null)
            {
                return;
            }
            AssertValidNodaPattern(culture, culture.DateTimeFormat.LongTimePattern);
        }

        [Test]
        [TestCaseSource(typeof(Cultures), nameof(Cultures.AllCultures))]
        public void BclShortTimePatternIsValidNodaPattern(CultureInfo culture)
        {
            AssertValidNodaPattern(culture, culture.DateTimeFormat.ShortTimePattern);
        }

        [Test]
        [TestCaseSource(typeof(Cultures), nameof(Cultures.AllCultures))]
        public void BclLongTimePatternGivesSameResultsInNoda(CultureInfo culture)
        {
            AssertBclNodaEquality(culture, culture.DateTimeFormat.LongTimePattern);
        }

        [Test]
        [TestCaseSource(typeof(Cultures), nameof(Cultures.AllCultures))]
        public void BclShortTimePatternGivesSameResultsInNoda(CultureInfo culture)
        {
            AssertBclNodaEquality(culture, culture.DateTimeFormat.ShortTimePattern);
        }

        [Test]
        public void CreateWithInvariantCulture_NullPatternText()
        {
            Assert.Throws<ArgumentNullException>(() => LocalTimePattern.CreateWithInvariantCulture(null!));
        }

        [Test]
        public void Create_NullFormatInfo()
        {
            Assert.Throws<ArgumentNullException>(() => LocalTimePattern.Create("HH", null!));
        }

        [Test]
        public void TemplateValue_DefaultsToMidnight()
        {
            var pattern = LocalTimePattern.CreateWithInvariantCulture("HH");
            Assert.AreEqual(LocalTime.Midnight, pattern.TemplateValue);
        }

        [Test]
        public void CreateWithCurrentCulture()
        {
            using (CultureSaver.SetCultures(Cultures.DotTimeSeparator))
            {
                var pattern = LocalTimePattern.CreateWithCurrentCulture("HH:mm");
                var text = pattern.Format(new LocalTime(13, 45));
                Assert.AreEqual("13.45", text);
            }
        }

        [Test]
        public void WithTemplateValue_PropertyFetch()
        {
            LocalTime newValue = new LocalTime(1, 23, 45);
            var pattern = LocalTimePattern.CreateWithInvariantCulture("HH").WithTemplateValue(newValue);
            Assert.AreEqual(newValue, pattern.TemplateValue);
        }

        [Test]
        [TestCase("00")]
        [TestCase("23")]
        [TestCase("05")]
        public void HourIso_Roundtrip(string text)
        {
            var result = LocalTimePattern.HourIso.Parse(text);
            Assert.True(result.Success);
            var time = result.Value;
            Assert.AreEqual(0, time.Minute);
            Assert.AreEqual(0, time.Second);
            Assert.AreEqual(0, time.NanosecondOfSecond);
            var formatted = LocalTimePattern.HourIso.Format(time);
            Assert.AreEqual(text, formatted);
        }

        [Test]
        [TestCase("-05")]
        [TestCase("05:00")]
        [TestCase("5")]
        [TestCase("24")]
        [TestCase("99")]
        public void HourIso_Invalid(string text)
        {
            var result = LocalTimePattern.HourIso.Parse(text);
            Assert.False(result.Success);
        }

        [Test]
        [TestCase("00:31")]
        [TestCase("23:10")]
        public void HourMinuteIso_Roundtrip(string text)
        {
            var result = LocalTimePattern.HourMinuteIso.Parse(text);
            Assert.True(result.Success);
            var time = result.Value;
            Assert.AreEqual(0, time.Second);
            Assert.AreEqual(0, time.NanosecondOfSecond);
            var formatted = LocalTimePattern.HourMinuteIso.Format(time);
            Assert.AreEqual(text, formatted);
        }

        [Test]
        [TestCase("-05:00")]
        [TestCase("5:00")]
        [TestCase("24:00")]
        [TestCase("99:00")]
        [TestCase("10:60")]
        [TestCase("10:70")]
        public void HourMinuteIso_Invalid(string text)
        {
            var result = LocalTimePattern.HourMinuteIso.Parse(text);
            Assert.False(result.Success);
        }

        [Test]
        [TestCase("03", "03:00", "03:00:00")]
        [TestCase("12", "12:00", "12:00:00", "12:00:00.000000", "12:00:00.000000000")]
        [TestCase("12:01", "12:01:00", "12:01:00.000000")]
        [TestCase("12:00:01", "12:00:01.000000")]
        [TestCase("12:00:01.123", "12:00:01.123000", "12:00:01.123000000")]
        [TestCase("12:00:01.123456789")]
        public void VariablePrecision_Valid(string canonical, params string[] alternatives)
        {
            var pattern = LocalTimePattern.VariablePrecisionIso;
            foreach (var text in new[] { canonical }.Concat(alternatives))
            {
                var result = pattern.Parse(text);
                Assert.True(result.Success);
                var time = result.Value;
                var formatted = pattern.Format(time);
                Assert.AreEqual(canonical, formatted);
            }
        }

        [Test]
        [TestCase("24:00")]
        [TestCase("24")]
        [TestCase("25")]
        [TestCase("25:61")]
        [TestCase("12:23:45.0000000000")] // Too many fractional digits
        [TestCase("05:63")]
        [TestCase("05:00:63")]
        public void VariablePrecision_Invalid(string text)
        {
            var result = LocalTimePattern.VariablePrecisionIso.Parse(text);
            Assert.False(result.Success);
        }

        private void AssertBclNodaEquality(CultureInfo culture, string patternText)
        {
            // On Mono, some general patterns include an offset at the end.
            // https://github.com/nodatime/nodatime/issues/98
            // For the moment, ignore them.
            // TODO(V1.2): Work out what to do in such cases...
            if (patternText.EndsWith("z"))
            {
                return;
            }
            var pattern = LocalTimePattern.Create(patternText, culture);

            Assert.AreEqual(SampleDateTime.ToString(patternText, culture), pattern.Format(SampleLocalTime));
        }

        private static void AssertValidNodaPattern(CultureInfo culture, string pattern)
        {
            PatternCursor cursor = new PatternCursor(pattern);
            while (cursor.MoveNext())
            {
                if (cursor.Current == '\'')
                {
                    cursor.GetQuotedString('\'');
                }
                else
                {
                    // We'll never do anything "special" with non-ascii characters anyway,
                    // so we don't mind if they're not quoted.
                    if (cursor.Current < '\u0080')
                    {
                        Assert.IsTrue(ExpectedCharacters.Contains(cursor.Current),
                            $"Pattern '{pattern}' contains unquoted, unexpected characters");
                    }
                }
            }
            // Check that the pattern parses
            LocalTimePattern.Create(pattern, culture);
        }

        /// <summary>
        /// A container for test data for formatting and parsing <see cref="LocalTime" /> objects.
        /// </summary>
        public sealed class Data : PatternTestData<LocalTime>
        {
            // Default to midnight
            protected override LocalTime DefaultTemplate => LocalTime.Midnight;

            protected override string? ValuePatternText => "HH:mm:ss.FFFFFFFFF";

            public Data(LocalTime value) : base(value)
            {
            }

            public Data(int hours, int minutes, int seconds)
                : this(new LocalTime(hours, minutes, seconds))
            {
            }

            public Data(int hours, int minutes, int seconds, int milliseconds)
                : this(new LocalTime(hours, minutes, seconds, milliseconds))
            {
            }

            public Data(int hours, int minutes, int seconds, int milliseconds, int ticksWithinMillisecond)
                : this(LocalTime.FromHourMinuteSecondMillisecondTick(hours, minutes, seconds, milliseconds, ticksWithinMillisecond))
            {
            }

            public Data(int hours, int minutes, int seconds, long nanoOfSecond)
                : this(new LocalTime(hours, minutes, seconds).PlusNanoseconds(nanoOfSecond))
            {
            }

            public Data() : this(LocalTime.Midnight)
            {
            }

            internal override IPattern<LocalTime> CreatePattern() =>
                LocalTimePattern.CreateWithInvariantCulture(Pattern!)
                    .WithTemplateValue(Template)
                    .WithCulture(Culture);
        }
    }
}
