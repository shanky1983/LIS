using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Attune.Kernel.Build
{
    public class ApiConfig
    {
        private string _name;
        private string _solutionPath;
        private bool _isSelected;
        private string _apipublishpath;

        public string Name
        {
            get
            {
                return _name;
            }

            set
            {
                _name = value;
            }
        }     
        public string SolutionPath
        {
            get
            {
                return _solutionPath;
            }

            set
            {
                _solutionPath = value;
            }
        }        
        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }

            set
            {
                _isSelected = value;
            }
        }
        public string ApiPublishPath
        {
            get
            {
                return _apipublishpath;
            }

            set
            {
                _apipublishpath = value;
            }
        }
    }
}
