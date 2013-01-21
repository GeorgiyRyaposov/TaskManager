using System;
using System.Collections.Generic;
using System.Data.Objects.DataClasses;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using TaskManager.Models;
using TaskManager.ViewModels;

namespace TaskManager
{
    public partial class Tasks : EntityObject
    {
        #region Fields

        private ObservableCollection<Tasks> _children;
        private bool _isSelected;

        #endregion //Fields

        #region Properties

        //Collection of child tasks
        public ObservableCollection<Tasks> Children
        {
            get
            {
                GetChildren();
                return _children;
            }
            set { _children = value; }
        }

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

        public short NewStatusSelected { get; set; }

        //Count Planned total runtime of child tasks
        public int PlannedRunTimeTotal
        {
            get
            {
                if (PlannedRunTime != null)
                    return (CountPlannedRunTimeSum(Children) + PlannedRunTime.Value);
                return CountPlannedRunTimeSum(Children);
            }
        }

        //Count Actual total runtime of child tasks
        public int ActualRunTimeTotal
        {
            get
            {
                if (ActualRunTime != null)
                    return (CountActualRunTimeSum(Children) + ActualRunTime.Value);
                return CountActualRunTimeSum(Children);
            }
        }

        #endregion //Properties

        #region Methods

        //Method which called before _Status = value
        partial void OnStatusChanging(global::System.Int16 value)
        {
            NewStatusSelected = value;
            if (TaskManagerViewModel.SelectedTask != null){
                if (value == (short) StatusEnum.Complete){
                    if (CheckStatus(TaskManagerViewModel.SelectedTask.Children)){
                        SetCompleteStatus(TaskManagerViewModel.SelectedTask.Children);
                    }
                    else{
                        Status = _Status;
                    }
                }
            }
        }

        //Get child tasks of current task
        private void GetChildren()
        {
            using (TaskManagerEntities taskManagerEntities = new TaskManagerEntities()) {
                Children =
                    new ObservableCollection<Tasks>(
                        taskManagerEntities.Tasks.Where(item => item.ParentID == ID).ToList());
            }
        }

        /// <summary>
        /// Return false if any child task status is 'Assigned'
        /// </summary>
        /// <param name="children"></param>
        /// <returns></returns>
        public bool CheckStatus(ObservableCollection<Tasks> children)
        {
            if (children.Count > 0)
            {
                foreach (Tasks task in children)
                {
                    if (CheckStatus(task.Children) == false)
                        return false;
                    if (task.Status == (short)StatusEnum.Assigned)
                        return false;
                }
            }
            return true;
        }

        //Set 'Complete' status to all child tasks
        private void SetCompleteStatus(ObservableCollection<Tasks> children)
        {
            if (children.Count > 0)
            {
                foreach (Tasks task in children)
                {
                    SetCompleteStatus(task.Children);
                    task.ReportPropertyChanging("Status");
                    task._Status = SetValidValue((short)StatusEnum.Complete);
                    task.ReportPropertyChanged("Status");
                }
            }
        }
        

        //Count sum of all child planned runtime tasks
        private int CountPlannedRunTimeSum(ObservableCollection<Tasks> childrenTasks)
        {
            int sum = 0;

            if (childrenTasks.Count > 0)
            {
                foreach (Tasks task in childrenTasks)
                {
                    if (task.PlannedRunTime != null)
                        sum += task.PlannedRunTime.Value;
                    sum += CountPlannedRunTimeSum(task.Children);
                }
            }
            return sum;
        }

        //Count sum of all child actual runtime tasks
        private int CountActualRunTimeSum(ObservableCollection<Tasks> childrenTasks)
        {
            int sum = 0;

            if (childrenTasks.Count > 0)
            {
                foreach (Tasks task in childrenTasks)
                {
                    if (task.ActualRunTime != null)
                        sum += task.ActualRunTime.Value;
                    sum += CountActualRunTimeSum(task.Children);
                }
            }
            return sum;
        }

        #endregion
    }
}
