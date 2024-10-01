using Myras.Types;
using Myras.Utils;

namespace MyrasTest.Utils
{
    [TestClass]
    public class MathMTests
    {
        [TestMethod]
        public void DotProduct_Positive1()
        {
            Matrix a = new(new float[2, 3]
            {
                { 1, 2, 3 },
                { 4, 5, 6 },
            });

            Matrix b = new(new float[3, 2]
            {
                { 7, 8 },
                { 9, 10 },
                { 11, 12 }
            });

            Matrix c = MathM.DotProduct(a, b);

            Assert.AreEqual(new(new float[2, 2]
            {
                { 58, 64 },
                { 139, 154 },
            }), c);
        }

        [TestMethod]
        public void DotProduct_Positive2()
        {
            Matrix a = new(new float[2, 2]
            {
                { 1, 2 },
                { 3, 4 },
            });

            Matrix b = new(new float[2, 2]
            {
                { 2, 0 },
                { 1, 2 },
            });

            Matrix c = MathM.DotProduct(a, b);

            Assert.AreEqual(new(new float[2, 2]
            {
                { 4, 4 },
                { 10, 8 },
            }), c);
        }

        [TestMethod]
        public void Transpose_Positive1()
        {
            Matrix x = new(new float[3, 4]
            {
                { 1, 2, 3, 4 },
                { 5, 6, 7, 8 },
                { 9, 10, 11, 12 }
            });

            Matrix xT = MathM.Transpose(x);

            Assert.AreEqual(new(new float[4, 3]
            {
                { 1, 5, 9 },
                { 2, 6, 10 },
                { 3, 7, 11 },
                { 4, 8, 12 },
            }), xT);
        }

        [TestMethod]
        public void Transpose_Positive2()
        {
            Matrix x = new(new float[3, 4, 2]
            {
                { { 1, 2 }, { 3, 4 }, { 5, 6 }, { 7, 8 } },
                { { 9, 10 }, { 11, 12 }, { 13, 14 }, { 15, 16 } },
                { { 17, 18 }, { 19, 20 }, { 21, 22 }, { 23, 24 } },
            });

            Matrix xT = MathM.Transpose(x);

            Assert.AreEqual(new(new float[2, 4, 3]
            {
                { { 1, 9, 17 }, { 3, 11, 19 }, { 5, 13, 21 }, { 7, 15, 23 } },
                { { 2, 10, 18 }, { 4, 12, 20 }, { 6, 14, 22 }, { 8, 16, 24 } },
            }), xT);
        }

        [TestMethod]
        public void Broadcast_Basic_1()
        {
            Matrix a = new(new float[2]
            {
                1, 2
            });

            Matrix b = new(new float[2, 2]
            {
                { 1, 2 },
                { 3, 4 }
            });

            Console.WriteLine($"a = {a}");
            Console.WriteLine($"b = {b}");
            Console.WriteLine($"c = a + b");
            Matrix c = a + b;
            Console.WriteLine($"c = {c}");

            Assert.AreEqual(new(new float[2, 2]
            {
                { 2, 4 },
                { 4, 6 }
            }), c);
        }

        [TestMethod]
        public void Broadcast_Basic_2()
        {
            Matrix a = new(new float[2]
            {
                1, 2
            });

            Matrix b = new(new float[3, 2]
            {
                { 1, 2 },
                { 3, 4 },
                { 5, 6 }
            });

            Console.WriteLine($"a = {a}");
            Console.WriteLine($"b = {b}");
            Console.WriteLine($"c = a + b");
            Matrix c = a + b;
            Console.WriteLine($"c = {c}");

            Assert.AreEqual(new(new float[3, 2]
            {
                { 2, 4 },
                { 4, 6 },
                { 6, 8 }
            }), c);
        }

        [TestMethod]
        public void Broadcast_Basic_3()
        {
            Matrix a = new(new float[1]
            {
                1
            });

            Matrix b = new(new float[3, 2]
            {
                { 1, 2 },
                { 3, 4 },
                { 5, 6 }
            });

            Console.WriteLine($"a = {a}");
            Console.WriteLine($"b = {b}");
            Console.WriteLine($"c = a + b");
            Matrix c = a + b;
            Console.WriteLine($"c = {c}");

            Assert.AreEqual(new(new float[3, 2]
            {
                { 2, 3 },
                { 4, 5 },
                { 6, 7 }
            }), c);
        }

