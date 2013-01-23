using System;
using System.Data.Objects.DataClasses;
using System.Linq;
using System.Collections.ObjectModel;
using System.Windows;
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

        public ObservableCollection<Tasks> TasksCollection { get; set; }
        public StatusModel StatusModels { get; set; }
        public static Tasks SelectedTask { get; set; }

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
                              IsSelected = true
                          };

            TasksCollection.Add(_newTask);
            _taskManagerEntities.Tasks.AddObject(_newTask);
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
            if (MessageBox.Show(Properties.Resources.Error_CantSaveChanges + "'" + SelectedTask.Name + "'?",
                Properties.Resources.UI_Confirmation, MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                Tasks removeTask = _taskManagerEntities.Tasks.First(task => task.ID == SelectedTask.ID);
                if (removeTask != null)
                    _taskManagerEntities.Tasks.DeleteObject(removeTask);

                //Remove task from collection
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
            SelectedTask.IsExpanded = true;

            _newTask = new Tasks
                          {
                              Name = Properties.Resources.NewSubTaskName,
                              ParentID = SelectedTask.ID,
                              Performer = Properties.Resources.NewPerformer,
                              PlannedRunTime = 0,
                              Status = (short)StatusEnum.Assigned,
                              ActualRunTime = 0,
                              Date = DateTime.Now,
                              IsSelected = true,
                          };

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
