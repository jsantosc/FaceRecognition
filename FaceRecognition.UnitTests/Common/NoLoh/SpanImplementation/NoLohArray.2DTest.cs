using System;
using System.Collections.Generic;
using FaceRecognition.Common.NoLoh.SpanImplementation;
using FluentAssertions;
using Xunit;

namespace FaceRecognition.UnitTests.Common.NoLoh.SpanImplementation
{
    public class NoLohArray2DTest
    {
        [Theory]
        [InlineData(100, 100, 10)]
        [InlineData(1000, 1000, 50)]
        public void GivenIHaveTheLengthOfABigArray_WhenICreateABigArrayTest_AnInstanceIsCreated(int xLength, int yLength, int filledIndex)
        {
            var random = new Random();
            var filledValues = new Dictionary<string, GeneratedValues>();
            for (int i = 0; i < filledIndex; i++)
            {
                var x = random.Next(xLength - 1);
                var y = random.Next(yLength - 1);
                filledValues[$"{x},{y}"] = new GeneratedValues(x, y, random.NextDouble());
            }

            using (var array = new NoLohArray2D<double>(xLength, yLength, true))
            {
                array.Strides.StrideX.Should().Be(1);
                array.Strides.StrideY.Should().Be(xLength);
                array.XLength.Should().Be(xLength);
                array.YLength.Should().Be(yLength);
                foreach (var item in filledValues)
                {
                    array[item.Value.X, item.Value.Y] = item.Value.Value;
                }

                foreach (var item in filledValues)
                {
                    array[item.Value.X, item.Value.Y].Should().Be(item.Value.Value);
                }
            }
        }

        [Theory]
        [InlineData(0, 10)]
        [InlineData(10, 0)]
        public void GivenIWantToCreateABigArray_WhenIPassALengthOfZero_AnExceptionIsThrow(int xIndex, int yIndex)
        {
            Action action = () => new NoLohArray2D<double>(xIndex, yIndex);

            action.ShouldThrow<ArgumentException>();
        }

        [Theory]
        [InlineData(10, 20, -1, 0)]
        [InlineData(10, 20, 0, -1)]
        public void GivenIHaveABigArray_WhenISetANegativeIndex_AnExceptionIsThrow(int xLength, int yLength, int xIndex, int yIndex)
        {
            Action action = () =>
            {
                using (var noLohArray = new NoLohArray2D<double>(xLength, yLength))
                {
                    noLohArray[xIndex, yIndex] = 10;
                }
            };

            action.ShouldThrow<ArgumentException>();
        }

        [Theory]
        [InlineData(10, 20, -1, 0)]
        [InlineData(10, 20, 0, -1)]
        public void GivenIHaveABigArray_WhenIGetANegativeIndex_AnExceptionIsThrow(int xLength, int yLength, int xIndex, int yIndex)
        {
            Action action = () =>
            {
                using (var noLohArray = new NoLohArray2D<double>(xLength, yLength))
                {
                    var value = noLohArray[xIndex, yIndex];
                }
            };

            action.ShouldThrow<ArgumentException>();
        }

        [Theory]
        [InlineData(10, 20, 10, 0)]
        [InlineData(10, 20, 0, 20)]
        public void GivenIHaveABigArray_WhenISetAnIndexGreaterThanLength_AnExceptionIsThrow(int xLength, int yLength, int xIndex, int yIndex)
        {
            Action action = () =>
            {
                using (var noLohArray = new NoLohArray2D<double>(xLength, yLength))
                {
                    noLohArray[xIndex, yIndex] = 10;
                }
            };

            action.ShouldThrow<ArgumentException>();
        }

        [Theory]
        [InlineData(10, 20, 10, 0)]
        [InlineData(10, 20, 0, 20)]
        public void GivenIHaveABigArray_WhenIGetAnIndexGreaterThanLength_AnExceptionIsThrow(int xLength, int yLength, int xIndex, int yIndex)
        {
            Action action = () =>
            {
                using (var noLohArray = new NoLohArray2D<double>(xLength, yLength))
                {
                    var value = noLohArray[xIndex, yIndex];
                }
            };

            action.ShouldThrow<ArgumentException>();
        }

        [Fact]
        public void GivenIHaveANoLohArray_WhenITransposeAxes_ThenStridesAreChangedAndValuesAreSwapped()
        {
            using (var noLohArray = new NoLohArray2D<double>(2, 3))
            {
                noLohArray[0, 0] = 10;
                noLohArray[0, 1] = 20;
                noLohArray[0, 2] = 30;
                noLohArray[1, 0] = 40;
                noLohArray[1, 1] = 50;
                noLohArray[1, 2] = 60;

                noLohArray.Transpose();

                noLohArray.Strides.StrideX.Should().Be(2);
                noLohArray.Strides.StrideY.Should().Be(1);
                noLohArray.XLength.Should().Be(3);
                noLohArray.YLength.Should().Be(2);

                noLohArray[0, 0].Should().Be(10);
                noLohArray[0, 1].Should().Be(40);
                noLohArray[1, 0].Should().Be(20);
                noLohArray[1, 1].Should().Be(50);
                noLohArray[2, 0].Should().Be(30);
                noLohArray[2, 1].Should().Be(60);
            }
        }

        internal struct GeneratedValues
        {
            public int X { get; }
            public int Y { get; }
            public double Value { get; }

            public GeneratedValues(int x, int y, double value)
            {
                X = x;
                Y = y;
                Value = value;
            }
        }
    }
}