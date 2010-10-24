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
using Microsoft.Phone.Tasks;

namespace BennettApp
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Set the data context of the listbox control to the sample data
            DataContext = App.ViewModel;
            this.Loaded += new RoutedEventHandler(MainPage_Loaded);
        }

        // Load data for the ViewModel Items
        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (!App.ViewModel.IsDataLoaded)
            {
                App.ViewModel.LoadData();
                
            }
        }

        private void Blog_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selected = ((ItemViewModel)((ListBox)sender).SelectedItem).Title;

            ItemViewModel blog = App.ViewModel.BlogItems.Where(b => b.Title == selected).First();

            if (blog != null)
            {
                App.ViewModel.ClickedBlog = blog.ID;
                NavigationService.Navigate(new Uri("/SingleBlogItem.xaml", UriKind.Relative));
            }
        }

        private void Job_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selected = ((ItemViewModel)((ListBox)sender).SelectedItem).Title;

            ItemViewModel job = App.ViewModel.JobItems.Where(j => j.Title == selected).First();

            if (job != null)
            {
                App.ViewModel.ClickedJob = job.ID;
                NavigationService.Navigate(new Uri("/SingleJobPage.xaml", UriKind.Relative));
            }
        }

        private void Twitter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selected = ((ItemViewModel)((ListBox)sender).SelectedItem).Content; //using content field as id for tweets

            ItemViewModel tweet = App.ViewModel.TweetItems.Where(t => t.Content == selected).First();

            if (tweet != null)
            {
                WebBrowserTask task = new WebBrowserTask();
                task.URL = "http://www.twitter.com/bennett_tweet/status/" + tweet.Content;
                task.Show();
            }
        }
    }
}