namespace SsisUnit
{
    public class CommandProperty
    {
        private string _name;
        private string _value;

        public CommandProperty(string name, string value)
        {
            _name = name;
            _value = value;
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string Value
        {
            get { return _value; }
            set { _value = value; }
        }
    }
}