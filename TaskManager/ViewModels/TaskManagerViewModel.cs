using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
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
        private readonly ObservableCollection<Tasks> _parentTasks;
        private Tasks _newTask;
        
        public ObservableCollection<TasksModel> TasksModels { get; set; }
        public static TasksModel SelectedTaskModel { get; set; }

        #endregion //Fields

        //Constructor
        public TaskManagerViewModel()
        {
            _taskManagerEntities = new TaskManagerEntities();
            
            //Fill treeview with tasks
            _parentTasks = new ObservableCollection<Tasks>(_taskManagerEntities.Tasks.ToList().Where(task => task.ParentID == 0));

            TasksModels = new ObservableCollection<TasksModel>();
            foreach (Tasks task in _parentTasks)
            {
                TasksModels.Add(new TasksModel(task, _taskManagerEntities));
            }

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
            TasksModels.Add(new TasksModel(_newTask, _taskManagerEntities) { IsSelected = true });
            
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
                throw new Exception("Не удалось сохранить изменения\n" + ex.Message + "\n" +ex.StackTrace);
            }
        }

        //Remove selected task
        private void RemoveSelectedTask()
        {
            if (MessageBox.Show("Вы действительно хотите удалить задачу:'" + SelectedTaskModel.Task.Name + "'?",
                "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                Tasks removeTask = _taskManagerEntities.Tasks.First(task => task.ID == SelectedTaskModel.Task.ID);
                if (removeTask != null)
                    _taskManagerEntities.Tasks.DeleteObject(removeTask);

                RemoveTask(TasksModels);
                
                try
                {
                    _taskManagerEntities.SaveChanges();
                    base.RaisePropertyChanged("TasksModels");
                }
                catch(Exception ex)
                {
                    throw new Exception("Не удалось удалить выбраную задачу\n" + ex.Message);
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
                              ParentID = SelectedTaskModel.Task.ID,
                              Performer = "",
                              PlannedRunTime = 0,
                              StatusID = 1,
                              ActualRunTime = 0,
                              Date = DateTime.Now                             
                          };

            SelectedTaskModel.Children.Add(new TasksModel(_newTask, _taskManagerEntities) { IsSelected = true });

            _taskManagerEntities.Tasks.AddObject(_newTask);
            try
            {
                _taskManagerEntities.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Не удалось сохранить изменения."+ex.Message);
            }
        }

        private bool AddChildTaskCanExecute()
        {
            if (SelectedTaskModel == null)
                return false;
            return true;
        }
        #endregion //Commands

        #region Methods

        //Runs through all TasksModels and removes selected Task
        private void RemoveTask(ObservableCollection<TasksModel> taskModels)
        {
            if (taskModels.Contains(SelectedTaskModel))
            {
                taskModels.Remove(SelectedTaskModel);
            }
            else
            {
                foreach (TasksModel tasksModel in taskModels)
                {
                    if (tasksModel.Children.Count > 0)
                    {
                        RemoveTask(tasksModel.Children);
                    }
                }
            }
        }
        
        #endregion //Methods
    }
}
