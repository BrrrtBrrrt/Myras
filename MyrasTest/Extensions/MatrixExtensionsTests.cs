using Myras.Extensions;
using Myras.Types;

namespace MyrasTest.Extensions
{
    [TestClass]
    public class MatrixExtensionsTests
    {
        [TestMethod]
        public void GetMultidimensionalIndex_Positive1()
        {
            Matrix x = new(new float[3, 2, 4]
            {
                { { 1, 2, 3, 4 }, { 5, 6, 7, 8 } },
                { { 9, 10, 11, 12 }, { 13, 14, 15, 16 } },
                { { 17, 18, 19, 20 }, { 21, 22, 23, 24 } }
            });

            int[] multiDimensionalIndex = x.GetMultiDimensionalIndex(13);
            int[] multiDimensionalIndexExpected = [1, 1, 1];
            CollectionAssert.AreEqual(multiDimensionalIndexExpected, multiDimensionalIndex);
        }

        [TestMethod]
        public void GetMultidimensionalIndex_Positive2()
        {
            Matrix x = new(new float[3, 2, 4]
            {
                { { 1, 2, 3, 4 }, { 5, 6, 7, 8 } },
                { { 9, 10, 11, 12 }, { 13, 14, 15, 16 } },
                { { 17, 18, 19, 20 }, { 21, 22, 23, 24 } }
            });

            int[] multiDimensionalIndex = x.GetMultiDimensionalIndex(20);
            int[] multiDimensionalIndexExpected = [2, 1, 0];
            CollectionAssert.AreEqual(multiDimensionalIndexExpected, multiDimensionalIndex);
        }

        [TestMethod]
        public void GetMultidimensionalIndex_Positive3()
        {
            Matrix x = new(new float[3, 2, 4]
            {
                { { 1, 2, 3, 4 }, { 5, 6, 7, 8 } },
                { { 9, 10, 11, 12 }, { 13, 14, 15, 16 } },
                { { 17, 18, 19, 20 }, { 21, 22, 23, 24 } }
            });

            int[] multiDimensionalIndex = x.GetMultiDimensionalIndex(23);
            int[] multiDimensionalIndexExpected = [2, 1, 3];
            CollectionAssert.AreEqual(multiDimensionalIndexExpected, multiDimensionalIndex);
        }

        [TestMethod]
        public void ReduceTest0()
        {
            Matrix matrix = new(new float[3, 4, 3]
            {
                {{1, 2, 3}, {1, 2, 3}, {1, 2, 3}, {1, 2, 3}},
                {{4, 5, 6}, {4, 5, 6}, {4, 5, 6}, {4, 5, 6}},
                {{7, 8, 9}, {7, 8, 9}, {7, 8, 9}, {7, 8, 9}},
            });

            Shape targetShape = new([3, 1, 3]);

            Matrix matrixReduced = matrix.ReduceSum(targetShape);

            Assert.AreEqual(new(new float[3, 1, 3]
            {
                {{4, 8, 12}},
                {{16, 20, 24}},
                {{28, 32, 36}},
            }), matrixReduced);
        }

        [TestMethod]
        public void ReduceTest1()
        {
            Matrix matrix = new(new float[3, 4, 3]
            {
                {{1, 1, 1}, {1, 1, 1}, {1, 1, 1}, {1, 1, 1}},
                {{2, 2, 2}, {2, 2, 2}, {2, 2, 2}, {2, 2, 2}},
                {{3, 3, 3}, {3, 3, 3}, {3, 3, 3}, {3, 3, 3}},
            });

            Shape targetShape = new([3, 1, 3]);

            Matrix matrixReduced = matrix.ReduceSum(targetShape);

            Assert.AreEqual(new(new float[3, 1, 3]
            {
                {{4, 4, 4}},
                {{8, 8, 8}},
                {{12, 12, 12}},
            }), matrixReduced);
        }

