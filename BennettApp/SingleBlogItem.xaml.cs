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
    public partial class SingleBlogItem : PhoneApplicationPage
    {
        public SingleBlogItem()
        {
            InitializeComponent();

            var blogItem = App.ViewModel.BlogItems.Where(b => b.ID == App.ViewModel.ClickedBlog).SingleOrDefault();

            PageTitle.Text = blogItem.Title;

            ImageSourceConverter imgs=new ImageSourceConverter();
            BlogImage.SetValue(Image.SourceProperty, imgs.ConvertFromString(blogItem.ImageSource));

            textBlock1.Text = blogItem.Content;
        }
    }
}