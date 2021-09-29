using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Attune.Kernel.Build
{
    public class ModuleConfig
    {
        private string _name;

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
        private bool _hasDependency;
        public bool HasDependency
        {
            get
            {
                return _hasDependency;
            }

            set
            {
                _hasDependency = value;
            }
        }

        public List<ModuleConfig> DependencyCollection
        {
            get
            {
                return _dependencyCollection;
            }

            set
            {
                _dependencyCollection = value;
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

        public List<string> Category
        {
            get
            {
                return _category;
            }

            set
            {
                _category = value;
            }
        }

        public string NameSpace
        {
            get
            {
                return nameSpace;
            }

            set
            {
                nameSpace = value;
            }
        }

        public List<string> SpName
        {
            get
            {
                return _spName;
            }

            set
            {
                _spName = value;
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

        private List<string> _spName;

        private List<ModuleConfig> _dependencyCollection;

        private string _solutionPath;

        private List<string> _category;

        private string nameSpace;

        private bool _isSelected;

        private double _FrameWork;

        public double FrameWork
        {
            get { return _FrameWork; }
            set { _FrameWork = value; }
        }
    }
}
