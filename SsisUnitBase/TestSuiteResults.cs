using System.Collections.Generic;

using SsisUnitBase.Enums;

namespace SsisUnitBase
{
    public class TestSuiteResults
    {
        private readonly Dictionary<StatisticEnum, TestSuiteStatistic> _statistics = new Dictionary<StatisticEnum, TestSuiteStatistic>(6);
        // private List<string> _results = new List<string>();

        public TestSuiteResults()
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

        public void Reset()
        {
            foreach (TestSuiteStatistic tss in _statistics.Values)
            {
                tss.Reset();
            }
        }

        public void IncrementStatistic(StatisticEnum statistic)
        {
            _statistics[statistic].IncrementValue();
        }

        public int GetStatistic(StatisticEnum statistic)
        {
            return _statistics[statistic].Value;
        }

        #endregion

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