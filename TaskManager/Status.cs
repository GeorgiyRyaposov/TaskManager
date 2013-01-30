namespace TaskManager
{
    public class Status
    {
        #region Properties

        public StatusEnum Number { get; set; }
        public string Name { get; set; }

        #endregion //Properties

        public Status(StatusEnum number, string name)
        {
            Number = number;
            Name = name;
        }
    }
}
