using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using Cake.Core;
using Cake.Core.Diagnostics;

namespace Cake.Bridge.DependencyInjection
{
    public sealed class VerbosityConverter : TypeConverter
    {
        private static readonly Dictionary<string, Verbosity> Lookup = new Dictionary<string, Verbosity>(StringComparer.OrdinalIgnoreCase)
        {
            { "q", Verbosity.Quiet },
            { "quiet", Verbosity.Quiet },
            { "m", Verbosity.Minimal },
            { "minimal", Verbosity.Minimal },
            { "n", Verbosity.Normal },
            { "normal", Verbosity.Normal },
            { "v", Verbosity.Verbose },
            { "verbose", Verbosity.Verbose },
            { "d", Verbosity.Diagnostic },
            { "diagnostic", Verbosity.Diagnostic }
        };


        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (!(value is string stringValue))
            {
                throw new NotSupportedException("Can't convert value to verbosity.");
            }

            var result = Lookup.TryGetValue(stringValue, out var verbosity);
            if (result)
            {
                return verbosity;
            }

            const string format = "The value '{0}' is not a valid verbosity.";
            var message = string.Format(CultureInfo.InvariantCulture, format, value);
            throw new CakeException(message);
        }
    }
}
