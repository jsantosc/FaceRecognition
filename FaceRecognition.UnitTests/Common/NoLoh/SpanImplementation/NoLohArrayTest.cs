using System;
using System.Collections.Generic;
using FaceRecognition.Common.NoLoh.SpanImplementation;
using FluentAssertions;
using Xunit;

namespace FaceRecognition.UnitTests.Common.NoLoh.SpanImplementation
{
    public class NoLohArrayTest
    {
        [Theory]
        [InlineData(100, 10)]
        [InlineData(1000, 50)]
        public void GivenIHaveTheLengthOfABigArray_WhenICreateABigArrayTest_AnInstanceIsCreated(int length, int filledIndex)
        {
            var random = new Random();
            var filledValues = new Dictionary<int, double>();

            for (int i = 0; i < filledIndex; i++)
            {
                filledValues[random.Next(length - 1)] = random.NextDouble();
            }

            using (var array = new NoLohArray<double>(length, true))
            {
                foreach (var item in filledValues)
                {
                    array[item.Key] = item.Value;
                }

                foreach (var item in filledValues)
                {
                    array[item.Key].Should().Be(item.Value);
                }
            }
        }

        [Fact]
        public void GivenIWantToCreateABigArray_WhenIPassALengthOfZero_AnExceptionIsThrow()
        {
            Action action = () => new NoLohArray<double>(0);

            action.ShouldThrow<ArgumentException>();
        }

        [Fact]
        public void GivenIHaveABigArray_WhenISetANegativeIndex_AnExceptionIsThrow()
        {
            Action action = () =>
            {
                using (var noLohArray = new NoLohArray<double>(10))
                {
                    noLohArray[-1] = 10;
                }
            };

            action.ShouldThrow<ArgumentException>();
        }

        [Fact]
        public void GivenIHaveABigArray_WhenIGetANegativeIndex_AnExceptionIsThrow()
        {
            Action action = () =>
            {
                using (var noLohArray = new NoLohArray<double>(10))
                {
                    var value = noLohArray[-1];
                }
            };

            action.ShouldThrow<ArgumentException>();
        }

        [Fact]
        public void GivenIHaveABigArray_WhenISetAnIndexGreaterThanLength_AnExceptionIsThrow()
        {
            Action action = () =>
            {
                using (var noLohArray = new NoLohArray<double>(10))
                {
                    noLohArray[10] = 10;
                }
            };

            action.ShouldThrow<ArgumentException>();
        }

        [Fact]
        public void GivenIHaveABigArray_WhenIGetAnIndexGreaterThanLength_AnExceptionIsThrow()
        {
            Action action = () =>
            {
                using (var noLohArray = new NoLohArray<double>(10))
                {
                    var value = noLohArray[10];
                }
            };

            action.ShouldThrow<ArgumentException>();
        }

    }
}