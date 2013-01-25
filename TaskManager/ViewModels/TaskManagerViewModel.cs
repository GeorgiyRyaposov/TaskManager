using System;
using System.Data.Objects.DataClasses;
using System.Linq;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Interactivity;
using GalaSoft.MvvmLight;
using TaskManager.Models;
using GalaSoft.MvvmLight.Command;

namespace TaskManager.ViewModels
{
    class TaskManagerViewModel : ViewModelBase
    {
        #region Fields

        private readonly TaskManagerEntities _taskManagerEntities;
        private Tasks _newTask;
        private bool _isExpanded;
        private bool _isSelected;
        
        public ObservableCollection<Tasks> TasksCollection { get; set; }
        public StatusModel StatusModels { get; set; }
        public static Tasks SelectedTask { get; set; }

        
        //When item of tree is selected
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (value != _isSelected)
                {
                    _isSelected = value;
                    
                    base.RaisePropertyChanged("IsSelected");
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
                    base.RaisePropertyChanged("IsExpanded");
                }
            }
        }

        #endregion //Fields

        //Constructor
        public TaskManagerViewModel()
        {
            _taskManagerEntities = new TaskManagerEntities();
            StatusModels = new StatusModel();

            //Fill treeview with tasks
            TasksCollection = new ObservableCollection<Tasks>(_taskManagerEntities.Tasks.Where(task => task.ParentID == null).ToList());
            
            //Initialize commands
            AddCommands();
        }

        #region Commands

        #region fields
        public RelayCommand AddNewTaskCommand
        {
            get;
            private set;
        }

        public RelayCommand SaveChangesCommand
        {
            get;
            private set;
        }

        public RelayCommand RemoveSelectedTaskCommand
        {
            get;
            private set;
        }

        public RelayCommand AddChildTaskCommand
        {
            get;
            private set;
        }

        #endregion //fields

        //Initialize commands
        private void AddCommands()
        {
            AddNewTaskCommand = new RelayCommand(AddNewTask);
            SaveChangesCommand = new RelayCommand(SaveChanges, SaveChangesCanExecute);
            RemoveSelectedTaskCommand = new RelayCommand(RemoveSelectedTask, RemoveSelectedTaskCanExecute);
            AddChildTaskCommand = new RelayCommand(AddChildTask, AddChildTaskCanExecute);
        }

        //Add new root task
        private void AddNewTask()
        {
            _newTask = new Tasks
                          {
                              Name = Properties.Resources.NewTaskName,
                              ParentID = null,
                              Performer = Properties.Resources.NewPerformer,
                              PlannedRunTime = 0,
                              Status = (short)StatusEnum.Assigned,
                              ActualRunTime = 0,
                              Date = DateTime.Now,
                          };

            TasksCollection.Add(_newTask);
            _taskManagerEntities.Tasks.AddObject(_newTask);
            base.RaisePropertyChanged("TasksCollection");
        }

        //Save all changes in current entity
        private void SaveChanges()
        {
            try
            {
                _taskManagerEntities.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(Properties.Resources.Error_CantSaveChanges + "\n" + ex.Message + "\n" +ex.StackTrace);
            }
        }

        //Remove selected task
        private void RemoveSelectedTask()
        {
            if (MessageBox.Show(Properties.Resources.UI_AreYouSure_Remove + "'" + SelectedTask.Name + "'?",
                Properties.Resources.UI_Confirmation, MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                object removeTask;
                if (_taskManagerEntities.TryGetObjectByKey(SelectedTask.EntityKey, out removeTask))
                    _taskManagerEntities.Tasks.DeleteObject((Tasks)removeTask);

                //Remove task from collection
                if (TasksCollection.Contains(SelectedTask))
                    TasksCollection.Remove(SelectedTask);
                else
                    RemoveTask(SelectedTask.ChildTask);
            }
        }

        private bool RemoveSelectedTaskCanExecute()
        {
            if (SelectedTask == null)
                return false;
            if (SelectedTask.ChildTask.Count > 0)
                return false;
            return true;
        }

        //Add child task to selected task
        private void AddChildTask()
        {
            _newTask = new Tasks
                          {
                              Name = Properties.Resources.NewSubTaskName,
                              ParentID = SelectedTask.ID,
                              Performer = Properties.Resources.NewPerformer,
                              PlannedRunTime = 0,
                              Status = (short)StatusEnum.Assigned,
                              ActualRunTime = 0,
                              Date = DateTime.Now,
                          };
            SelectedTask.ChildTask.Add(_newTask);
            _taskManagerEntities.Tasks.AddObject(_newTask);            
            base.RaisePropertyChanged("TasksCollection");
        }
        
        private bool AddChildTaskCanExecute()
        {
            if (SelectedTask == null)
                return false;
            return true;
        }

        private bool SaveChangesCanExecute()
        {
            if (SelectedTask == null)
                return false;
            if (!String.IsNullOrEmpty(SelectedTask.Error))
                return false;
            return true;
        }
        #endregion //Commands
        
        #region Methods

        //Runs through all TasksModels and removes selected Task
        private void RemoveTask(EntityCollection<Tasks> tasksList)
        {
            if (tasksList.Contains(SelectedTask))
            {
                tasksList.Remove(SelectedTask);
            }
            else
            {
                foreach (Tasks task in tasksList)
                {
                    if (task.ChildTask.Count > 0)
                    {
                        RemoveTask(task.ChildTask);
                    }
                }
            }
        }
        
        #endregion //Methods

        
    }
}
