using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;

namespace BennettApp
{
    public partial class SingleJobPage : PhoneApplicationPage
    {
        public SingleJobPage()
        {
            InitializeComponent();

            var jobItem = App.ViewModel.JobItems.Where(j => j.ID == App.ViewModel.ClickedJob).SingleOrDefault();
            PageTitle.Text = jobItem.Title;

            ImageSourceConverter imgs = new ImageSourceConverter();
            var img = imgs.ConvertFromString(jobItem.LargeImage);

            JobImage.SetValue(Image.SourceProperty, img);

        }
    }
}