using Asana.Library.Models;

namespace Asana.Maui.ViewModels
{
    public class ProjectViewModel
    {
        public Project? Model { get; set; }

        public string DisplayName
        {
            get
            {
                if (Model == null) return "No Project";
                return $"{Model.Id}. {Model.Name}";
            }
        }

        public override string ToString()
        {
            return DisplayName;
        }
    }
}