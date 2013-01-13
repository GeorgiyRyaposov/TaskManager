using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using TaskManager.ViewModels;

namespace TaskManager.Models
{
    class TasksModel : INotifyPropertyChanged, IDataErrorInfo
    {
        #region Data
        
        private readonly ObservableCollection<Tasks> _childrenList;
        private readonly TasksModel _parent;
        private Tasks _task;
        private short _newTaskStatus;

        public ObservableCollection<Status> TaskStatusList { get; set; }
        public ObservableCollection<TasksModel> Children { get; set; }

        bool _isExpanded = true;
        bool _isSelected = true;

        #endregion //Data

        #region Constructors

        public TasksModel(Tasks task, TaskManagerEntities taskManagerEntities)
            : this(task, null, taskManagerEntities)
        {
        }

        public TasksModel(Tasks task, TasksModel parent, TaskManagerEntities taskManagerEntities)
        {
            SelectedTask = task;
            _parent = parent;
            
            //Load items in 'Status' combobox
            UpdateStatusList();
            
            //Gets all child items of current task
            Children = new ObservableCollection<TasksModel>();
            
            _childrenList =
                new ObservableCollection<Tasks>(
                    taskManagerEntities.Tasks.ToList().Where(item => item.ParentID == SelectedTask.ID && item.ParentID != 0));

            //Add child tasks to model
            if (_childrenList.Count > 0)
                foreach (Tasks child in _childrenList)
                {
                    Children.Add(new TasksModel(child, this, taskManagerEntities));
                }
        }

        #endregion // Constructors

        #region Task Properties

        public string Name
        {
            get { return SelectedTask.Name; }
            set { SelectedTask.Name = value;
                  OnPropertyChanged("Name");}
        }

        public short Status
        {
            get
            {
                UpdateStatusList(); //Update items of 'Status' combo
                return SelectedTask.StatusID;
            }
            set
            {
                _newTaskStatus = value;
                if (value == 4) //Status 'Завершена'
                {
                    if (CheckStatus(Children))
                    {
                        SetCompleteStatus(Children);
                        SelectedTask.StatusID = value;
                        OnPropertyChanged("Status");
                    }
                }
                else
                {
                    SelectedTask.StatusID = value;
                    OnPropertyChanged("Status");
                }
            }
        }

        public int PlannedRunTimeTotal
        {
            get 
            {
                if (_task.PlannedRunTime != null)
                    return (CountPlannedRunTimeSum(Children) +_task.PlannedRunTime.Value);
                return CountPlannedRunTimeSum(Children);
            }
        }

        public int ActualRunTimeTotal
        {
            get 
            { 
                if (_task.ActualRunTime != null) 
                    return (CountActualRunTimeSum(Children) + _task.ActualRunTime.Value);
                return CountActualRunTimeSum(Children);
            }
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

        public TasksModel Parent
        {
            get { return _parent; }
        }

        #endregion //Task Properties

        #region Presentation Members
        
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
                    OnPropertyChanged("IsExpanded");
                }

                // Expand all the way up to the root.
                if (_isExpanded && _parent != null)
                    _parent.IsExpanded = true;
            }
        }

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
                    TaskManagerViewModel.SelectedTaskModel = this;
                    OnPropertyChanged("IsSelected");
                }
            }
        }

        #endregion // Presentation Members

        #region Error validation

        public string Error
        {
            get { throw new NotImplementedException(); }
        }

        public string this[string propertyName]
        {
            get
            {
                string validationResult = null;
                switch (propertyName)
                {
                    case "Status":
                        validationResult = ValidateStatus();
                        break;
                    case "Name":
                        validationResult = ValidateName();
                        break;
                    default:
                        throw new ApplicationException("Unknow property being validated on status");
                }
                return validationResult;
            }
        }

        //Validate Task name
        private string ValidateName()
        {
            if (String.IsNullOrEmpty(Name))
                return "Введите имя задачи";

            return String.Empty;
        }

        //Validate Task status
        private string ValidateStatus()
        {
            if ((_newTaskStatus == 4) && (CheckStatus(Children) == false))
                return "Задача не может быть переведена в статус\n'Завершена', т.к. одна из подзадач не завершена.";

            return String.Empty;
        }

        #endregion // Error validation

        #region Methods

        /// <summary>
        /// Return false if any child task status is 'Назначена'
        /// </summary>
        /// <param name="children"></param>
        /// <returns></returns>
        private bool CheckStatus(ObservableCollection<TasksModel> children)
        {
            if (children.Count > 0)
            {
                foreach (TasksModel tasksModel in children)
                {
                    if (CheckStatus(tasksModel.Children) == false)
                        return false;
                    if (tasksModel.Status == 1)
                        return false;
                }
            }
            return true;
        }

        //Update items of Status combobox
        private void UpdateStatusList()
        {
            using (TaskManagerEntities taskStatusEntities = new TaskManagerEntities())
            {
                if(SelectedTask.StatusID == 1)
                    TaskStatusList = new ObservableCollection<Status>(taskStatusEntities.Status.ToList().Where(status => status.ID < 4));
                else
                    TaskStatusList = new ObservableCollection<Status>(taskStatusEntities.Status.ToList());
            }
        }

        //Set 'Complete' status to all child tasks
        private void SetCompleteStatus(ObservableCollection<TasksModel> children)
        {
            if (children.Count > 0)
            {
                foreach (TasksModel tasksModel in children)
                {
                    SetCompleteStatus(tasksModel.Children);
                    tasksModel.SelectedTask.StatusID = 4;
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

        //Gets TaskModel by task id
        public TasksModel GetTaskById(ObservableCollection<TasksModel> taskModels, int id)
        {
            TasksModel tempModel = taskModels.FirstOrDefault(model => model.SelectedTask.ID == id);
            if (tempModel != null)
                return tempModel;

            return taskModels.Select(tasksModel => GetTaskById(tasksModel.Children, id)).FirstOrDefault();
        }

        #endregion //Methods

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion // INotifyPropertyChanged Members
    }
}