        [TestMethod]
        public void ReduceTest2()
        {
            Matrix matrix = new(new float[3, 4, 3]
            {
                {{1, 1, 1}, {1, 1, 1}, {1, 1, 1}, {1, 1, 1}},
                {{2, 2, 2}, {2, 2, 2}, {2, 2, 2}, {2, 2, 2}},
                {{3, 3, 3}, {3, 3, 3}, {3, 3, 3}, {3, 3, 3}},
            });

            Shape targetShape = new([1, 3]);

            Matrix matrixReduced = matrix.ReduceSum(targetShape);

            Assert.AreEqual(new(new float[1, 3]
            {
                {24, 24, 24},
            }), matrixReduced);
        }

        [TestMethod]
        public void ReduceTest3()
        {
            Matrix matrix = new(new float[3, 4, 3]
            {
                {{1, 1, 1}, {1, 1, 1}, {1, 1, 1}, {1, 1, 1}},
                {{2, 2, 2}, {2, 2, 2}, {2, 2, 2}, {2, 2, 2}},
                {{3, 3, 3}, {3, 3, 3}, {3, 3, 3}, {3, 3, 3}},
            });

            Shape targetShape = new([1, 1, 1]);

            Matrix matrixReduced = matrix.ReduceSum(targetShape);

            Assert.AreEqual(new(new float[1, 1, 1]
            {
                {{72}},
            }), matrixReduced);
        }

        [TestMethod]
        public void ReduceTest4()
        {
            Matrix matrix = new(new float[3, 4, 3]
            {
                {{1, 1, 1}, {1, 1, 1}, {1, 1, 1}, {1, 1, 1}},
                {{2, 2, 2}, {2, 2, 2}, {2, 2, 2}, {2, 2, 2}},
                {{3, 3, 3}, {3, 3, 3}, {3, 3, 3}, {3, 3, 3}},
            });

            Shape targetShape = new([1]);

            Matrix matrixReduced = matrix.ReduceSum(targetShape);

            Assert.AreEqual(new(new float[1]
            {
                72,
            }), matrixReduced);
        }

        [TestMethod]
        public void GetFlatIndexTest1()
        {
            Matrix matrix = new(new Shape([3, 5, 2]));
            int flatIndex = matrix.GetFlatIndex(0, 0, 0);
            Assert.AreEqual(0, flatIndex);
        }

        [TestMethod]
        public void GetFlatIndexTest2()
        {
            Matrix matrix = new(new Shape([3, 5, 2]));
            int flatIndex = matrix.GetFlatIndex(0, 0, 1);
            Assert.AreEqual(1, flatIndex);
        }

        [TestMethod]
        public void GetFlatIndexTest3()
        {
            Matrix matrix = new(new Shape([3, 5, 2]));
            int flatIndex = matrix.GetFlatIndex(0, 1, 0);
            Assert.AreEqual(2, flatIndex);
        }

        [TestMethod]
        public void GetFlatIndexTest4()
        {
            Matrix matrix = new(new Shape([3, 5, 2]));
            int flatIndex = matrix.GetFlatIndex(2, 3, 1);
            Assert.AreEqual(27, flatIndex);
        }

        [TestMethod]
        public void GetFlatIndexTest5()
        {
            Matrix matrix = new(new Shape([5, 3, 4]));
            int flatIndex = matrix.GetFlatIndex(3, 2, 2);
            Assert.AreEqual(46, flatIndex);
        }

        [TestMethod]
        public void GetFlatIndexTest6()
        {
            Matrix matrix = new(new Shape([1]));
            int flatIndex = matrix.GetFlatIndex(0);
            Assert.AreEqual(0, flatIndex);
        }

        [TestMethod]
        public void When__IndexIsOutOfRange__Then__ArgumentOutOfRangeExceptionIsThrown__1()
        {
            Matrix matrix = new(new Shape([1]));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => matrix.GetFlatIndex(1));
        }

