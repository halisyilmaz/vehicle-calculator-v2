using LiveChartsCore.SkiaSharpView;
using LiveChartsCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reflection.Metadata;

namespace VehicleCalculatorV2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public class ViewModel
    {
        public ISeries[] Series { get; set; }
            = new ISeries[]
            {
                new LineSeries<double>
                {
                    Values = new double[] { 2, 1, 3, 5, 3, 4, 6 },
                    Fill = null
                }
            };
    }


    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }


        private void bCalculate_Click(object sender, EventArgs e)
        {
            

        }

        private void bAutoFill_Click(object sender, RoutedEventArgs e)
        {
            cbPlanet.SelectedIndex = 0;
            tbWeight.Text = "1600";
            tbFrontalArea.Text = "0";
            tbWidth.Text = "1,8";
            tbHeight.Text = "1,4";
            tbDragCoefficient.Text = "0,42";
            tbDriveRatio.Text = "4,15";
            tbGearRatio1.Text = "2,97";
            tbGearRatio2.Text = "2,04";
            tbGearRatio3.Text = "1,45";
            tbGearRatio4.Text = "1";
            tbGearRatio5.Text = "0,8";
            tbGearEfficiency1.Text = "0,85";
            tbGearEfficiency2.Text = "0,88";
            tbGearEfficiency3.Text = "0,88";
            tbGearEfficiency4.Text = "0,92";
            tbGearEfficiency5.Text = "0,95";
            tbMaxPower.Text = "90";
            tbMaxPowerRPM.Text = "5250";
            tbMaxTorque.Text = "16";
            tbMaxTorqueRPM.Text = "3000";
            tbMaxRPM.Text = "6000";
            tbAngle.Text = "0";
            tbWheelWidth.Text = "165";
            tbWheelAspectRatio.Text = "60";
            tbWheelDiameter.Text = "13";
        }

        private void bCalculate_Click(object sender, RoutedEventArgs e)
        {
            int wheelWidth,
                wheelAspectRatio,
                wheelDiameter;

            double angle,
                    weight = 0,
                    width = 0,
                    height = 0,
                    frontalArea = 0,
                    maxPower,
                    maxTorque,
                    maxRPM,
                    DragCoefficient,
                    driveRatio,
                    maxPowerRPM,
                    maxTorqueRPM;

            double P1, P2, n1, n2, A, B, C;
            double gForce = 9.81, airResistanceCoef = 0.047;

            List<double> slipFactors = new List<double>() { 1.08, 1.06, 1.04, 1.02, 1.01 };
            double gearRatio1, gearRatio2, gearRatio3, gearRatio4, gearRatio5;
            double transmissionEfficiency1, transmissionEfficiency2, transmissionEfficiency3, transmissionEfficiency4, transmissionEfficiency5;

            if (double.TryParse(tbWeight.Text, out weight) &&
                double.TryParse(tbFrontalArea.Text, out frontalArea) &&
                double.TryParse(tbWidth.Text, out width) &&
                double.TryParse(tbHeight.Text, out height) &&
                double.TryParse(tbDragCoefficient.Text, out DragCoefficient) &&
                double.TryParse(tbDriveRatio.Text, out driveRatio) &&
                double.TryParse(tbGearRatio1.Text, out gearRatio1) &&
                double.TryParse(tbGearRatio2.Text, out gearRatio2) &&
                double.TryParse(tbGearRatio3.Text, out gearRatio3) &&
                double.TryParse(tbGearRatio4.Text, out gearRatio4) &&
                double.TryParse(tbGearRatio5.Text, out gearRatio5) &&
                double.TryParse(tbGearEfficiency1.Text, out transmissionEfficiency1) &&
                double.TryParse(tbGearEfficiency2.Text, out transmissionEfficiency2) &&
                double.TryParse(tbGearEfficiency3.Text, out transmissionEfficiency3) &&
                double.TryParse(tbGearEfficiency4.Text, out transmissionEfficiency4) &&
                double.TryParse(tbGearEfficiency5.Text, out transmissionEfficiency5) &&
                double.TryParse(tbMaxPower.Text, out maxPower) &&
                double.TryParse(tbMaxPowerRPM.Text, out maxPowerRPM) &&
                double.TryParse(tbMaxTorque.Text, out maxTorque) &&
                double.TryParse(tbMaxTorqueRPM.Text, out maxTorqueRPM) &&
                double.TryParse(tbMaxRPM.Text, out maxRPM) &&
                double.TryParse(tbAngle.Text, out angle) &&
                int.TryParse(tbWheelWidth.Text, out wheelWidth) &&
                int.TryParse(tbWheelAspectRatio.Text, out wheelAspectRatio) &&
                int.TryParse(tbWheelDiameter.Text, out wheelDiameter))
            {
                //Define G-Force & Air Resistance Coef.
                if (cbPlanet.SelectedIndex == 0)
                {
                    gForce = 9.81;
                    airResistanceCoef = 0.047;
                }
                else if (cbPlanet.SelectedIndex == 1)
                {
                    gForce = 3.72;
                    airResistanceCoef = 0.047 * 0.02 / 1.2;
                }
                else
                {
                    MessageBox.Show("Did you just discovered a planet?", "Unknown Planet Error");
                }


                P1 = maxPower;
                n1 = maxPowerRPM;
                P2 = maxTorque * gForce * maxTorqueRPM / 7124;
                n2 = maxTorqueRPM;
                Debug.WriteLine("P1: " + P1.ToString());
                Debug.WriteLine("n1: " + n1.ToString());
                Debug.WriteLine("P2: " + P2.ToString());
                Debug.WriteLine("n2: " + n2.ToString());


                //Rolling Resistance Calculation
                //double rollingResistance = (0.0136 + 0.4 * Math.Pow(10, -7) * Math.Pow(speed, 2)) * weight * gForce;

                //Gradient Resistance Calculation
                double sinAlpha = Math.Sin(angle * (Math.PI) / 180);
                double gradientResistance = weight * sinAlpha * gForce;
                Debug.WriteLine("GradientResistance: " + gradientResistance.ToString());

                //Frontal Area Calculation
                if (frontalArea == 0 && width != 0 && height != 0)
                {
                    frontalArea = 0.8 * width * height;
                }
                else
                {
                    MessageBox.Show("Please input Frontal Area or W/H Values", "Frontal Area Calculation Error");
                }
                Debug.WriteLine("Frontal Area: " + frontalArea.ToString());

                //Air Resistance Calculation
                //double airResistance = airResistanceCoef * DragCoefficient * frontalArea * Math.Pow(speed, 2);


                //Rolling Radius Calculation
                double rollingRadius = 0.96 * (wheelDiameter * 25.4 / 2000 + 0.8 * wheelWidth / 1000);
                Debug.WriteLine("rw: " + rollingRadius.ToString());


                //A = ((P2/P1) + Math.Pow((n2 / n1), 2) * (2 * n2 / n1 - 3)) / ((n2 / n1) * (Math.Pow((1 - n2/n1), 2)));
                //B = 3 - (2 * A);
                //C = A - 2;

                A = (Math.Pow((n2 / n1), 2) - (P2 / P1) * (2 - 1 / (n2 / n1))) / (Math.Pow(1 - (n2 / n1), 2));
                B = (1 - A) / (1 - 1 / (2 * n2 / n1));
                C = -B / (2 * n2 / n1);

                Debug.WriteLine("A,B,C: " + A.ToString() + "," + B.ToString() + "," + C.ToString());
                //A' = P1 * A / n1 ; B' = P1 * B / Math.Powe(n1,2) ; C' =  P1 * C / Math.Powe(n1,3) ; V = rw*ne/(Nf*Nt*s) ; ne = (Nf*Nt*s*V)/rw
                // s: slip ration, Nt: Gear Ratio, Nf: Final ratio, nw: Wheel speed, rw: Wheel(rolling) radius, ne: engine speed
                // V = rw*nw = rw * Nf*Nt*s

                List<double> gearRatios = new List<double>() { gearRatio1, gearRatio2, gearRatio3, gearRatio4, gearRatio5 };
                List<double> transmissionEfficiencies = new List<double>() { transmissionEfficiency1, transmissionEfficiency2, transmissionEfficiency3, transmissionEfficiency4, transmissionEfficiency5 };
                List<double> maxSpeeds = new List<double>();
                for (int i = 0; i < 5; i++)
                {
                    double a = 50025 * Math.Pow(driveRatio, 3) * Math.Pow(gearRatios[i], 3) * transmissionEfficiencies[i] * slipFactors[i] * P1 * C / Math.Pow(n1, 3) / Math.Pow(rollingRadius, 3)
                                - (0.4 * Math.Pow(10, -7) * weight * gForce) - (0.047 * DragCoefficient * frontalArea); //Coefficient of V^2 terms
                    double b = 18878 * Math.Pow(driveRatio, 2) * Math.Pow(gearRatios[i], 2) * transmissionEfficiencies[i] * slipFactors[i] * P1 * B / Math.Pow(n1, 2) / Math.Pow(rollingRadius, 2); ////Coefficient of V terms
                    double c = (7124 * driveRatio * gearRatios[i] * transmissionEfficiencies[i] * P1 * A / n1 / rollingRadius) - (0.0136 * weight * gForce) - gradientResistance;
                    Debug.WriteLine("a,b,c: " + a.ToString() + "," + b.ToString() + "," + c.ToString());//Constant terms
                    maxSpeeds.Add((-b - (Math.Sqrt(Math.Pow(b, 2) - 4 * a * c))) / (2 * a));
                }


                //double Fnet = Ft - (rollingResistance + gradientResistance + airResistance);

                /*lMaxSpeed.Text = "Max Speed: " + Math.Round(maxSpeeds.Max(), 2).ToString() + " km/h";
                lMaxSpeed1.Text = "1st Gear Max Speed: " + Math.Round(maxSpeeds[0], 2).ToString() + " km/h";
                lMaxSpeed2.Text = "2ndt Gear Max Speed: " + Math.Round(maxSpeeds[1], 2).ToString() + " km/h";
                lMaxSpeed3.Text = "3rd Gear Max Speed: " + Math.Round(maxSpeeds[2], 2).ToString() + " km/h";
                lMaxSpeed4.Text = "4th Gear Max Speed: " + Math.Round(maxSpeeds[3], 2).ToString() + " km/h";
                lMaxSpeed5.Text = "5th Gear Max Speed: " + Math.Round(maxSpeeds[4], 2).ToString() + " km/h";

                lMaxAcceleration.Text = "Max Acceleration: "; //To be changed*/

                // Draw Max Speed vs Gear Curve
                List<string> Gears = new List<string>() { "1st", "2nd", "3rd", "4th", "5th" };
                MaxSpeedGearChart.Series = new ObservableCollection<ISeries>
                {
                    new LineSeries<double>
                    {
                        Values = maxSpeeds,
                        Name = "Max Speed",
                        Fill = null,
                        GeometrySize = 5,
                        Stroke = new SolidColorPaint(new SKColor(25, 118, 210), 2),
                        GeometryStroke = new SolidColorPaint(new SKColor(25, 118, 210), 2),
                        TooltipLabelFormatter =
                        (chartPoint) => $"{chartPoint.Context.Series.Name} is {Math.Round(chartPoint.PrimaryValue,2)} at {Gears[chartPoint.Context.Index]} Gear"

                    }
                };

                MaxSpeedGearChart.XAxes = new List<Axis>
                {
                    new Axis
                    {
                        // Use the labels property to define named labels.
                        Name = "Gear",
                        NameTextSize = 14,
                        TextSize = 12,
                        MinStep = 1,
                        Labels = Gears
                    }
                };

                MaxSpeedGearChart.YAxes = new List<Axis>
                {
                    new Axis
                    {
                        Name = "Max Speed(km/h)",
                        NameTextSize = 14,
                        TextSize = 12,
                        MinStep = 1,

                    }
                };


                // Draw Power vs RPM Curve

                List<string> RPMs = new List<string>();
                List<double> Powers = new List<double>();
                for (int rpm = 100; rpm <= maxRPM; rpm += 100)
                {
                    RPMs.Add(rpm.ToString());
                    double x = rpm / maxPowerRPM;
                    Powers.Add(maxPower * (A * x + B * Math.Pow(x, 2) + C * Math.Pow(x, 3)));
                }

                PowerRPMChart.ZoomMode = LiveChartsCore.Measure.ZoomAndPanMode.X;

                PowerRPMChart.Series = new ObservableCollection<ISeries>
                {
                    new LineSeries<double>
                    {
                        Values = Powers,
                        Fill = null,
                        GeometrySize = 5,
                        Stroke = new SolidColorPaint(new SKColor(118, 25, 210), 2),
                        GeometryStroke = new SolidColorPaint(new SKColor(118, 25, 210), 2),
                        Name = "Power",
                        TooltipLabelFormatter =
                        (chartPoint) => $"{chartPoint.Context.Series.Name} is {Math.Round(chartPoint.PrimaryValue,2)} hp at {RPMs[chartPoint.Context.Index]} RPM"

                    }
                };

                PowerRPMChart.XAxes = new List<Axis>
                {
                    new Axis
                    {
                        // Use the labels property to define named labels.
                        Name = "RPM",
                        NameTextSize = 14,
                        TextSize = 12,
                        MinStep = 1,
                        Labels = RPMs

                    }
                };

                PowerRPMChart.YAxes = new List<Axis>
                {
                    new Axis
                    {
                        Name = "Power (hp)",
                        NameTextSize = 14,
                        TextSize = 12,
                        MinStep = 1,

                    }
                };



            }

            // At least one input is empty or not number
            else
            {
                MessageBox.Show("Please fill all areas and input numbers only", "Unaccaptable Input Error");
            }
        }
    }
}
    
