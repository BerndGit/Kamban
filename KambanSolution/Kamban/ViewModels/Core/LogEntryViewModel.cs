using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Kamban.Repository.Attributes;
using ReactiveUI.Fody.Helpers;

namespace Kamban.ViewModels.Core
{
    public class LogEntryViewModel : ILogEntry
    {
        // Fixme: Check how to make the [Reactive] attribute work. See CardViewModel

        private int Idvalue { get; set; }

        private DateTime Timevalue { get; set; }
        private String Topicvalue { get; set; }

        private String Boardvalue { get; set; }
        private String Columnvalue { get; set; }
        private String Rowvalue { get; set; }
        private String Propertyvalue { get; set; }
        private String OldValuevalue { get; set; }
        private String NewValuevalue { get; set; }

        private String Notevalue { get; set; }

        private int RowIdvalue { get; set; }
        private int ColumnIdvalue { get; set; }
        private int BoardIdvalue { get; set; }
        private int CardIdvalue { get; set; }

        private bool Automaticvalue { get; set; } = true;


        [AutoSave,Reactive] public int Id
        { get
            {
                return this.Idvalue;
            }
            set
            {
                if (value != this.Idvalue)
                {
                    this.Idvalue = value;
                    NotifyPropertyChanged();
                }
            }
        }

        [AutoSave,Reactive] public DateTime Time
        {
            get
            {
                return this.Timevalue;
            }
            set
            {
                if (value != this.Timevalue)
                {
                    this.Timevalue = value;
                    NotifyPropertyChanged();
                }
            }
        }

        [AutoSave,Reactive] public string Topic
        {
            get
            {
                return this.Topicvalue;
            }
            set
            {
                if (value != this.Topicvalue)
                {
                    this.Topicvalue = value;
                    NotifyPropertyChanged();
                }
            }
        }
        [AutoSave,Reactive] public string Board
        {
            get
            {
                return this.Boardvalue;
            }
            set
            {
                if (value != this.Boardvalue)
                {
                    this.Boardvalue = value;
                    NotifyPropertyChanged();
                }
            }
        }

        [AutoSave,Reactive] public string Column
        {
            get
            {
                return this.Columnvalue;
            }
            set
            {
                if (value != this.Columnvalue)
                {
                    this.Columnvalue = value;
                    NotifyPropertyChanged();
                }
            }
        }

        [AutoSave,Reactive] public string Row
        {
            get
            {
                return this.Rowvalue;
            }
            set
            {
                if (value != this.Rowvalue)
                {
                    this.Rowvalue = value;
                    NotifyPropertyChanged();
                }
            }
        }

        [AutoSave,Reactive] public string Property
        {
            get
            {
                return this.Propertyvalue;
            }
            set
            {
                if (value != this.Propertyvalue)
                {
                    this.Propertyvalue = value;
                    NotifyPropertyChanged();
                }
            }
        }

        [AutoSave,Reactive] public string OldValue
        {
            get
            {
                return this.OldValuevalue;
            }
            set
            {
                if (value != this.OldValuevalue)
                {
                    this.OldValuevalue = value;
                    NotifyPropertyChanged();
                }
            }
        }
        [AutoSave,Reactive] public string NewValue
        {
            get
            {
                return this.NewValuevalue;
            }
            set
            {
                if (value != this.NewValuevalue)
                {
                    this.NewValuevalue = value;
                    NotifyPropertyChanged();
                }
            }
        }
        [AutoSave,Reactive] public string Note
        {
            get
            {
                return this.Notevalue;
            }
            set
            {
                if (value != this.Notevalue)
                {
                    this.Notevalue = value;
                    NotifyPropertyChanged();
                }
            }
        }

        [AutoSave,Reactive] public int CardId
        {
            get
            {
                return this.CardIdvalue;
            }
            set
            {
                if (value != this.CardIdvalue)
                {
                    this.CardIdvalue = value;
                    NotifyPropertyChanged();
                }
            }
        }
        [AutoSave,Reactive] public int RowId
        {
            get
            {
                return this.RowIdvalue;
            }
            set
            {
                if (value != this.RowIdvalue)
                {
                    this.RowIdvalue = value;
                    NotifyPropertyChanged();
                }
            }
        }
        [AutoSave,Reactive] public int ColumnId
        {
            get
            {
                return this.ColumnIdvalue;
            }
            set
            {
                if (value != this.ColumnIdvalue)
                {
                    this.ColumnIdvalue = value;
                    NotifyPropertyChanged();
                }
            }
        }
        [AutoSave,Reactive] public int BoardId
        {
            get
            {
                return this.BoardIdvalue;
            }
            set
            {
                if (value != this.BoardIdvalue)
                {
                    this.BoardIdvalue = value;
                    NotifyPropertyChanged();
                }
            }
        }

        [AutoSave,Reactive] public bool Automatic
        {
            get
            {
                return this.Automaticvalue;
            }
            set
            {
                if (value != this.Automaticvalue)
                {
                    this.Automaticvalue = value;
                    NotifyPropertyChanged();
                }
            }
        } 

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
           if (PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            } 
        }
    }
}
