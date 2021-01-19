using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace SmartTimer.Models
{
    public class Template : INotifyPropertyChanged
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; }
        public int FirstStepDurationSec { get; set; }
        public int SecondStepDurationSec { get; set; }
        public int NumberOfUses
        {
            get
            {
                return _NumberOfUses;
            }
            set
            {
                _NumberOfUses = value;
                OnPropertyChanged("NumberOfUses");
            }
        }
        private int _NumberOfUses;

        [Ignore]
        public int Number
        {
            get
            {
                return _Number;
            }
            set
            {
                _Number = value;
                OnPropertyChanged("Number");
            }
        }
        private int _Number;

        [Ignore]
        public string FirstStepDurationFormated 
        {
            get
            {
                return TimeSpan.FromSeconds(FirstStepDurationSec).ToString(@"hh\:mm");
            } 
        }
        
        [Ignore]
        public string SecondStepDurationFormated 
        {
            get
            {
                if (SecondStepDurationSec != 0)
                    return TimeSpan.FromSeconds(SecondStepDurationSec).ToString(@"hh\:mm");

                else
                    return "brak";
            } 
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, e);
        }
    }
}
