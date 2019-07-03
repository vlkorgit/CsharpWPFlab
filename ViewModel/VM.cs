using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Model;
using System.Windows.Input;
using Microsoft.Win32;
using System.ComponentModel;
using System.Windows.Forms.DataVisualization.Charting;

using System.Drawing;

namespace ViewModel
{
    public interface IServices
    {
        bool UISaveMessage();
        bool UIChooseSaveLoc(ref string str);
        bool UIChooseOpenLoc(ref string str);
        void UICustomMessage(string header, string body);
    }
    public class VM : INotifyPropertyChanged
    {
        private int selectedIndex; //поле и свойство для отслеживания выбранных объектов в списке (одного достаточно)
        private ObservableModelData omd;
        private ModelData md;
        private Chart chart;
        private IList selectedItems;
        public event PropertyChangedEventHandler PropertyChanged;
        private IServices services;
        public VM(IServices services)
        {
            md = new ModelData(3, 3);
            omd = new ObservableModelData();
            omd.AddDefaults();
            chart = new Chart();
            this.services = services;
            InitializeCommands();
        }
        public IList SelectedItems
        {
            get
            {
                return selectedItems;
            }
            set
            {
                selectedItems = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedItems"));
            }
        }
        public Chart Chart
        {
            get
            {
                return chart;
            }
            set
            {
                chart = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Chart"));
            }
        }
        public int SelectedIndex
        {
            get
            {
                return selectedIndex;
            }
            set
            {
                selectedIndex = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedIndex"));
            }
        }
        public ModelData MD
        {
            get
            {
                return md;
            }
            set
            {
                md = value;
                //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MD"));
            }
        }
        public ObservableModelData OMD
        {
            get
            {
                return omd;
            }
            set
            {
                omd = value;
                // PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("OMD"));
            }
        }

        public bool checkPropertyChanged()
        {
            if (PropertyChanged != null)
                return true;
            else return false;
        }
        private string testTextBlockField;
        private string testTextBoxField;
        public string TestTextBlockProperty
        {
            get
            {
                return testTextBlockField;
            }
            set
            {
                testTextBlockField = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TestTextBlockProperty"));
            }
        }
        public string TestTextBoxProperty
        {
            get
            {
                return testTextBoxField;
            }
            set
            {
                testTextBoxField = value;
                TestTextBlockProperty = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TestTextBoxProperty"));
            }
        }

        ///commands
        public ICommand Add { get; set; }
        public ICommand Remove { get; set; }
        public ICommand Draw { get; set; }
        public ICommand New { get; set; }
        public ICommand Open { get; set; }
        public ICommand Save { get; set; }
        private void InitializeCommands()
        {
            Add = new RelayCommand();
            (Add as RelayCommand).setExecute(AddExecuteHandler);
            (Add as RelayCommand).setCanExecute(AddCanExecuteHandler);
            Save = new RelayCommand();
            (Save as RelayCommand).setExecute(SaveExecuteHandler);
            (Save as RelayCommand).setCanExecute(SaveCanExecuteHandler);
            Remove = new RelayCommand();
            (Remove as RelayCommand).setExecute(RemoveExecuteHandler);
            (Remove as RelayCommand).setCanExecute(RemoveCanExecuteHandler);
            Draw = new RelayCommand();
            (Draw as RelayCommand).setExecute(DrawExecuteHandler);
            (Draw as RelayCommand).setCanExecute(DrawCanExecuteHandler);
            New = new RelayCommand();
            (New as RelayCommand).setExecute(NewExecuteHandler);
            (New as RelayCommand).setCanExecute(NewCanExecuteHandler);
            Open = new RelayCommand();
            (Open as RelayCommand).setExecute(OpenExecuteHandler);
            (Open as RelayCommand).setCanExecute(OpenCanExecuteHandler);

        }
        private bool OpenCanExecuteHandler(object obj)
        {
            return true;
        }

        private void OpenExecuteHandler(object obj)
        {
            if (omd.Changed)
            {
                if (services.UISaveMessage())
                    SaveExecuteHandler(obj);
            }
            string str = "";
            if (services.UIChooseOpenLoc(ref str))
            {
                ObservableModelData tmp = new ObservableModelData();
                if (!ObservableModelData.Deserialize(str, ref tmp))
                {
                    services.UICustomMessage("Error", "Deserialization error occured");
                }
                else
                {
                    omd.TransitFrom(tmp);
                }
            }
        }

        private bool NewCanExecuteHandler(object obj)
        {
            return true;
        }

        private void NewExecuteHandler(object obj)
        {
            if (omd.Changed)
            {
                if (services.UISaveMessage())
                    SaveExecuteHandler(obj);
            }
            omd.Clear();
        }

