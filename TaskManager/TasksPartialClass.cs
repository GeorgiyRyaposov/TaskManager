using System;
using System.ComponentModel;
using System.Data.Objects.DataClasses;
using TaskManager.ViewModels;

namespace TaskManager
{
    public partial class Tasks : EntityObject, IDataErrorInfo
    {
        #region Variables

        private bool _isExpanded;
        private bool _isSelected;
        private string _validationResult;

        #endregion //Variables

        #region Properties

        public short NewStatusSelected { get; set; }
        
        //When item of tree is selected
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (value != _isSelected)
                {
                    _isSelected = value;
                    TaskManagerViewModel.SelectedTask = this;
                    OnPropertyChanged("IsSelected");
                }
            }
        }

        //When item of tree is expanded
        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                if (value != _isExpanded)
                {
                    _isExpanded = value;
                    OnPropertyChanged("IsExpanded");
                }
            }
        }
        
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

        partial void OnStatusChanging(global::System.Int16 value)
        {
            NewStatusSelected = value;
        }

        partial void OnStatusChanged()
        {
            if (TaskManagerViewModel.SelectedTask != null)
            {
                if (Status == (short)StatusEnum.Complete)
                {
                    if (CheckStatus(TaskManagerViewModel.SelectedTask.ChildTask))
                    {
                        SetCompleteStatus(TaskManagerViewModel.SelectedTask.ChildTask);
                    }
                    else
                    {
                        _Status = (short)StatusEnum.InProgress;
                        OnPropertyChanged("Status");
                    }
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

        public string Error
        {
            get { return _validationResult; }
        }

        public string this[string propertyName]
        {
            get
            {
                
                switch (propertyName)
                {
                    case "Status":
                        _validationResult = ValidateStatus();
                        break;
                    case "Name":
                        _validationResult = ValidateName();
                        break;
                    case "ActualRunTime":
                        _validationResult = ValidateActualRunTime();
                        break;
                    case "PlannedRunTime":
                        _validationResult = ValidatePlannedRunTime();
                        break;
                    case "Date":
                        _validationResult = ValidatePlannedDate();
                        break;
                    default:
                        throw new ApplicationException(Properties.Resources.Valid_Error_UnknowProperty);
                }
                return _validationResult;
            }
        }

        //Validate Task name
        private string ValidateName()
        {
            if (TaskManagerViewModel.SelectedTask != null)
            {
                if (String.IsNullOrEmpty(TaskManagerViewModel.SelectedTask.Name))
                    return Properties.Resources.Valid_EnterTaskName;
            }
            return String.Empty;
        }

        //Validate Date
        private string ValidatePlannedDate()
        {
            if (TaskManagerViewModel.SelectedTask != null)
            {
                if (String.IsNullOrEmpty(TaskManagerViewModel.SelectedTask.Date.ToString()))
                    return Properties.Resources.Valid_NullField;
            }
            return String.Empty;
        }
        
        //Validate ActualRunTime
        private string ValidateActualRunTime()
        {
            if (TaskManagerViewModel.SelectedTask != null)
            {
                if (TaskManagerViewModel.SelectedTask.ActualRunTime == null)
                    return Properties.Resources.Valid_NullField;
            }
            return String.Empty;
        }

        //Validate PlannedRunTime
        private string ValidatePlannedRunTime()
        {
            if (TaskManagerViewModel.SelectedTask != null)
            {
                if (TaskManagerViewModel.SelectedTask.PlannedRunTime == null)
                    return Properties.Resources.Valid_NullField;
            }
            return String.Empty;
        }

        //Validate Task status
        private string ValidateStatus()
        {
            if (TaskManagerViewModel.SelectedTask != null)
            {
                if (TaskManagerViewModel.SelectedTask.NewStatusSelected == (short) StatusEnum.Complete
                    &&
                    (TaskManagerViewModel.SelectedTask.CheckStatus(TaskManagerViewModel.SelectedTask.ChildTask) == false)){
                    return Properties.Resources.Valid_TaskCantBeSetToCompleted;
                }
            }
            return String.Empty;
        }

        #endregion // Error validation
    }
}
