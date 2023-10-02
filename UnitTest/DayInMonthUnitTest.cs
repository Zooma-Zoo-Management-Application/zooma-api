using zooma_api.Controllers;

namespace DateTimeTracker
{
    public class DayInMonthUnitTest
    {
        private DateTimeTrackerController _dateTimeTracker;

        [SetUp]
        public void SetUp()
        {
            _dateTimeTracker = new DateTimeTrackerController();
        }

        [Test]
        public void DaysInMonth_InputIsMonth12Year2000_ReturnTrue()
        {
            // ARRANGE
            var month = 12;
            var year = 2000;
            var expectedOutput = 31;

            // ACT
            var result = _dateTimeTracker.DaysInMonth(year, month);

            // ASSERT
            Assert.AreEqual(result, expectedOutput);
        }

        [Test]
        public void DaysInMonth_InputIsMonth4Year2004_Return30()
        {
            // ARRANGE
            var month = 4;
            var year = 2004;
            var expectedOutput = 30;

            // ACT
            var result = _dateTimeTracker.DaysInMonth(year, month);

            // ASSERT
            Assert.AreEqual(result, expectedOutput);
        }

        [Test]
        public void DaysInMonth_InputIsMonth2Year2000_Return29()
        {
            // ARRANGE
            var month = 2;
            var year = 2000;
            var expectedOutput = 29;

            // ACT
            var result = _dateTimeTracker.DaysInMonth(year, month);

            // ASSERT
            Assert.AreEqual(result, expectedOutput);
        }

        [Test]
        public void DaysInMonth_InputIsMonth2Year2004_Return29()
        {
            // ARRANGE
            var month = 2;
            var year = 2004;
            var expectedOutput = 29;

            // ACT
            var result = _dateTimeTracker.DaysInMonth(year, month);

            // ASSERT
            Assert.AreEqual(result, expectedOutput);
        }

        [Test]
        public void DaysInMonth_InputIsMonth2Year2005_Return28()
        {
            // ARRANGE
            var month = 2;
            var year = 2005;
            var expectedOutput = 28;

            // ACT
            var result = _dateTimeTracker.DaysInMonth(year, month);

            // ASSERT
            Assert.AreEqual(result, expectedOutput);
        }

        [Test]
        public void DaysInMonth_InputIsMonth13Year2004_Return0()
        {
            // ARRANGE
            var month = 13;
            var year = 2004;
            var expectedOutput = 0;

            // ACT
            var result = _dateTimeTracker.DaysInMonth(year, month);

            // ASSERT
            Assert.AreEqual(result, expectedOutput);
        }

        [Test]
        public void DaysInMonth_InputIsMonthMinus3Year2005_Return0()
        {
            // ARRANGE
            var month = -3;
            var year = 2005;
            var expectedOutput = 0;

            // ACT
            var result = _dateTimeTracker.DaysInMonth(year, month);

            // ASSERT
            Assert.AreEqual(result, expectedOutput);
        }
    }
}