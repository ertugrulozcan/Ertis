using System;
using NUnit.Framework;

namespace Ertis.Tests.Ertis.Schema.Tests
{
    public class FieldInfoTests
    {
        #region Methods

        [Test]
        public void FloatFieldInfo()
        {
            var probabilities = new int?[]
            {
                1,
                5,
                10,
                15,
                null
            };

            var i = 1;
            foreach (var minimum in probabilities)
            {
                foreach (var exclusiveMinimum in probabilities)
                {
                    foreach (var maximum in probabilities)
                    {
                        foreach (var exclusiveMaximum in probabilities)
                        {
                            var minimumValue = minimum != null ? $"{minimum:D4}" : "null";
                            var exclusiveMinimumValue = exclusiveMinimum != null ? $"{exclusiveMinimum:D4}" : "null";
                            var maximumValue = maximum != null ? $"{maximum:D4}" : "null";
                            var exclusiveMaximumValue = exclusiveMaximum != null ? $"{exclusiveMaximum:D4}" : "null";
                            
                            Console.WriteLine($"[{i++:D5}] {minimumValue} {exclusiveMinimumValue} {maximumValue} {exclusiveMaximumValue}");
                        }
                    }
                }   
            }
        }

        #endregion
    }
}