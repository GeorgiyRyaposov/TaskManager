using System;
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
        private readonly ObservableCollection<Tasks> _parentTasks;
        private Tasks _newTask;

        public ObservableCollection<TasksModel> TasksModels { get; set; }

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
            _taskManagerEntities = new TaskManagerEntities();

            _parentTasks = new ObservableCollection<Tasks>(_taskManagerEntities.Tasks.ToList().Where(task => task.ParentID == 0));

            
            TasksModels = new ObservableCollection<TasksModel>();
            foreach (Tasks task in _parentTasks)
            {
                TasksModels.Add(new TasksModel(task, _taskManagerEntities));
            }
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
            RemoveSelectedTaskCommand = new RelayCommand(RemoveSelectedTask, RemoveSelectedTaskCanExecute);
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
            TasksModels.Add(new TasksModel(_newTask, _taskManagerEntities));
            
            _taskManagerEntities.Tasks.AddObject(_newTask);
            _taskManagerEntities.SaveChanges();
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
                throw new Exception("Can't save changes\n" + ex.Message + "\n" +ex.StackTrace);
            }
        }


        //Remove selected task
        private void RemoveSelectedTask()
        {
            if (MessageBox.Show("Вы действительно хотите удалить задачу:'" + SelectedTaskModel.SelectedTask.Name + "'?",
                "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                Tasks removeTask = _taskManagerEntities.Tasks.FirstOrDefault(task => task.ID == SelectedTaskModel.SelectedTask.ID);
                _taskManagerEntities.Tasks.DeleteObject(removeTask);
                try
                {
                    TasksModel tempTask = null;
                    foreach (TasksModel tasksModel in TasksModels)
                    {
                        tempTask = tasksModel.GetTaskById(tasksModel.Children, SelectedTaskModel.SelectedTask.ID);
                        if (tempTask != null)
                            TasksModels.Remove(tempTask);
                    }
                    //TasksModels.Remove(TasksModels.FirstOrDefault(task => task.SelectedTask.ID == SelectedTaskModel.SelectedTask.ID));
                    _taskManagerEntities.SaveChanges();
                    base.RaisePropertyChanged("TasksModels");
                }
                catch(Exception ex)
                {
                    throw new Exception("Can't remove selected task\n" + ex.Message);
                }
            }
        }

        private bool RemoveSelectedTaskCanExecute()
        {
            if (SelectedTaskModel == null)
                return false;
            if (SelectedTaskModel.Children.Count > 0)
                return false;
            return true;
        }

        //Add child task to selected task
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
            TasksModel parentModel = null;
            foreach (TasksModel tasksModel in TasksModels)
            {
                parentModel = tasksModel.GetTaskById(tasksModel.Children, SelectedTaskModel.SelectedTask.ID);
            }
            if (parentModel != null)
                parentModel.Children.Add(new TasksModel(_newTask, _taskManagerEntities));
            _taskManagerEntities.SaveChanges();
        }

        private bool AddChildTaskCanExecute()
        {
            if (SelectedTaskModel == null)
                return false;
            return true;
        }
        #endregion
    }
}
