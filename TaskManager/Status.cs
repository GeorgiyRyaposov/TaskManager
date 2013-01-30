namespace TaskManager
{
    public class Status
    {
        #region Properties

        public short Number { get; set; }
        public string Name { get; set; }

        #endregion //Properties

        public Status(short number, string name)
        {
            Number = number;
            Name = name;
        }
    }
}
