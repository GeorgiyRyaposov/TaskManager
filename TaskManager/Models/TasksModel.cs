using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using GalaSoft.MvvmLight.Command;

namespace TaskManager.Models
{
    class TasksModel : INotifyPropertyChanged
    {
        #region Data
        private TaskManagerEntities taskManagerEntities;
        private ObservableCollection<TasksModel> _children;
        private ObservableCollection<Tasks> _childrenList;
        
        private TasksModel _parent;
        private Tasks _task;

        public ObservableCollection<Status> TaskStatus { get; set; }

        bool _isExpanded = true;
        bool _isSelected;

        #endregion // Data

        #region Constructors

        public TasksModel(Tasks task)
            : this(task, null)
        {
        }

        public TasksModel(Tasks task, TasksModel parent)
        {
            SelectedTask = task;
            _parent = parent;
            
            taskManagerEntities = new TaskManagerEntities();
            if(SelectedTask.StatusID == 1)
                TaskStatus = new ObservableCollection<Status>(taskManagerEntities.Status.ToList().Where(status => status.ID < 4));
            else
                TaskStatus = new ObservableCollection<Status>(taskManagerEntities.Status.ToList());
            
            //Gets all child items of current task
            _children = new ObservableCollection<TasksModel>();
            
            _childrenList =
                new ObservableCollection<Tasks>(
                    taskManagerEntities.Tasks.ToList().Where(item => item.ParentID == SelectedTask.ID && item.ParentID != 0));

            //Add child tasks to model
            if (_childrenList.Count > 0)
                foreach (Tasks child in _childrenList)
                {
                    _children.Add(new TasksModel(child, this));
                }
        }

        #endregion // Constructors

        #region Task Properties

        public ObservableCollection<TasksModel> Children
        {
            get { return _children; }
        }

        public string Name
        {
            get { return SelectedTask.Name; }
            set { SelectedTask.Name = value;
                  OnPropertyChanged("Name");}
        }

        public short Status
        {
            get { return SelectedTask.StatusID; }
            set
            {
                if ((value == 4) && (SelectedTask.StatusID != 1) && (CheckStatus(_children)))
                {
                    SetCompleteStatus(_children);
                    SelectedTask.StatusID = value;
                }
                else
                {
                    return;
                }
                SelectedTask.StatusID = value;
                OnPropertyChanged("Status");
            }
        }

        public int PlannedRunTimeTotal
        {
            get { return (CountPlannedRunTimeSum(Children)+_task.PlannedRunTime.Value); }
        }

        public int ActualRunTimeTotal
        {
            get { return (CountActualRunTimeSum(Children) + _task.ActualRunTime.Value); }
        }
        


        public Tasks SelectedTask
        {
            get { return _task; }
            set 
            { 
                _task = value;
                OnPropertyChanged("SelectedTask");
            }
        }

        #endregion // Person Properties

        #region Presentation Members

        #region IsExpanded

        /// <summary>
        /// Gets/sets whether the TreeViewItem 
        /// associated with this object is expanded.
        /// </summary>
        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                if (value != _isExpanded)
                {
                    _isExpanded = value;
                    this.OnPropertyChanged("IsExpanded");
                }

                // Expand all the way up to the root.
                if (_isExpanded && _parent != null)
                    _parent.IsExpanded = true;
            }
        }

        #endregion // IsExpanded

        #region IsSelected

        /// <summary>
        /// Gets/sets whether the TreeViewItem 
        /// associated with this object is selected.
        /// </summary>
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (value != _isSelected)
                {
                    _isSelected = value;
                    OnPropertyChanged("IsSelected");
                }
            }
        }


        #endregion // IsSelected
        
        #region Parent

        public TasksModel Parent
        {
            get { return _parent; }
        }

        #endregion // Parent

        //#region Error validation

        //public string Error
        //{
        //    get { throw new NotImplementedException(); }
        //}


        //public string this[string propertyName]
        //{
        //    get
        //    {
        //        string validationResult = null;
        //        switch (propertyName)
        //        {
        //            case "Status":
        //                validationResult = ValidateStatus();
        //                break;
        //            default:
        //                throw new ApplicationException("Unknow property being validated on status");
        //        }
        //        return validationResult;
        //    }
        //}

        //private string ValidateStatus()
        //{
        //    if (Status == 4 && SelectedTask.StatusID == 1)
        //        return "Задача не может быть переведена в статус 'Завершена', т.к. она не выполнялась.";

        //    return String.Empty;
        //}

        //#endregion

        #endregion // Presentation Members

        #region Methods

        private bool CheckStatus(ObservableCollection<TasksModel> children)
        {
            if (children.Count > 0)
            {
                foreach (TasksModel child in children)
                {
                    if (CheckStatus(child.Children) == false)
                        return false;
                    if (child.Status == 1)
                        return false;
                }

            }
            return true;
        }

        private void SetCompleteStatus(ObservableCollection<TasksModel> children)
        {
            if (children.Count > 0)
            {
                foreach (TasksModel child in children)
                {
                    SetCompleteStatus(child.Children);
                    child.SelectedTask.StatusID = 4;
                }
            }
        }

        //Count sum of all child planned runtime tasks
        private int CountPlannedRunTimeSum(ObservableCollection<TasksModel> children)
        {
            int sum = 0;
            
            if (children.Count > 0)
            {
                foreach (TasksModel tasksModel in children)
                {
                    if (tasksModel.SelectedTask.PlannedRunTime != null)
                        sum += tasksModel.SelectedTask.PlannedRunTime.Value;
                    sum += CountPlannedRunTimeSum(tasksModel.Children);
                }
            }
            return sum;
        }

        //Count sum of all child actual runtime tasks
        private int CountActualRunTimeSum(ObservableCollection<TasksModel> children)
        {
            int sum = 0;

            if (children.Count > 0)
            {
                foreach (TasksModel tasksModel in children)
                {
                    if (tasksModel.SelectedTask.ActualRunTime != null)
                        sum += tasksModel.SelectedTask.ActualRunTime.Value;
                    sum += CountActualRunTimeSum(tasksModel.Children);
                }
            }
            return sum;
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion // INotifyPropertyChanged Members
    }
}
