﻿using System;
using System.Windows.Media;
using Wpf.Ui.Appearance;

namespace CSharpQuiz.Helpers;

class Elements
{
    public static SolidColorBrush ModifyBrush(
        SolidColorBrush brush,
        double multiplier)
    {
        double red = Math.Min(255, Math.Max(0, brush.Color.R * multiplier));
        double green = Math.Min(255, Math.Max(0, brush.Color.G * multiplier));
        double blue = Math.Min(255, Math.Max(0, brush.Color.B * multiplier));

        return new(Color.FromArgb(brush.Color.A, (byte)red, (byte)green, (byte)blue));
    }


    public static DrawingImage IconImage
    {
        get
        {
            DrawingGroup geometryGroup = new();

            geometryGroup.Children.Add(new GeometryDrawing(ApplicationAccentColorManager.PrimaryAccentBrush, null, Geometry.Parse("F1 M256,288z M0,0z M255.569,84.452376C255.567,79.622376 254.534,75.354376 252.445,71.691376 250.393,68.089376 247.32,65.070376 243.198,62.683376 209.173,43.064376 175.115,23.505376 141.101,3.86637605000001 131.931,-1.42762394999999 123.04,-1.23462394999999 113.938,4.13537605000001 100.395,12.122376 32.59,50.969376 12.385,62.672376 4.06400000000001,67.489376 0.0150000000000059,74.861376 0.0130000000000052,84.443376 5.23019128007007E-15,123.898376 0.0130000000000052,163.352376 5.23019128007007E-15,202.808376 5.23019128007007E-15,207.532376 0.991000000000005,211.717376 2.98800000000001,215.325376 5.04100000000001,219.036376 8.15700000000001,222.138376 12.374,224.579376 32.58,236.282376 100.394,275.126376 113.934,283.115376 123.04,288.488376 131.931,288.680376 141.104,283.384376 175.119,263.744376 209.179,244.186376 243.209,224.567376 247.426,222.127376 250.542,219.023376 252.595,215.315376 254.589,211.707376 255.582,207.522376 255.582,202.797376 255.582,202.797376 255.582,123.908376 255.569,84.452376")));
            geometryGroup.Children.Add(new GeometryDrawing(ModifyBrush((SolidColorBrush)ApplicationAccentColorManager.PrimaryAccentBrush, 0.6), null, Geometry.Parse("F1 M256,288z M0,0z M128.182,143.241376L2.98799999999999,215.325376C5.04099999999999,219.036376 8.15699999999999,222.138376 12.374,224.579376 32.58,236.282376 100.394,275.126376 113.934,283.115376 123.04,288.488376 131.931,288.680376 141.104,283.384376 175.119,263.744376 209.179,244.186376 243.209,224.567376 247.426,222.127376 250.542,219.023376 252.595,215.315376z")));
            geometryGroup.Children.Add(new GeometryDrawing(ModifyBrush((SolidColorBrush)ApplicationAccentColorManager.PrimaryAccentBrush, 0.4), null, Geometry.Parse("F1 M256,288z M0,0z M255.569,84.452376C255.567,79.622376,254.534,75.354376,252.445,71.691376L128.182,143.241376 252.595,215.315376C254.589,211.707376 255.58,207.522376 255.582,202.797376 255.582,202.797376 255.582,123.908376 255.569,84.452376")));
            geometryGroup.Children.Add(new GeometryDrawing(Brushes.White, null, Geometry.Parse("F1 M256,288z M0,0z M201.892326,116.294008L201.892326,129.767692 215.36601,129.767692 215.36601,116.294008 222.102852,116.294008 222.102852,129.767692 235.576537,129.767692 235.576537,136.504534 222.102852,136.504534 222.102852,149.978218 235.576537,149.978218 235.576537,156.71506 222.102852,156.71506 222.102852,170.188744 215.36601,170.188744 215.36601,156.71506 201.892326,156.71506 201.892326,170.188744 195.155484,170.188744 195.155484,156.71506 181.6818,156.71506 181.6818,149.978218 195.155484,149.978218 195.155484,136.504534 181.6818,136.504534 181.6818,129.767692 195.155484,129.767692 195.155484,116.294008z M215.36601,136.504534L201.892326,136.504534 201.892326,149.978218 215.36601,149.978218z")));
            geometryGroup.Children.Add(new GeometryDrawing(Brushes.White, null, Geometry.Parse("F1 M256,288z M0,0z M128.456752,48.625876C163.600523,48.625876,194.283885,67.7121741,210.718562,96.0819435L210.558192,95.808876 169.209615,119.617159C161.062959,105.823554,146.128136,96.5150717,128.996383,96.3233722L128.456752,96.3203544C102.331178,96.3203544 81.1506705,117.499743 81.1506705,143.625316 81.1506705,152.168931 83.4284453,160.17752 87.3896469,167.094792 95.543745,181.330045 110.872554,190.931398 128.456752,190.931398 146.149522,190.931398 161.565636,181.208041 169.67832,166.820563L169.481192,167.165876 210.767678,191.083913C194.51328,219.21347,164.25027,238.240861,129.514977,238.620102L128.456752,238.625876C93.2021701,238.625876 62.4315028,219.422052 46.0382398,190.902296 38.0352471,176.979327 33.4561922,160.837907 33.4561922,143.625316 33.4561922,91.1592636 75.9884604,48.625876 128.456752,48.625876z")));

            return new(geometryGroup);
        }
    }

}