using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace TaskManager.Models
{
    class StatusModel : IDataErrorInfo
    {
        #region Status properties

        private string _name;
        public string Name
        {
            get { return _name; } 
            set 
            { 
                Previous = _name;
                _name = value;
            }
        }
        public short ID { get; set; }
        public string Previous { get; set; }
        
        #endregion

        public StatusModel(string name, short id)
        {
            Name = name;
            Previous = name;
            ID = id;
        }

        public void Save()
        {
            using (TaskManagerEntities taskManagerEntities = new TaskManagerEntities())
            {
                

            }
        }

        public string Error
        {
            get { throw new NotImplementedException();}
        }


        public string this[string propertyName]
        {
            get 
            { 
                string validationResult = null;
                switch (propertyName)
                {
                    case "Name":
                        validationResult = ValidateName();
                        break;
                    default:
                        throw new ApplicationException("Unknow property being validated on status");
                }
                return validationResult;
            }
        }

        private string ValidateName()
        {
            if (String.IsNullOrEmpty(this.Name))
                return "Выберите статус.";

            if (Name == "Завершена" && Previous == "Назначена")
                return "Задача не может быть переведена в статус 'Завершена', т.к. она не выполнялась.";

            return String.Empty;
        }
    }
}
