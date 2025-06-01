using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Asana.Library.Models
{
    public class ToDo
    {
        public string? id;
        public string? Id
        {
            get
            {
                return id;
            }
            set
            {
                if (id != value)
                {
                    id = value;
                }
            }
        }
        public string? name;
        public string? Name
        {
            get
            {
                return name;
            }
            set
            {
                if (name != value)
                {
                    name = value;
                }
            }
        }
        public string? description;
        public string? Description
        {
            get
            {
                return description;
            }
            set
            {
                if (description != value)
                {
                    description = value;
                }
            }
        }
        public int? priority;
        public int? Priority
        {
            get
            {
                return priority;
            }
            set
            {
                if (priority != value)
                {
                    priority = value;
                }
            }
        }
        public bool? isComplete;
        public bool? IsComplete
        {
            get
            {
                return isComplete;
            }
            set
            {
                if (isComplete != value)
                {
                    isComplete = value;
                }
            }
        }
        public int? projectId;
        public int? ProjectId
        {
            get
            {
                return projectId;
            }
            set
            {
                if (projectId != value)
                {
                    projectId = value;
                }
            }
        }

        public ToDo()
        {

        }

    }
}