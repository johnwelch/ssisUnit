namespace SsisUnitBase.Enums
{
    public enum CommandParentType
    {
        Unknown = 0,

        /// <summary>
        /// An assert is the command's parent type.
        /// </summary>
        Assert = 1,

        /// <summary>
        /// The test suite's unit test setup collection is the command's parent type.
        /// </summary>
        UnitTestSetup = 2,

        /// <summary>
        /// The test suite's unit test teardown collection is the command's parent type.
        /// </summary>
        UnitTestTeardown = 3,

        /// <summary>
        /// The test suite's setup collection is the command's parent type.
        /// </summary>
        TestSuiteSetup = 4,

        /// <summary>
        /// The test suite's teardown collection is the command's parent type.
        /// </summary>
        TestSuiteTeardown = 5,

        /// <summary>
        /// A unit test's setup collection is the command's parent type.
        /// </summary>
        TestSetup = 6,

        /// <summary>
        /// A unit test's teardown collection is the command's parent type.
        /// </summary>
        TestTeardown = 7
    }
}