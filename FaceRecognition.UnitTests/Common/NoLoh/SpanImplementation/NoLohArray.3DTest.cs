using System;
using System.Collections.Generic;
using FaceRecognition.Common.NoLoh.SpanImplementation;
using FluentAssertions;
using Xunit;

namespace FaceRecognition.UnitTests.Common.NoLoh.SpanImplementation
{
    public class NoLohArray3DTest
    {
        [Theory]
        [InlineData(10, 20, 30, 10)]
        [InlineData(100, 200, 300, 50)]
        public void GivenIHaveTheLengthOfABigArray_WhenICreateABigArrayTest_AnInstanceIsCreated(int xLength, int yLength, int zLength,
            int filledIndex)
        {
            var random = new Random();
            var filledValues = new Dictionary<string, GeneratedValues>();
            for (int i = 0; i < filledIndex; i++)
            {
                var x = random.Next(xLength - 1);
                var y = random.Next(yLength - 1);
                var z = random.Next(zLength - 1);
                filledValues[$"{x},{y},{z}"] = new GeneratedValues(x, y, z, random.NextDouble());
            }

            using (var array = new NoLohArray3D<double>(xLength, yLength, zLength, true))
            {
                foreach (var item in filledValues)
                {
                    array[item.Value.X, item.Value.Y, item.Value.Z] = item.Value.Value;
                }

                foreach (var item in filledValues)
                {
                    array[item.Value.X, item.Value.Y, item.Value.Z].Should().Be(item.Value.Value);
                }
            }
        }

        [Theory]
        [InlineData(0, 10, 10)]
        [InlineData(10, 0, 10)]
        [InlineData(10, 10, 0)]
        public void GivenIWantToCreateABigArray_WhenIPassALengthOfZero_AnExceptionIsThrow(int xIndex, int yIndex, int zIndex)
        {
            Action action = () => new NoLohArray3D<double>(xIndex, yIndex, zIndex);

            action.ShouldThrow<ArgumentException>();
        }

        [Theory]
        [InlineData(10, 20, 30, -1, 0, 0)]
        [InlineData(10, 20, 30, 0, -1, 0)]
        [InlineData(10, 20, 30, 0, 0, -1)]
        public void GivenIHaveABigArray_WhenISetANegativeIndex_AnExceptionIsThrow(int xLength, int yLength, int zLength,
            int xIndex, int yIndex, int zIndex)
        {
            Action action = () =>
            {
                using (var noLohArray = new NoLohArray3D<double>(xLength, yLength, zLength))
                {
                    noLohArray[xIndex, yIndex, zIndex] = 10;
                }
            };

            action.ShouldThrow<ArgumentException>();
        }

        [Theory]
        [InlineData(10, 20, 30, -1, 0, 0)]
        [InlineData(10, 20, 30, 0, -1, 0)]
        [InlineData(10, 20, 30, 0, 0, -1)]
        public void GivenIHaveABigArray_WhenIGetANegativeIndex_AnExceptionIsThrow(int xLength, int yLength, int zLength,
            int xIndex, int yIndex, int zIndex)
        {
            Action action = () =>
            {
                using (var noLohArray = new NoLohArray3D<double>(xLength, yLength, zLength))
                {
                    var value = noLohArray[xIndex, yIndex, zIndex];
                }
            };

            action.ShouldThrow<ArgumentException>();
        }

        [Theory]
        [InlineData(10, 20, 30, 0, 20, 30)]
        [InlineData(10, 20, 30, 10, 0, 30)]
        [InlineData(10, 20, 30, 10, 20, 0)]
        public void GivenIHaveABigArray_WhenISetAnIndexGreaterThanLength_AnExceptionIsThrow(int xLength, int yLength, int zLength,
            int xIndex, int yIndex, int zIndex)
        {
            Action action = () =>
            {
                using (var noLohArray = new NoLohArray3D<double>(xLength, yLength, zLength))
                {
                    noLohArray[xIndex, yIndex, zIndex] = 10;
                }
            };

            action.ShouldThrow<ArgumentException>();
        }

        [Theory]
        [InlineData(10, 20, 30, 0, 20, 30)]
        [InlineData(10, 20, 30, 10, 0, 30)]
        [InlineData(10, 20, 30, 10, 20, 0)]
        public void GivenIHaveABigArray_WhenIGetAnIndexGreaterThanLength_AnExceptionIsThrow(int xLength, int yLength, int zLength,
            int xIndex, int yIndex, int zIndex)
        {
            Action action = () =>
            {
                using (var noLohArray = new NoLohArray3D<double>(xLength, yLength, zLength))
                {
                    var value = noLohArray[xIndex, yIndex, zIndex];
                }
            };

            action.ShouldThrow<ArgumentException>();
        }

        internal struct GeneratedValues
        {
            public int X { get; }
            public int Y { get; }
            public int Z { get; }
            public double Value { get; }

            public GeneratedValues(int x, int y, int z, double value)
            {
                X = x;
                Y = y;
                Z = z;
                Value = value;
            }
        }
    }
}