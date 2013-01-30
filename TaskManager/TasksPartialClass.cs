using System;
using System.ComponentModel;
using System.Data.Objects.DataClasses;
using System.Globalization;

namespace TaskManager
{
    public partial class Tasks : IDataErrorInfo
    {
        #region Variables

        public int ErrorCounter;
        #endregion //Variables

        #region Properties

        public short NewStatusSelected { get; set; }
        
        //Count Planned total runtime of child tasks
        public int PlannedRunTimeTotal
        {
            get
            {
                if (PlannedRunTime != null)
                    return (CountPlannedRunTimeSum(ChildTask) + PlannedRunTime.Value);
                return CountPlannedRunTimeSum(ChildTask);
            }
        }

        //Count Actual total runtime of child tasks
        public int ActualRunTimeTotal
        {
            get
            {
                if (ActualRunTime != null)
                    return (CountActualRunTimeSum(ChildTask) + ActualRunTime.Value);
                return CountActualRunTimeSum(ChildTask);
            }
        }
        
        #endregion //Properties

        #region Methods

        partial void OnStatusChanging(Int16 value)
        {
            NewStatusSelected = value;
        }

        partial void OnStatusChanged()
        {
            if (Status == (short)StatusEnum.Complete)
            {
                if (CheckStatus(ChildTask))
                {
                    SetCompleteStatus(ChildTask);
                }
                else
                {
                    _Status = (short)StatusEnum.InProgress;
                    OnPropertyChanged("Status");
                }
            }
        }
        
        /// <summary>
        /// Return false if any child task status is 'Assigned'
        /// </summary>
        /// <param name="children"></param>
        /// <returns></returns>
        public bool CheckStatus(EntityCollection<Tasks> children)
        {
            if (children.Count > 0)
            {
                foreach (Tasks task in children)
                {
                    if (CheckStatus(task.ChildTask) == false)
                        return false;
                    if (task.Status == (short)StatusEnum.Assigned)
                        return false;
                }
            }
            return true;
        }

        //Set 'Complete' status to all child tasks
        private void SetCompleteStatus(EntityCollection<Tasks> children)
        {
            if (children.Count > 0)
            {
                foreach (Tasks task in children)
                {
                    SetCompleteStatus(task.ChildTask);
                    task.ReportPropertyChanging("Status");
                    task._Status = SetValidValue((short)StatusEnum.Complete);
                    task.ReportPropertyChanged("Status");
                }
            }
        }
        

        //Count sum of all child planned runtime tasks
        private int CountPlannedRunTimeSum(EntityCollection<Tasks> childrenTasks)
        {
            int sum = 0;

            if (childrenTasks.Count > 0)
            {
                foreach (Tasks task in childrenTasks)
                {
                    if (task.PlannedRunTime != null)
                        sum += task.PlannedRunTime.Value;
                    sum += CountPlannedRunTimeSum(task.ChildTask);
                }
            }
            return sum;
        }

        //Count sum of all child actual runtime tasks
        private int CountActualRunTimeSum(EntityCollection<Tasks> childrenTasks)
        {
            int sum = 0;

            if (childrenTasks.Count > 0)
            {
                foreach (Tasks task in childrenTasks)
                {
                    if (task.ActualRunTime != null)
                        sum += task.ActualRunTime.Value;
                    sum += CountActualRunTimeSum(task.ChildTask);
                }
            }
            return sum;
        }

        #endregion

        #region Error validation

        public string Error { get; set; }

        public string this[string propertyName]
        {
            get
            {
                Error = String.Empty;
                switch (propertyName)
                {
                    case "Status":
                        if (NewStatusSelected == (short)StatusEnum.Complete &&
                            (CheckStatus(ChildTask) == false))
                        {Error = Properties.Resources.Valid_TaskCantBeSetToCompleted;}
                        break;
                    case "Name":
                        if (String.IsNullOrEmpty(Name))
                        {Error = Properties.Resources.Valid_EnterTaskName;}
                        break;
                    case "ActualRunTime":
                        if (ActualRunTime == null)
                        {Error = Properties.Resources.Valid_NullField;}
                        break;
                    case "PlannedRunTime":
                        if (PlannedRunTime == null)
                        {Error = Properties.Resources.Valid_NullField;}
                        break;
                    case "Date":
                        if (String.IsNullOrEmpty(Date.ToString(CultureInfo.InvariantCulture)))
                        {Error = Properties.Resources.Valid_NullField;}
                        break;
                    default:
                        throw new ApplicationException(Properties.Resources.Valid_Error_UnknowProperty);
                }
                return Error;
            }
        }
        #endregion // Error validation
    }
}
