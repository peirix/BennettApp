using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Net;
using System.Xml.Linq;
using System.Linq;
using System.Text.RegularExpressions;


namespace BennettApp
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public MainViewModel()
        {
            this.BlogItems = new ObservableCollection<ItemViewModel>();
            this.JobItems = new ObservableCollection<ItemViewModel>();
            this.TweetItems = new ObservableCollection<ItemViewModel>();
            this.ContactItems = new ObservableCollection<ItemViewModel>();
        }

        /// <summary>
        /// A collection for ItemViewModel objects.
        /// </summary>
        public ObservableCollection<ItemViewModel> BlogItems { get; private set; }
        public ObservableCollection<ItemViewModel> JobItems { get; private set; }
        public ObservableCollection<ItemViewModel> TweetItems { get; private set; }
        public ObservableCollection<ItemViewModel> ContactItems { get; private set; }
        public int ClickedBlog { get; set; }
        public int ClickedJob { get; set; }

        public bool IsDataLoaded
        {
            get;
            private set;
        }

        /// <summary>
        /// Creates and adds a few ItemViewModel objects into the Items collection.
        /// </summary>
        public void LoadData()
        {
            // Load the blog
            WebClient blog = new WebClient();

            blog.DownloadStringCompleted += new DownloadStringCompletedEventHandler(blog_DownloadStringCompleted);
            blog.DownloadStringAsync(new Uri("http://www.bennett.no/ressurser/blogg"));

            //Load jobs
            WebClient jobs = new WebClient();

            jobs.DownloadStringCompleted += new DownloadStringCompletedEventHandler(jobs_DownloadStringCompleted);
            jobs.DownloadStringAsync(new Uri("http://www.bennett.no/ressurser/jobber"));
            
            //Load contacts
            WebClient contacts = new WebClient();

            contacts.DownloadStringCompleted += new DownloadStringCompletedEventHandler(contacts_DownloadStringCompleted);
            contacts.DownloadStringAsync(new Uri("http://www.bennett.no/ressurser/ansatte"));

            //Load Twitter feed
            WebClient tweets = new WebClient();

            tweets.DownloadStringCompleted += new DownloadStringCompletedEventHandler(tweets_DownloadStringCompleted);
            tweets.DownloadStringAsync(new Uri("http://api.twitter.com/1/statuses/user_timeline.xml?screen_name=bennett_tweet"));

            this.IsDataLoaded = true;
        }

        void blog_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error != null)
                return;

            XElement xml = XElement.Parse(e.Result);

            var blogItems = CreateListFromXml(xml);

            blogItems.OrderByDescending(i => i.Date.DayOfYear).ToList();

            foreach (var blog in blogItems.OrderByDescending(b => b.Date))
            {
                this.BlogItems.Add(blog);
            }
        }

        void jobs_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error != null)
                return;

            XElement xml = XElement.Parse(e.Result);

            var jobItems = CreateListFromXml(xml);

            foreach (var job in jobItems)
            {
                this.JobItems.Add(job);
            }
        }

        void contacts_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error != null)
                return;

            XElement xml = XElement.Parse(e.Result);

            foreach (var contact in xml.Element("Item").Element("Items").Descendants("Item"))
            {
                if (contact.Element("Status").Value == "Active")
                {
                    this.ContactItems.Add(new ItemViewModel
                    {
                        Title = contact.Element("Name").Value,
                        Description = contact.Element("JobTitle").Value + "\n" + 
                                      contact.Element("PhoneWork").Value + " / " + contact.Element("PhoneMobile").Value + "\n" +
                                      contact.Element("Email").Value + "\n" + 
                                      contact.Element("Twitter").Value,
                        ImageSource = "http://bennett.no/" + contact.Element("ThumbnailImagePath").Value
                    });
                }
            }
        }

        void tweets_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error != null)
                return;

            XElement xml = XElement.Parse(e.Result);

            foreach (var tweet in xml.Elements("status"))
            {
                this.TweetItems.Add(new ItemViewModel
                {
                    Content = tweet.Element("id").Value, //Hack because the id is to big for an int
                    Title = tweet.Element("user").Element("screen_name").Value,
                    Description = TwitterText(tweet.Element("text").Value),
                    ImageSource = tweet.Element("user").Element("profile_image_url").Value
                });
            }
            
        }

        

        private List<ItemViewModel> CreateListFromXml(XElement xml)
        {
            List<ItemViewModel> xmlItems = new List<ItemViewModel>();

            foreach (var item in xml.Element("Item").Element("Items").Descendants("Item"))
            {
                if (item.Element("Status").Value == "Active" &&
                    item.Element("ItemType").Element("ID").Value != "162")
                {
                    xmlItems.Add(new ItemViewModel()
                    {
                        ID = int.Parse(item.Element("ID").Value),
                        Title = item.Element("Name").Value,
                        Description = item.Element("Description").Value,
                        Content = CleanHtml(item.Element("HtmlContent").Value),
                        ImageSource = "http://bennett.no/" + item.Element("ThumbnailImagePath").Value,
                        LargeImage = "http://bennett.no/" + item.Element("ImagePath").Value,
                        Date = DateTime.Parse(item.Element("ChangedDate").Value)
                    });
                }
            }

            return xmlItems;
        }

        private string TwitterText(string text)
        {
            if (text.Contains("http"))
            {
                var btn = new HyperlinkButton();
                int linkStart = text.IndexOf("http");
                int linkLength = text.IndexOf(" ", text.IndexOf("http"));
                string link = (linkLength > 0) ? text.Substring(linkStart, linkLength) : text.Substring(linkStart);
                btn.Content = link;
                btn.NavigateUri = new Uri(link);
                text = text.Replace(link, btn.ToString());
            }
            return text;
        }

        private string CleanHtml(string text)
        {
            string pattern = @"<(.|\n)*?>";
            return Regex.Replace(text, pattern, string.Empty);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}