        [TestMethod]
        public void Broadcast_HigherDimensions_1()
        {
            Matrix a = new(new float[2, 1]
            {
                { 1 },
                { 2 }
            });

            Matrix b = new(new float[2, 3]
            {
                { 1, 2, 3 },
                { 4, 5, 6 }
            });

            Console.WriteLine($"a = {a}");
            Console.WriteLine($"b = {b}");
            Console.WriteLine($"c = a + b");
            Matrix c = a + b;
            Console.WriteLine($"c = {c}");

            Assert.AreEqual(new(new float[2, 3]
            {
                { 2, 3, 4 },
                { 6, 7, 8 }
            }), c);
        }

        [TestMethod]
        public void Broadcast_HigherDimensions_2()
        {
            Matrix a = new(new float[1, 3]
            {
                { 1, 2, 3 }
            });

            Matrix b = new(new float[2, 3]
            {
                { 1, 2, 3 },
                { 4, 5, 6 }
            });

            Console.WriteLine($"a = {a}");
            Console.WriteLine($"b = {b}");
            Console.WriteLine($"c = a + b");
            Matrix c = a + b;
            Console.WriteLine($"c = {c}");

            Assert.AreEqual(new(new float[2, 3]
            {
                { 2, 4, 6 },
                { 5, 7, 9 }
            }), c);
        }

        [TestMethod]
        public void Broadcast_DifferentDimensionality_1()
        {
            Matrix a = new(new float[3, 1]
            {
                { 1 },
                { 2 },
                { 3 }
            });

            Matrix b = new(new float[3, 2]
            {
                { 1, 2 },
                { 3, 4 },
                { 5, 6 }
            });

            Console.WriteLine($"a = {a}");
            Console.WriteLine($"b = {b}");
            Console.WriteLine($"c = a + b");
            Matrix c = a + b;
            Console.WriteLine($"c = {c}");

            Assert.AreEqual(new(new float[3, 2]
            {
                { 2, 3 },
                { 5, 6 },
                { 8, 9 }
            }), c);
        }

        [TestMethod]
        public void Broadcast_HigherDimensionality_1()
        {
            Matrix a = new(new float[2, 1, 4]
            {
                { { 1, 2, 3, 4 } },
                { { 5, 6, 7, 8 } }
            });

            Matrix b = new(new float[2, 3, 4]
            {
                { { 1, 2, 3, 4 }, { 5, 6, 7, 8 }, { 9, 10, 11, 12 } },
                { { 13, 14, 15, 16 }, { 17, 18, 19, 20 }, { 21, 22, 23, 24 } }
            });

            Console.WriteLine($"a = {a}");
            Console.WriteLine($"b = {b}");
            Console.WriteLine($"c = a + b");
            Matrix c = a + b;
            Console.WriteLine($"c = {c}");

            Assert.AreEqual(new(new float[2, 3, 4]
            {
                { { 2, 4, 6, 8 }, { 6, 8, 10, 12 }, { 10, 12, 14, 16 } },
                { { 18, 20, 22, 24 }, { 22, 24, 26, 28 }, { 26, 28, 30, 32 } },
            }), c);
        }

        [TestMethod]
        public void Broadcast_HigherDimensionality_2()
        {
            Matrix a = new(new float[1, 3, 4]
            {
                { { 1, 2, 3, 4 }, { 5, 6, 7, 8 }, { 9, 10, 11, 12 } },
            });

            Matrix b = new(new float[2, 3, 4]
            {
                { { 1, 2, 3, 4 }, { 5, 6, 7, 8 }, { 9, 10, 11, 12 } },
                { { 13, 14, 15, 16 }, { 17, 18, 19, 20 }, { 21, 22, 23, 24 } }
            });

            Console.WriteLine($"a = {a}");
            Console.WriteLine($"b = {b}");
            Console.WriteLine($"c = a + b");
            Matrix c = a + b;
            Console.WriteLine($"c = {c}");

            Assert.AreEqual(new(new float[2, 3, 4]
            {
                { { 2, 4, 6, 8 }, { 10, 12, 14, 16 }, { 18, 20, 22, 24 } },
                { { 14, 16, 18, 20 }, { 22, 24, 26, 28 }, { 30, 32, 34, 36 } }
            }), c);
        }

