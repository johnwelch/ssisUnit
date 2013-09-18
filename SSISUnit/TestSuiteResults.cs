using System.Collections.Generic;

namespace SsisUnit
{
    public class TestSuiteResults
    {
        private readonly Dictionary<StatisticEnum, TestSuiteStatistic> _statistics = new Dictionary<StatisticEnum, TestSuiteStatistic>(6);
        // private List<string> _results = new List<string>();

        internal TestSuiteResults()
        {
            _statistics.Add(StatisticEnum.TestCount, new TestSuiteStatistic());
            _statistics.Add(StatisticEnum.AssertCount, new TestSuiteStatistic());
            _statistics.Add(StatisticEnum.AssertPassedCount, new TestSuiteStatistic());
            _statistics.Add(StatisticEnum.AssertFailedCount, new TestSuiteStatistic());
            _statistics.Add(StatisticEnum.TestPassedCount, new TestSuiteStatistic());
            _statistics.Add(StatisticEnum.TestFailedCount, new TestSuiteStatistic());
            // _statistics.Add(StatisticEnum.TaskFailedCount, new TestSuiteStatistic());
        }

        #region Methods

        internal void Reset()
        {
            foreach (TestSuiteStatistic tss in _statistics.Values)
            {
                tss.Reset();
            }
        }

        internal void IncrementStatistic(StatisticEnum statistic)
        {
            _statistics[statistic].IncrementValue();
        }

        public int GetStatistic(StatisticEnum statistic)
        {
            return _statistics[statistic].Value;
        }

        #endregion

        public enum StatisticEnum
        {
            TestCount = 0,
            AssertCount = 1,
            TestPassedCount = 2,
            TestFailedCount = 3,
            AssertPassedCount = 4,
            AssertFailedCount = 5 // ,
            // TaskFailedCount = 6
        }

        private class TestSuiteStatistic
        {
            public int Value { get; private set; }

            public void IncrementValue()
            {
                Value++;
            }

            public void Reset()
            {
                Value = 0;
            }
        }
    }
}