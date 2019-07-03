using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections;

namespace Model
{
    [Serializable]
    public class ModelData: IDataErrorInfo,INotifyPropertyChanged
    {
        public static double pMin;
        public static double pMax;
        [field:NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, args);
            }
        }
        public void F(int n, double p)
        {
            for (int i = 0; i < n; i++)
            {
                X[i] = i * 2 * Math.PI / (double)(n-1);
                //Y[i] = X[i] * X[i] + p;
                Y[i] = Math.Sin(X[i])*Math.Log10(p);
            }
        }
        private int nodesamount;
        private double p;
        public string NodesAmount {
            get
            {
                return nodesamount.ToString() ;
            }
            set
            {
                if (!Int32.TryParse(value, out nodesamount))
                    nodesamount = 0;
                else 
                    OnPropertyChanged(new PropertyChangedEventArgs("NodesAmount"));
            }
        } //число узлов
        public string P {
            get
            {
                return p.ToString();
            }
            set
            {
                if (!double.TryParse(value, out p)) p = -1;
                else
                OnPropertyChanged(new PropertyChangedEventArgs("P"));
            }
        } //параметр р
        public double[] X { get; set; } //узлы
        public double[] Y { get; set; } //значения
        public double Fmin {
            get
            {
                return
                    (from d in Y
                     select d).Min();
            }
        }
        public double Fmax
        {
            get
            {
                return
                    (from d in Y
                     select d).Max();
            }
        }

        //реалзиация IDataErrorInfo
        //public string Error => throw new NotImplementedException();

        public string Error
        {
            get
            {
                return "Error Text";
            }
        }

        public string this[string property]
        {
            get
            {
                string msg = null;
                switch (property)
                {
                    case "P":
                        if (p < pMin || p > pMax) msg = "enter P in ["+pMin+";"+ pMax+" range, with comma separator";
                        break;
                    case "NodesAmount":
                        if (nodesamount < 3) msg = "Enter nodes amount more than 2";
                        break;
                    default:
                        break;
                }
                return msg;

            }
        }

        public ModelData(int nodesAmount, double p)
        {
            nodesamount = nodesAmount;
            this.p = p;
            X = new double[nodesAmount];
            Y = new double[nodesAmount];
            F(nodesAmount, p);
        }
        public ModelData(ref ModelData md)
        {
            P = md.P;
            NodesAmount = md.NodesAmount;
            X = new double[nodesamount];
            Y = new double[nodesamount];
            F(nodesamount, p);
        }
        //статический конструктор
        static ModelData()
        {
            pMax = 10;
            pMin = 0;
        }
        public override string ToString()
        {
            string buf = "P = "+p+" Nodes = " +NodesAmount+" ; ";
            for (int i = 0; i < X.Length; i++)
            {
                //buf += $"{X[i]:0.##}->{Y[i]:0.##}; ";
                buf += X[i] + " -> " + Y[i] + "; ";
            }
            return buf;
        }
        public static bool Compare(ModelData md1, ModelData md2)
        {
            if (md1.nodesamount != md2.nodesamount) return false;
            if (md1.P != md2.P) return false;
            return true;
        }

    }
}