        [TestMethod]
        public void Broadcast_MismatchedShapes_1()
        {
            Matrix a = new(new float[2, 3]
            {
                { 1, 2, 3 },
                { 4, 5, 6 }
            });

            Matrix b = new(new float[3, 2]
            {
                { 1, 2 },
                { 3, 4 },
                { 5, 6 }
            });

            Console.WriteLine($"a = {a}");
            Console.WriteLine($"b = {b}");
            Console.WriteLine($"c = a + b");
            Console.WriteLine("This should fail due to incompatible shapes");

            Assert.ThrowsException<ArgumentException>(() =>
            {
                Matrix c = a + b;
            });
        }

        [TestMethod]
        public void Broadcast_MismatchedShapes_2()
        {
            Matrix a = new(new float[2, 3]
            {
                { 1, 2, 3 },
                { 4, 5, 6 }
            });

            Matrix b = new(new float[2, 3, 4]
            {
                { { 3, 4, 5, 6 }, { 7, 8, 9, 10 }, { 11, 12, 13, 14 } },
                { { 15, 16, 17, 18 }, { 19, 20, 21, 22 }, { 23, 24, 25, 26 } }
            });

            Console.WriteLine($"a = {a}");
            Console.WriteLine($"b = {b}");
            Console.WriteLine($"c = a + b");
            Console.WriteLine("This should fail due to incompatible shapes");

            Assert.ThrowsException<ArgumentException>(() =>
            {
                Matrix c = a + b;
            });
        }

        [TestMethod]
        public void Broadcast_Advanced_1()
        {
            Matrix a = new(new float[3, 1, 4]
            {
                { { 1, 2, 3, 4 } },
                { { 5, 6, 7, 8 } },
                { { 9, 10, 11, 12 } }
            });

            Matrix b = new(new float[3, 2, 4]
            {
                { { 1, 2, 3, 4 }, { 5, 6, 7, 8 } },
                { { 9, 10, 11, 12 }, { 13, 14, 15, 16 } },
                { { 17, 18, 19, 20 }, { 21, 22, 23, 24 } }
            });

            Console.WriteLine($"a = {a}");
            Console.WriteLine($"b = {b}");
            Console.WriteLine($"c = a + b");
            Matrix c = a + b;
            Console.WriteLine($"c = {c}");

            Assert.AreEqual(new(new float[3, 2, 4]
            {
                { { 2, 4, 6, 8 }, { 6, 8, 10, 12 } },
                { { 14, 16, 18, 20 }, { 18, 20, 22, 24 } },
                { { 26, 28, 30, 32 }, { 30, 32, 34, 36 } }
            }), c);
        }

        [TestMethod]
        public void Broadcast_Advanced_2()
        {
            Matrix a = new(new float[1, 3, 1]
            {
                { { 1 }, { 2 }, { 3 } }
            });

            Matrix b = new(new float[2, 3, 4]
            {
                { { 1, 2, 3, 4 }, { 5, 6, 7, 8 }, { 9, 10, 11, 12 } },
                { { 13, 14, 15, 16 }, { 17, 18, 19, 20 }, { 21, 22, 23, 24 } }
            });

            Console.WriteLine($"a = {a}");
            Console.WriteLine($"b = {b}");
            Console.WriteLine($"c = a + b");
            Matrix c = a + b;
            Console.WriteLine($"c = {c}");

            Assert.AreEqual(new(new float[2, 3, 4]
            {
                { { 2, 3, 4, 5 }, { 7, 8, 9, 10 }, { 12, 13, 14, 15 } },
                { { 14, 15, 16, 17 }, { 19, 20, 21, 22 }, { 24, 25, 26, 27 } },
            }), c);
        }
    }
}
