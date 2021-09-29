using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Attune.Kernel.Build
{
    public class SpNames
    {
        private string _SpName;

        public string SpName
        {
            get { return _SpName; }
            set { _SpName = value; }
        }
        private string _ParameterName;

        public string ParameterName
        {
            get { return _ParameterName; }
            set { _ParameterName = value; }
        }

        private string _SystemType;

        public string SystemType
        {
            get { return _SystemType; }
            set { _SystemType = value; }
        }
        private int _Length;

        public int Length
        {
            get { return _Length; }
            set { _Length = value; }
        }

        private int _Numeric_Precision;

        public int Numeric_Precision
        {
            get { return _Numeric_Precision; }
            set { _Numeric_Precision = value; }
        }

        private int _Numeric_Scale;

        public int Numeric_Scale
        {
            get { return _Numeric_Scale; }
            set { _Numeric_Scale = value; }
        }

        private bool _IsOutputParameter;

        public bool IsOutputParameter
        {
            get { return _IsOutputParameter; }
            set { _IsOutputParameter = value; }
        }

        private int _ORDINAL_POSITION;

        public int ORDINAL_POSITION
        {
            get { return _ORDINAL_POSITION; }
            set { _ORDINAL_POSITION = value; }
        }

    }
}
