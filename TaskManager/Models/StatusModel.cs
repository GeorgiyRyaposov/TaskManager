using System.Collections.ObjectModel;
using System.ComponentModel;

namespace TaskManager.Models
{
    public class StatusModel : ObservableCollection<Status>
    {
        private Status _selectedStatus;

        private readonly Status _assigned = new Status(StatusEnum.Assigned, Properties.Resources.Status_Assigned);
        private readonly Status _inProgress = new Status(StatusEnum.InProgress, Properties.Resources.Status_InProgress);
        private readonly Status _stopped = new Status(StatusEnum.Stopped, Properties.Resources.Status_Stopped);
        private readonly Status _complete = new Status(StatusEnum.Complete, Properties.Resources.Status_Complete);

        public StatusModel()
        {
            Add(_assigned);
            Add(_inProgress);
            Add(_stopped);
            Add(_complete);
        }

        public Status SelectedStatus
        {
            get { return _selectedStatus; } 
            set
            {
                if (value == _assigned)
                    Remove(_complete);
                else
                {
                    if(!Contains(_complete))
                        Add(_complete);
                }
                base.OnPropertyChanged(new PropertyChangedEventArgs("SelectedStatus"));
                _selectedStatus = value;
            } 
        }
    }
}