        [TestMethod]
        public void When__IndexIsOutOfRange__Then__ArgumentOutOfRangeExceptionIsThrown__2()
        {
            Matrix matrix = new(new Shape([1]));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => matrix.GetFlatIndex(0, 0));
        }

        [TestMethod]
        public void When__IndexIsOutOfRange__Then__ArgumentOutOfRangeExceptionIsThrown__3()
        {
            Matrix matrix = new(new Shape([3, 4, 2]));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => matrix.GetFlatIndex(2, 4, 1));
        }

        [TestMethod]
        public void When__IndexIsOutOfRange__Then__ArgumentOutOfRangeExceptionIsThrown__4()
        {
            Matrix matrix = new(new Shape([3, 4, 2]));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => matrix.GetFlatIndex(2, -1, 1));
        }

        [TestMethod]
        public void BroadcastPositive1()
        {
            Matrix matrix = new(new float[2, 1, 3]
            {
                {{1, 2, 3}},
                {{4, 5, 6}},
            });

            Shape shape = new([2, 3, 3]);

            Matrix broadcasted = matrix.Broadcast(shape);

            Assert.AreEqual(new(new float[2, 3, 3]
            {
                {{1, 2, 3}, {1, 2, 3}, {1, 2, 3}},
                {{4, 5, 6}, {4, 5, 6}, {4, 5, 6}},
            }), broadcasted);
        }

        [TestMethod]
        public void BroadcastPositive2()
        {
            Matrix matrix = new(new float[2, 1, 1]
            {
                {{1}},
                {{2}},
            });

            Shape shape = new([2, 3, 3]);

            Matrix broadcasted = matrix.Broadcast(shape);

            Assert.AreEqual(new(new float[2, 3, 3]
            {
                {{1, 1, 1}, {1, 1, 1}, {1, 1, 1}},
                {{2, 2, 2}, {2, 2, 2}, {2, 2, 2}},
            }), broadcasted);
        }

        [TestMethod]
        public void BroadcastPositive3()
        {
            Matrix matrix = new(new float[1, 1, 1, 4]
            {
                {{{1, 2, 3, 4}}},
            });

            Shape shape = new([2, 3, 3, 4]);

            Matrix broadcasted = matrix.Broadcast(shape);

            Assert.AreEqual(new(new float[2, 3, 3, 4]
            {
                {{{1, 2, 3, 4}, {1, 2, 3, 4}, {1, 2, 3, 4}}, {{1, 2, 3, 4}, {1, 2, 3, 4}, {1, 2, 3, 4}}, {{1, 2, 3, 4}, {1, 2, 3, 4}, {1, 2, 3, 4}}},
                {{{1, 2, 3, 4}, {1, 2, 3, 4}, {1, 2, 3, 4}}, {{1, 2, 3, 4}, {1, 2, 3, 4}, {1, 2, 3, 4}}, {{1, 2, 3, 4}, {1, 2, 3, 4}, {1, 2, 3, 4}}},
            }), broadcasted);
        }

        [TestMethod]
        public void BroadcastPositive4()
        {
            Matrix matrix = new(new float[1, 1, 1, 1]
            {
                {{{1}}},
            });

            Shape shape = new([2, 2, 3, 3, 4]);

            Matrix broadcasted = matrix.Broadcast(shape);

            Assert.AreEqual(shape, broadcasted.Shape);

            Assert.AreEqual(new(new float[2, 2, 3, 3, 4]
            {
                {{{{1, 1, 1, 1}, {1, 1, 1, 1}, {1, 1, 1, 1}}, {{1, 1, 1, 1}, {1, 1, 1, 1}, {1, 1, 1, 1}}, {{1, 1, 1, 1}, {1, 1, 1, 1}, {1, 1, 1, 1}}}, {{{1, 1, 1, 1}, {1, 1, 1, 1}, {1, 1, 1, 1}}, {{1, 1, 1, 1}, {1, 1, 1, 1}, {1, 1, 1, 1}}, {{1, 1, 1, 1}, {1, 1, 1, 1}, {1, 1, 1, 1}}}},
                {{{{1, 1, 1, 1}, {1, 1, 1, 1}, {1, 1, 1, 1}}, {{1, 1, 1, 1}, {1, 1, 1, 1}, {1, 1, 1, 1}}, {{1, 1, 1, 1}, {1, 1, 1, 1}, {1, 1, 1, 1}}}, {{{1, 1, 1, 1}, {1, 1, 1, 1}, {1, 1, 1, 1}}, {{1, 1, 1, 1}, {1, 1, 1, 1}, {1, 1, 1, 1}}, {{1, 1, 1, 1}, {1, 1, 1, 1}, {1, 1, 1, 1}}}},
            }), broadcasted);
        }
    }
}
