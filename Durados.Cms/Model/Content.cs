using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.Cms.Model
{
    public partial class Content
    {
        

        public string ViewName
        {
            get
            {
                if (!ContentTypeReference.IsLoaded)
                    ContentTypeReference.Load();
                return ContentType.Name;
            }
        }

        public Slider Slider
        {
            get
            {
                if (!ContentTypeReference.IsLoaded)
                    ContentTypeReference.Load();

                if (ContentType.Name == "Slider")
                    return Singleton.Cms.SliderSet.Where(s => s.ID == CustomTypeID).FirstOrDefault();
                else
                    return null;
            }
        }

        public List<SliderImage> GetSliderImages()
        {
            if (Slider == null)
                return new List<SliderImage>();

            if (!Slider.SliderImage.IsLoaded)
                Slider.SliderImage.Load();

            return Slider.SliderImage.OrderBy(s => s.Ordinal).ToList();
        }

        public SliderImage GetFirstSliderImage()
        {
            return GetSliderImages().FirstOrDefault();
        }

        public string GetFirstSliderImageHtml()
        {
            SliderImage sliderImage = GetSliderImages().FirstOrDefault();
            if (sliderImage == null)
                return string.Empty;

            return sliderImage.Html;
        }

        public string GetFirstSliderImageDisplay()
        {
            SliderImage sliderImage = GetSliderImages().FirstOrDefault();
            if (sliderImage == null)
                return string.Empty;

            return sliderImage.DisplayImage;
        }

        public Rotator Rotator
        {
            get
            {
                if (!ContentTypeReference.IsLoaded)
                    ContentTypeReference.Load();

                if (ContentType.Name == "Rotator")
                    return Singleton.Cms.RotatorSet.Where(s => s.ID == CustomTypeID).FirstOrDefault();
                else
                    return null;
            }
        }

        public List<RotatorImage> GetRotatorImages()
        {
            if (Rotator == null)
                return new List<RotatorImage>();

            if (!Rotator.RotatorImage.IsLoaded)
                Rotator.RotatorImage.Load();

            return Rotator.RotatorImage.OrderBy(i=>i.Ordinal).ToList();
        }

        public RotatorImage GetFirstRotatorImage()
        {
            return GetRotatorImages().FirstOrDefault();
        }

        public string GetFirstRotatorImageSrc()
        {
            RotatorImage rotatorImage = GetRotatorImages().FirstOrDefault();
            if (rotatorImage == null)
                return string.Empty;

            return rotatorImage.Image;
        }

        public Json.Content GetJsonContent(string path)
        {
            List<RotatorImage> rotatorImages = GetRotatorImages();

            List<Json.Image> rotatorImagesSrc = new List<Json.Image>();
            foreach (RotatorImage rotatorImage in rotatorImages)
            {
                rotatorImagesSrc.Add(new Json.Image() { Path = path + rotatorImage.Image, AdditionalDelay = rotatorImage.AdditionalDelay });
            }

            Json.Content content = new Cms.Model.Json.Content();
            content.Random = this.Rotator.Random;
            content.Images = rotatorImagesSrc.ToArray();
            content.Delay = this.Rotator.Delay;
            return content;
        }

        public Fader Fader
        {
            get
            {
                if (!ContentTypeReference.IsLoaded)
                    ContentTypeReference.Load();

                if (ContentType.Name == "Fader")
                    return Singleton.Cms.FaderSet.Where(s => s.ID == CustomTypeID).FirstOrDefault();
                else
                    return null;
            }
        }

        public Fader GetFader()
        {
            return this.Fader;
        }


        public Menu GetFaderMenu()
        {
            return Fader.GetMenu();
        }

        public HtmlScroller HtmlScroller
        {
            get
            {
                if (!ContentTypeReference.IsLoaded)
                    ContentTypeReference.Load();

                if (ContentType.Name == "HtmlScroller")
                    return Singleton.Cms.HtmlScrollerSet.Where(s => s.Id == CustomTypeID).FirstOrDefault();
                else
                    return null;
            }
        }

        public HtmlScroller GetHtmlScroller()
        {
            return this.HtmlScroller;
        }

        public string Url
        {
            get
            {
                if (!this.FrameReference.IsLoaded)
                    this.FrameReference.Load();

                if (this.Frame == null)
                    return string.Empty;

                return this.Frame.Url;
            }
        }

        public string ImageSrc
        {
            get
            {
                if (!this.ImageReference.IsLoaded)
                    this.ImageReference.Load();

                if (this.Image == null)
                    return string.Empty;

                return this.Image.Src;
            }
        }

        public string ImageToolTip
        {
            get
            {
                if (!this.ImageReference.IsLoaded)
                    this.ImageReference.Load();

                if (this.Image == null)
                    return string.Empty;

                return this.Image.ToolTip;
            }
        }

        public string HtmlText
        {
            get
            {
                if (!this.HtmlReference.IsLoaded)
                    this.HtmlReference.Load();

                if (this.Html == null)
                    return string.Empty;

                return this.Html.Text;
            }
        }

        public Form Form
        {
            get
            {
                if (!ContentTypeReference.IsLoaded)
                    ContentTypeReference.Load();

                if (ContentType.Name == "Form")
                    return Singleton.Cms.FormSet.Where(s => s.ID == CustomTypeID).FirstOrDefault();
                else
                    return null;
            }
        }

        public Contact Contact
        {
            get
            {
                if (!ContentTypeReference.IsLoaded)
                    ContentTypeReference.Load();

                if (ContentType.Name == "Contact")
                    return Singleton.Cms.ContactSet.Where(s => s.ID == CustomTypeID).FirstOrDefault();
                else
                    return null;
            }
        }


        public FormTypeEnum FormTypeEnum
        {
            get
            {
                return Form.FormTypeEnum;
            }
        }

        public string FormViewName
        {
            get
            {
                return Form.ViewName;
            }
        }

        public Durados.DataAction FormDataAction
        {
            get
            {
                return Form.DataActionEnum;
            }
        } 
    }
}
