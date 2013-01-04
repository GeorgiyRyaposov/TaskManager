using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Objects;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using TaskManager.Models;
using GalaSoft.MvvmLight.Command;

namespace TaskManager.ViewModels
{
    class TaskManagerViewModel : ViewModelBase
    {
        #region Fields
        //private TaskManagerEntities _taskManagerEntities;

        private ICollectionView _coolectionView;
        private ObservableCollection<Tasks> ParentTasks;
        private Tasks _newTask;
        private ObservableCollection<TasksModel> _tasksModels;
        
        public ObservableCollection<TasksModel> TasksModels 
        {
            get { return _tasksModels; }
            set { _tasksModels = value;
            base.RaisePropertyChanged("TasksModels");
            } 
        }

        public TasksModel SelectedTaskModel {get; set; }

        #endregion
        //public string Error
        //{
        //    get
        //    {
        //        return (CurrentTaskModel as IDataErrorInfo).Error;
        //    }
        //}

        //public string this[string columnName]
        //{
        //    get 
        //    {
        //        string error = (CurrentTaskModel as IDataErrorInfo)[columnName];
        //        CommandManager.InvalidateRequerySuggested();
        //        return error;
        //    }
        //}

        //Constructor
        public TaskManagerViewModel()
        {
            using (TaskManagerEntities taskManagerEntities = new TaskManagerEntities())
            {
                ParentTasks = new ObservableCollection<Tasks>(taskManagerEntities.Tasks.ToList().Where(task => task.ParentID == 0));
            }
            
            TasksModels = new ObservableCollection<TasksModel>();
            foreach (Tasks task in ParentTasks)
            {
                TasksModels.Add(new TasksModel(task));
            }
            _coolectionView = CollectionViewSource.GetDefaultView(TasksModels);
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
            SaveChangesCommand = new RelayCommand(SaveChanges);
            RemoveSelectedTaskCommand = new RelayCommand(RemoveSelectedTask);
            AddChildTaskCommand = new RelayCommand(AddChildTask, AddChildTaskCanExecute);
        }

        //Add new root task
        private void AddNewTask()
        {
            _newTask = new Tasks
                          {
                              Name = "<New task>",
                              ParentID = 0,
                              Performer = "",
                              PlannedRunTime = 0,
                              StatusID = 1,
                              ActualRunTime = 0,
                              Date = DateTime.Now
                          };
            TasksModels.Add(new TasksModel(_newTask));
        }

        //Save all changes in current entity
        private void SaveChanges()
        {
            try
            {
                using (TaskManagerEntities taskManagerEntities = new TaskManagerEntities())
                {
                    if (_newTask != null)
                    {
                        taskManagerEntities.Tasks.AddObject(_newTask);
                        base.RaisePropertyChanged("TasksModels");
                    }
                    taskManagerEntities.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Can't save changes" + ex.Message + ex.StackTrace);
            }
        }


        //Removing selected task
        private void RemoveSelectedTask()
        {
            if (MessageBox.Show("Do you realy want to remove task :'" + SelectedTaskModel.SelectedTask.Name + "'?",
  "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                using (TaskManagerEntities taskManagerEntities = new TaskManagerEntities())
                {
                    Tasks removeTask = taskManagerEntities.Tasks.First(task => task.ID == SelectedTaskModel.SelectedTask.ID);
                    taskManagerEntities.Tasks.DeleteObject(removeTask);
                    try
                    {
                        TasksModels.Remove(TasksModels.FirstOrDefault(task => task.SelectedTask.ID == SelectedTaskModel.SelectedTask.ID));
                        taskManagerEntities.SaveChanges();
                        base.RaisePropertyChanged("TasksModels");
                    }

                    catch
                        (Exception ex)
                    {
                        throw new Exception("Can't remove selected task\n" + ex.Message);
                    }
                }
            }
        }

        private void AddChildTask()
        {
            _newTask = new Tasks
                          {
                              Name = "<New child task>",
                              ParentID = SelectedTaskModel.SelectedTask.ID,
                              Performer = "",
                              PlannedRunTime = 0,
                              StatusID = 1,
                              ActualRunTime = 0,
                              Date = DateTime.Now
                          };
            TasksModels.Add(new TasksModel(_newTask));
        }

        private bool AddChildTaskCanExecute()
        {
            if (_coolectionView.CurrentItem == null)
                return false;
            return true;
        }
        #endregion
    }
}