        private bool DrawCanExecuteHandler(object obj)
        {
            if (SelectedItems == null) return false;
            if (SelectedItems.Count < 1) return false;
            return true;
        }

        private void DrawExecuteHandler(object obj)
        {
            int n = SelectedItems.Count;
            chart.ChartAreas.Clear();
            chart.Series.Clear();
            chart.Legends.Clear();
            chart.ChartAreas.Add(new ChartArea("Selected"));
            chart.ChartAreas.Add(new ChartArea("Dependency"));
            chart.ChartAreas["Selected"].AxisX.Title = "X axis";
            chart.Legends.Add(new Legend("SelectedLegend"));
            chart.Legends.Add(new Legend("DependencyLegend"));
            chart.Legends[0].Title = "The legend";
            chart.Legends[0].DockedToChartArea = "Selected";
            chart.Legends["DependencyLegend"].Title = "Dependency legend";
            chart.Legends["DependencyLegend"].DockedToChartArea = "Dependency";
            chart.BackColor = System.Drawing.Color.AliceBlue;
            chart.ChartAreas[0].AxisX.LabelStyle.Format = "{0.0}";
            chart.ChartAreas[1].AxisX.LabelStyle.Format = "{0.0}";
            for (int i = 0; i < n; i++)
            {
                chart.Series.Add(i.ToString());
                chart.Series[i].ChartType = SeriesChartType.Spline;
                chart.Series[i].ChartArea = "Selected";
                chart.Series[i].Points.DataBindXY((SelectedItems[i] as ModelData).X, (SelectedItems[i] as ModelData).Y);
                chart.Series[i].Legend = "SelectedLegend";
                //chart.Series[i].LegendToolTip = $"Nodes amount = {(SelectedItems[i] as ModelData).NodesAmount}";
                //chart.Series[i].LegendText = $"P = {(SelectedItems[i] as ModelData).P}";
                chart.Series[i].LegendToolTip = "Nodes amount = " + (SelectedItems[i] as ModelData).NodesAmount;
                chart.Series[i].LegendText = "P = " + (SelectedItems[i] as ModelData).P;
            }
            int j = n;
            chart.Series.Add(j.ToString());
            chart.Series[j].ChartArea = "Dependency";
            chart.Series[j].ChartType = SeriesChartType.Spline;
            chart.Series[j].Points.DataBindXY(omd.PValues.ToArray<double>(), omd.FMin.ToArray<double>());
            chart.Series[j].IsVisibleInLegend = false;
            chart.Series.Add((j + 1).ToString());
            chart.Series[j + 1].ChartArea = "Dependency";
            chart.Series[j + 1].ChartType = SeriesChartType.Spline;
            //без приведения к массиву не работает, notimplemented exception
            chart.Series[j + 1].Points.DataBindXY(omd.PValues.ToArray<double>(), omd.FMax.ToArray<double>());
            chart.Series[j + 1].IsVisibleInLegend = false;
            chart.Series[j + 1].LabelToolTip = "label tooltip";
            chart.Series[j + 1].MarkerBorderColor = System.Drawing.Color.Black;
            chart.Series[j + 1].MarkerBorderWidth = 10;
            chart.Series[j + 1].MarkerStyle = MarkerStyle.Circle;
            chart.Series[j + 1].MarkerSize = 5;
            chart.Series[j].MarkerBorderColor = System.Drawing.Color.Black;
            chart.Series[j].MarkerBorderWidth = 10;
            chart.Series[j].MarkerStyle = MarkerStyle.Circle;
            chart.Series[j].MarkerSize = 5;
            chart.Series[j].ToolTip = "P = #VALX{F2} , Fmin = #VALY{F2}";
            chart.Series[j + 1].ToolTip = "P = #VALX{F2} , FMax = #VALY{F2}";
        }

        private bool RemoveCanExecuteHandler(object obj)
        {
            if (SelectedIndex == -1) return false;
            return true;
        }

        private void RemoveExecuteHandler(object obj)
        {
            while (SelectedIndex != -1)
            {
                if (SelectedIndex > omd.Count - 1) break; //костыль для тестирования
                omd.Remove_At(SelectedIndex);
            }
        }
        public bool AddCanExecuteHandler(object obj)
        {
            if (md == null) return false;
            if (md["NodesAmount"] == null && md["P"] == null)
                return true;
            else return false;
        }
        public void AddExecuteHandler(object obj)
        {
            OMD.Add_ModelData(new ModelData(ref md));

        }
        public void SaveExecuteHandler(object obj)
        {
            string str = "";
            if (services.UIChooseSaveLoc(ref str))
            {
                if (ObservableModelData.Serialize(str, ref omd))
                {
                    omd.Changed = false;
                }
            }
        }
        public bool SaveCanExecuteHandler(object obj)
        {
            if (omd.Changed) return true;
            else return false;
        }
    }
}