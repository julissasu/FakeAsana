using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Asana.Library.Models
{
    public class ToDo : INotifyPropertyChanged
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int Priority { get; set; }

        private bool _isComplete;
        public bool IsComplete
        {
            get => _isComplete;
            set
            {
                if (_isComplete != value)
                {
                    _isComplete = value;
                    OnPropertyChanged();
                }
            }
        }

        public int? ProjectId { get; set; }
        public DateTime DueDate { get; set; } = DateTime.Now;

        // For displaying project names in the UI
        public string ProjectDisplayName { get; set; } = "";

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}