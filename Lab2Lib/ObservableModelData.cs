using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
namespace Model
{   
    [Serializable]
    public class ObservableModelData: ObservableCollection<ModelData>
    {
        private bool changed;
        public bool Changed
        {
            get
            {
                return changed;
            }
            set
            {
                changed = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Changed"));
            }
        }
        public void Add_ModelData(ModelData modelData)
        {
            this.Add(modelData);
            changed = true;
        }
        public void Remove_At(int index)
        {
            this.RemoveAt(index);
            changed = true;
        }
        public void AddDefaults()
        {
            this.Add_ModelData(new ModelData(10, 0.5));
            this.Add_ModelData(new ModelData(20, 0.7));
            this.Add_ModelData(new ModelData(30, 0.3));
        }
        public IEnumerable<double> PValues
        {
            get
            {
                return
                    from md in this orderby double.Parse(md.P) select double.Parse(md.P);
            }
        }
        public IEnumerable<double> FMin
        {
            get
            {
                return
                    from md in this
                    orderby double.Parse(md.P)
                    select md.Fmin;
            }
        }
        public IEnumerable<double> FMax
        {
            get
            {
                return
                    from md in this
                    orderby double.Parse(md.P)
                    select md.Fmax;
            }
        }
        public override string ToString()
        {
            string buf = "";
            foreach (ModelData md in this)
            {
                buf += md.ToString();
            }
            return buf;
        }
        //метод для копирования данных из одного экземпляра в другой,
        //нужен при десериализации, если просто ссылку менять, то датаконтекст теряется
        //а ссылку на поле экземпляра невозможно передать в качестве параметра
        public void TransitFrom(ObservableModelData obj)
        {
            this.Clear();
            foreach (var md in obj) Add(md);
            changed = false;
        }
        //serialization
        public static bool Serialize(string filename, ref ObservableModelData obj)
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream fs = null;
            bool ret = false;
            try
            {
                fs = File.Create(filename);
                bf.Serialize(fs, obj);
                ret = true;
            }
            //catch (Exception)
            //{
            //}
            finally
            {
                if (fs != null) fs.Close();
            }
            return ret;

        }
        public static bool Deserialize(string filename, ref ObservableModelData obj)
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream fs = null;
            bool ret = false;
            try
            {
                fs = File.OpenRead(filename);
                obj = bf.Deserialize(fs) as ObservableModelData;
                ret = true;
            }
            //catch (Exception)
            //{
            //}
            finally
            {
                if (fs != null) fs.Close();
            }
            return ret;
        }
    }
}
